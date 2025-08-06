using UnityEngine;

public enum NoteType
{
	Short,
	Long
}

public class NoteMove : MonoBehaviour
{
    public float targetTime; // 도달해야 할 시간
    public float timeProgress; // 사이 시간이 얼마나 지났는지
    public Transform judgeLine; // 판정선

    private bool judged = false;
    public bool IsJudged => judged;
    
    public NoteJudge result { get; private set; }
    
    // 롱노트 기능
    
    public NoteType noteType = NoteType.Short;
    
    private bool isHolding = false; // 롱노트를 누르고 있는지
    private float nextTickTime = 0f; // 다음 틱까지의 시간
    private float tickInterval = 0f; // 틱 간격
    public int tickNumber; //틱 수
    private bool hasBeenHit = false; // 롱노트를 입력했는지 여부

    void Update()
    {
        if (targetTime <= 0f || judgeLine == null)
            return; // 아직 설정 안 됐으면 무시

        float t = 10 / (NoteSpawn.noteTarget * Application.targetFrameRate); // 프레임당 이동거리 계산
        transform.position += new Vector3(0, -t, 0); // 시작 위치부터 종료 위치까지 움직이게 하기

        // 롱노트 틱 계산
        if (noteType == NoteType.Long && tickInterval == 0f)
        {
            // BPM * 비트 = 초당 노트 개수 * 240
            tickInterval = Metronome.bpm / 30;
        }
        
        // 롱노트 자동 파괴: targetTime을 기준으로 제거
        if (noteType == NoteType.Long && Time.time >= targetTime + tickNumber * tickInterval) 
        {
            if (!hasBeenHit)
            {
                result = NoteJudge.Miss;
                judgeTextDisplay?.ResultPrefixed(result, "L.");
            }

            Destroy(gameObject);
            return;
        }
        
        // 롱노트 판정
        if (noteType == NoteType.Long && isHolding && hasBeenHit)
        {
            if (Time.time >= nextTickTime)
            {
                result = NoteJudge.Perfect;
                judgeTextDisplay?.ResultPrefixed(result, "L.");
                nextTickTime += tickInterval;
            }
            
            if (!Input.GetKey(KeyCode.Space))
            {
                result = NoteJudge.Miss;
                judgeTextDisplay?.ResultPrefixed(result, "L.");
                isHolding = false;
                hasBeenHit = false;
            }
        }
    }

    public JudgeText judgeTextDisplay;
    
    public void TryHit(float inputTime)
    {
        if (judged && noteType == NoteType.Short)
        {
            Destroy(gameObject); // 중복 방지용
            return;
        }
        
        // 첫 판정만 일반 판정
        if (noteType == NoteType.Long)
        { 
            result = Judge.Judgement(inputTime, targetTime);
            judgeTextDisplay?.Result(result);
            isHolding = true;
            hasBeenHit = true;
            
            nextTickTime = Time.time + tickInterval;
            float timeSinceStart = inputTime - targetTime;
            float ticksPassed = Mathf.Floor(timeSinceStart / tickInterval);
            nextTickTime = targetTime + (ticksPassed + 1) * tickInterval;
        }
        
        else
        {
            if (judged) return;
            
            judged = true;
            result = Judge.Judgement(inputTime, targetTime); // 판정
            judgeTextDisplay.Result(result);
            Destroy(gameObject);
            return;
        }
        
        
    }
    
    public System.Action<NoteMove> onDestroyed;
    void OnDestroy()
    {
        isHolding = false;
        onDestroyed?.Invoke(this);
    }
    
}