using UnityEngine;
using System.IO;


public class KeyBindManager : MonoBehaviour
{

    string _filePath = Application.dataPath + "/Json/" + "AddressDataBase.json";
    string _resetPath = Application.dataPath + "/Json/" + "ResetDataBase.json";
    private int key = -1;

    void Awake()
    {
        LoadKeyData(_filePath);


    }





    public void ResetKeyData()
    {
        LoadKeyData(_resetPath);
    }

    
    public void SaveKeyData()
    {
        SaveKeyData keydata = new SaveKeyData(KeyBindData.Keys);

        string jData = JsonUtility.ToJson(keydata,true);
        File.WriteAllText(_filePath,jData);

        print(_filePath);
    }

    private void LoadKeyData(string path)
    {
        string json = File.ReadAllText(path);
        SaveKeyData LoadData = JsonUtility.FromJson<SaveKeyData>(json);

        

        KeyBindData.Keys.Clear();

        for(int i = 0; i < LoadData.keys.Count; i++)
        {
            KeyBindData.Keys.Add(LoadData.keys[i].keyAction,LoadData.keys[i].keyCode);
        }

        
    }


    private void OnGUI()
    {
        Event keyEvent = Event.current;

        if (keyEvent.isKey&&key>=0)
        {
            if(keyEvent.keyCode != KeyCode.Escape && !KeyBindData.Keys.ContainsValue(keyEvent.keyCode))
            {
                KeyBindData.Keys[(KeyAction)key] = keyEvent.keyCode;
            }
            
            key = -1;
        }
    }

    public void ChangeKey(int num)
    {
        key = num;
    }
}