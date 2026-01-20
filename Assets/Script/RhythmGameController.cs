
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RhythmGameController : MonoBehaviour
{
    
    [Header("리듬 데이터")]
    public RhythmChart rhythmChart;

    [Header("노트 설정")]
    [SerializeField] private GameObject notePrefeb;
    [SerializeField] Transform noteSpawnerA;
    [SerializeField] Transform noteSpawnerB;
    [Tooltip("노트가 생성직후 위치부터 판정선까지 도달하는데 걸리는 \"시간\"")]
    [SerializeField] private float noteSpeed;
    
    [SerializeField,] private Transform noteLine;
    




    private AudioSource audioSource;
    private int beatIndex = 0;
    private float currentTime;

    public float beatPerSec
    {
        get
        {
            return 60f/rhythmChart.bpm;
        }
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = rhythmChart.clip;
        audioSource.Play();
    }


    void Update()
    {
        if(beatIndex>=rhythmChart.notes.Count) return;
        
        float beatTime = beatPerSec*rhythmChart.notes[beatIndex].beat;
        if (beatTime - noteSpeed >= 0)
        {
            currentTime = audioSource.time;
        }
        else
        {
            currentTime+=Time.deltaTime;
        }

        if (currentTime >= beatTime-noteSpeed)
        {
            Transform noteSpawnPos = rhythmChart.notes[beatIndex].type == NoteType.A ? noteSpawnerA : noteSpawnerB;
            
            GameObject noteObj = Instantiate(notePrefeb,noteSpawnPos.position,Quaternion.identity);
            if(rhythmChart.notes[beatIndex].type == NoteType.A)
            {
                RhythmNoteManager.Instance.ALaneList.Add(noteObj);
            }
            else
            {
                RhythmNoteManager.Instance.BLaneList.Add(noteObj);
            }
            NoteContext noteContext = new NoteContext(
                this.noteLine,
                noteSpeed,
                rhythmChart.notes[beatIndex].isLongNote,
                rhythmChart.notes[beatIndex].longNoteSize,
                rhythmChart.notes[beatIndex].type

            );
            noteObj.GetComponent<NoteScript>().Init(noteContext);
           
            beatIndex++;
            currentTime = 0f;
        }
        
    }
}

