using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

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
    [SerializeField] private AudioSource musicAudioSource; // Inspector에서 연결

    [Header("패턴")]
    [SerializeField] private Metronome metronome;

    private List<Song> songs = new();
    private Song selectedSong;

    void Start()
    {
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

        // 2. 선택 곡 ID 찾기
        uint selectedSongID = (uint)PlayerPrefs.GetInt("selectedSongID", -1);
        selectedSong = songs.Find(s => s.getSongID() == selectedSongID);
        if (selectedSong == null)
        {
            Debug.LogError($"선택된 곡을 찾을 수 없습니다. (ID: {selectedSongID})");
            return;
        }

        // 3. 커버 이미지 적용
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

        // 4. 곡 정보 UI 적용
        introSongNameText.text = selectedSong.getsongName();
        introSongArtistText.text = selectedSong.getsongArtist();
        if (gameSongNameText != null) gameSongNameText.text = selectedSong.getsongName();

        // 5. 인트로 패널 활성화 & 페이드 시작
        introPanel.alpha = 1f;
        introPanel.gameObject.SetActive(true);
        
        StartCoroutine(IntroFadeAndStartMusic());
    }

    IEnumerator IntroFadeAndStartMusic()
    {
        metronome.setPlayData(selectedSong, true); //TODO: 난이도 선택 구현 이후 선택한 난이도에 맞게 수정 필요
        yield return new WaitForSeconds(2f);
        
        float duration = 0.5f;
        float t = 0f;

        // 인트로 페이드 아웃
        while (t < duration)
        {
            t += Time.deltaTime;
            introPanel.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }
        introPanel.alpha = 0f;
        introPanel.gameObject.SetActive(false);

        yield return new WaitForSeconds(1f);
            
        PlaySelectedSong();
    }

    void PlaySelectedSong()
    {
        string songFileName = selectedSong.getSongFileName();
        string filePath = Application.dataPath + "/Resources/Audio/" + songFileName;
        if (!File.Exists(filePath))
        {
            Debug.LogError("오디오 파일이 없습니다: " + filePath);
            return;
        }

        //TODO: 싱크 조절 기능을 넣을 시, 아래 두 이벤트의 간격을 조절해 구현합니다
        metronome.StartPlay();
        StartCoroutine(PlayWavAndGotoNextScene(filePath));
    }
    
    private IEnumerator PlayWavAndGotoNextScene(string filePath)
    {
        using (var www = UnityWebRequestMultimedia.GetAudioClip("file:///" + filePath, AudioType.WAV))
        {
            yield return www.SendWebRequest();

#if UNITY_2020_2_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("PlayWavAndGotoNextScene 실패: " + www.error);
            }
            else
            {
                var clip = DownloadHandlerAudioClip.GetContent(www);
                musicAudioSource.clip = clip;
                musicAudioSource.volume = 1f;
                musicAudioSource.time = 0f;
                musicAudioSource.Play();

                yield return new WaitForSeconds(clip.length);

                yield return new WaitForSeconds(1f);
                
                SceneManager.LoadScene("Result");
            }
        }
    }
}
