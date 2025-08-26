using System;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Result : MonoBehaviour
{
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [SerializeField] private AudioSource audioSource;
    
    [Header("결과창")]
    [SerializeField] private TextMeshProUGUI totalScore;
    [SerializeField] private TextMeshProUGUI totalPerfect;
    [SerializeField] private TextMeshProUGUI totalGreat;
    [SerializeField] private TextMeshProUGUI totalGood;
    [SerializeField] private TextMeshProUGUI totalMiss;
    
    [SerializeField] private TextMeshProUGUI p1Score;
    [SerializeField] private TextMeshProUGUI p1Perfect;
    [SerializeField] private TextMeshProUGUI p1Great;
    [SerializeField] private TextMeshProUGUI p1Good;
    [SerializeField] private TextMeshProUGUI p1Miss;
    
    [SerializeField] private TextMeshProUGUI p2Score;
    [SerializeField] private TextMeshProUGUI p2Perfect;
    [SerializeField] private TextMeshProUGUI p2Great;
    [SerializeField] private TextMeshProUGUI p2Good;
    [SerializeField] private TextMeshProUGUI p2Miss;

    [SerializeField] private Image grade;
    [SerializeField] private Sprite[] Grade = new Sprite[4];
    
    
    
    private float score;
    private float perfect;
    private float great;
    private float good;
    private float miss;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        score = Score.nowScore1P + Score.nowScore2P;
        perfect = Score.Perfect1P + Score.Perfect2P;
        great = Score.Great1P + Score.Great2P;
        good = Score.Good1P + Score.Good2P;
        miss = Score.Miss1P + Score.Miss2P;
        
        totalScore.text = score.ToString();
        totalPerfect.text = perfect.ToString();
        totalGreat.text = great.ToString();
        totalGood.text = good.ToString();
        totalMiss.text = miss.ToString();
        p1Score.text = Score.nowScore1P.ToString();
        p1Perfect.text = Score.Perfect1P.ToString();
        p1Great.text = Score.Great1P.ToString();
        p1Good.text = Score.Good1P.ToString();
        p1Miss.text = Score.Miss1P.ToString();
        p2Score.text = Score.nowScore2P.ToString();
        p2Perfect.text = Score.Perfect2P.ToString();
        p2Great.text = Score.Great2P.ToString();
        p2Good.text = Score.Good2P.ToString();
        p2Miss.text = Score.Miss2P.ToString();

        if (score > 97000)
        {
            grade.preserveAspect = true;
            grade.sprite = Grade[0];
        }
        else if (score > 90000)
        {
            grade.preserveAspect = true;
            grade.sprite = Grade[1];
        }
        else if (score > 80000)
        {
            grade.preserveAspect = true;
            grade.sprite = Grade[2];
        }
        else
        {
            grade.preserveAspect = true;
            grade.sprite = Grade[3];
        }
            
        
        if (SceneManager.GetActiveScene().name == "4_Result")
        {
            if (bgmGroup != null) audioSource.outputAudioMixerGroup = bgmGroup;
            audioSource.loop = true;
            if (!audioSource.isPlaying) audioSource.Play();
            CheckRecord();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!scene.name.Equals("4_Result")) return;

        if (bgmGroup != null) audioSource.outputAudioMixerGroup = bgmGroup;
        audioSource.loop = true;
        audioSource.Play();

        CheckRecord();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            audioSource.Stop();
            SceneManager.LoadScene("2_MusicSelect");
        }
    }

    private void CheckRecord()
    {
        int score1P, score2P, combo;
        int lineNum = -1;
        string line = "";
        bool isNewRecord = false;
        int selectedSong = PlayerPrefs.GetInt("selectedSongID");
        string selectedDiff = PlayerPrefs.GetString("SelectedDifficulty", "easy");
        bool isEasy = !selectedDiff.Equals("hard", StringComparison.OrdinalIgnoreCase);
        var filePath = Application.dataPath + "/Data/Rlist";
        if (File.Exists(filePath))
        {
            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    lineNum++;
                    line = reader.ReadLine();
                    if (string.IsNullOrWhiteSpace(line) || line[0] == '#') continue;

                    string[] lines = line.Split("|");
                    if (lines.Length < 7)
                    {
                        Debug.LogWarning($"Rlist malformed: {line}");
                        continue;
                    }

                    if (lines[0].Equals(selectedSong.ToString()))
                    {
                        if (isEasy)
                        {
                            score1P = Convert.ToInt32(lines[1]);
                            score2P = Convert.ToInt32(lines[2]);
                            combo = Convert.ToInt32(lines[3]);
                        }
                        else
                        {
                            score1P = Convert.ToInt32(lines[4]);
                            score2P = Convert.ToInt32(lines[5]);
                            combo = Convert.ToInt32(lines[6]);
                        }

                        isNewRecord = score1P + score2P < Score.nowScore1P + Score.nowScore2P;
                        break;
                    }
                }
            }

            if (isNewRecord) WriteNewRecord(lineNum, line);
        }
        else
        {
            //Rlist 파일 찾지 못함
            Debug.Log("RecordList file not found(read)");
            //Application.Quit();
        }
    }

    private void WriteNewRecord(int lineNum, string line)
    {
        // 수정할 문자열 조합
        string[] lines = line.Split("|");
        if (lines.Length < 7)
        {
            Debug.LogWarning($"Rlist malformed: {line}");
            return;
        }
        
        string selectedDiff = PlayerPrefs.GetString("SelectedDifficulty", "easy");
        bool isEasy = !selectedDiff.Equals("hard", StringComparison.OrdinalIgnoreCase);
        
        if (isEasy)
        {
            lines[1] = Score.nowScore1P.ToString();
            lines[2] = Score.nowScore2P.ToString();
            lines[3] = Combo.maxCombo.ToString();
        }
        else
        {
            lines[4] = Score.nowScore1P.ToString();
            lines[5] = Score.nowScore2P.ToString();
            lines[6] = Combo.maxCombo.ToString();
        }
        line = string.Join("|", lines);

        //파일에 쓰기
        var filePath = Application.dataPath + "/Data/Rlist";
        if (File.Exists(filePath))
        {
            string[] arrLine = File.ReadAllLines(filePath, Encoding.Default);
            arrLine[lineNum] = line;
            File.WriteAllLines(filePath, arrLine, Encoding.Default);
        }
        else
        {
            //Rlist 파일 찾지 못함
            Debug.Log("RecordList file not found(write)");
            //Application.Quit();
        }
    }
}
