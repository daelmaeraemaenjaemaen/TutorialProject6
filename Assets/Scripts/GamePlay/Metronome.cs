using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class Metronome : MonoBehaviour
{
    public static float bpm = 150f; // BPM 설정
    public static string nowGim;
    private float _interval, _intervalL, _intervalR; // 박자 간 시간
    private float _nextTick, _nextTickL, _nextTickR;
    private float songEnd;
    private bool _isPlaying = false;

    private List<NotePart> noteParts;
    private NotePart notePart;
    private NoteBeat noteBeatL, noteBeatR;
    private int partCount, lCount, rCount;
    private Song song;
    private PatternReader patternReader = new();
    [SerializeField] private NoteSpawn noteSpawn;
    [SerializeField] private GameObject patternLogo;
    [SerializeField] private Sprite defImg;
    [SerializeField] private Sprite refImg;
    [SerializeField] private Sprite remImg;
    [SerializeField] private Sprite revImg;
    [SerializeField] private Sprite rflImg;

    public void setPlayData(Song s, bool isEasy)
    {
        // 곡 및 패턴 데이터 불러오기
        _isPlaying = false;
        song = s;
        bpm = song.getSongBPM();
        string fileName = isEasy ? song.getEasyFileName() : song.getHardFileName();
        noteParts = patternReader.ReadPattern(fileName);

        //패턴 구성
        //bpm * 비트 = 초당 노트 수 * 240
        _interval = 60f / bpm; // 4비트
        partCount = lCount = rCount = 0;
        notePart = noteParts[0];
        noteBeatL = noteParts[0].noteL[0];
        noteBeatR = noteParts[0].noteR[0];
        _intervalL = 240 / (notePart.beatL * bpm);
        _intervalR = 240 / (notePart.beatR * bpm);

        //기믹 데이터 먼저 불러오기
        nowGim = noteParts[0].gim;
        Image img = patternLogo.GetComponent<Image>();
        switch (nowGim)
        {
            case "def":
                img.sprite = defImg;
                break;
            case "ref":
                img.sprite = refImg;
                break;
            case "rem":
                img.sprite = remImg;
                break;
            case "rev":
                img.sprite = revImg;
                break;
            case "rfl":
                img.sprite = rflImg;
                break;
        }

        //점수, 콤보 등 초기화
        Combo.ComboReset();
        Life.LifeReset();
        Judge.ResetJudgeCount();
    }

    public void StartPlay()
    {
        float startTime = Time.time;
        songEnd = startTime + song.getSongLength();
        _nextTick = startTime + _interval;
        _nextTickL = startTime + _intervalL;
        _nextTickR = startTime + _intervalR;
        _isPlaying = true;
    }

    void Update()
    {
        if (!_isPlaying) return;
        if (Life.lifeNum <= 0)
        {
            //TODO: 게임 오버 화면 이동
        }
        float frameTime = Time.time;
        //while (frameTime >= songEnd) _isPlaying = false;
        while (frameTime >= _nextTick)
        {
            if (partCount < noteParts.Count)
            {
                notePart = noteParts[partCount];
                _intervalL = 240 / (notePart.beatL * bpm);
                _intervalR = 240 / (notePart.beatR * bpm);
                lCount = rCount = 0;
                if (!notePart.gim.Equals(nowGim))
                {
                    nowGim = notePart.gim;
                    Invoke(nameof(setGimImg), 10 / NoteMove.moveSpeed);
                }
                noteSpawn.setReverse(notePart.gim.Equals("rev"));
            }
            partCount++;
            _nextTick += _interval; // 다음 틱 시간 처리
        }
        while (frameTime >= _nextTickL)
        {
            if (lCount < notePart.noteL.Length && partCount > 0)
            {
                noteBeatL = notePart.noteL[lCount];
                if (noteBeatL.isexist[0]) noteSpawn.SpawnNote(1, noteBeatL.tick[0], noteBeatL.isVisible[0]);
                if (noteBeatL.isexist[1]) noteSpawn.SpawnNote(2, noteBeatL.tick[1], noteBeatL.isVisible[1]);
                if (noteBeatL.isexist[2]) noteSpawn.SpawnNote(3, noteBeatL.tick[2], noteBeatL.isVisible[2]);
            }
            lCount++;
            _nextTickL += _intervalL;
        }
        while (frameTime >= _nextTickR)
        {
            if (rCount < notePart.noteR.Length && partCount > 0)
            {
                noteBeatR = notePart.noteR[rCount];
                if (noteBeatR.isexist[0]) noteSpawn.SpawnNote(4, noteBeatR.tick[0], noteBeatR.isVisible[0]);
                if (noteBeatR.isexist[1]) noteSpawn.SpawnNote(5, noteBeatR.tick[1], noteBeatR.isVisible[1]);
                if (noteBeatR.isexist[2]) noteSpawn.SpawnNote(6, noteBeatR.tick[2], noteBeatR.isVisible[2]);
            }
            rCount++;
            _nextTickR += _intervalR;
        }
    }

    private void setGimImg()
    {
        Image img = patternLogo.GetComponent<Image>();
        switch (nowGim)
        {
            case "def":
                img.sprite = defImg;
                break;
            case "ref":
                img.sprite = refImg;
                break;
            case "rem":
                img.sprite = remImg;
                break;
            case "rev":
                img.sprite = revImg;
                break;
            case "rfl":
                img.sprite = rflImg;
                break;
        }
    }
}
