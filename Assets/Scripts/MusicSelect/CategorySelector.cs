using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Audio;

public class CategorySelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("오디오")]
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioClip click;
    
    [SerializeField] private TextMeshProUGUI CategoryName;
    private string code;

    private void Start()
    {
        if (sfxAudioSource == null)
        {
            GameObject obj = GameObject.Find("AudioSource_sfx");
            if (obj != null)
                sfxAudioSource = obj.GetComponent<AudioSource>();
        }

        if (click == null)
            click = Resources.Load<AudioClip>("sfx/click");

        if (sfxGroup == null)
        {
            AudioMixerGroup[] groups = Resources.FindObjectsOfTypeAll<AudioMixerGroup>();
            foreach (var g in groups)
            {
                if (g.name == "SFX")
                {
                    sfxGroup = g;
                    break;
                }
            }
        }

        if (sfxGroup != null && sfxAudioSource != null)
            sfxAudioSource.outputAudioMixerGroup = sfxGroup;
    }
    
    public void setCategorySelector(Category c)
    {
        CategoryName.text = c.getName();
        code = c.getCode();
    }

    // 추후 선택된 카테고리의 디자인을 바꿔야 한다면 update 함수 추가해 구현

    public void OnPointerDown(PointerEventData eventData)
    {
        MusicSelect.selectedCategory = code;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("Pointer Up");
    }
    
    public void Click()
    {
        if (click != null && sfxAudioSource != null)
            sfxAudioSource.PlayOneShot(click, 1f);
    }
}