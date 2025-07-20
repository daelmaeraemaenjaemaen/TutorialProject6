using UnityEngine;
using TMPro;
public class JudgeText : MonoBehaviour
{
    public TextMeshProUGUI judgeText;
    
    private string lastShownResult = "";
    
    public void Result(NoteJudge result)
    {
        string text = result.ToString();
        
        if (text != lastShownResult)
        {
            judgeText.text = text;
            lastShownResult = text;
        }
    }
}
