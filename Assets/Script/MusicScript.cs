using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicScript",menuName ="Music/MusicData")]
public class MusicScript : ScriptableObject
{

    [Header("사운드 관련 설정")]
    public int bpm;

    public float[] beats;
    public AudioClip Music;
    public AudioClip HitSound;

    [Header("Json 저장 관련 설정")]

    [SerializeField] private string jsonFileName;

    [ContextMenu("Save Json Data")]
    void SaveData()
    {
        SheetContext savedata = new SheetContext(beats);
        string path = Path.Combine(Application.dataPath+"/Json", jsonFileName+".json");
        string json = JsonUtility.ToJson(savedata, true);
        
        File.WriteAllText(path, json);

    }

    [ContextMenu("From Json Data")]
    void LoadDataFromJson()
    {
        
        string path = Path.Combine(Application.dataPath+"/Json", jsonFileName+".json");
        
        string jsonData = File.ReadAllText(path);
        
        SheetContext readData = JsonUtility.FromJson<SheetContext>(jsonData);

        this.beats = readData.beats;
    }
}

public class SheetContext
{
    public float[] beats;

    public SheetContext(float[] beats)=>this.beats = beats;
}