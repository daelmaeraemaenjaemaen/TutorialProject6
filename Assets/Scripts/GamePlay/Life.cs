using UnityEngine;
public class Life : MonoBehaviour
{
    public static int lifeNum = 100;
    [SerializeField] private GameObject lifeBar1p;
    [SerializeField] private GameObject lifeBar2p;
    private float height1p;
    private float height2p;

    void Awake()
    {
        RectTransform rectTran1 = lifeBar1p.GetComponent<RectTransform>();
        RectTransform rectTran2 = lifeBar2p.GetComponent<RectTransform>();
        height1p = rectTran1.localScale.y;
        height2p = rectTran2.localScale.y;
    }

    public static void LifeReset()
    {
        lifeNum = 100;
    }

    public void SetLife(NoteJudge judge)
    {
        //perfect: +3% great: +2% good: +1% miss: -10%
        int i = 0;
        switch (judge)
        {
            case NoteJudge.FastMiss:
                i = -10;
                break;
            case NoteJudge.Miss:
                i = -10;
                break;
            case NoteJudge.Good:
                i = 1;
                break;
            case NoteJudge.Great:
                i = 2;
                break;
            case NoteJudge.Perfect:
                i = 3;
                break;
        }

        lifeNum += i;
        if (lifeNum >= 100) lifeNum = 100;
        else if (lifeNum <= 0) lifeNum = 0;
        RectTransform rectTran1 = lifeBar1p.GetComponent<RectTransform>();
        RectTransform rectTran2 = lifeBar2p.GetComponent<RectTransform>();
        rectTran1.localScale = new Vector3(1f, height1p * lifeNum / 100, 1f);
        rectTran2.localScale = new Vector3(1f, height2p * lifeNum / 100, 1f);
    }
}