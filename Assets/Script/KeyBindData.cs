using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum KeyAction{NoteA,NoteB,Switch}

[Serializable]
public class SaveKeyData
{
    public List<KeyBindContext> keys = new List<KeyBindContext>();

    public SaveKeyData(Dictionary<KeyAction,KeyCode> KeyDic)
    {
        foreach(var pair in KeyDic)
        {
            keys.Add(new KeyBindContext(pair.Key,pair.Value));
        }
    }
}

[Serializable]
public class KeyBindContext
{
    public KeyAction keyAction;
    public KeyCode keyCode;

    public KeyBindContext(KeyAction keyAction,KeyCode keyCode)
    {
        this.keyAction = keyAction;
        this.keyCode = keyCode;
    }
}

public static class KeyBindData
{
    

    public static Dictionary<KeyAction,KeyCode> Keys = new Dictionary<KeyAction, KeyCode>();
}




