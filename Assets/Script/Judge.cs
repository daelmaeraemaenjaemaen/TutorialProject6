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
    public static NoteJudge Judgement(float inputTime, float noteTime)
    {
        float diff = (inputTime - noteTime) * 1000f; // 노트를 친 시간 차이(ms)
        
        /**
         * 판정
         * Perfect: ±45㎳ (*디맥 Max 100%: ±41.67㎳)
         * Great: ±80㎳
         * Good: ±150㎳
         * 빠른 미스: -200~-150㎳
         * Miss: 그 외
        **/
        
        if (diff >= -700f && diff <= -150f)
            return NoteJudge.FastMiss;
        
        float abs = Mathf.Abs(diff);
        
        if (abs <= 45f) return NoteJudge.Perfect;
        else if (abs <= 80f) return NoteJudge.Great;
        else if (abs <= 150f) return NoteJudge.Good;
        else return NoteJudge.Miss;
    }
}