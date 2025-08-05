//플레이어가 설정한 설정값을 저장하는 클래스
public class PlayerSettings
{
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
    
}
