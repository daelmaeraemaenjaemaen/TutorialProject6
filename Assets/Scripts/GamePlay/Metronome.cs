using UnityEngine;
using System;
public class Metronome : MonoBehaviour
{
    public static float bpm = 150f; // BPM 설정
    private float _interval; // 박자 간 시간
    private float _nextTick;

    public static event Action OnTick; // 틱 이벤트

    void Start()
    {
        _interval = 120f / bpm; // 1박자당 시간 계산
        _nextTick = Time.time + _interval;
    }

    void Update()
    {
        while (Time.time >= _nextTick)
        {
            OnTick?.Invoke(); // 이벤트 호출
            _nextTick += _interval; // 다음 틱 시간 처리
        }
    }
}
