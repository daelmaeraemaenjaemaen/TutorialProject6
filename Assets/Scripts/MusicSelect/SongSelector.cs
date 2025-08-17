using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Audio; // ★ Mixer 라우팅을 위해 추가

public class SongSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image miniCover;
    [SerializeField] private TextMeshProUGUI miniName;
    [SerializeField] private Button StartBtn;
    [SerializeField] private AudioSource previewAudioSource;   // 프리뷰 재생용 오디오 소스 (없으면 Awake에서 생성)
    private Coroutine previewLoopRoutine;

    // === 추가: Mixer 라우팅 ===
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup bgmGroup; // GameMixer의 BGM 그룹을 드래그

    private uint songCode;
    public string songFileName; // 실제 파일명(예: "Track01.ogg")

    private void Awake()
    {
        // 프리뷰용 소스가 비어 있으면 안전하게 확보
        if (previewAudioSource == null)
        {
            var go = GameObject.Find("AudioSource"); // 기존에 쓰던 이름이 있으면 먼저 시도
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

        // Mixer 라우팅
        if (bgmGroup != null && previewAudioSource != null)
            previewAudioSource.outputAudioMixerGroup = bgmGroup;
    }

    public void setSongSelector(Song song)
    {
        // 커버 이미지 로드 (기존 방식 유지)
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

    private void Start()
    {
        if (StartBtn != null)
            StartBtn.onClick.AddListener(OnStartBtnClicked);
    }

    void Update()
    {
        // 현재 선택곡과 같을 때만 Start 버튼 노출
        if (StartBtn != null)
            StartBtn.gameObject.SetActive(songCode == MusicSelect.selectedSong);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MusicSelect.selectedSong = songCode;
        PlayPreviewFromFile(songFileName); // 프리뷰 재생(아래에서 Resources 우선)

        if (MusicSelect.instance != null)
            MusicSelect.instance.SetSelectedSongImmediate(songCode);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // 필요하면 포인터 업 시점 처리
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

    // === 빌드 안전: Resources 우선 → 실패 시 기존 절대경로(에디터 폴백) ===
    public void PlayPreviewFromFile(string fileName)
    {
        StopPreview(); // 중복 재생 방지

        var nameNoExt = Path.GetFileNameWithoutExtension(fileName);
        var resClip = Resources.Load<AudioClip>($"PreviewAudio/{nameNoExt}");
        if (resClip != null)
        {
            previewLoopRoutine = StartCoroutine(LoopPreviewWithFadeOut(resClip));
            return; // Resources에 있으면 여기서 끝
        }

        // 폴백(주로 에디터에서만): Assets/Resources/PreviewAudio/<fileName>
        string absPath = Path.Combine(Application.dataPath, "Resources/PreviewAudio/" + fileName);
        if (!File.Exists(absPath))
        {
            Debug.LogWarning("미리듣기 파일 없음: " + absPath);
            return;
        }
        StartCoroutine(PlayAudioFromAbsolutePath(absPath));
    }

    private IEnumerator PlayAudioFromAbsolutePath(string filePath)
    {
        // 확장자에 맞춰 AudioType 추정(간단 버전)
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
                Debug.LogError("오디오 파일 불러오기 실패: " + www.error);
            }
            else
            {
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                previewLoopRoutine = StartCoroutine(LoopPreviewWithFadeOut(clip));
            }
        }
    }

    private IEnumerator LoopPreviewWithFadeOut(AudioClip clip)
    {
        while (true)
        {
            if (previewAudioSource == null) yield break;

            previewAudioSource.Stop();
            previewAudioSource.clip = clip;
            previewAudioSource.volume = 1f;
            previewAudioSource.time = 0f;
            previewAudioSource.Play();

            // 끝에서 1초는 페이드아웃
            yield return new WaitForSeconds(Mathf.Max(0.1f, clip.length - 1f));

            float fadeDuration = 1f;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                if (previewAudioSource == null) yield break;
                previewAudioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
            if (previewAudioSource == null) yield break;
            previewAudioSource.Stop();
            previewAudioSource.volume = 1f;
        }
    }

    private void OnDisable() => StopPreview();
    private void OnDestroy() => StopPreview();
}
