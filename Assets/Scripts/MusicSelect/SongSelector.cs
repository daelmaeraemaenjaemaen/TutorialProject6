using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class SongSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image miniCover;
    [SerializeField] private TextMeshProUGUI miniName;
    [SerializeField] private Button StartBtn;
    [SerializeField] private AudioSource previewAudioSource;
    private Coroutine previewLoopRoutine;

    // ★ 추가: 전역 프리뷰 토큰(가장 최근에 시작한 프리뷰만 유효하게)
    private static int s_GlobalPreviewToken = 0;

    private uint songCode;
    public string songFileName; // 실제 파일명

    public void setSongSelector(Song song)
    {
        string cover = song.getsongCover() == "" ? "dummy.png" : song.getsongCover();
        byte[] fileData = File.ReadAllBytes(Application.dataPath + "/Images/Cover/" + cover);
        Texture2D texture = new Texture2D(500, 500);
        if (texture.LoadImage(fileData))
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            miniCover.sprite = sprite;
        }

        miniName.text = song.getsongName();
        songCode = song.getSongID();
        songFileName = song.getSongFileName(); // 실제 파일명
    }

    void Update()
    {
        StartBtn.gameObject.SetActive(songCode == MusicSelect.selectedSong);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MusicSelect.selectedSong = songCode;
        PlayPreviewFromFile(songFileName);

        if (MusicSelect.instance != null)
            MusicSelect.instance.SetSelectedSongImmediate(songCode);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Pointer Up");
    }

    private void Start()
    {
        StartBtn.onClick.AddListener(OnStartBtnClicked);
    }

    private void OnStartBtnClicked()
    {
        PlayerPrefs.SetInt("selectedSongID", (int)songCode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GamePlay");
    }

    public void StopPreview()
    {
        if (previewLoopRoutine != null)
        {
            StopCoroutine(previewLoopRoutine);
            previewLoopRoutine = null;
        }
        if (previewAudioSource != null && previewAudioSource.isPlaying)
        {
            previewAudioSource.Stop();
            previewAudioSource.clip = null;
        }
    }

    // === 핵심: Resources 우선 → 없으면 절대경로 폴백 + 전역 토큰으로 경쟁 차단 ===
    public void PlayPreviewFromFile(string fileName)
    {
        StopPreview(); // 내 코루틴 중지
        if (previewAudioSource == null)
        {
            // 씬에 "AudioSource" 오브젝트가 있다면 먼저 가져옴(프로젝트 기존 구조 유지)
            var go = GameObject.Find("AudioSource");
            if (go != null) previewAudioSource = go.GetComponent<AudioSource>();
            if (previewAudioSource == null)
            {
                var newGO = new GameObject("[PreviewAudioSource]");
                previewAudioSource = newGO.AddComponent<AudioSource>();
                previewAudioSource.playOnAwake = false;
                previewAudioSource.loop = false;
                previewAudioSource.spatialBlend = 0f; // 2D
            }
        }

        // ★ 전역 프리뷰 토큰 갱신(이 호출이 "가장 최신"임을 선언)
        int myToken = ++s_GlobalPreviewToken;

        // 1) Resources/PreviewAudio/<name> 우선(빌드 안전)
        var nameNoExt = Path.GetFileNameWithoutExtension(fileName);
        var resClip = Resources.Load<AudioClip>($"PreviewAudio/{nameNoExt}");
        if (resClip != null)
        {
            previewLoopRoutine = StartCoroutine(LoopPreviewWithFadeOut(resClip, myToken));
            return;
        }

        // 2) (주로 에디터) Assets/Resources/PreviewAudio/<fileName> 폴백
        string absPath = Path.Combine(Application.dataPath, "Resources/PreviewAudio/" + fileName);
        if (!File.Exists(absPath))
        {
            Debug.LogWarning("[Preview] 파일 없음: " + absPath);
            return;
        }
        previewLoopRoutine = StartCoroutine(PlayAudioFromAbsolutePath(absPath, myToken));
    }

    private IEnumerator PlayAudioFromAbsolutePath(string filePath, int myToken)
    {
        // 확장자에 맞춰 AudioType 추정
        AudioType type = AudioType.WAV;
        string lower = filePath.ToLowerInvariant();
        if (lower.EndsWith(".ogg")) type = AudioType.OGGVORBIS;
        else if (lower.EndsWith(".mp3")) type = AudioType.MPEG;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, type))
        {
            yield return www.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("[Preview] 로드 실패: " + www.error);
                yield break;
            }

            // ★ 도중에 더 최신 프리뷰가 시작됐다면 즉시 중단
            if (myToken != s_GlobalPreviewToken) yield break;

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            previewLoopRoutine = StartCoroutine(LoopPreviewWithFadeOut(clip, myToken));
        }
    }

    private IEnumerator LoopPreviewWithFadeOut(AudioClip clip, int myToken)
    {
        while (true)
        {
            // ★ 최신이 아니면 즉시 종료(이전 코루틴 무력화)
            if (myToken != s_GlobalPreviewToken) yield break;
            if (previewAudioSource == null) yield break;

            previewAudioSource.Stop();
            previewAudioSource.clip = clip;
            previewAudioSource.volume = 1f;
            previewAudioSource.time = 0f;
            previewAudioSource.Play();

            // 본재생 구간
            float playFor = Mathf.Max(0.1f, clip.length - 1f);
            float t = 0f;
            while (t < playFor)
            {
                if (myToken != s_GlobalPreviewToken) yield break; // ★ 중간에도 체크
                t += Time.deltaTime;
                yield return null;
            }

            // 페이드아웃
            float fadeDuration = 1f;
            t = 0f;
            while (t < fadeDuration)
            {
                if (myToken != s_GlobalPreviewToken) yield break; // ★ 중간에도 체크
                t += Time.deltaTime;
                previewAudioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            // 다음 루프(재시작) 전에 최신 여부 재확인
            if (myToken != s_GlobalPreviewToken) yield break;

            previewAudioSource.Stop();
            previewAudioSource.volume = 1f;
            // loop 계속
        }
    }
}
