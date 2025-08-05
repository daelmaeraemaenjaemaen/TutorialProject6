using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

using System.IO;

using UnityEngine.SceneManagement;

public class SongSelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image miniCover;
    [SerializeField] private TextMeshProUGUI miniName;
    [SerializeField] private Button StartBtn;
    private uint songCode;

    public void setSongSelector(Song song)
    {
        byte[] fileData = File.ReadAllBytes(Application.dataPath + "/Images/Cover/" + song.getsongCover());
        Texture2D texture = new Texture2D(500, 500);
        if (texture.LoadImage(fileData))
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            miniCover.sprite = sprite;
        }

        miniName.text = song.getsongName();
        songCode = song.getSongID();
    }

    // Update is called once per frame
    void Update()
    {
        if (songCode != MusicSelect.selectedSong) StartBtn.gameObject.SetActive(false);
        else StartBtn.gameObject.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        MusicSelect.selectedSong = songCode;
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
        PlayerPrefs.SetInt("selectedSong", (int)songCode);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GamePlay");
    }
}