using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class Metronome : MonoBehaviour
{
    public static float bpm = 150f; // BPM 설정
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

        //점수, 콤보 등 초기화
        Combo.ComboReset();
        Life.LifeReset();
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
                noteBeatL = notePart.noteL[lCount];
                noteBeatR = notePart.noteR[rCount];
                if (noteBeatL.is1) noteSpawn.SpawnNote(1, noteBeatL.tick1);
                if (noteBeatL.is2) noteSpawn.SpawnNote(2, noteBeatL.tick2);
                if (noteBeatL.is3) noteSpawn.SpawnNote(3, noteBeatL.tick3);
                if (noteBeatR.is1) noteSpawn.SpawnNote(4, noteBeatR.tick1);
                if (noteBeatR.is2) noteSpawn.SpawnNote(5, noteBeatR.tick2);
                if (noteBeatR.is3) noteSpawn.SpawnNote(6, noteBeatR.tick3);
            }
            partCount++;
            _nextTick += _interval; // 다음 틱 시간 처리
        }
        while (frameTime >= _nextTickL)
        {
            if (lCount < notePart.noteL.Length && lCount != 0)
            {
                noteBeatL = notePart.noteL[lCount];
                if (noteBeatL.is1) noteSpawn.SpawnNote(1, noteBeatL.tick1);
                if (noteBeatL.is2) noteSpawn.SpawnNote(2, noteBeatL.tick2);
                if (noteBeatL.is3) noteSpawn.SpawnNote(3, noteBeatL.tick3);
            }
            lCount++;
            _nextTickL += _intervalL;
        }
        while (frameTime >= _nextTickR)
        {
            if (rCount < notePart.noteR.Length && rCount != 0)
            {
                noteBeatR = notePart.noteR[rCount];
                if (noteBeatR.is1) noteSpawn.SpawnNote(4, noteBeatR.tick1);
                if (noteBeatR.is2) noteSpawn.SpawnNote(5, noteBeatR.tick2);
                if (noteBeatR.is3) noteSpawn.SpawnNote(6, noteBeatR.tick3);
            }
            rCount++;
            _nextTickR += _intervalR;
        }
    }
}
