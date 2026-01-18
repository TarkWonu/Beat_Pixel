using System;
using System.Collections.Generic;
using UnityEngine;

public enum NoteType
{
    A,
    B
}

[Serializable]
public class RhythmNote
{
    public float beat;      // 몇 번째 비트(소수 가능: 13.25 같은)
    public bool isLongNote;
    public int longNoteSize;
    public NoteType type;   // A/B
}

public class NoteContext
{
    public Transform noteLine{get; private set;}
    public float noteSpeed{get; private set;}
    public bool isLongNote{get; private set;}
    public float longNoteSize{get; private set;}

    public NoteContext(Transform noteLine,float noteSpeed,bool isLongNote,float longNoteSize)
    {
        this.noteLine = noteLine;
        this.noteSpeed = noteSpeed;
        this.isLongNote = isLongNote;
        this.longNoteSize = longNoteSize;
    }
}

[CreateAssetMenu(menuName = "Rhythm/Rhythm Chart", fileName = "RhythmChart")]
public class RhythmChart : ScriptableObject
{
    public AudioClip clip;
    public float bpm = 120f;

    // 스냅(예: 4면 1/4비트 단위로 반올림)
    public int snapDiv = 4;

    public List<RhythmNote> notes = new List<RhythmNote>();
}
