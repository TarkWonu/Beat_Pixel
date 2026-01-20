
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
        
        JudgeRhythmNote();
        
    }

    private void JudgeRhythmNote()
    {
        if (closet != null) 
            Debug.DrawLine(transform.position, closet.transform.position, Color.cyan);
        if (closet == null)
        {
            closet = FindClosetNote();
            if (closet == null) return; 
        }

        float distance = closet.transform.position.y - transform.position.y;

        if (closet.transform.position.y<transform.position.y-badDistance)
        {
            Debug.Log("Miss");
            
            if (judgeLine == NoteType.A)
                RhythmNoteManager.Instance.ALaneList.Remove(closet);
            else
                RhythmNoteManager.Instance.BLaneList.Remove(closet);
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
            if (judgeLine == NoteType.A)
                RhythmNoteManager.Instance.ALaneList.Remove(closet);
            else
                RhythmNoteManager.Instance.BLaneList.Remove(closet);
            closet = null; 
        }
    }

    GameObject FindClosetNote()
    {
        if (judgeLine == NoteType.A)
        {
            if (RhythmNoteManager.Instance.ALaneList.Count != 0)
            {
                return RhythmNoteManager.Instance.ALaneList[0];
            }
            
        }
        else
        {
            if (RhythmNoteManager.Instance.BLaneList.Count != 0)
            {
                return RhythmNoteManager.Instance.BLaneList[0];
            }
        }
        return null;
    }


    void OnDrawGizmos()
    {

        Vector3 pos = transform.position;
    
    // 미스 판정 경계선을 빨간색 선으로 그립니다.
        Gizmos.color = Color.red;
        Vector3 missLinePos = new Vector3(pos.x - 1f, pos.y - badDistance, pos.z);
        Vector3 missLineEnd = new Vector3(pos.x + 1f, pos.y - badDistance, pos.z);
        Gizmos.DrawLine(missLinePos, missLineEnd);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position,badDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position,goodDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,perfectDistance);
        
        
        
    }
}
