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
    public string songFileName;

    private void Awake()
    {
        if (previewAudioSource == null)
        {
            previewAudioSource = GameObject.Find("AudioSource")?.GetComponent<AudioSource>();
        }
    }

    private uint songCode;

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

        // 곡의 실제 파일명 저장
        songFileName = song.getSongFileName();
    }

    void Update()
    {
        if (songCode != MusicSelect.selectedSong) StartBtn.gameObject.SetActive(false);
        else StartBtn.gameObject.SetActive(true);
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

    // 파일에서 WAV를 직접 불러와 AudioSource로 재생
    public void PlayPreviewFromFile(string fileName)
    {
        StopPreview();
        
        string filePath = Application.dataPath + "/Resources/PreviewAudio/" + fileName;
        if (!File.Exists(filePath))
        {
            Debug.LogWarning("[프리뷰 에러] 파일이 없습니다: " + filePath);
            return;
        }

        StartCoroutine(PlayAudioFromFile(filePath));
    }

    // WAV 파일 로드 및 재생
    private IEnumerator PlayAudioFromFile(string filePath)
    {
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.WAV))
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

    
    // 반복 프리뷰 + 페이드아웃 루프
    private IEnumerator LoopPreviewWithFadeOut(AudioClip clip)
    {
        while (true)
        {
            previewAudioSource.Stop();
            previewAudioSource.clip = clip;
            previewAudioSource.volume = 1f;
            previewAudioSource.time = 0f;
            previewAudioSource.Play();

            yield return new WaitForSeconds(clip.length - 1f);

            float fadeDuration = 1f;
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                previewAudioSource.volume = Mathf.Lerp(1f, 0f, t / fadeDuration);
                yield return null;
            }
            previewAudioSource.Stop();
            previewAudioSource.volume = 1f;
        }
    }
}
