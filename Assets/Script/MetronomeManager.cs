using UnityEngine;

public class MetronomeManager : MonoBehaviour
{
    public static MetronomeManager Instance;
    public float bpm = 120f;

    public float TickInterval => 60f / bpm / 2f;
    public event Action<float> OnTick;
    
    private float nextTick;

    private void Awake()
    {
        Instance = this;
        nextTick = Time.time;
    }

    void Update()
    {
        if (Time.time >= nextTick)
        {
            OnTick?.Invoke(Time.time);
            nextTick = TickInterval;
        }
    }
}
