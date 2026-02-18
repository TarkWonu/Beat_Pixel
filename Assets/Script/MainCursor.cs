using System;
using UnityEngine;

public class MainCursor : MonoBehaviour
{
    private int cursorIndex;


    [SerializeField] private MainMenuContext[] cursorContexts;

    private void CursorMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)&&cursorIndex>0)
        {
            cursorIndex --;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && cursorIndex < cursorContexts.Length-1)
        {
            cursorIndex++;
        }

        transform.position = cursorContexts[cursorIndex].cursorPos.position;
    }

    private void RunMenuContext()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            cursorContexts[cursorIndex].mainMenuText.Context();
        }
        
    }


    private void Update()
    {
        CursorMove();
        RunMenuContext();
    }
}

[Serializable]
public class MainMenuContext
{
    public Transform cursorPos;
    public MainMenuText mainMenuText;
}
