using UnityEngine;

public enum NoteType
{
    Short,
    Long
}

public class NoteMove : MonoBehaviour
{
    public float headTime;
    public float tailTime; // 롱노트 끝 판정 시간
    public float spawnTime; // 노트가 생긴 시간

    private bool judged = false;
    public bool IsJudged => judged;

    public NoteJudge result { get; private set; }
    public int lineNumber;

    // 롱노트 기능
    public NoteType noteType = NoteType.Short;

    private float nextTickTime = 0f; // 다음 틱까지의 시간
    public float tickInterval; // 틱 간격
    public float tickNumber;
    
    // 이동 속도, 이것만 변경하면 관련 수치 변경이 자동으로 이루어지게 static으로 변경했습니다
    // 옵션에서 이 수치를 0.1 단위로 변경할 수 있게 하면 됩니다
    public static float moveSpeed = 7.2f; 

    public GameText gameTextDisplay;

    void Update()
    {
        if (headTime <= 0f)
            return;

        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        // 롱노트 길이 조정(롱노트 길이 = 초당 이동거리 * 틱 간격 * 틱 수)
        if (noteType == NoteType.Long) transform.localScale = new Vector3(1f, tickInterval * moveSpeed * tickNumber, 1f);
        
        // 롱노트 자동 Miss 판정
        if (noteType == NoteType.Long && !judged)
        {
            if ((Time.time - headTime) * 1000f > Judge.MissLate)
            {
                result = NoteJudge.Miss;
                gameTextDisplay?.Result(result, lineNumber, 0);
                Debug.Log(result);
                judged = true;  // Head 판정 완료 표시
                return;
            }
        }

        // Body~Tail 틱별 판정
        if (noteType == NoteType.Long && judged) // Head가 먼저 판정되도록 함
        {
            if (nextTickTime == 0f) nextTickTime = headTime;

            while (Time.time >= nextTickTime && nextTickTime < tailTime)
            {
                if (Input.GetKey(NoteInput.getLineKey(lineNumber)))
                {
                    NoteJudge tickJudge = Judge.Judgement(Time.time, nextTickTime);
                    result = (tickJudge == NoteJudge.Miss) ? NoteJudge.Miss : NoteJudge.Perfect;
                }
                else
                {
                    result = NoteJudge.Miss;
                }
                gameTextDisplay?.Result(result, lineNumber, (int)tickNumber);
                Debug.Log("L." + result);

                nextTickTime += tickInterval;
            }

            if (Time.time >= tailTime)
            {
                Destroy(gameObject);
            }
        }

        // 단노트 자동 Miss 판정
        if (noteType == NoteType.Short && !IsJudged && (headTime - Time.time) * 1000f < Judge.FMm)
        {
            TryHit(Time.time);
        }
    }

    public void TryHit(float inputTime)
    {
        // 단노트 판정
        if (noteType == NoteType.Short)
        {
            if (judged) return;
            judged = true;
            result = Judge.Judgement(inputTime, headTime);
            gameTextDisplay.Result(result, lineNumber, 0);
            Destroy(gameObject);
            return;
        }

        // 롱노트 판정
        if (noteType == NoteType.Long && !judged)
        {
            judged = true;
            result = Judge.Judgement(inputTime, headTime);
            gameTextDisplay?.Result(result, lineNumber, 0);
            Debug.Log(result);
        }
    }

    public System.Action<NoteMove> onDestroyed;
    void OnDestroy()
    {
        onDestroyed?.Invoke(this);
    }
}
