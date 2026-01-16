using UnityEngine;

public class NoteScript : MonoBehaviour
{
    private Transform rhytmeLine;
    private float noteSpeed;

    private float distance;


    public void Init(Transform pos,float speed)
    {
        rhytmeLine = pos;
        noteSpeed = speed;

        distance = Mathf.Abs(transform.position.y - rhytmeLine.position.y)/noteSpeed;
    }

    void Update()
    {
        transform.Translate(Vector2.down*distance*Time.deltaTime);
    }








}
