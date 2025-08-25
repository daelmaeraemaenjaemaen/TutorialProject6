using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-100)]
public class Design : MonoBehaviour
{
    public static Design I;

    [Header("노트 스프라이트")]
    [SerializeField] private Sprite[] shortSkins = new Sprite[3];
    [SerializeField] private Sprite[] longSkins  = new Sprite[3];

    [Header("P1 라인 3개")]
    [SerializeField] private SpriteRenderer[] p1Lines = new SpriteRenderer[3];

    [Header("P2 라인 3개")]
    [SerializeField] private SpriteRenderer[] p2Lines = new SpriteRenderer[3];

    [System.Serializable] public class TrioPalette { public Color[] colors = new Color[3]; }

    [Header("P1 라인 팔레트 3세트")]
    [SerializeField] private TrioPalette[] p1LinePalettes = new TrioPalette[3] { new(), new(), new() };

    [Header("P2 라인 팔레트 3세트")]
    [SerializeField] private TrioPalette[] p2LinePalettes = new TrioPalette[3] { new(), new(), new() };

    void Awake() => I = this;

    static Color Opaque(Color c) => new Color(c.r, c.g, c.b, 1f);

    Sprite GetShort(int idx) => shortSkins[Mathf.Clamp(idx, 0, shortSkins.Length - 1)];
    Sprite GetLong (int idx) => longSkins [Mathf.Clamp(idx, 0, longSkins .Length - 1)];

    void ApplyP1Lines(int setIndex)
    {
        int si = Mathf.Clamp(setIndex, 0, p1LinePalettes.Length - 1);
        var pal = p1LinePalettes[si].colors;
        for (int i = 0; i < p1Lines.Length && i < pal.Length; i++)
            if (p1Lines[i]) p1Lines[i].color = Opaque(pal[i]);
    }

    void ApplyP2Lines(int setIndex)
    {
        int si = Mathf.Clamp(setIndex, 0, p2LinePalettes.Length - 1);
        var pal = p2LinePalettes[si].colors;
        for (int i = 0; i < p2Lines.Length && i < pal.Length; i++)
            if (p2Lines[i]) p2Lines[i].color = Opaque(pal[i]);
    }

    public void ApplyAllRuntime()
    {
#if UNITY_6000_0_OR_NEWER
        var notes = Object.FindObjectsByType<NoteMove>(FindObjectsInactive.Include, FindObjectsSortMode.None);
#else
        var notes = Object.FindObjectsOfType<NoteMove>(true);
#endif
        foreach (var nm in notes) ApplyNoteSkin(nm);

        ApplyP1Lines(PlayerSettings.backNum);
        ApplyP2Lines(PlayerSettings.backNum2);
    }

    public void ApplyNoteSkin(NoteMove nm)
    {
        if (!nm) return;

        bool isP2 = nm.lineNumber >= 4;
        var shortSprite = GetShort(isP2 ? PlayerSettings.shortNum2 : PlayerSettings.shortNum);
        var longSprite  = GetLong (isP2 ? PlayerSettings.longNum2  : PlayerSettings.longNum );

        var shortTf = nm.transform.Find("ShortNote");
        if (shortTf)
        {
            var sr = shortTf.GetComponent<SpriteRenderer>();
            if (sr) sr.sprite = shortSprite;
            else { var img = shortTf.GetComponent<Image>(); if (img) img.sprite = shortSprite; }
        }

        var longTf = nm.transform.Find("LongNote");
        if (longTf)
        {
            var sr = longTf.GetComponent<SpriteRenderer>();
            if (sr) sr.sprite = longSprite;
            else { var img = longTf.GetComponent<Image>(); if (img) img.sprite = longSprite; }
        }
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (p1LinePalettes != null)
            foreach (var p in p1LinePalettes)
                if (p != null && p.colors != null)
                    for (int i = 0; i < p.colors.Length; i++) p.colors[i] = Opaque(p.colors[i]);

        if (p2LinePalettes != null)
            foreach (var p in p2LinePalettes)
                if (p != null && p.colors != null)
                    for (int i = 0; i < p.colors.Length; i++) p.colors[i] = Opaque(p.colors[i]);
    }
#endif
}
