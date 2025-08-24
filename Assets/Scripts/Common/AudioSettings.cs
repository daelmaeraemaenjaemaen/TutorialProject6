using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSettings : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;  // GameMixer 할당

    [Header("Sliders (0~1)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Mute Toggles")]
    [SerializeField] private Toggle masterMute;
    [SerializeField] private Toggle bgmMute;
    [SerializeField] private Toggle sfxMute;
    
    [Header("오디오")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioClip click;

    // Exposed parameter names (AudioMixer)
    private const string MASTER = "MasterVol"; 
    private const string BGM    = "BGMVol";
    private const string SFX    = "SFXVol";

    // 저장키
    private const string K_M = "vol_master";
    private const string K_B = "vol_bgm";
    private const string K_S = "vol_sfx";
    private const string K_MM = "mute_master";
    private const string K_MB = "mute_bgm";
    private const string K_MS = "mute_sfx";

    // 음소거 전에 기억해둘 dB 캐시
    private float cacheMasterDb = 0f;
    private float cacheBgmDb    = 0f;
    private float cacheSfxDb    = 0f;

    // dB 클램프 (AudioMixer가 -80dB이면 사실상 무음)
    private const float MIN_DB = -80f;
    private const float MAX_DB = 0f;

    void Awake()
    {
        // 리스너 연결
        masterSlider.onValueChanged.AddListener(OnMasterSlider);
        bgmSlider.onValueChanged.AddListener(OnBgmSlider);
        sfxSlider.onValueChanged.AddListener(OnSfxSlider);

        masterMute.onValueChanged.AddListener(OnMasterMute);
        bgmMute.onValueChanged.AddListener(OnBgmMute);
        sfxMute.onValueChanged.AddListener(OnSfxMute);
    }

    void Start()
    {
        if (sfxGroup != null && sfxAudioSource != null)
            sfxAudioSource.outputAudioMixerGroup = sfxGroup;
        
        Load();
        // 처음 적용
        ApplyAll();
    }

    private void ApplyAll()
    {
        // 슬라이더 → 믹서
        OnMasterSlider(masterSlider.value, applyEvenIfMuted: true);
        OnBgmSlider(bgmSlider.value, applyEvenIfMuted: true);
        OnSfxSlider(sfxSlider.value, applyEvenIfMuted: true);

        // 토글(뮤트) 적용
        OnMasterMute(masterMute.isOn);
        OnBgmMute(bgmMute.isOn);
        OnSfxMute(sfxMute.isOn);
    }

    // 0~1 → dB
    private static float LinearToDb(float v01)
    {
        if (v01 <= 0.0001f) return MIN_DB;
        return Mathf.Clamp(Mathf.Log10(v01) * 20f, MIN_DB, MAX_DB);
    }

    // dB → 0~1 (로드 시 유용)
    private static float DbToLinear(float db)
    {
        if (db <= MIN_DB) return 0f;
        return Mathf.Clamp01(Mathf.Pow(10f, db / 20f));
    }

    private void OnMasterSlider(float v) => OnMasterSlider(v, false);
    private void OnBgmSlider(float v)    => OnBgmSlider(v, false);
    private void OnSfxSlider(float v)    => OnSfxSlider(v, false);

    private void OnMasterSlider(float v, bool applyEvenIfMuted)
    {
        float db = LinearToDb(v);
        if (!masterMute.isOn || applyEvenIfMuted)
            mixer.SetFloat(MASTER, db);
        PlayerPrefs.SetFloat(K_M, db);
    }

    private void OnBgmSlider(float v, bool applyEvenIfMuted)
    {
        float db = LinearToDb(v);
        if (!bgmMute.isOn || applyEvenIfMuted)
            mixer.SetFloat(BGM, db);
        PlayerPrefs.SetFloat(K_B, db);
    }

    private void OnSfxSlider(float v, bool applyEvenIfMuted)
    {
        float db = LinearToDb(v);
        if (!sfxMute.isOn || applyEvenIfMuted)
            mixer.SetFloat(SFX, db);
        PlayerPrefs.SetFloat(K_S, db);
    }

    private void OnMasterMute(bool on)
    {
        if (on)
        {
            mixer.GetFloat(MASTER, out cacheMasterDb);
            mixer.SetFloat(MASTER, MIN_DB);
        }
        else
        {
            float db = PlayerPrefs.GetFloat(K_M, 0f);
            mixer.SetFloat(MASTER, db);
        }
        PlayerPrefs.SetInt(K_MM, on ? 1 : 0);
    }

    private void OnBgmMute(bool on)
    {
        if (on)
        {
            mixer.GetFloat(BGM, out cacheBgmDb);
            mixer.SetFloat(BGM, MIN_DB);
        }
        else
        {
            float db = PlayerPrefs.GetFloat(K_B, 0f);
            mixer.SetFloat(BGM, db);
        }
        PlayerPrefs.SetInt(K_MB, on ? 1 : 0);
    }

    private void OnSfxMute(bool on)
    {
        if (on)
        {
            mixer.GetFloat(SFX, out cacheSfxDb);
            mixer.SetFloat(SFX, MIN_DB);
        }
        else
        {
            float db = PlayerPrefs.GetFloat(K_S, 0f);
            mixer.SetFloat(SFX, db);
        }
        PlayerPrefs.SetInt(K_MS, on ? 1 : 0);
    }

    private void Load()
    {
        // 저장된 dB 불러오기 (기본값 0dB)
        float m = PlayerPrefs.GetFloat(K_M, 0f);
        float b = PlayerPrefs.GetFloat(K_B, 0f);
        float s = PlayerPrefs.GetFloat(K_S, 0f);

        // 슬라이더 보이는 값은 0~1로 환산해 채우기
        masterSlider.SetValueWithoutNotify(DbToLinear(m));
        bgmSlider.SetValueWithoutNotify(DbToLinear(b));
        sfxSlider.SetValueWithoutNotify(DbToLinear(s));

        // Mute 토글
        masterMute.SetIsOnWithoutNotify(PlayerPrefs.GetInt(K_MM, 0) == 1);
        bgmMute.SetIsOnWithoutNotify(PlayerPrefs.GetInt(K_MB, 0) == 1);
        sfxMute.SetIsOnWithoutNotify(PlayerPrefs.GetInt(K_MS, 0) == 1);
    }

    public void Click()
    {
        if (click != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(click, 1f);
    }
}
