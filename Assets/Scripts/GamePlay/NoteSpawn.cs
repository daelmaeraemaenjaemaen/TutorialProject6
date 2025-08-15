using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    [SerializeField] private GameObject shortNotePrefab; // 단노트 프리팹
    [SerializeField] private GameObject longNotePrefab; // 롱노트 프리팹

    [SerializeField] private JudgeText judgeTextDisplayL;
    [SerializeField] private JudgeText judgeTextDisplayR;

    [SerializeField] private NoteInput noteInput; // NoteInput 컴포넌트를 연결

    public void SpawnNote(int line, float tickNumber) //단노트는 tickNumber == 0
    {
        GameObject prefab;
        bool isLong;
        Transform spawnpoint = GameObject.Find("SpawnPoint" + line.ToString())?.transform; //SpawnPoint1~SpawnPoint6

        if (tickNumber == 0)
        {
            // 단노트
            prefab = shortNotePrefab;
            isLong = false;
        }
        else
        {
            // 롱노트
            prefab = longNotePrefab;
            isLong = true;
        }

        GameObject note = Instantiate(prefab, spawnpoint.position, Quaternion.identity); // 노트 복제

        NoteMove noteScript = note.GetComponent<NoteMove>();
        noteScript.noteType = isLong ? NoteType.Long : NoteType.Short;
        noteScript.tickInterval = 30 / Metronome.bpm;
        noteScript.tickNumber = tickNumber;
        noteScript.judgeTextDisplay = line <= 3 ? judgeTextDisplayL : judgeTextDisplayR;
        noteScript.lineNumber = line;

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
