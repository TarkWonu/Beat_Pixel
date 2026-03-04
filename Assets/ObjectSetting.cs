using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class ObjectSetting : MonoBehaviour
{
    private Image pixel;
    private TMP_Text title;
    private Image icon;
    private TMP_Text accuracy;

    [SerializeField] Sprite[] iconlist;

    public void Init(Texture2D texture,string title,float accuracy)
    {
        this.pixel = transform.GetChild(0).GetComponent<Image>();
        this.title = transform.GetChild(1).GetComponent<TMP_Text>();
        this.accuracy = transform.GetChild(2).GetComponent<TMP_Text>();
        this.icon = transform.GetChild(3).GetComponent<Image>();
        


        Rect rect = new Rect(0, 0, texture.width, texture.height);
        this.pixel.sprite = Sprite.Create(texture,rect,new Vector2(0.5f,0.5f));

        this.title.text = title;

        
        icon.sprite = accuracy>=100 ? iconlist[0] : iconlist[1];

        this.accuracy.text = accuracy.ToString()+"%";

        GetComponent<RectTransform>().localScale = new Vector2(1,1);
        



    }

}
