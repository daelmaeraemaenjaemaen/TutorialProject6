using UnityEngine;
using TMPro;
public class JudgeText : MonoBehaviour
{
    public TextMeshProUGUI judgeText;

    private string _lastShownResult = "";

    public void Result(NoteJudge result)
    {
        string text = result.ToString();

        judgeText.text = text;
        _lastShownResult = text;
    }

    //실제 게임에선 prefix를 사용하지 않을 예정이지만, 코드는 남겨둡니다
    //롱노트 틱 판정을 따로 표시하려고 할 경우 별도의 판정을 추가하는 것이 좋습니다
    private void ResultPrefixed(NoteJudge result, string prefix)
    {
        string text = prefix + result.ToString();

        judgeText.text = text;
        _lastShownResult = text;
    }
}
