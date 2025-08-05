using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CategorySelector : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI CategoryName;
    private string code;

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
}