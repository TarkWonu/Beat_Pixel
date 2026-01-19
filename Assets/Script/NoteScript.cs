using UnityEngine;

public class NoteScript : MonoBehaviour
{
    private Transform rhytmeLine;
    private float noteSpeed;

    private float distance;

    private bool isLongNote;
    private float longNoteLength;
    private float longNoteSize;
    public NoteType noteType;
    LineRenderer lineRenderer;

    
    public void Init(NoteContext noteContext)
    {
        rhytmeLine = noteContext.noteLine;
        noteSpeed = noteContext.noteSpeed;
        isLongNote = noteContext.isLongNote;
        longNoteSize = noteContext.longNoteSize;
        noteType = noteContext.NoteType;

        distance = Mathf.Abs(transform.position.y - rhytmeLine.position.y)/noteSpeed;

        if (isLongNote)
        {
            MakeLongNote();
        }

        
    }

    void Update()
    {
        transform.Translate(Vector2.down*distance*Time.deltaTime);
        if(isLongNote)
        {
            lineRenderer.positionCount =2;
            lineRenderer.SetPositions(new Vector3[2]{transform.position,transform.position+new Vector3(0,longNoteLength,0)});
        }
    }

    private void MakeLongNote()
    {
        lineRenderer = GetComponent<LineRenderer>();
        RhythmGameController rhythmGameController = FindObjectOfType<RhythmGameController>().GetComponent<RhythmGameController>();
        float longNoteTime = rhythmGameController.beatPerSec *  1/rhythmGameController.rhythmChart.snapDiv * longNoteSize;   
        longNoteLength = distance * longNoteTime; 
    }







}
