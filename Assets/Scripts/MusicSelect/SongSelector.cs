using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Audio; // ★ Mixer 사용

public class SongSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image miniCover;
    [SerializeField] private TextMeshProUGUI miniName;
    [SerializeField] private Button StartBtn;
    [SerializeField] private AudioSource previewAudioSource;
    private Coroutine previewLoopRoutine;

    // Mixer 라우팅
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup bgmGroup;
    
    private static int s_GlobalPreviewToken = 0;

    private uint songCode;
    public string songFileName;

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
        songFileName = song.getSongFileName();
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

    public void OnPointerUp(PointerEventData eventData) { }

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
    
    public void PlayPreviewFromFile(string fileName)
    {
        StopPreview(); // 내 코루틴 중지

        // 오디오소스 확보
        if (previewAudioSource == null)
        {
            var go = GameObject.Find("AudioSource"); // 기존 오브젝트 시도
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
        if (bgmGroup != null && previewAudioSource.outputAudioMixerGroup != bgmGroup)
            previewAudioSource.outputAudioMixerGroup = bgmGroup;
        
        int myToken = ++s_GlobalPreviewToken;

        // 1) Resources/PreviewAudio/<name>
        var nameNoExt = Path.GetFileNameWithoutExtension(fileName);
        var resClip = Resources.Load<AudioClip>($"PreviewAudio/{nameNoExt}");
        if (resClip != null)
        {
            previewLoopRoutine = StartCoroutine(LoopPreviewWithFadeOut(resClip, myToken));
            return;
        }

        // 2) (에디터용) Assets/Resources/PreviewAudio/<file>
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
            if (myToken != s_GlobalPreviewToken) yield break;

            AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
            previewLoopRoutine = StartCoroutine(LoopPreviewWithFadeOut(clip, myToken));
        }
    }

    private IEnumerator LoopPreviewWithFadeOut(AudioClip clip, int myToken)
    {
        while (true)
        {
            if (myToken != s_GlobalPreviewToken) yield break;
            if (previewAudioSource == null) yield break;

            previewAudioSource.Stop();
            previewAudioSource.clip = clip;
            previewAudioSource.volume = 1f;
            previewAudioSource.time = 0f;
            previewAudioSource.Play();

            float playFor = Mathf.Max(0.1f, clip.length - 1f);
            float t = 0f;
            while (t < playFor)
            {
                if (myToken != s_GlobalPreviewToken) yield break;
                t += Time.deltaTime;
                yield return null;
            }

            float fadeDuration = 1f;
            t = 0f;
            while (t < fadeDuration)
            {
                if (myToken != s_GlobalPreviewToken) yield break;
                t += Time.deltaTime;
                previewAudioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }

            if (myToken != s_GlobalPreviewToken) yield break;

            previewAudioSource.Stop();
            previewAudioSource.volume = 1f;
        }
    }
}
