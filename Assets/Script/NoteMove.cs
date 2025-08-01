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
    public Transform judgeLine; // 판정선

    private bool judged = false;
    public bool IsJudged => judged;

    public NoteJudge result { get; private set; }

    // 롱노트 기능
    public NoteType noteType = NoteType.Short;

    private float nextTickTime = 0f; // 다음 틱까지의 시간
    private float tickInterval = 0.2f; // 틱 간격

    public float moveSpeed = 5f;

    public JudgeText judgeTextDisplay;

    void Update()
    {
        if (headTime <= 0f || judgeLine == null)
            return;

        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        
        // 롱노트 자동 Miss 판정
        if (noteType == NoteType.Long && !judged)
        {
            if ((Time.time - headTime) * 1000f > Judge.MissLate)
            {
                result = NoteJudge.Miss;
                judgeTextDisplay?.Result(result);
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
                if (Input.GetKey(KeyCode.Space))
                {
                    NoteJudge tickJudge = Judge.Judgement(Time.time, nextTickTime);
                    result = (tickJudge == NoteJudge.Miss) ? NoteJudge.Miss : NoteJudge.Perfect;
                }
                else
                {
                    result = NoteJudge.Miss;
                }
                judgeTextDisplay?.ResultPrefixed(result, "L.");
                Debug.Log("L." + result);

                nextTickTime += tickInterval;
            }

            if (Time.time >= tailTime)
            {
                NoteJudge tailJudge = Input.GetKey(KeyCode.Space)
                    ? Judge.Judgement(Time.time, tailTime)
                    : NoteJudge.Miss;
                result = (tailJudge == NoteJudge.Miss) ? NoteJudge.Miss : NoteJudge.Perfect;
                judgeTextDisplay?.ResultPrefixed(result, "L.");
                Debug.Log("L." + result);
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
            judgeTextDisplay.Result(result);
            Destroy(gameObject);
            return;
        }

        // 롱노트 판정
        if (noteType == NoteType.Long && !judged)
        {
            judged = true;
            result = Judge.Judgement(inputTime, headTime);
            judgeTextDisplay?.Result(result);
            Debug.Log(result);
        }
    }

    public System.Action<NoteMove> onDestroyed;
    void OnDestroy()
    {
        onDestroyed?.Invoke(this);
    }
}
