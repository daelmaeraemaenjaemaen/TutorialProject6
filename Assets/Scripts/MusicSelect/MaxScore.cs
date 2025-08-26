public class MaxScore
{
    public static uint songCount{ get; private set; }
    private readonly uint MusicCode;
    private float easy1PScore;
    private float easy2PScore;
    private float easyCombo;
    private float hard1PScore;
    private float hard2PScore;
    private float hardCombo;
    
    public MaxScore(float EScore1, float EScore2, float ECombo, float HScore1, float HScore2, float HCombo)
    {
        MusicCode = songCount;
        songCount++;
        easy1PScore = EScore1;
        easy2PScore = EScore2;
        easyCombo = ECombo;
        hard1PScore = HScore1;
        hard2PScore = HScore2;
        hardCombo = HCombo;
    }
    
    public static void resetSongCount()
    {
        songCount = 0;
    }
    
    public void setEasy1pScore(float EScore1)
    {
        easy1PScore = EScore1;
    }
    
    public void setEasy2pScore(float EScore2)
    {
        easy2PScore = EScore2;
    }
    
    public void setEasyCombo(float ECombo)
    {
        easyCombo = ECombo;
    }
    
    public void setHard1pScore(float HScore1)
    {
        hard1PScore = HScore1;
    }
    
    public void setHard2pScore(float HScore2)
    {
        hard2PScore = HScore2;
    }
    
    public void setHardCombo(float HCombo)
    {
        hardCombo = HCombo;
    }
    
    public uint getMusicCode()
    {
        return MusicCode;
    }

    public float getEasy1pScore()
    {
        return easy1PScore;
    }
    
    public float getEasy2pScore()
    {
        return easy2PScore;
    }
    
    public float getEasyCombo()
    {
        return easyCombo;
    }
    
    public float getHard1pScore()
    {
        return hard1PScore;
    }
    
    public float getHard2pScore()
    {
        return hard2PScore;
    }
    
    public float getHardCombo()
    {
        return hardCombo;
    }
}

