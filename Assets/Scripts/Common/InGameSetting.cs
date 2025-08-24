using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InGameSetting : MonoBehaviour
{
    [Header("Key labels (버튼 안 TMP_Text)")]
    [SerializeField] TMP_Text p1k1Text, p1k2Text, p1k3Text;
    [SerializeField] TMP_Text p2k1Text, p2k2Text, p2k3Text;

    [Header("Sync (sec)")]
    [SerializeField] Slider syncSlider;      // Min=-0.5, Max=0.5
    [SerializeField] TMP_Text syncValueText;
    [SerializeField] Button syncDownBtn;     // -0.01
    [SerializeField] Button syncUpBtn;       // +0.01

    [Header("Speed")]
    [SerializeField] Slider speedSlider;     // Min=1, Max=10
    [SerializeField] TMP_Text speedValueText;
    [SerializeField] Button speedDownBtn;    // -0.1
    [SerializeField] Button speedUpBtn;      // +0.1

    bool listening = false;
    System.Action<KeyCode> onKeyBound;

    void OnEnable()
    {
        PlayerSettings.Load();

        if (syncSlider != null) syncSlider.wholeNumbers = false;
        if (speedSlider != null) speedSlider.wholeNumbers = false;

        RefreshUI();

        syncSlider.onValueChanged.RemoveAllListeners();
        speedSlider.onValueChanged.RemoveAllListeners();
        syncDownBtn.onClick.RemoveAllListeners();
        syncUpBtn.onClick.RemoveAllListeners();
        speedDownBtn.onClick.RemoveAllListeners();
        speedUpBtn.onClick.RemoveAllListeners();

        syncSlider.onValueChanged.AddListener(OnSyncChanged);
        speedSlider.onValueChanged.AddListener(OnSpeedChanged);

        syncDownBtn.onClick.AddListener(() => {
            syncSlider.value = Round(syncSlider.value - 0.01f, 2);
            OnSyncChanged(syncSlider.value);
        });
        syncUpBtn.onClick.AddListener(() => {
            syncSlider.value = Round(syncSlider.value + 0.01f, 2);
            OnSyncChanged(syncSlider.value);
        });

        speedDownBtn.onClick.AddListener(() => {
            speedSlider.value = Round(speedSlider.value - 0.1f, 1);
            OnSpeedChanged(speedSlider.value);
        });
        speedUpBtn.onClick.AddListener(() => {
            speedSlider.value = Round(speedSlider.value + 0.1f, 1);
            OnSpeedChanged(speedSlider.value);
        });
    }

    void OnDisable()
    {
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
    }

    void RefreshUI()
    {
        p1k1Text.text = Label(PlayerSettings.p1k1);
        p1k2Text.text = Label(PlayerSettings.p1k2);
        p1k3Text.text = Label(PlayerSettings.p1k3);
        p2k1Text.text = Label(PlayerSettings.p2k1);
        p2k2Text.text = Label(PlayerSettings.p2k2);
        p2k3Text.text = Label(PlayerSettings.p2k3);

        syncSlider.SetValueWithoutNotify(PlayerSettings.syncSec);
        syncValueText.text = $"{PlayerSettings.syncSec:0.00}";
        speedSlider.SetValueWithoutNotify(PlayerSettings.velocity);
        speedValueText.text = $"{PlayerSettings.velocity:0.0}";
    }

    public void BindP1K1() => StartBind(k => { ClearDuplicate(k); PlayerSettings.p1k1 = k; p1k1Text.text = Label(k); });
    public void BindP1K2() => StartBind(k => { ClearDuplicate(k); PlayerSettings.p1k2 = k; p1k2Text.text = Label(k); });
    public void BindP1K3() => StartBind(k => { ClearDuplicate(k); PlayerSettings.p1k3 = k; p1k3Text.text = Label(k); });
    public void BindP2K1() => StartBind(k => { ClearDuplicate(k); PlayerSettings.p2k1 = k; p2k1Text.text = Label(k); });
    public void BindP2K2() => StartBind(k => { ClearDuplicate(k); PlayerSettings.p2k2 = k; p2k2Text.text = Label(k); });
    public void BindP2K3() => StartBind(k => { ClearDuplicate(k); PlayerSettings.p2k3 = k; p2k3Text.text = Label(k); });

    void StartBind(System.Action<KeyCode> onBound)
    {
        if (listening) return;
        listening = true;
        onKeyBound = onBound;
        StartCoroutine(ListenAnyKey());
    }

    IEnumerator ListenAnyKey()
    {
        yield return null;
        while (listening)
        {
            var k = DetectKey();
            if (k != KeyCode.None)
            {
                onKeyBound?.Invoke(k);
                listening = false;
                PlayerSettings.Save();
                PlayerSettings.ApplyToRuntime();
                break;
            }
            yield return null;
        }
    }

    static KeyCode DetectKey()
    {
        foreach (KeyCode k in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (k >= KeyCode.Mouse0 && k <= KeyCode.Mouse6) continue; // 마우스 무시
            if (Input.GetKeyDown(k)) return k;
        }
        return KeyCode.None;
    }
    
    void ClearDuplicate(KeyCode k)
    {
        if (k == KeyCode.None) return;

        if (PlayerSettings.p1k1 == k) { PlayerSettings.p1k1 = KeyCode.None; if (p1k1Text) p1k1Text.text = ""; }
        if (PlayerSettings.p1k2 == k) { PlayerSettings.p1k2 = KeyCode.None; if (p1k2Text) p1k2Text.text = ""; }
        if (PlayerSettings.p1k3 == k) { PlayerSettings.p1k3 = KeyCode.None; if (p1k3Text) p1k3Text.text = ""; }
        if (PlayerSettings.p2k1 == k) { PlayerSettings.p2k1 = KeyCode.None; if (p2k1Text) p2k1Text.text = ""; }
        if (PlayerSettings.p2k2 == k) { PlayerSettings.p2k2 = KeyCode.None; if (p2k2Text) p2k2Text.text = ""; }
        if (PlayerSettings.p2k3 == k) { PlayerSettings.p2k3 = KeyCode.None; if (p2k3Text) p2k3Text.text = ""; }
    }

    static string Label(KeyCode k)
    {
        if (k == KeyCode.None) return ""; // 비워진 슬롯은 공백 표시

        switch (k)
        {
            case KeyCode.LeftArrow:  return "←";
            case KeyCode.RightArrow: return "→";
            case KeyCode.UpArrow:    return "↑";
            case KeyCode.DownArrow:  return "↓";
        }
        string s = k.ToString();
        if (s.Length == 1) return s.ToUpper();
        if (s.StartsWith("Alpha")) return s.Substring(5);
        if (k == KeyCode.Space) return "SPACE";
        if (k == KeyCode.LeftShift || k == KeyCode.RightShift) return "SHIFT";
        if (k == KeyCode.LeftControl || k == KeyCode.RightControl) return "CTRL";
        if (k == KeyCode.LeftAlt || k == KeyCode.RightAlt) return "ALT";
        return s.ToUpper();
    }

    void OnSyncChanged(float v)
    {
        PlayerSettings.syncSec = Mathf.Clamp(Round(v, 2), -0.50f, 0.50f);
        syncValueText.text = $"{PlayerSettings.syncSec:0.00}";
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
    }

    void OnSpeedChanged(float v)
    {
        PlayerSettings.velocity = Mathf.Clamp(Round(v, 1), 1f, 10f);
        speedValueText.text = $"{PlayerSettings.velocity:0.0}";
        PlayerSettings.Save();
        PlayerSettings.ApplyToRuntime();
    }

    static float Round(float v, int decimals)
    {
        float m = Mathf.Pow(10f, decimals);
        return Mathf.Round(v * m) / m;
    }
}
