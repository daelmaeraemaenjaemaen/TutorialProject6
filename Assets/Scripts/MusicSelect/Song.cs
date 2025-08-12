public class Song
{
    public static uint songCount{ get; private set; } //총 곡 수
    private readonly uint songID; //곡 고유 ID(곡 호출 시 곡명이 아닌 이걸로 호출해야 함)
    private string songName; //곡명
    private string songArtist; //작곡가
    private string songCategory; //카테고리
    private string songFileName; //곡 파일명
    private string songCover; //앨범아트 파일명
    private float songBPM; //곡 BPM
    private float songLength; //곡 길이
    private string easyFileName;
    private string hardFileName;

    //클래스 생성자(앨범커버 미완성 상태일 수 있어 2개 만듬)
    public Song(string name, string artist, string category, string fileName)
    {
        songID = songCount;
        songCount++;
        songName = name;
        songArtist = artist;
        songCategory = category;
        songFileName = fileName;
        songCover = ""; //더미 커버 추가 시 해당 커버 파일 경로로 수정 필요
        easyFileName = "";
        hardFileName = "";
    }

    public Song(string name, string artist, string category, string fileName, string coverFileName)
    {
        songID = songCount;
        songCount++;
        songName = name;
        songArtist = artist;
        songCategory = category;
        songFileName = fileName;
        songCover = coverFileName;
        easyFileName = "";
        hardFileName = "";
    }

    public Song(string name, string artist, string category, string fileName, string coverFileName, float BPM, float Length, string easyFileName, string hardFileName)
    {
        songID = songCount;
        songCount++;
        songName = name;
        songArtist = artist;
        songCategory = category;
        songFileName = fileName;
        songCover = coverFileName;
        songBPM = BPM;
        songLength = Length;
        this.easyFileName = easyFileName;
        this.hardFileName = hardFileName;
    }

    //기능 구현 함수(기능 추가 시 여기 추가하면 됨)
    public static void resetSongCount()
    {
        songCount = 0;
    }
    public void setSongCover(string coverFileName)
    {
        songCover = coverFileName;
    }

    public void setSongBPM(float bpm)
    {
        songBPM = bpm;
    }

    public void setSongLength(float length) {
        songLength = length;
    }

    public void setEasyFileName(string fileName)
    {
        easyFileName = fileName;
    }

    public void setHardFileName(string fileName)
    {
        hardFileName = fileName;
    }

    //로컬 변수 리턴 함수들
    public uint getSongID()
    {
        return songID;
    }

    public string getsongName()
    {
        return songName;
    }

    public string getsongArtist()
    {
        return songArtist;
    }

    public string getSongFileName()
    {
        return songFileName;
    }

    public string getSongCategory()
    {
        return songCategory;
    }

    public string getsongCover()
    {
        return songCover;
    }

    public float getSongBPM()
    {
        return songBPM;
    }

    public float getSongLength()
    {
        return songLength;
    }

    public string getEasyFileName()
    {
        return easyFileName;
    }

    public string getHardFileName()
    {
        return hardFileName;
    }
}
