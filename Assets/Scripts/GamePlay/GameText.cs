using UnityEngine;
using TMPro;
public class GameText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI judgeText1;
    [SerializeField] private TextMeshProUGUI judgeText2;
    [SerializeField] private TextMeshProUGUI comboText1;
    [SerializeField] private TextMeshProUGUI comboText2;
    [SerializeField] private GameObject lifeBar1p;
    [SerializeField] private GameObject lifeBar2p;
    [SerializeField] private TextMeshProUGUI scoreText;

    private float height1p;
    private float height2p;

    void Awake()
    {
        RectTransform rectTran1 = lifeBar1p.GetComponent<RectTransform>();
        RectTransform rectTran2 = lifeBar2p.GetComponent<RectTransform>();
        height1p = rectTran1.localScale.y;
        height2p = rectTran2.localScale.y;
    }

    // 판정에서 파생되는 이벤트가 많아질 것 같아 이를 묶어 처리하기 위한 함수입니다
    public void Result(NoteJudge result, int line, int tick)
    {
        JudgeResult(result, line);
        ComboResult(result);
        LifeResult(result);
        ScoreResult(result, tick);
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

    private void LifeResult(NoteJudge result)
    {
        int lifeNum = Life.SetLife(result);

        RectTransform rectTran1 = lifeBar1p.GetComponent<RectTransform>();
        RectTransform rectTran2 = lifeBar2p.GetComponent<RectTransform>();
        rectTran1.localScale = new Vector3(1f, height1p * lifeNum / 100, 1f);
        rectTran2.localScale = new Vector3(1f, height2p * lifeNum / 100, 1f);
    }

    private void ScoreResult(NoteJudge result, int tick)
    {
        if (result == NoteJudge.FastMiss || result == NoteJudge.Miss) return;
        scoreText.text = Score.setScore(result, tick).ToString();
    }
}
