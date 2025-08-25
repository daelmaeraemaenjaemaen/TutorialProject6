//플레이어가 설정한 설정값을 저장하는 클래스
using UnityEngine;
public static class PlayerSettings
{
    // 키 입력
    public static KeyCode p1k1 = KeyCode.S;
    public static KeyCode p1k2 = KeyCode.D;
    public static KeyCode p1k3 = KeyCode.F;
    public static KeyCode p2k1 = KeyCode.J;
    public static KeyCode p2k2 = KeyCode.K;
    public static KeyCode p2k3 = KeyCode.L;

    // 싱크, 스피드
    public static float syncSec = 0f;     // -5.0 ~ +5.0
    public static float velocity = 7.2f;  // 1.0 ~ 10.0

    // PlayerPrefs keys
    const string K_P1_1 = "p1k1", K_P1_2 = "p1k2", K_P1_3 = "p1k3";
    const string K_P2_1 = "p2k1", K_P2_2 = "p2k2", K_P2_3 = "p2k3";
    const string K_SYNC = "sync_sec";
    const string K_VEL  = "speed_vel";
    private const string K_Short  = "RG1/style/short",  K_Long  = "RG1/style/long",  K_Line  = "RG1/style/lineset";
    private const string K_Short2 = "RG1/style/short2", K_Long2 = "RG1/style/long2", K_Line2 = "RG1/style/lineset2";
    
    // 배경
    public static int shortNum = 0;
    public static int longNum = 0;
    public static int backNum = 0;

    public static int shortNum2 = 0;
    public static int longNum2 = 0;
    public static int backNum2 = 0;

    public static void Load()
    {
        p1k1 = (KeyCode)PlayerPrefs.GetInt(K_P1_1, (int)KeyCode.S);
        p1k2 = (KeyCode)PlayerPrefs.GetInt(K_P1_2, (int)KeyCode.D);
        p1k3 = (KeyCode)PlayerPrefs.GetInt(K_P1_3, (int)KeyCode.F);
        p2k1 = (KeyCode)PlayerPrefs.GetInt(K_P2_1, (int)KeyCode.J);
        p2k2 = (KeyCode)PlayerPrefs.GetInt(K_P2_2, (int)KeyCode.K);
        p2k3 = (KeyCode)PlayerPrefs.GetInt(K_P2_3, (int)KeyCode.L);

        syncSec = Mathf.Clamp(PlayerPrefs.GetFloat(K_SYNC, 0f), -5f, 5f);
        velocity = Mathf.Clamp(PlayerPrefs.GetFloat(K_VEL, 7.2f), 1f, 10f);
        
        shortNum = Mathf.Clamp(PlayerPrefs.GetInt(K_Short, 0), 0, 2);
        longNum  = Mathf.Clamp(PlayerPrefs.GetInt(K_Long,  0), 0, 2);
        backNum  = Mathf.Clamp(PlayerPrefs.GetInt(K_Line,  0), 0, 2);

        shortNum2 = Mathf.Clamp(PlayerPrefs.GetInt(K_Short2, 0), 0, 2);
        longNum2  = Mathf.Clamp(PlayerPrefs.GetInt(K_Long2,  0), 0, 2);
        backNum2  = Mathf.Clamp(PlayerPrefs.GetInt(K_Line2,  0), 0, 2);
    }

    public static void Save()
    {
        PlayerPrefs.SetInt(K_P1_1, (int)p1k1);
        PlayerPrefs.SetInt(K_P1_2, (int)p1k2);
        PlayerPrefs.SetInt(K_P1_3, (int)p1k3);
        PlayerPrefs.SetInt(K_P2_1, (int)p2k1);
        PlayerPrefs.SetInt(K_P2_2, (int)p2k2);
        PlayerPrefs.SetInt(K_P2_3, (int)p2k3);

        PlayerPrefs.SetFloat(K_SYNC, Mathf.Clamp(syncSec, -5f, 5f));
        PlayerPrefs.SetFloat(K_VEL,  Mathf.Clamp(velocity, 1f, 10f));
        
        PlayerPrefs.SetInt(K_Short, shortNum);
        PlayerPrefs.SetInt(K_Long,  longNum);
        PlayerPrefs.SetInt(K_Line,  backNum);

        PlayerPrefs.SetInt(K_Short2, shortNum2);
        PlayerPrefs.SetInt(K_Long2,  longNum2);
        PlayerPrefs.SetInt(K_Line2,  backNum2);
        
        PlayerPrefs.Save();
    }

    // 런타임 반영
    public static void ApplyToRuntime()
    {
        NoteInput.key1 = p1k1;
        NoteInput.key2 = p1k2;
        NoteInput.key3 = p1k3;
        NoteInput.key4 = p2k1;
        NoteInput.key5 = p2k2;
        NoteInput.key6 = p2k3;

        NoteMove.moveSpeed = Mathf.Clamp(velocity, 1f, 10f);

        if (Design.I != null) Design.I.ApplyAllRuntime();
    }
    
    
    // ReSharper disable once InvalidXmlDocComment
    /**
    //설정 화면에서 설정 가능
    public int masterVol; //마스터 볼륨
    public int songVol; //곡 볼륨
    public int fxVol; //효과음 볼륨
    public int syncSetting; //화면 싱크 설정값
    //1~6번 레인 키코드
    public int keySet1;
    public int keySet2;
    public int keySet3;
    public int keySet4;
    public int keySet5;
    public int keySet6;
    //인게임 배속 설정 키코드
    public int keySetslow;
    public int keySetFast;

    //곡 선택 화면에서 설정 가능
    public float velocity; // 배속
    public string noteCode; //노트 종류 코드
    public string gearCode; //기어 종류 코드
    **/
}
