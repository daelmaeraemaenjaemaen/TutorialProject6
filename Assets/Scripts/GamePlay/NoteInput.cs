using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;
using System.Collections;

public class NoteInput : MonoBehaviour
{
    public List<NoteMove> activeNotes; // 현재 화면에 존재하는 노트 리스트

    // 라인별 키코드, 설정에서 변경 가능하도록 static 처리
    public static KeyCode key1 = KeyCode.S;
    public static KeyCode key2 = KeyCode.D;
    public static KeyCode key3 = KeyCode.F;
    public static KeyCode key4 = KeyCode.J;
    public static KeyCode key5 = KeyCode.K;
    public static KeyCode key6 = KeyCode.L;
    
    [Header("오디오")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioClip note;
    
    private bool isStart;

    public static KeyCode getLineKey(int line)
    {
        switch (line)
        {
            case 1:
                return key1;
            case 2:
                return key2;
            case 3:
                return key3;
            case 4:
                return key4;
            case 5:
                return key5;
            case 6:
                return key6;
            default:
                return KeyCode.None; // error
        }
    }

    void Start()
    {
        isStart = false;
        
        if (sfxGroup != null && sfxAudioSource != null)
            sfxAudioSource.outputAudioMixerGroup = sfxGroup;
        
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(3.5f);
        isStart = true;
    }
    void Update()
    {
        List<NoteMove> toRemove = new List<NoteMove>(); // 삭제 대상 저장

        // 사용자 입력 판정
        if (Input.GetKeyDown(key1)) // 1번 키를 눌렀을 때
        {
            if (isStart) PlayClickSound();
            
            NoteMove nearest = FindNote(1); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }
        if (Input.GetKeyDown(key2)) // 2번 키를 눌렀을 때
        {
            if (isStart) PlayClickSound();
            
            NoteMove nearest = FindNote(2); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }
        if (Input.GetKeyDown(key3)) // 3번 키를 눌렀을 때
        {
            if (isStart) PlayClickSound();
            
            NoteMove nearest = FindNote(3); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }
        if (Input.GetKeyDown(key4)) // 4번 키를 눌렀을 때
        {
            if (isStart) PlayClickSound();

            NoteMove nearest = FindNote(4); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }
        if (Input.GetKeyDown(key5)) // 5번 키를 눌렀을 때
        {
            if (isStart) PlayClickSound();

            NoteMove nearest = FindNote(5); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }
        if (Input.GetKeyDown(key6)) // 6번 키를 눌렀을 때
        {
            if (isStart) PlayClickSound();

            NoteMove nearest = FindNote(6); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }

        // 실제 삭제 처리
        foreach (var note in toRemove)
        {
            if (note != null && activeNotes.Contains(note))
                activeNotes.Remove(note); // 리스트에서 제거
        }
    }

    NoteMove FindNote(int line)
    {
        float minDiff = float.MaxValue; // 가장 작은 차이를 찾기 위해 초기값 설정
        NoteMove best = null; // best: 지금까지 찾은 가장 가까운 노트

        foreach (var note in activeNotes) // activeNotes에 있는 모든 노트를 돌면서
        {
            // targetTime -> headTime으로 변경!
            float diff = (note.headTime - Time.time) * 1000f; // 오차 (ms)

            // 유효한 판정 범위 내에 있는 노트 + 해당 라인에 있는 노트만 고려
            if (diff >= -150f && diff <= 700f && note.lineNumber == line)
            {
                float abs = Mathf.Abs(diff); // 절대값으로 가장 가까운 걸 찾기

                if (abs < minDiff)
                {
                    minDiff = abs; // 가장 최소 오차를 찾기 위해 minDiff 갱신
                    best = note; // best: 오차가 가장 작은 노트 = 가장 가까운 노트
                }
            }
        }

        return best;
    }
    
    void PlayClickSound()
    {
        if (note != null)
        {
            sfxAudioSource.PlayOneShot(note, 1f);
        }
    }
}