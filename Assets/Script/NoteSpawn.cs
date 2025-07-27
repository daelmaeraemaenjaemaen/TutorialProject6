using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    public float noteTarget = 2.0f; // 쳐야 할 시간
    
    public GameObject shortNotePrefab; // 단노트 프리팹
    public GameObject longNotePrefab; // 롱노트 프리팹
    
    [SerializeField] private JudgeText judgeTextDisplay;
    
    private bool isSubscribed = false; // 중복 방지
    
    public NoteInput noteInput; // NoteInput 컴포넌트를 연결
    
    private int beatCounter = 0;
    public int beatsPerSpawn = 5; // 몇 박자마다 노트 생성할지
    
    
    void OnEnable() // 오브젝트를 활성화 시켰을 때
    {
        if (!isSubscribed)
        {
            Metronome.OnTick += SpawnNote; // SpawnNote() 함수 실행
            isSubscribed = true;
        }
    }

    void OnDisable() // 오브젝트를 비활성화 시켰을 때
    {
        if (isSubscribed)
        {
            Metronome.OnTick -= SpawnNote; // SpawnNote() 함수 해제
            isSubscribed = false;
        }
    }

    void SpawnNote()
    {
        beatCounter++;
        if (beatCounter % beatsPerSpawn != 0)
            return;

        GameObject prefab;
        bool isLong;
        Transform spawnpoint;
        Transform targetLine;
        Transform judgeLine;
        
        if (UnityEngine.Random.value < 0.5f)
        {
            // 롱노트
            spawnpoint = GameObject.Find("LongSpawnPoint")?.transform;
            targetLine = GameObject.Find("LongTargetLine")?.transform;
            judgeLine = GameObject.Find("LongJudgeLine")?.transform;
            prefab = longNotePrefab;
            isLong = true;
        }
        
        else
        {
            // 단노트
            spawnpoint = GameObject.Find("ShortSpawnPoint")?.transform;
            targetLine = GameObject.Find("ShortTargetLine")?.transform;
            judgeLine = GameObject.Find("ShortJudgeLine")?.transform;
            prefab = shortNotePrefab;
            isLong = false;
        }
        
        GameObject note = Instantiate(prefab, spawnpoint.position, Quaternion.identity); // 노트 복제
        
        NoteMove noteScript = note.GetComponent<NoteMove>();
        noteScript.noteType = isLong ? NoteType.Long : NoteType.Short;
        noteScript.targetTime = Time.time + noteTarget; // 다음 노트 쳐야 할 시간
        noteScript.targetLine = targetLine;
        noteScript.judgeLine = judgeLine;
        noteScript.judgeTextDisplay = judgeTextDisplay;
        
        noteInput.activeNotes.Add(noteScript); // 리스트에 등록
    }
}
