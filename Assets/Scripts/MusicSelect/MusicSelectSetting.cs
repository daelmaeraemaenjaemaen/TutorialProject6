using UnityEngine;
using System.Collections;
using TMPro;

public class MusicSelectSetting : MonoBehaviour
{
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject SettingUI;
    private bool isSetting = false;
    private bool isMenu = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuTurn(isMenu);
    }
    
    
    void MenuTurn(bool turn)
    {
        isMenu = !isMenu;
        MenuUI.SetActive(turn);
    }

    void SettingTurn(bool turn)
    {
        isSetting = !isSetting;
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
    }

    public void Setting()
    {
        SettingTurn(!isSetting);
    }
}
