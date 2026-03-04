using UnityEngine;


public class GameSettingSaveManager : MonoBehaviour
{
    
     
    string _filePath = "AddressSettingData.Json";

    void Awake()
    {
        LoadSettingData();
    }

    

    public void ResetSettingData()
    {
        GameSettingData.NoteOffset = 0;
        GameSettingData.NoteSpeed = 1;
        
    }
    public void SaveSettingData()
    {
        GameSettingSaveData saveData = new GameSettingSaveData(GameSettingData.NoteSpeed,GameSettingData.NoteOffset);
        DataSaveManager.SaveData<GameSettingSaveData>(_filePath,saveData);


    }

    public void LoadSettingData()
    {
        GameSettingSaveData LoadData = DataSaveManager.LoadData<GameSettingSaveData>(_filePath);

        if(LoadData == null)
        {
            ResetSettingData();
            return;
        }

        GameSettingData.NoteOffset = LoadData.NoteOffset;
        GameSettingData.NoteSpeed = LoadData.NoteSpeed;
        
    }
}