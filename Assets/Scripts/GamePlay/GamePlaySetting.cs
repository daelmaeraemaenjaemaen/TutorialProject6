using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePlaySetting : MonoBehaviour
{
    [SerializeField] private GameObject SettingUI;
    [SerializeField] private GameObject CountDown;
    [SerializeField] private TMP_Text CountDownText;
    private bool isPaused = false;
    private bool intro =  false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        intro =  false;
        ApplyPause(false);
        CountDownText.gameObject.SetActive(false);
        CountDown.SetActive(false);

        StartCoroutine(introDelay());
    }

    IEnumerator introDelay()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        intro = true;
    }
    
    void ApplyPause(bool pause)
    {
        isPaused = pause;
        SettingUI.SetActive(pause);
        Time.timeScale = pause ? 0f : 1f;
        AudioListener.pause = pause;
    }
    
    IEnumerator ResumeWithCountdown()
    {
        SettingUI.SetActive(false);
        CountDownText.gameObject.SetActive(true);
        CountDown.SetActive(true);
        
        Time.timeScale = 0f;
        for (int i = 3; i > 0; i--)
        {
            CountDownText.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        CountDownText.gameObject.SetActive(false);
        CountDown.SetActive(false);
        ApplyPause(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!intro) return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                StartCoroutine(ResumeWithCountdown());
            else
                ApplyPause(true);
        }

    }

    public void Quit()
    {
        SceneManager.LoadScene("MusicSelect");
    }
}
