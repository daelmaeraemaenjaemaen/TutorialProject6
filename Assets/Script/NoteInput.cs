using UnityEngine;
using System.Collections.Generic;

public class NoteInput : MonoBehaviour
{
    public List<NoteMove> activeNotes; // 현재 화면에 존재하는 노트 리스트

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스 키를 눌렀을 때
        {
            NoteMove nearest = FindNote(); // 근처에 노트가 있는지 확인
            if (nearest != null) // 근처에 노트가 있다면
            {
                nearest.TryHit(Time.time); // 입력 시간으로 판정
                activeNotes.Remove(nearest); // 처리한 노트는 리스트에서 삭제
            }
        }
    }

    NoteMove FindNote()
    {
        float minDiff = 0.15f; // 150ms 안
        NoteMove best = null; // best: 지금까지 찾은 가장 가까운 노트

        foreach (var note in activeNotes) // activeNotes에 있는 모든 노트를 돌면서
        {
            float diff = Mathf.Abs(note.targetTime - Time.time); // 오차
            if (diff < minDiff)
            {
                minDiff = diff; // 가장 최소 오차를 찾기 위해 minDiff 갱신
                best = note; // best: 오차가 가장 작은 노트 = 가장 가까운 노트
            }
        }
        
        return best;
    }
}
