using System;
using UnityEditor;
using UnityEngine;

public class NoteJudge : MonoBehaviour
{
    [SerializeField] private NoteType judgeLine;
    [SerializeField] private float perfectDistance;
    [SerializeField] private float goodDistance;
    [SerializeField] private float badDistance;

    private GameObject closet = null;
     
    private KeyCode judgeKey
    {
        get
        {
            return judgeLine == NoteType.A ? KeyBindData.NoteA : KeyBindData.NoteB;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] notes = GameObject.FindGameObjectsWithTag("Note");
        JudgeRhythmNote(notes);
        
    }

    private void JudgeRhythmNote(GameObject[] noteList)
{
    if (closet == null)
    {
        closet = FindClosetNote(noteList);
        if (closet == null) return; // 💥 null 방어
    }

    float distance = closet.transform.position.y - transform.position.y;

    if (distance < -badDistance)
    {
        Debug.Log("Miss");
        closet.SetActive(false);
        closet = null; 
        return;
    }

    if (Input.GetKeyDown(judgeKey))
    {
        if (Mathf.Abs(distance) < perfectDistance)
        {
            Debug.Log("Perfect");
        }
        else if (Mathf.Abs(distance) < goodDistance)
        {
            Debug.Log("Good");
        }
        else if (Mathf.Abs(distance) < badDistance)
        {
            Debug.Log("Bad");
        }
        else
        {
            Debug.Log("Miss");
        }

        closet.SetActive(false);
        closet = null; 
    }
}

    GameObject FindClosetNote(GameObject[] noteList)
    {
        GameObject note = null;
        if(noteList.Length ==0 ) return null;
        foreach(var n in noteList)
        {
            NoteScript noteScript  = n.GetComponent<NoteScript>();
            
            
            if(noteScript.noteType == judgeLine)
            {
                if (note == null)
                {
                    note = n;
                    continue;
                }
                
                if (n.transform.position.y < note?.transform.position.y&&transform.position.y-n.transform.position.y<-badDistance)
                {
                    note = n;
                }
            }
        }
        return note;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,badDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position,goodDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position,perfectDistance);
        
        
        
    }
}
