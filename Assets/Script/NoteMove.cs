using UnityEngine;


public class NoteMove : MonoBehaviour
{
    public float targetTime; // 도달해야 할 시간
    public float spawnTime; // 노트가 생긴 시간
    public float timeProgress; // 사이 시간이 얼마나 지났는지
    public Transform judgeLine; // 판정선
    public Transform targetLine; // 실제 내려갈 위치

    private Vector3 start; // 시작 위치
    private Vector3 end; // 끝 위치
    private bool initialized = false;
    private bool judged = false;
    public bool IsJudged => judged;
    
    public NoteJudge result { get; private set; }


    void Update()
    {
        if (targetTime <= 0f || targetLine == null || judgeLine == null)
            return; // 아직 설정 안 됐으면 무시

        if (!initialized)
        {
            start = transform.position; // 시작 위치
            end = targetLine.position; // 실제 이동할 최종 위치
            initialized = true;
        }

        float totalTime = 2.0f;
        float spawnTime = targetTime - totalTime; // 노트가 생성된 시간 계산
        float timeProgress = (Time.time - spawnTime) / totalTime; // 시간이 얼마나 지났는지 계산
        float t = Mathf.Clamp01(timeProgress); // timeProgress 0과 1 사이로 제한
        transform.position = Vector3.Lerp(start, end, t); // 시작 위치부터 종료 위치까지 움직이게 하기
    }

    public JudgeText judgeTextDisplay;
    
    public void TryHit(float inputTime)
    {
        //Debug.Log($"[TryHit 진입] Time={Time.time:F3}, targetTime={targetTime:F3}");
        
        if (judged)
        {
            //Debug.Log("[TryHit] 이미 판정된 노트입니다. 제거만 진행."); 
            Destroy(gameObject); // 중복 방지용
            return;
        }

        judged = true;

        result = Judge.Judgement(inputTime, targetTime); // 판정

        if (judgeTextDisplay != null)
        {
            judgeTextDisplay.Result(result);
        }
        
        //Debug.Log($"판정 결과: {result} | 입력: {inputTime:F3}, 목표: {targetTime:F3}, 차이: {(inputTime - targetTime) * 1000f:F1}ms");

        Destroy(gameObject); // 노트 제거
    }
    
    public System.Action<NoteMove> onDestroyed;
    void OnDestroy()
    {
        //Debug.Log($"[OnDestroy] {gameObject.name} 파괴됨");
        onDestroyed?.Invoke(this);
    }
    
}