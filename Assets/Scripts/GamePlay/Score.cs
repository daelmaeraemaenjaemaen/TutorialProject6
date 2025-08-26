using UnityEngine;
public class Score : MonoBehaviour
{
    /*
    100000점 만점 
    perfect 100%
    great 50%
    good 10%
    롱노트 틱은 perfect 점수/해당 롱노트의 총 틱
    */
    private static int totalNote;
    private static float baseScore; // perfect 기준
    public static int nowScore1P{ get; private set; }
    public static int nowScore2P{ get; private set; }
    
    public static int Perfect1P{ get; private set; }
    public static int Great1P{ get; private set; }
    public static int Good1P{ get; private set; }
    public static int Miss1P{ get; private set; }
    
    public static int Perfect2P{ get; private set; }
    public static int Great2P{ get; private set; }
    public static int Good2P{ get; private set; }
    public static int Miss2P{ get; private set; }
    

    public static void setBaseScore(int total)
    {
        totalNote = total;
        baseScore = 100000 / totalNote;
        nowScore1P = 0;
        nowScore2P = 0;
        
        Perfect1P = 0;
        Great1P = 0;
        Good1P = 0;
        Miss1P = 0;
        
        Perfect2P = 0;
        Great2P = 0;
        Good2P = 0;
        Miss2P = 0;
    }

    public static int setScore(NoteJudge result, int line, int tickNum)
    {
        if (totalNote <= 0) return -1; // error
        float add;
        switch (result)
        {
            case NoteJudge.Good:
                add = baseScore / 10;
                if (line <= 3)
                {
                    nowScore1P += (int)add;
                    Good1P++;
                }
                else
                {
                    nowScore2P += (int)add;
                    Good2P++;
                }
                break;
            case NoteJudge.Great:
                add = baseScore / 5;
                if (line <= 3)
                {
                    nowScore1P += (int)add;
                    Great1P++;
                }
                else
                {
                    nowScore2P += (int)add;
                    Great2P++;
                }
                break;
            case NoteJudge.Perfect:
                if (tickNum == 0) add = baseScore;
                else add = baseScore / tickNum;
                if (line <= 3)
                {
                    nowScore1P += (int)add;
                    Perfect1P++;
                }
                else
                {
                    nowScore2P += (int)add;
                    Perfect2P++;
                }
                break;
            case NoteJudge.FastMiss:
                if (line <= 3) Miss1P++;
                else Miss2P++;
                break;
            
            case NoteJudge.Miss:
                if (line <= 3) Miss1P++;
                else Miss2P++;
                break;
        }

        if (nowScore1P + nowScore2P == 99999) nowScore1P++; // 버림으로 인한 1점 보정
        return nowScore1P + nowScore2P;
    }
}