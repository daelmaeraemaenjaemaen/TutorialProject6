using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MusicSelectSetting : MonoBehaviour
{
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject Bgset;
    [SerializeField] private GameObject Gpset;
    [SerializeField] private GameObject Adset;
    [SerializeField] private GameObject Gimmick;
    [SerializeField] private Toggle gimmickToggle;
    
    private bool isSetting = false;
    private bool isMenu = false;
    private bool bgset = true;
    private bool gpset = false;
    private bool adset = false;
    private bool isGimmick = false;
    
    public static bool noGimmick = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerSettings.Load();
        
        if (gimmickToggle)
        {
            gimmickToggle.onValueChanged.RemoveAllListeners();
            gimmickToggle.SetIsOnWithoutNotify(PlayerSettings.noGimmick);
            gimmickToggle.onValueChanged.AddListener(NoGimmickChanged);
        }
        
        MenuTurn(false);
        SettingTurn(false);

        noGimmick = PlayerSettings.noGimmick;
        
        if (!noGimmick)
        {
            Gimmick.SetActive(true);
            isGimmick = true;
        }
        
        else
        {
            Gimmick.SetActive(false);
            isGimmick = false;
        }
        
        RefreshUI();
    }
    
    void RefreshUI()
    {
        // 기믹 다시 보지 않기 토글
        if (gimmickToggle)
            gimmickToggle.SetIsOnWithoutNotify(PlayerSettings.noGimmick);
        
        // 메뉴, 설정
        if (MenuUI)    MenuUI.SetActive(isMenu);
        if (SettingUI) SettingUI.SetActive(isSetting);
        
        // 설정 하위
        if (Bgset) Bgset.SetActive(isSetting && bgset);
        if (Gpset) Gpset.SetActive(isSetting && gpset);
        if (Adset) Adset.SetActive(isSetting && adset);
        
        // 기믹 오버레이
        if (Gimmick) Gimmick.SetActive(isGimmick);
    }
    
    void OnDisable()
    {
        if (gimmickToggle)
            gimmickToggle.onValueChanged.RemoveListener(NoGimmickChanged);
        
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
    }
    
    void NoGimmickChanged(bool on)
    {
        PlayerSettings.noGimmick = on;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
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
            if (isGimmick)
            {
                Gimmick.SetActive(false);
                isGimmick = false;
                
                RefreshUI();
            }

            else
            {
                if (!isSetting)
                {
                    MenuTurn(!isMenu);
                    
                    RefreshUI();
                }

                else
                {
                    SettingTurn(!isSetting);
                    
                    RefreshUI();
                }
            }
        }
        
        if (isSetting)
        {
            if (bgset)
            {
                Bgset.SetActive(true);
                Gpset.SetActive(false);
                Adset.SetActive(false);
                
                RefreshUI();
            }
            
            else if (gpset)
            {
                Bgset.SetActive(false);
                Gpset.SetActive(true);
                Adset.SetActive(false);
                
                RefreshUI();
            }
            
            else if (adset)
            {
                Bgset.SetActive(false);
                Gpset.SetActive(false);
                Adset.SetActive(true);
                
                RefreshUI();
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

    public void OnGimmick()
    {
        Gimmick.SetActive(true);
        isGimmick = true;
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
