
using System.Collections;
using NUnit.Framework;
using PixeLadder.EasyTransition;
using TMPro;
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
    [SerializeField] public float noteSpeed;
    
    [SerializeField] private Transform noteLine;

    [Header("UI")]
    [SerializeField] TMP_Text cntDown;
    [SerializeField] TMP_Text comboText;

    private int timer;

    private bool gameStart = false;
    




    private AudioSource audioSource;
    private int beatIndex = 0;
    private float currentTime;

    private ImageFileSave image;

    public float beatPerSec
    {
        get
        {
            return 60f/rhythmChart.bpm;
        }
    }

    private void Awake()
    {
        try
        {
            rhythmChart = PlayDataManager.Instance.PlayChart.rhythmChart;
        }
        catch
        {
            Debug.Log("파일이 어ㅄ어요");
        }
        
    }

    private void Start()
    {
        image = FindFirstObjectByType<ImageFileSave>();
        
        noteSpeed= 2/GameSettingData.NoteSpeed;
         audioSource = GetComponent<AudioSource>();
        
        StartCoroutine(Countdown());
    }


    void Update()
    {

        if(!gameStart) return;
        if (beatIndex >= rhythmChart.notes.Count&&gameStart)
        {
            StartCoroutine(GameEnd());
            return;
        }
        
        
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

        comboText.text = RhythmGameScoreManager.Instance.combo.ToString();
        
    }


    private IEnumerator Countdown()
    {
        timer = 3;
        

        while (timer > 0)
        {
            cntDown.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }
        gameStart = true;
        audioSource.clip = rhythmChart.clip;
        audioSource.Play();
        RhythmGameScoreManager.Instance.init();
        cntDown.text = "";
    }

    private IEnumerator GameEnd()
    {
        gameStart = false;
        yield return new WaitForSeconds(noteSpeed+2);
        
        image.SaveImage();
        SceneTransitioner.Instance.LoadScene("ResultScene");
    }
}

