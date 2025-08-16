using UnityEngine;
public class Combo : MonoBehaviour
{
    private static int nowCombo;
    public static int maxCombo { get; private set; }

    public static void ComboReset()
    {
        nowCombo = 0;
        maxCombo = 0;
    }

    public static int UpdateCombo(bool isHit)
    {
        if (isHit)
        {
            nowCombo++;
            if (maxCombo < nowCombo) maxCombo = nowCombo;
        }
        else nowCombo = 0;
        return nowCombo;
    }
}