using UnityEngine;
using System;
public class Metronome : MonoBehaviour
{
    public static float bpm = 30f; // BPM 설정
    private float interval; // 박자 간 시간
    private float nextTick;

    public static event Action OnTick; // 틱 이벤트

    void Start()
    {
        interval = 120f / bpm; // 1박자당 시간 계산
        nextTick = Time.time + interval;

        Application.targetFrameRate = 60; //설정에서 변경 가능? 일단 이거 가지고 배속 계산
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
