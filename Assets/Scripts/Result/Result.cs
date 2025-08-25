using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Result : MonoBehaviour
{
    [Header("Mixer Routing")]
    [SerializeField] private AudioMixerGroup bgmGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;

    [SerializeField] private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
        string selectedDiff = PlayerPrefs.GetString("selectedDiff");
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
                        if (selectedDiff.Equals("easy"))
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
        if (PlayerPrefs.GetString("selectedDiff").Equals("easy"))
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
            arrLine[lineNum - 1] = line;
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
