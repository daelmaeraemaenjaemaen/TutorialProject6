using UnityEngine;

public class NoteMove : MonoBehaviour
{
    public float targetTime;
    public Transform targetLine;
    
    private Vector3 start;
    private Vector3 end;
    private bool initialized = false;
    private bool judged = false;

    void Update()
    {        
        if (targetTime <= 0f || targetLine == null)
            return; // 아직 설정 안 됐으면 무시

        if (!initialized)
        {
            start = transform.position; // 시작 위치
            end = new Vector3(start.x, targetLine.position.y, 0f); // 종료 위치
            initialized = true;
        }
        
        float remaining = targetTime - Time.time;
        
        if (remaining <= -0.15f && !judged) // 150ms 이상 지나면 Miss
        {
            Debug.Log("Miss 처리됨");
            judged = true;
            Destroy(gameObject);
            return;
        }
        
        float totalTime = 2.0f;
        float t = 1f - (remaining / totalTime);
        transform.position = Vector3.Lerp(start, end, t); // 시작 위치부터 종료 위치까지 움직이게 하기
    }
    
    public void TryHit(float inputTime)
    {
        NoteJudge result = Judge.Judgement(inputTime, targetTime); // 판정
        Debug.Log("판정 결과: " + result); // 판정 결과 보여주기
    
        Destroy(gameObject); // 노트 제거
    }
}
