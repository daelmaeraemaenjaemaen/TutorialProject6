using UnityEngine;
using TMPro;
public class GameText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI judgeText1;
    [SerializeField] private TextMeshProUGUI judgeText2;
    [SerializeField] private TextMeshProUGUI comboText1;
    [SerializeField] private TextMeshProUGUI comboText2;

    public void JudgeResult(NoteJudge result, int line)
    {
        string text = result.ToString();

        if (line <= 3) judgeText1.text = text;
        else judgeText2.text = text;
    }

    public void ComboResult(int combo)
    {
        string text = combo.ToString();
        comboText1.text = text;
        comboText2.text = text;
    }
}
