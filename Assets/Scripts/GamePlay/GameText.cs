using UnityEngine;
using TMPro;
public class GameText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI judgeText1;
    [SerializeField] private TextMeshProUGUI judgeText2;
    [SerializeField] private TextMeshProUGUI comboText1;
    [SerializeField] private TextMeshProUGUI comboText2;
    [SerializeField] private Life life;

    // 판정에서 파생되는 이벤트가 많아질 것 같아 이를 묶어 처리하기 위한 함수입니다
    public void Result(NoteJudge result, int line)
    {
        JudgeResult(result, line);
        ComboResult(result);
        life.SetLife(result);
    }

    private void JudgeResult(NoteJudge result, int line)
    {
        string text = result.ToString();

        if (line <= 3) judgeText1.text = text;
        else judgeText2.text = text;
    }

    private void ComboResult(NoteJudge result)
    {
        bool isHit = result != NoteJudge.Miss && result != NoteJudge.FastMiss;
        string text = Combo.UpdateCombo(isHit).ToString();
        comboText1.text = text;
        comboText2.text = text;
    }
}
