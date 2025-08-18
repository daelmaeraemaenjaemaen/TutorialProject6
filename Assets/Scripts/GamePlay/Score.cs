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
    public static int nowScore{ get; private set; }

    public static void setBaseScore(int total)
    {
        totalNote = total;
        baseScore = 100000 / totalNote;
    }

    public static int setScore(NoteJudge result, int tickNum)
    {
        if (totalNote <= 0) return -1; // error
        float add;
        switch (result)
        {
            case NoteJudge.Good:
                add = baseScore / 10;
                nowScore += (int)add;
                break;
            case NoteJudge.Great:
                add = baseScore / 5;
                nowScore += (int)add;
                break;
            case NoteJudge.Perfect:
                if (tickNum == 0) add = baseScore;
                else add = baseScore / tickNum;
                nowScore += (int)add;
                break;
        }

        if (nowScore == 99999) nowScore = 100000; // 버림으로 인한 1점 보정
        return nowScore;
    }
}