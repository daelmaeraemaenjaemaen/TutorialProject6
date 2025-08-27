using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlaySetting : MonoBehaviour
{
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject CountDown;
    [SerializeField] private TMP_Text CountDownText;
    [SerializeField] private GameObject Bgset;
    [SerializeField] private GameObject Gpset;
    [SerializeField] private GameObject Adset;

    [SerializeField] private GameObject barUI;
    [SerializeField] GameUIManager UI;
    private float length;
    private bool isStart = false;
    
    private bool isSetting = false;
    private bool isMenu = false;
    private bool intro = false;
    private bool bgset = true;
    private bool gpset = false;
    private bool adset = false;
    
    private bool isCountingDown = false;
    private Coroutine countdownRoutine = null;
    
    RectTransform rect;
    Coroutine moveRoutine;
    
    // 진행상태 저장
    Vector2 start, end;
    float duration;
    float t;
    bool isPaused = false;
    
    public static bool Paused { get; private set; }
    
    void Awake()
    {
        if (barUI == null)
        {
            Debug.LogWarning("[GamePlaySetting] barUI가 비어 있어요. 이름으로 탐색을 시도합니다.");
            var found = GameObject.Find("barUI");
            if (found != null) barUI = found;
        }
        if (barUI != null)
        {
            rect = barUI.GetComponent<RectTransform>();
        }
        else
        {
            Debug.LogError("[GamePlaySetting] barUI가 지정되지 않았습니다. 인스펙터에 할당하세요.");
        }
    }
    
    void Start()
    {
        intro = false;
        SettingUI.SetActive(false);
        MenuUI.SetActive(false);
        CountDownText.gameObject.SetActive(false);
        CountDown.SetActive(false);
        Time.timeScale = 1f;
        bgmSource.UnPause();
        barUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-950f, 590f);
        length = UI.length;
        isStart = false;
        PlayerSettings.Load();
        PlayerSettings.ApplyToRuntime();
        Paused = false;
        
        StartCoroutine(introDelay());
    }

    IEnumerator introDelay()
    {
        yield return new WaitForSecondsRealtime(3.5f);
        intro = true;
    }

    void SettingTurn(bool turn)
    {
        isSetting = turn;
        SettingUI.SetActive(turn);
    }

    IEnumerator ResumeWithCountdown()
    {
        isCountingDown = true;

        CountDownText.gameObject.SetActive(true);
        CountDown.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            CountDownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        CountDownText.gameObject.SetActive(false);
        CountDown.SetActive(false);

        Time.timeScale = 1f;
        bgmSource.UnPause();
        ResumeMove();

        isCountingDown = false;
        countdownRoutine = null;
    }

    void MenuTurn(bool turn)
    {
        isMenu = turn;
        MenuUI.SetActive(turn);

        if (turn)
        {
            Time.timeScale = 0f;
            bgmSource.Pause();
            PauseMove();

            if (countdownRoutine != null)
            {
                StopCoroutine(countdownRoutine);
                countdownRoutine = null;
            }
            CountDownText.gameObject.SetActive(false);
            CountDown.SetActive(false);
            isCountingDown = false;
        }
        else
        {
            // 메뉴 닫기
        }
    }

    void Update()
    {
        if (intro)
        {
            if (!isStart)
            {
                MoveUI(new Vector2(950f, 590f), length);
                isStart = true;
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!intro) return; // 인트로 중엔 무시

            if (!isMenu) // 메뉴 열기
            {
                MenuTurn(true);
            }
            else
            {
                if (isSetting) // 설정 닫기
                {
                    SettingTurn(false);
                }
                else
                {
                    // 메뉴 닫고 카운트다운 시작
                    MenuTurn(false);
                    if (!isCountingDown)
                        countdownRoutine = StartCoroutine(ResumeWithCountdown());
                }
            }
        }

        // 설정 탭 스위칭
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

    public void Quit()
    {
        CleanupBeforeSceneChange();
        SceneManager.LoadScene("2_MusicSelect");
    }

    public void Setting()
    {
        SettingTurn(true);
    }

    public void Restart()
    {
        CleanupBeforeSceneChange();
        SceneManager.LoadScene("3_GamePlay");
    }

    private void CleanupBeforeSceneChange()
    {
        Time.timeScale = 1f;
        if (bgmSource != null) bgmSource.UnPause();
        ResumeMove();

        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
        isCountingDown = false;

        CountDownText.gameObject.SetActive(false);
        CountDown.SetActive(false);
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
    
    public void MoveUI(Vector2 targetPos, float durationSec)
    {
        // 이동 세팅
        start = rect.anchoredPosition;
        end = targetPos;
        duration = durationSec;
        t = 0f;
        isPaused = false;

        if (moveRoutine != null) StopCoroutine(moveRoutine);
        moveRoutine = StartCoroutine(MoveRoutine());
    }
    
    public void PauseMove()
    {
        isPaused = true;
        Paused = true;
    }

    public void ResumeMove()
    {
        if (isPaused)
        {
            isPaused = false;
            if (moveRoutine != null) StopCoroutine(moveRoutine);
            moveRoutine = StartCoroutine(MoveRoutine());
            Paused = false;
        }
    }
    
    IEnumerator MoveRoutine()
    {
        while (t < 1f)
        {
            if (!isPaused)
            {
                t += Time.deltaTime / duration;
                rect.anchoredPosition = Vector2.Lerp(start, end, t);
            }
            yield return null;
        }

        rect.anchoredPosition = end;
        moveRoutine = null;
    }
}

