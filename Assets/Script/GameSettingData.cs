using UnityEngine;

public static class GameSettingData
{
    public static float NoteSpeed ;
    public static float NoteOffset;
}

[System.Serializable]
public class GameSettingSaveData
{
    public float NoteSpeed;
    public float NoteOffset;

    public GameSettingSaveData(float NoteSpeed,float NoteOffset)
    {
        this.NoteSpeed = NoteSpeed;
        this.NoteOffset = NoteOffset;
    }
}