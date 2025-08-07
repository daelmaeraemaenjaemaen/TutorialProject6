using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.IO;
using System.Collections.Generic;

public class MusicSelect : MonoBehaviour
{
    public static MusicSelect instance;
    
    //Unity Inspector에서 연결하는 객체
    public Image cover;
    public TextMeshProUGUI songName;
    public TextMeshProUGUI songArtist;
    public ScrollRect songScroll;
    [SerializeField] private Transform songContent;
    [SerializeField] private Transform categoryContent;
    [SerializeField] private GameObject songSelector;
    [SerializeField] private GameObject categorySelector;
    public ScrollRect category;

    //그 외 내부 변수
    private List<Song> songs = new();
    private List<Category> categories = new();
    private uint _selectedSong;
    public static uint selectedSong;
    private string _selectedCategory;
    public static string selectedCategory;

    void Awake()
    {
        instance = this;
    }
    
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

        //Category 리스트 불러오기
        filePath = Application.dataPath + "/Data/Clist";
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line[0] == '#') continue;
                    string[] lines = line.Split(" ");
                    Category c = new Category(lines[0], lines[1]);
                    categories.Add(c);
                }
            }
        }
        else
        {
            //Clist 파일 찾지 못함
            Debug.Log("CategoryList file not found");
            Application.Quit();
        }

        //Category ScrollView 초기화 및 구성(일회성이라 분리하지 않았음)
        for (int i = 0; i < categories.Count; i++)
        {
            var item = Instantiate(categorySelector);
            item.GetComponent<CategorySelector>().setCategorySelector(categories[i]);
            item.transform.SetParent(categoryContent);
            item.transform.localScale = Vector2.one;
        }
        selectedCategory = "all";
        _selectedCategory = "all";

        setSongList();
    }

    // Update is called once per frame
    void Update()
    {
        //static 설정한 두 변수를 변경하면 관련 이벤트가 발생
        if (selectedSong != _selectedSong) setSelectedSong(selectedSong);
        if (selectedCategory != _selectedCategory)
        {
            setSongList();
            _selectedCategory = selectedCategory;
        }
    }

    //Song ScrollView 초기화 및 구성
    private void setSongList()
    {
        //초기화
        for (int i = 0; i < songContent.childCount; i++)
        {
            Destroy(songContent.GetChild(i).gameObject);
        }

        int firstSong = -1; //정렬된 곡 리스트의 첫 곡
        GameObject firstSongSelectorObj = null;
        
        //구성
        for (int i = 0; i < Song.songCount; i++)
        {
            if (!selectedCategory.Equals("all") && !selectedCategory.Equals(songs[i].getSongCategory())) continue;
            var item = Instantiate(songSelector);
            item.GetComponent<SongSelector>().setSongSelector(songs[i]);
            item.transform.SetParent(songContent);
            item.transform.localScale = Vector2.one;
            if (firstSong == -1)
            {
                firstSong = (int)songs[i].getSongID();
                firstSongSelectorObj = item;
            }
        }

        //카테고리에 곡이 없을 시 예외 처리
        if (firstSong == -1)
        {
            //TODO: 오류 및 all 복귀 메시지 송출
            if (selectedCategory == "all")
            {
                Debug.Log("There is no song data");
                Application.Quit();
            }
            else selectedCategory = "all";
        }
        else
        {
            selectedSong = (uint)firstSong;
            _selectedSong = (uint)firstSong;
            setSelectedSong(selectedSong);
            
            if (firstSongSelectorObj != null)
            {
                var selector = firstSongSelectorObj.GetComponent<SongSelector>();
                selector.PlayPreviewFromFile(selector.songFileName);
            }
        }
    }

    //곡 선택 시 정보 표시
    private void setSelectedSong(uint ID)
    {
        Song song = songs.Find(s => s.getSongID() == ID);

        byte[] fileData = File.ReadAllBytes(Application.dataPath + "/Images/Cover/" + song.getsongCover());
        Texture2D texture = new Texture2D(500, 500);
        if (texture.LoadImage(fileData))
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            cover.sprite = sprite;
        }

        songName.text = song.getsongName();
        songArtist.text = song.getsongArtist();
        _selectedSong = selectedSong;
    }
    
    public void SetSelectedSongImmediate(uint ID)
    {
        setSelectedSong(ID); // 기존 곡 정보 갱신 함수
        _selectedSong = selectedSong = ID; // 내부 상태까지 동기화
    }
}
