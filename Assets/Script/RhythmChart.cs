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
    public NoteType type;   // A/B
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
