using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameUIManager : MonoBehaviour
{
    [Header("인트로(곡 정보) UI")]
    [SerializeField] private CanvasGroup introPanel;
    [SerializeField] private Image introCoverImage;
    [SerializeField] private TextMeshProUGUI introSongNameText;
    [SerializeField] private TextMeshProUGUI introSongArtistText;

    [Header("본 게임 UI")]
    [SerializeField] private Image gameCoverImage;
    [SerializeField] private TextMeshProUGUI gameSongNameText;

    [Header("오디오")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioMixerGroup bgmGroup;

    [Header("패턴")]
    [SerializeField] private Metronome metronome;

    private List<Song> songs = new();
    private Song selectedSong;
    
    private string diff;
    private bool isEasy;
    
    public float length { get; private set; }
    
    const float INTRO_DELAY = 2f;
    const float FADE_DURATION = 0.5f;
    const float AFTER_FADE_DELAY = 1f;

    bool isFading;
    float fadeTimer;
    
    void Start()
    {
        // 0. 난이도
        diff = PlayerPrefs.GetString("SelectedDifficulty", "easy");
        isEasy = !diff.Equals("hard", System.StringComparison.OrdinalIgnoreCase);
        
        // 1. Slist에서 Song 읽기
        string filePath = Application.dataPath + "/Data/Slist";
        if (!File.Exists(filePath))
        {
            Debug.LogError("Slist 파일을 찾을 수 없습니다: " + filePath);
            return;
        }

        using (var reader = new StreamReader(filePath))
        {
            Song.resetSongCount();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line) || line[0] == '#') continue;
                string[] lines = line.Split("|");
                if (lines.Length < 5) continue;
                Song song = new(
                    lines[0],
                    lines[1],
                    lines[2],
                    lines[3],
                    lines[4],
                    lines[5] == "" ? 0 : float.Parse(lines[5]),
                    lines[6] == "" ? 0 : float.Parse(lines[6]),
                    lines[7],
                    lines[8]
                    );
                songs.Add(song);
            }
        }

        // 2. 선택 곡 & 곡 길이
        uint selectedSongID = (uint)PlayerPrefs.GetInt("selectedSongID", -1);
        selectedSong = songs.Find(s => s.getSongID() == selectedSongID);
        if (selectedSong == null)
        {
            Debug.LogError($"선택된 곡을 찾을 수 없습니다. (ID: {selectedSongID})");
            return;
        }

        length = selectedSong.getSongLength();

        // 3. 커버 이미지
        string coverPath = Application.dataPath + "/Images/Cover/" + selectedSong.getsongCover();
        if (File.Exists(coverPath))
        {
            byte[] fileData = File.ReadAllBytes(coverPath);
            Texture2D texture = new Texture2D(500, 500);
            if (texture.LoadImage(fileData))
            {
                Sprite songSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                introCoverImage.sprite = songSprite;
                if (gameCoverImage != null) gameCoverImage.sprite = songSprite;
            }
        }

        // 4. 곡 정보 UI
        introSongNameText.text = selectedSong.getsongName();
        introSongArtistText.text = selectedSong.getsongArtist();
        if (gameSongNameText != null) gameSongNameText.text = selectedSong.getsongName();

        // 5. 인트로 패널 & 페이드 시작
        introPanel.alpha = 1f;
        introPanel.gameObject.SetActive(true);
        
        if (bgmGroup != null && musicAudioSource != null)
            musicAudioSource.outputAudioMixerGroup = bgmGroup;
        

        StartIntroSequence();
    }

    void StartIntroSequence()
    {
        metronome.setPlayData(selectedSong, isEasy);
        // PlayerPrefs.GetString("selectedDiff", diff);

        introPanel.alpha = 1f;
        introPanel.gameObject.SetActive(true);

        Invoke(nameof(BeginIntroFade), INTRO_DELAY);
    }

    void BeginIntroFade()
    {
        isFading = true;
        fadeTimer = 0f;
    }

    void PlaySelectedSong()
    {
        string fileName = selectedSong.getSongFileName();
        string nameNoExt = Path.GetFileNameWithoutExtension(fileName);

        // 1) Resources/Audio
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{nameNoExt}");
        if (clip != null)
        {
            // metronome.StartPlay(); (싱크)
            PlayClipAndGotoNextScene(clip);
            return;
        } 
        
        // ReSharper disable once InvalidXmlDocComment
        /**
        // 2) StreamingAssets
        string saPath = Path.Combine(Application.streamingAssetsPath, "Audio/" + fileName);
        StartCoroutine(PlayFromFileAndGoto(saPath));
        **/
    }

    private void PlayClipAndGotoNextScene(AudioClip clip)
    {
        musicAudioSource.clip = clip;
        musicAudioSource.volume = 1f;
        musicAudioSource.time = 0f;

        // 오디오 렉 문제 해결?
        musicAudioSource.mute = true;
        musicAudioSource.Play();
        
        float offsetSec = PlayerSettings.syncSec + 10 / NoteMove.moveSpeed; // 배속에 따라 판정이 변경되지 않도록 수정

        if (offsetSec >= 0f)
        {
            metronome.StartPlay();
            Invoke(nameof(StartMusic), offsetSec);
        }
        else
        {
            StartMusic();
            Invoke(nameof(StartMetronome), -offsetSec);
        }
    }

    void StartMusic()
    {
        musicAudioSource.time = 0f;
        musicAudioSource.mute = false;
        monitoring = true; // 재생 종료 감시 시작
    }
    
    void StartMetronome()
    {
        metronome.StartPlay();
    }
    
    bool monitoring;
    void Update()
    {
        if (isFading)
        {
            fadeTimer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(fadeTimer / FADE_DURATION);
            introPanel.alpha = Mathf.Lerp(1f, 0f, t);

            if (fadeTimer >= FADE_DURATION)
            {
                isFading = false;
                introPanel.alpha = 0f;
                introPanel.gameObject.SetActive(false);
                
                Invoke(nameof(PlaySelectedSong), AFTER_FADE_DELAY);
            }
        }
        
        if (monitoring && !musicAudioSource.isPlaying && !GamePlaySetting.Paused)
        {
            bool ended =
                !musicAudioSource.isPlaying &&
                musicAudioSource.time >= (musicAudioSource.clip.length - 0.05f);
            
            if (ended)
            {
                monitoring = false;
                Invoke(nameof(GoResult), 1f);
            }
        }
    }
    
    void GoResult()
    {
        SceneManager.LoadScene("4_Result");
    }
    
    // ReSharper disable once InvalidXmlDocComment
    /**
    private IEnumerator PlayFromFileAndGoto(string fullPath)
    {
        // 확장자에 따른 타입 추정
        AudioType type = AudioType.WAV;
        string lower = fullPath.ToLowerInvariant();
        if (lower.EndsWith(".ogg")) type = AudioType.OGGVORBIS;
        else if (lower.EndsWith(".mp3")) type = AudioType.MPEG;

        using (var www = UnityWebRequestMultimedia.GetAudioClip(fullPath.StartsWith("file://") ? fullPath : "file:///" + fullPath, type))
        {
            yield return www.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("오디오 로드 실패: " + www.error);
                yield break;
            }
            var clip = DownloadHandlerAudioClip.GetContent(www);
            PlayClipAndGotoNextScene(clip);
        }
    }
    **/
}
