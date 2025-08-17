using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlaySetting : MonoBehaviour
{
    [SerializeField] private GameObject MenuUI;
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject CountDown;
    [SerializeField] private TMP_Text CountDownText;
    [SerializeField] private GameObject Bgset;
    [SerializeField] private GameObject Gpset;
    [SerializeField] private GameObject Adset;

    private bool isSetting = false;
    private bool isMenu = false;
    private bool intro = false;
    private bool bgset = true;
    private bool gpset = false;
    private bool adset = false;

    // ★ 추가: 카운트다운 제어
    private bool isCountingDown = false;
    private Coroutine countdownRoutine = null;

    void Start()
    {
        intro = false;
        SettingUI.SetActive(false);
        MenuUI.SetActive(false);
        CountDownText.gameObject.SetActive(false);
        CountDown.SetActive(false);
        Time.timeScale = 1f;
        AudioListener.pause = false;

        StartCoroutine(introDelay());
    }

    IEnumerator introDelay()
    {
        yield return new WaitForSecondsRealtime(2.5f);
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
        AudioListener.pause = false;

        isCountingDown = false;
        countdownRoutine = null;
    }

    void MenuTurn(bool turn)
    {
        isMenu = turn;
        MenuUI.SetActive(turn);

        if (turn)
        {
            // 메뉴 열기: 즉시 일시정지 + 진행 중 카운트다운 중단
            Time.timeScale = 0f;
            AudioListener.pause = true;

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
            // 메뉴 닫기: 일시정지 해제는 카운트다운에서만!
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!intro) return; // 인트로 중엔 무시 (원하면 허용 가능)

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
                    // 메뉴 닫고 카운트다운 시작(중복 방지)
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
        SceneManager.LoadScene("MusicSelect");
    }

    public void Setting()
    {
        SettingTurn(true);
    }

    public void Restart()
    {
        CleanupBeforeSceneChange();
        SceneManager.LoadScene("GamePlay");
    }

    private void CleanupBeforeSceneChange()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

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
}
