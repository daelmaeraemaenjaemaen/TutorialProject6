using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    public GameObject shortNotePrefab; // 단노트 프리팹
    public GameObject longNotePrefab; // 롱노트 프리팹

    [SerializeField] private JudgeText judgeTextDisplay;

    private bool _isSubscribed = false; // 중복 방지

    public NoteInput noteInput; // NoteInput 컴포넌트를 연결

    private int _beatCounter = 0;
    public int beatsPerSpawn = 8; // 몇 박자마다 노트 생성할지

    void OnEnable() // 오브젝트를 활성화 시켰을 때
    {
        if (!_isSubscribed)
        {
            Metronome.OnTick += SpawnNote; // SpawnNote() 함수 실행
            _isSubscribed = true;
        }
    }

    void OnDisable() // 오브젝트를 비활성화 시켰을 때
    {
        if (_isSubscribed)
        {
            Metronome.OnTick -= SpawnNote; // SpawnNote() 함수 해제
            _isSubscribed = false;
        }
    }

    void SpawnNote()
    {
        _beatCounter++;
        if (_beatCounter % beatsPerSpawn != 0)
            return;

        GameObject prefab;
        bool isLong;
        Transform spawnpoint = GameObject.Find("SpawnPoint")?.transform;
        Transform judgeLine = GameObject.Find("JudgeLine")?.transform;

        if (Random.value < 0.5f)
        {
            // 롱노트
            prefab = longNotePrefab;
            isLong = true;
        }
        else
        {
            // 단노트
            prefab = shortNotePrefab;
            isLong = false;
        }

        GameObject note = Instantiate(prefab, spawnpoint.position, Quaternion.identity); // 노트 복제

        NoteMove noteScript = note.GetComponent<NoteMove>();
        noteScript.noteType = isLong ? NoteType.Long : NoteType.Short;
        noteScript.tickInterval = 30 / Metronome.bpm;
        noteScript.tickNumber = isLong ? 4 : 0;
        noteScript.judgeLine = judgeLine;
        noteScript.judgeTextDisplay = judgeTextDisplay;

        // headTime, tailTime 자동 계산
        // 롱노트 길이를 기반으로 tailtime을 계산하는 것은 부정확하다고 여겨 틱 기반으로 교체했습니다
        float headTime = Time.time + 10 / NoteMove.moveSpeed;
        float tailTime = headTime + noteScript.tickInterval * noteScript.tickNumber;

        if (isLong)
        {
            noteScript.headTime = headTime;
            noteScript.tailTime = tailTime;
        }
        else
        {
            // 단노트는 tailTime = headTime
            noteScript.headTime = headTime;
            noteScript.tailTime = headTime;
        }

        noteInput.activeNotes.Add(noteScript); // 리스트에 등록
    }
}
