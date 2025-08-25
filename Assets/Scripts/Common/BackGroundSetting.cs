using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BackGroundSetting : MonoBehaviour
{
    [Header("이미지")]
    [SerializeField] private Image shortNote;
    [SerializeField] private Image longNote;
    [SerializeField] private Image backGround;
    [SerializeField] private Image shortNote2;
    [SerializeField] private Image longNote2;
    [SerializeField] private Image backGround2;

    [Header("버튼")]
    [SerializeField] private Button shortLeft;
    [SerializeField] private Button shortRight;
    [SerializeField] private Button longLeft;
    [SerializeField] private Button longRight;
    [SerializeField] private Button backLeft;
    [SerializeField] private Button backRight;
    [SerializeField] private Button shortLeft2;
    [SerializeField] private Button longLeft2;
    [SerializeField] private Button backLeft2;
    [SerializeField] private Button shortRight2;
    [SerializeField] private Button longRight2;
    [SerializeField] private Button backRight2;
    
    [Header("스프라이트")]
    [SerializeField] private Sprite[] shortSkins = new Sprite[3];
    [SerializeField] private Sprite[] longSkins  = new Sprite[3];
    [SerializeField] private Sprite[] backSkins  = new Sprite[3];
    [SerializeField] private Sprite[] shortSkins2 = new Sprite[3];
    [SerializeField] private Sprite[] longSkins2  = new Sprite[3];
    [SerializeField] private Sprite[] backSkins2  = new Sprite[3];

    void Start()
    {
        PlayerSettings.Load();
        RefreshUI();

        shortLeft.onClick.RemoveAllListeners();
        shortRight.onClick.RemoveAllListeners();
        longLeft.onClick.RemoveAllListeners();
        longRight.onClick.RemoveAllListeners();
        backLeft.onClick.RemoveAllListeners();
        backRight.onClick.RemoveAllListeners();
        shortLeft2.onClick.RemoveAllListeners();
        shortRight2.onClick.RemoveAllListeners();
        longLeft2.onClick.RemoveAllListeners();
        longRight2.onClick.RemoveAllListeners();
        backLeft2.onClick.RemoveAllListeners();
        backRight2.onClick.RemoveAllListeners();

        shortLeft.onClick.AddListener(() =>
        {
            int len = shortSkins.Length;
            int v = (PlayerSettings.shortNum - 1 + len) % len;
            OnShort1Changed(v);
        });
        shortRight.onClick.AddListener(() =>
        {
            int len = shortSkins.Length;
            int v = (PlayerSettings.shortNum + 1) % len;
            OnShort1Changed(v);
        });

        longLeft.onClick.AddListener(() =>
        {
            int len = longSkins.Length;
            int v = (PlayerSettings.longNum - 1 + len) % len;
            OnLong1Changed(v);
        });
        longRight.onClick.AddListener(() =>
        {
            int len = longSkins.Length;
            int v = (PlayerSettings.longNum + 1) % len;
            OnLong1Changed(v);
        });

        backLeft.onClick.AddListener(() =>
        {
            int len = backSkins.Length;
            int v = (PlayerSettings.backNum - 1 + len) % len;
            OnBack1Changed(v);
        });
        backRight.onClick.AddListener(() =>
        {
            int len = backSkins.Length;
            int v = (PlayerSettings.backNum + 1) % len;
            OnBack1Changed(v);
        });

        shortLeft2.onClick.AddListener(() =>
        {
            int len = shortSkins2.Length;
            int v = (PlayerSettings.shortNum2 - 1 + len) % len;
            OnShort2Changed(v);
        });
        shortRight2.onClick.AddListener(() =>
        {
            int len = shortSkins2.Length;
            int v = (PlayerSettings.shortNum2 + 1) % len;
            OnShort2Changed(v);
        });

        longLeft2.onClick.AddListener(() =>
        {
            int len = longSkins2.Length;
            int v = (PlayerSettings.longNum2 - 1 + len) % len;
            OnLong2Changed(v);
        });
        longRight2.onClick.AddListener(() =>
        {
            int len = longSkins2.Length;
            int v = (PlayerSettings.longNum2 + 1) % len;
            OnLong2Changed(v);
        });

        backLeft2.onClick.AddListener(() =>
        {
            int len = backSkins2.Length;
            int v = (PlayerSettings.backNum2 - 1 + len) % len;
            OnBack2Changed(v);
        });
        backRight2.onClick.AddListener(() =>
        {
            int len = backSkins2.Length;
            int v = (PlayerSettings.backNum2 + 1) % len;
            OnBack2Changed(v);
        });
    }

    void OnDisable()
    {
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
    }
    
    void RefreshUI()
    {
        if (shortSkins.Length  > 0) shortNote.sprite  = shortSkins [Mathf.Clamp(PlayerSettings.shortNum,  0, shortSkins.Length  - 1)];
        if (longSkins.Length   > 0) longNote.sprite   = longSkins  [Mathf.Clamp(PlayerSettings.longNum,   0, longSkins.Length   - 1)];
        if (backSkins.Length   > 0) backGround.sprite = backSkins  [Mathf.Clamp(PlayerSettings.backNum,   0, backSkins.Length   - 1)];

        if (shortSkins2.Length > 0) shortNote2.sprite  = shortSkins2[Mathf.Clamp(PlayerSettings.shortNum2, 0, shortSkins2.Length - 1)];
        if (longSkins2.Length  > 0) longNote2.sprite   = longSkins2 [Mathf.Clamp(PlayerSettings.longNum2,  0, longSkins2.Length  - 1)];
        if (backSkins2.Length  > 0) backGround2.sprite = backSkins2 [Mathf.Clamp(PlayerSettings.backNum2,  0, backSkins2.Length  - 1)];
    }

    void OnShort1Changed(int v)
    {
        PlayerSettings.shortNum = v;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
    }

    void OnLong1Changed(int v)
    {
        PlayerSettings.longNum = v;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
    }
    
    void OnBack1Changed(int v)
    {
        PlayerSettings.backNum = v;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
    }
    
    void OnShort2Changed(int v)
    {
        PlayerSettings.shortNum2 = v;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
    }
    
    void OnLong2Changed(int v)
    {
        PlayerSettings.longNum2 = v;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
    }
    
    void OnBack2Changed(int v)
    {
        PlayerSettings.backNum2 = v;
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
        RefreshUI();
    }
}
