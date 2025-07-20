using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    public GameObject notePrefab; // 노트 프리팹
    public Transform spawnPoint; // 노트가 생성될 위치
    public float noteTarget = 2.0f; // 쳐야 할 시간
    [SerializeField] private Transform targetLine;
    [SerializeField] private Transform judgeLine;
    [SerializeField] private JudgeText judgeTextDisplay;
    private bool isSubscribed = false; // 중복 방지
    public NoteInput noteInput; // NoteInput 컴포넌트를 연결
    
    
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
        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity); // 노트 복제
        NoteMove noteScript = note.GetComponent<NoteMove>();
        noteScript.targetTime = Time.time + noteTarget; // 다음 노트 쳐야 할 시간
        noteScript.targetLine = targetLine;
        noteScript.judgeLine = judgeLine;
        noteScript.judgeTextDisplay = judgeTextDisplay;
        
        noteInput.activeNotes.Add(noteScript); // 리스트에 등록
    }
    
    
}
