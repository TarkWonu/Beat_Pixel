using UnityEngine;

public class NoteScript : MonoBehaviour
{
    private Transform rhytmeLine;
    private float noteSpeed;

    private float distance;

    public bool isLongNote{get; private set;}
    public float longNoteLength{get; private set;}

    public bool Triggered = true;
    private float longNoteSize;
    
    LineRenderer lineRenderer;

    
    public void Init(NoteContext noteContext)
    {
        rhytmeLine = noteContext.noteLine;
        noteSpeed = noteContext.noteSpeed;
        isLongNote = noteContext.isLongNote;
        longNoteSize = noteContext.longNoteSize;
        

        distance = Mathf.Abs(transform.position.y - rhytmeLine.position.y)/noteSpeed;

        if (isLongNote)
        {
            MakeLongNote();
        }

        
    }

    void Update()
    {
        transform.Translate(Vector2.down*distance*Time.deltaTime);
        if(isLongNote&&Triggered)
        {
            lineRenderer.positionCount =2;
            lineRenderer.SetPositions(new Vector3[2]{transform.position,transform.position+new Vector3(0,longNoteLength,0)});
        }
    }

    private void MakeLongNote()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        float longNoteTime = longNoteSize;   
        longNoteLength = distance * longNoteTime; 
    }







}
