using UnityEngine;
using TMPro;
public class JudgeText : MonoBehaviour
{
    public TextMeshProUGUI judgeText;
    
    private string lastShownResult = "";
    
    public void Result(NoteJudge result)
    {
        string text = result.ToString();
        
        judgeText.text = text;
        lastShownResult = text;
    }

    public void ResultPrefixed(NoteJudge result, string prefix)
    {
        string text = prefix + result.ToString();

        judgeText.text = text;
        lastShownResult = text;
    }
}
