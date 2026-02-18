using UnityEngine;
using TMPro;

public class KeyBindUi : MonoBehaviour
{

    [SerializeField] TMP_Text[] textList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < textList.Length; i++)
        {
            textList[i].text = KeyBindData.Keys[(KeyAction)i].ToString();
        }
    }
}
