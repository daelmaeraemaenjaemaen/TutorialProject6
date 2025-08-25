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

    [Header("효과")]
    [SerializeField] private GameObject L1;
    [SerializeField] private GameObject L2;
    [SerializeField] private GameObject L3;
    [SerializeField] private GameObject L4;
    [SerializeField] private GameObject L5;
    [SerializeField] private GameObject L6;
    
    [Header("오디오")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioClip note;
    
    private bool isStart;

    KeyCode[] keys = new KeyCode[6];
    GameObject[] lights = new GameObject[6];

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

        lights[0] = L1; lights[1] = L2; lights[2] = L3;
        lights[3] = L4; lights[4] = L5; lights[5] = L6;

        SyncKeysFromSettings();

        StartCoroutine(Delay());
    }

    void SyncKeysFromSettings()
    {
        key1 = PlayerSettings.p1k1;
        key2 = PlayerSettings.p1k2;
        key3 = PlayerSettings.p1k3;
        key4 = PlayerSettings.p2k1;
        key5 = PlayerSettings.p2k2;
        key6 = PlayerSettings.p2k3;

        keys[0] = key1; keys[1] = key2; keys[2] = key3;
        keys[3] = key4; keys[4] = key5; keys[5] = key6;
    }

    IEnumerator Delay()
    {
        yield return new WaitForSecondsRealtime(3.5f);
        isStart = true;
    }
    void Update()
    {
        if (activeNotes == null) return;

        List<NoteMove> toRemove = new List<NoteMove>();

        for (int i = 0; i < 6; i++)
        {
            var light = lights[i];
            if (light) light.SetActive(Input.GetKey(keys[i]));
        }

        for (int i = 0; i < 6; i++)
        {
            if (Input.GetKeyDown(keys[i]))
            {
                if (isStart) PlayClickSound();

                int line = i + 1;
                NoteMove nearest = FindNote(line);
                if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
                {
                    nearest.TryHit(Time.time);
                    if (nearest.noteType == NoteType.Short)
                        toRemove.Add(nearest);
                }
            }
        }

        foreach (var note in toRemove)
        {
            if (note != null && activeNotes.Contains(note))
                activeNotes.Remove(note);
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
