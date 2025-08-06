using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    public float noteTarget = 2.0f; // 노트가 판정선(judgeLine)에 도착하는데 걸리는 시간

    public GameObject shortNotePrefab; // 단노트 프리팹
    public GameObject longNotePrefab; // 롱노트 프리팹

    [SerializeField] private JudgeText judgeTextDisplay;

    private bool _isSubscribed = false; // 중복 방지

    public NoteInput noteInput; // NoteInput 컴포넌트를 연결

    private int _beatCounter = 0;
    public int beatsPerSpawn = 5; // 몇 박자마다 노트 생성할지

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
        Transform spawnpoint;
        Transform judgeLine;

        if (UnityEngine.Random.value < 0.5f)
        {
            // 롱노트
            spawnpoint = GameObject.Find("LongSpawnPoint")?.transform;
            judgeLine = GameObject.Find("JudgeLine")?.transform;
            prefab = longNotePrefab;
            isLong = true;
        }
        else
        {
            // 단노트
            spawnpoint = GameObject.Find("ShortSpawnPoint")?.transform;
            judgeLine = GameObject.Find("JudgeLine")?.transform;
            prefab = shortNotePrefab;
            isLong = false;
        }

        GameObject note = Instantiate(prefab, spawnpoint.position, Quaternion.identity); // 노트 복제

        NoteMove noteScript = note.GetComponent<NoteMove>();
        noteScript.noteType = isLong ? NoteType.Long : NoteType.Short;
        noteScript.judgeLine = judgeLine;
        noteScript.judgeTextDisplay = judgeTextDisplay;

        // headTime, tailTime 자동 계산
        float headTime = Time.time + noteTarget;

        if (isLong)
        {
            // 롱노트 길이를 프리팹 구조에서 자동 계산
            Transform head = note.transform.Find("Head"); // Head 오브젝트
            Transform tail = note.transform.Find("Tail"); // Tail 오브젝트

            float noteLengthY = 1.0f; // 기본값(실패시)

            if (head != null && tail != null)
            {
                noteLengthY = Mathf.Abs(head.localPosition.y - tail.localPosition.y); // Head~Tail Y좌표 차이로 길이 자동 계산
            }

            float moveSpeed = noteScript.moveSpeed; // NoteMove의 moveSpeed와 동일하게

            float tailTime = headTime + (noteLengthY / moveSpeed);

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
