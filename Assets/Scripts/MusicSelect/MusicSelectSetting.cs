using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicSelectSetting : MonoBehaviour
{
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject Bgset;
    [SerializeField] private GameObject Gpset;
    [SerializeField] private GameObject Adset;
    
    private bool isSetting = false;
    private bool isMenu = false;
    private bool bgset = true;
    private bool gpset = false;
    private bool adset = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuTurn(false);
        SettingTurn(false);
    }
    
    
    void MenuTurn(bool turn)
    {
        isMenu = turn;
        MenuUI.SetActive(turn);
    }

    void SettingTurn(bool turn)
    {
        isSetting = turn;
        SettingUI.SetActive(turn);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isSetting)
            {
                MenuTurn(!isMenu);
            }

            else
            {
                SettingTurn(!isSetting);
            }
        }
        
        if (isSetting)
        {
            if (bgset)
            {
                Bgset.SetActive(true);
                Gpset.SetActive(false);
                Adset.SetActive(false);
            }
            
            else if (gpset)
            {
                Bgset.SetActive(false);
                Gpset.SetActive(true);
                Adset.SetActive(false);
            }
            
            else if (adset)
            {
                Bgset.SetActive(false);
                Gpset.SetActive(false);
                Adset.SetActive(true);
            }
        }
    }

    public void Setting()
    {
        SettingTurn(!isSetting);
    }

    public void Quit()
    {
        SceneManager.LoadScene("1_Main");
    }
    
    public void BackGroundSet()
    {
        bgset = true;
        gpset = false;
        adset = false;
    }

    public void GameplaySet()
    {
        bgset = false;
        gpset = true;
        adset = false;
    }

    public void AudioSet()
    {
        bgset = false;
        gpset = false;
        adset = true;
    }
}
