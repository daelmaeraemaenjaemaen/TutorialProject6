using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.IO;
using System.Collections.Generic;

public class MusicSelect : MonoBehaviour
{
    public Image cover;
    public TextMeshProUGUI songName;
    public TextMeshProUGUI songArtist;
    public ScrollRect songScroll;
    [SerializeField] private Transform scrollContent;
    [SerializeField] private GameObject songSelector;
    public ScrollRect category;

    private List<Song> songs = new();
    private uint _selectedSong;
    public static uint selectedSong;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Song 리스트 불러오기
        var filePath = Application.dataPath + "/Data/Slist";
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line[0] == '#') continue;
                    string[] lines = line.Split(" ");
                    Song song = new Song(lines[0], lines[1], lines[2], lines[4] == "-" ? "dummy.png" : lines[4]);
                    songs.Add(song);
                }
            }
            Debug.Log("SongCount = " + Song.songCount.ToString());
        }
        else
        {
            //Slist 파일 찾지 못함
            Debug.Log("SongList file not found");
            Application.Quit();
        }

        for (int i = 0; i < Song.songCount; i++)
        {
            var item = Instantiate(songSelector);
            item.GetComponent<SongSelector>().setSongSelector(songs[i]);
            item.transform.SetParent(scrollContent);
            item.transform.localScale = Vector2.one;
        }
        selectedSong = 0;
        _selectedSong = 0;
        setSelectedSong(selectedSong);
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedSong != _selectedSong) setSelectedSong(selectedSong);
    }

    private void setSelectedSong(uint ID)
    {
        Song song = songs.Find(s => s.getSongID() == ID);

        byte[] fileData = File.ReadAllBytes(Application.dataPath + "/Images/Cover/" + song.getsongCover());
        Texture2D texture = new Texture2D(1920, 1080); //1920x1080 
        if (texture.LoadImage(fileData)) //텍스처에 로딩하기
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            cover.sprite = sprite;
        }

        songName.text = song.getsongName();
        songArtist.text = song.getsongArtist();
        _selectedSong = selectedSong;
    }
}
