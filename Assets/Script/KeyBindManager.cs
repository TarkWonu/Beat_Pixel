using UnityEngine;
using System.IO;


public class KeyBindManager : MonoBehaviour
{

    string _filePath = "AddressKeyData.json";
    
    private int key = -1;

    void Awake()
    {
        
        LoadKeyData();
        

    }

    void Start()
    {
        
    }




    public void ResetKeyData()
    {
        KeyBindData.Keys.Clear();

        KeyBindData.Keys.Add(KeyAction.NoteA,KeyCode.LeftArrow);
        KeyBindData.Keys.Add(KeyAction.NoteB,KeyCode.RightArrow);
        KeyBindData.Keys.Add(KeyAction.Switch,KeyCode.Space);
        SaveKeyData();
    }

    
    public void SaveKeyData()
    {
        SaveKeyData keydata = new SaveKeyData(KeyBindData.Keys);

        DataSaveManager.SaveData<SaveKeyData>(_filePath,keydata);
    }

    private void LoadKeyData()
    {

        
        SaveKeyData LoadData = DataSaveManager.LoadData<SaveKeyData>(_filePath);

        if(LoadData == null)
        {
            ResetKeyData();
            return;
        }

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