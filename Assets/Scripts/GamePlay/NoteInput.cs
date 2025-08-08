using UnityEngine;
using System.Collections.Generic;

public class NoteInput : MonoBehaviour
{
    public List<NoteMove> activeNotes; // 현재 화면에 존재하는 노트 리스트

    void Update()
    {
        List<NoteMove> toRemove = new List<NoteMove>(); // 삭제 대상 저장

        // 사용자 입력 판정
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 키를 눌렀을 때
        {
            NoteMove nearest = FindNote(); // 근처에 노트가 있는지 확인
            // 단노트는 !IsJudged, 롱노트는 무조건 TryHit
            if (nearest != null && (nearest.noteType == NoteType.Long || !nearest.IsJudged))
            {
                nearest.TryHit(Time.time);

                if (nearest.noteType == NoteType.Short)
                    toRemove.Add(nearest);
            }
        }

        // 실제 삭제 처리
        foreach (var note in toRemove)
        {
            if (note != null && activeNotes.Contains(note))
                activeNotes.Remove(note); // 리스트에서 제거
        }
    }

    NoteMove FindNote()
    {
        float minDiff = float.MaxValue; // 가장 작은 차이를 찾기 위해 초기값 설정
        NoteMove best = null; // best: 지금까지 찾은 가장 가까운 노트

        foreach (var note in activeNotes) // activeNotes에 있는 모든 노트를 돌면서
        {
            // targetTime -> headTime으로 변경!
            float diff = (note.headTime - Time.time) * 1000f; // 오차 (ms)

            // 유효한 판정 범위 내에 있는 노트만 고려
            if (diff >= -150f && diff <= 700f)
            {
                float abs = Mathf.Abs(diff); // 절대값으로 가장 가까운 걸 찾기

                if (abs < minDiff)
                {
                    minDiff = abs; // 가장 최소 오차를 찾기 위해 minDiff 갱신
                    best = note; // best: 오차가 가장 작은 노트 = 가장 가까운 노트
                }
            }
        }

        return best;
    }
}