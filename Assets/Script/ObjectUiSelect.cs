using UnityEngine;
using UnityEngine.EventSystems;

public enum objectType{Sky,Wall,Floor,Obj}

public class ObjectUiSelect : MonoBehaviour,IPointerClickHandler
{
    [SerializeField] public objectType type;
    

    private Builder builder;

    void Awake()
    {
        builder = FindFirstObjectByType<Builder>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        builder.buildSprite = this.GetComponent<SpriteRenderer>().sprite;
        builder.objectType = type;
        
    }
}