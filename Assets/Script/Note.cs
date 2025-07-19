using UnityEngine;

public class Note : MonoBehaviour
{
    public NoteData data;
    public bool judged = false;
    private bool isHolding = false;
    private List<float> tickTimes = new();

    void Start()
    {
        if (data.isLongNote)
        {
            GenerateTickTimes();
            MetronomeManager.Instance.OnTick += OnTick;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(data.assignedKey))
        {
            if (!data.isLongNote && !judged)
            {
                NoteJudge judge = JudgeManager.Instance.JudgeNote(Time.time, data.targetTime);
                ScoreManager.Instance.ProcessJudge(judge);
                judged = true;
                Destroy(gameObject);
            }

            else
            {
                {
                    isHolding = true;
                }
            }
        }

        if (Input.GetKeyUp(data.assignedKey))
        {
            isHolding = false;
        }
        
        float fallSpeed = NoteManger
    }
}
