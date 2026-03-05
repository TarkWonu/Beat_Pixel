using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Rhythm/PlayData", fileName = "PlayData")]
public class MusicPlayData : ScriptableObject
{
    public RhythmChart rhythmChart;

    public Sprite pixelImage;

    public string title;
    public objectType type;
    

    [TextArea]
    public String infoText;

    
}
