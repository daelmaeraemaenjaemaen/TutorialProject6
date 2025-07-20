using UnityEngine;
using System;
public class Metronome : MonoBehaviour
{
    public float bpm = 30f; // BPM 설정
    private float interval; // 박자 간 시간
    private float nextTick;

    public static event Action OnTick; // 틱 이벤트

    void Start()
    {
        interval = 120f / bpm; // 1박자당 시간 계산
        nextTick = Time.time + interval;
    }

    void Update()
    {
        while (Time.time >= nextTick)
        {
            OnTick?.Invoke(); // 이벤트 호출
            nextTick += interval; // 다음 틱 시간 처리
        }
    }
}
