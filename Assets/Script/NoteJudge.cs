
using UnityEngine;

enum RhythmState
{
    Perfect,
    Good,
    Bad,
    Miss
}

public class NoteJudge : MonoBehaviour
{
    [SerializeField] private NoteType judgeLine;
    [SerializeField] private float perfectDistance;
    [SerializeField] private float goodDistance;
    [SerializeField] private float badDistance;

    private GameObject closet = null;

    private bool isPressed = false;
     
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

        

        

        if (closet.GetComponent<NoteScript>().isLongNote)
        {
            JudgeLongNote();
        }
        else
        {
            if (closet.transform.position.y<transform.position.y-badDistance)
            {
                Debug.Log("Miss");

                RemoveNote();
                return;
            }
            JudgeShortNote();
        }

        
    }

    public GameObject FindClosetNote()
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


    private void RemoveNote()
    {
        closet.SetActive(false);
            
            if (judgeLine == NoteType.A)
                RhythmNoteManager.Instance.ALaneList.Remove(closet);
            else
                RhythmNoteManager.Instance.BLaneList.Remove(closet);
            closet = null;
    }
    private void JudgeShortNote()
    {
        float distance = closet.transform.position.y - transform.position.y;

        if (Input.GetKeyDown(judgeKey)&&Mathf.Abs(distance) < badDistance)
        {
           

            
        
         if (Mathf.Abs(distance) < perfectDistance)
            {
                Debug.Log("Perfect");
                
            }
            else if (Mathf.Abs(distance) < goodDistance)
            {
                Debug.Log("Good");
                
            }
            else 
            {
                Debug.Log("Bad");
                
            }
            RemoveNote();
        }
    }

    private void JudgeLongNote()
    {
        //롱노트 시작 포지션(오브젝트 포지션)
        //롱노트 끝 (롱노트 시작포지션 + longNoteLength)

        float longNoteEndPos = closet.transform.position.y + closet.GetComponent<NoteScript>().longNoteLength;
        float startDistance = closet.transform.position.y - transform.position.y;
        

        

        

        if (Input.GetKeyDown(judgeKey)&&Mathf.Abs(startDistance) < badDistance&&!isPressed)
        {
            
            isPressed = true;
            
            
        
         if (Mathf.Abs(startDistance) < perfectDistance)
            {
                Debug.Log("Perfect");
                
            }
            else if (Mathf.Abs(startDistance) < goodDistance)
            {
                Debug.Log("Good");
                
            }
            else 
            {
                Debug.Log("Bad");
                
            }
            
        }

        if (Input.GetKey(judgeKey) && isPressed)
        {
            
            LongNoteEffect();
        }

        if (Input.GetKeyUp(judgeKey)&&isPressed)
        {
           
        isPressed = false;
        float endDistance = longNoteEndPos - transform.position.y;
            
        
            if (Mathf.Abs(endDistance) < perfectDistance)
            {
                Debug.Log("Perfect");
                
            }
            else if (Mathf.Abs(endDistance) < goodDistance)
            {
                Debug.Log("Good");
                
            }
            else 
            {
                Debug.Log("Bad");
                
            }
            Debug.Log("우히히");
            RemoveNote();
            
        }

    }

    private void LongNoteEffect()
    {
        if (closet == null) return;

        NoteScript noteScript = closet.GetComponent<NoteScript>();
        LineRenderer lr = closet.GetComponent<LineRenderer>();
        SpriteRenderer sr = closet.GetComponent<SpriteRenderer>();

        noteScript.Triggered = false;
        if (sr.enabled)
            sr.enabled = false;

       
        lr.positionCount = 2;

       
        float tailY = closet.transform.position.y + noteScript.longNoteLength;

        
        

        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, new Vector3(transform.position.x, tailY, 0));

        // 3. 전부 소모되면 라인 제거
        if (tailY <= transform.position.y)
        {
            lr.positionCount = 0;
        }
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
