using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class ButtonSetting : MonoBehaviour
{
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject Bgset;
    [SerializeField] private GameObject Gpset;
    [SerializeField] private GameObject Adset;
    
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    
    [SerializeField] private AudioSource audioSource;
    
    private bool isSetting = false;
    private bool bgset = true;
    private bool gpset = false;
    private bool adset = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SettingTurn(false);
        
        if (bgmGroup != null) audioSource.outputAudioMixerGroup = bgmGroup;
        audioSource.loop = true;
        audioSource.Play();
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
            if (isSetting == true)
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

    public void Play()
    {
        SceneManager.LoadScene("2_MusicSelect");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
