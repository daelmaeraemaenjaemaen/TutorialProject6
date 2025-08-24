using UnityEngine;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    [SerializeField] private GameObject GameOver;
    [SerializeField] private AudioSource bgmSource;
    
    public static int lifeNum { get; private set; } = 100;

    public static void LifeReset()
    {
        lifeNum = 100;
    }

    public static int SetLife(NoteJudge judge)
    {
        //perfect: +3% great: +2% good: +1% miss: -10%
        int i = 0;
        switch (judge)
        {
            case NoteJudge.FastMiss:
                i = -10;
                break;
            case NoteJudge.Miss:
                i = -10;
                break;
            case NoteJudge.Good:
                i = 1;
                break;
            case NoteJudge.Great:
                i = 2;
                break;
            case NoteJudge.Perfect:
                i = 3;
                break;
        }

        lifeNum += i;
        if (lifeNum >= 100) lifeNum = 100;
        else if (lifeNum <= 0) lifeNum = 0;
        return lifeNum;
    }

    public void Start()
    {
        Time.timeScale = 1f;
        bgmSource.UnPause();
    }
    public void Update()
    {
        if (lifeNum <= 0)
        {
            Time.timeScale = 0f;
            
            GameOver.SetActive(true);
            bgmSource.Pause();
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("3_GamePlay");
    }

    public void Exit()
    {
        SceneManager.LoadScene("2_MusicSelect");
        bgmSource.UnPause();
        Time.timeScale = 1f;
    }
}