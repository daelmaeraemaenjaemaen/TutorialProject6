using UnityEngine;

public enum NoteJudge // 판정 종류
{
    FastMiss,
    Miss,
    Good,
    Great,
    Perfect
}

public static class Judge
{
    public static float FMm = -300f;
    public static float FMM = -150f;

    public static float PR = 45f;
    public static float GrR = 80f;
    public static float GoR = 150f;
    
    public static float MissLate = 150f;
    
    public static NoteJudge Judgement(float inputTime, float noteTime)
    {
        float diff = (inputTime - noteTime) * 1000f; // 노트를 친 시간 차이(ms)
        
        if (diff >= FMm && diff <= FMM)
            return NoteJudge.FastMiss;
        
        float abs = Mathf.Abs(diff);
        
        if (abs <= PR) return NoteJudge.Perfect;
        else if (abs <= GrR) return NoteJudge.Great;
        else if (abs <= GoR) return NoteJudge.Good;
        else return NoteJudge.Miss;
    }
    
    public static NoteJudge LongTickJudge(float inputTime, float noteTime)
    {
        float diff = (inputTime - noteTime) * 1000f; // 노트를 친 시간 차이(ms)
        
        if (diff >= FMm && diff <= FMM)
            return NoteJudge.FastMiss;
        
        float abs = Mathf.Abs(diff);
        
        if (abs <= PR) return NoteJudge.Perfect;
        else if (abs <= GrR) return NoteJudge.Great;
        else if (abs <= GoR) return NoteJudge.Good;
        else return NoteJudge.Miss;
    }
}