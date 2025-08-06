using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    // 인트로(곡 정보) UI
    [SerializeField] private CanvasGroup introPanel;               
    [SerializeField] private Image introCoverImage;                
    [SerializeField] private TextMeshProUGUI introSongNameText;    
    [SerializeField] private TextMeshProUGUI introSongArtistText;  

    // 본 게임 UI               
    [SerializeField] private Image gameCoverImage;                 
    [SerializeField] private TextMeshProUGUI gameSongNameText;     


    private List<Song> songs = new();

    void Start()
    {
        Song.songCount = 0;

        // Slist에서 Song 읽기
        var filePath = Application.dataPath + "/Data/Slist";
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line) || line[0] == '#') continue;
                    string[] lines = line.Split(" ");
                    if (lines.Length < 5) continue;
                    Song song = new Song(
                        lines[0],
                        lines[1],
                        lines[2],
                        lines[4] == "-" ? "dummy.png" : lines[4]
                    );
                    songs.Add(song);
                }
            }
        }
        else
        {
            Debug.LogError("Slist 파일을 찾을 수 없습니다.");
            return;
        }

        // 선택 곡 ID 읽기
        int selectedSongId = PlayerPrefs.GetInt("selectedSong", 0);
        Song selectedSong = songs.Find(s => s.getSongID() == (uint)selectedSongId);
        if (selectedSong == null)
        {
            Debug.LogError("선택된 곡을 찾을 수 없습니다. (ID: " + selectedSongId + ")");
            return;
        }

        // 커버 이미지
        string coverPath = Application.dataPath + "/Images/Cover/" + selectedSong.getsongCover();
        Sprite songSprite = null;
        if (File.Exists(coverPath))
        {
            byte[] fileData = File.ReadAllBytes(coverPath);
            Texture2D texture = new Texture2D(500, 500);
            if (texture.LoadImage(fileData))
            {
                songSprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                introCoverImage.sprite = songSprite;
                if (gameCoverImage != null) gameCoverImage.sprite = songSprite; // 본 게임 UI에도 커버 이미지 세팅
            }
        }

        // 곡 제목/아티스트
        introSongNameText.text = selectedSong.getsongName();
        introSongArtistText.text = selectedSong.getsongArtist();
        if (gameSongNameText != null) gameSongNameText.text = selectedSong.getsongName();
        

        // UI 전환
        introPanel.alpha = 1f;
        introPanel.gameObject.SetActive(true);

        StartCoroutine(IntroFadeAndSwitch());
    }

    IEnumerator IntroFadeAndSwitch()
    {
        float duration = 5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            introPanel.alpha = Mathf.Lerp(1f, 0f, t / duration);
            yield return null;
        }
        introPanel.alpha = 0f;
        introPanel.gameObject.SetActive(false); // 인트로 숨김

        StartGame();
    }

    void StartGame()
    {
        
        
        
    }
}
