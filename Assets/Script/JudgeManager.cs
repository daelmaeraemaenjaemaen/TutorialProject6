using UnityEngine;

public enum NoteJudge { Perfect, Great, Good, Miss, FastMiss} // 판정 <- Perfect, Great, Good, Miss, FastMiss가 있음
public class JudgeManager : MonoBehaviour
{
    public static JudgeManager Instance;

    private void Awake() => Instance = this;

    public NoteJudge JudgeNote(float inputTime, float noteTime)
    {
        float diff = (inputTime - noteTime) * 1000f; // 차이: 실제 친 시간 - 쳐야 하는 시간
        
        if (diff < -200f) return NoteJudge.FastMiss;
        else if (Mathf.Abs(diff) <= 150f) return NoteJudge.Good;
        else if (Mathf.Abs(diff) <= 80f) return NoteJudge.Great;
        else if (Mathf.Abs(diff) <= 45f) return NoteJudge.Perfect;
        else return NoteJudge.Miss;
    }

    public NoteJudge JudgeTick(float currentTime, float tickTime)
    {
        float diff = (currentTime - tickTime) * 1000f; // 차이: 실제 친 시간 - 쳐야 하는 시간
        
        if (diff < -200f) return NoteJudge.FastMiss;
        else if (Mathf.Abs(diff) <= 150f) return NoteJudge.Good;
        else if (Mathf.Abs(diff) <= 80f) return NoteJudge.Great;
        else if (Mathf.Abs(diff) <= 45f) return NoteJudge.Perfect;
        else return NoteJudge.Miss;
    }
}
