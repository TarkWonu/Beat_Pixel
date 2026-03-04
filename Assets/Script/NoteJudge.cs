
using TMPro;
using UnityEngine;

public enum RhythmState
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

    [SerializeField] PixelEdit pixelEdit;


    

    private RhythmGameController rhythmGameController;    

    private GameObject closet = null;

    private bool isPressed = false;

    private float holdTimer = 0f;
    private float unitBeatTime;
    private int totalFillCount = 1;
    private int currentFillIndex;

    

    [SerializeField]private Transform judgePos;
    [SerializeField] TMP_Text judgeText;
     
    private KeyCode judgeKey
    {
        get
        {
            return judgeLine == NoteType.A ? KeyBindData.Keys[KeyAction.NoteA] : KeyBindData.Keys[KeyAction.NoteB];
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rhythmGameController = FindFirstObjectByType<RhythmGameController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        JudgeRhythmNote();
        
    }

    private void JudgeRhythmNote()
    {
        if (closet != null) 
            Debug.DrawLine(judgePos.position, closet.transform.position, Color.cyan);
        if (closet == null)
        {
            closet = FindClosetNote();
            if (closet == null) return; 
        }

        

        

        if (closet.GetComponent<NoteScript>().isLongNote)
        {
            
            NoteScript note = closet.GetComponent<NoteScript>();

            // 👇 아직 계산 안 되어있으면 미리 계산
            if (totalFillCount == 0)
            {
                float longBeat = note.longNoteSize;
                int snapDiv = rhythmGameController.rhythmChart.snapDiv;

                totalFillCount = Mathf.RoundToInt(longBeat * snapDiv);
            }

            float longNoteEndPos =
                closet.transform.position.y + note.longNoteLength;

            // 🎯 롱노트를 아예 안 눌렀는데 끝이 지나가면
            if (longNoteEndPos < judgePos.position.y - badDistance)
            {
                if (!isPressed) // 한번도 안 눌렀으면
                {
                    for (int i = 0; i < totalFillCount; i++)
                    {
                        JudgeState(RhythmState.Miss);
                    }
                }

                RemoveNote();
                return;
            }

            JudgeLongNote();
        }
        else
        {
            if (closet.transform.position.y<judgePos.position.y-badDistance)
            {
                JudgeState(RhythmState.Miss);

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
        
            
            if (judgeLine == NoteType.A)
                RhythmNoteManager.Instance.ALaneList.Remove(closet);
            else
                RhythmNoteManager.Instance.BLaneList.Remove(closet);
            Destroy(closet);
            
    }
    private void JudgeShortNote()
    {
        float distance = closet.transform.position.y - judgePos.position.y;

        if (Input.GetKeyDown(judgeKey)&&Mathf.Abs(distance) < badDistance)
        {
           

            
        
         if (Mathf.Abs(distance) < perfectDistance)
            {
                JudgeState(RhythmState.Perfect);
                
            }
            else if (Mathf.Abs(distance) < goodDistance)
            {
                JudgeState(RhythmState.Good);
                
            }
            else 
            {
                JudgeState(RhythmState.Bad);
                
            }
            RemoveNote();
        }
    }

    private void JudgeLongNote()
{
    NoteScript note = closet.GetComponent<NoteScript>();
    float startDistance = closet.transform.position.y - judgePos.position.y;

    
    if (Input.GetKeyDown(judgeKey) &&
        Mathf.Abs(startDistance) < badDistance &&
        !isPressed)
    {
        isPressed = true;

        float longBeat = note.longNoteSize;
        int snapDiv = rhythmGameController.rhythmChart.snapDiv;

        totalFillCount = Mathf.RoundToInt(longBeat * snapDiv);

        unitBeatTime = rhythmGameController.beatPerSec / snapDiv;

        holdTimer = 0f;

       
        if (Mathf.Abs(startDistance) < perfectDistance)
            {
                JudgeState(RhythmState.Perfect);
            }
            
        else if (Mathf.Abs(startDistance) < goodDistance){
            JudgeState(RhythmState.Good);
        }
            else
            {
               JudgeState(RhythmState.Bad);
            }
            

        currentFillIndex = 1; 
    }

   
    if (Input.GetKey(judgeKey) && isPressed)
    {
        holdTimer += Time.deltaTime;

        while (holdTimer >= unitBeatTime &&
               currentFillIndex < totalFillCount)
        {
            holdTimer -= unitBeatTime;
            currentFillIndex++;

            JudgeState(RhythmState.Perfect);
        }

        LongNoteEffect();
    }

    
    if (Input.GetKeyUp(judgeKey) && isPressed)
    {
        isPressed = false;

        float longNoteEndPos =
            closet.transform.position.y + note.longNoteLength;

        float endDistance =
            longNoteEndPos - judgePos.position.y;

        if (Mathf.Abs(endDistance) < perfectDistance)
            {
                JudgeState(RhythmState.Perfect);
            }
            
        else if (Mathf.Abs(endDistance) < goodDistance)
            {
                JudgeState(RhythmState.Good);
            }

            else
            {
                JudgeState(RhythmState.Bad);
            }
            

        
        int remain = totalFillCount - currentFillIndex;
        for (int i = 0; i < remain; i++)
        {
            JudgeState(RhythmState.Miss);
        }

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


    private void JudgeState(RhythmState state)
    {
        switch (state)
        {
            case RhythmState.Perfect:
                RhythmGameScoreManager.Instance.AddCombo();
                RhythmGameScoreManager.Instance.perfect++;
                judgeText.text = "Perfect";
                break;
            case RhythmState.Good:
                RhythmGameScoreManager.Instance.AddCombo();
                RhythmGameScoreManager.Instance.good++;
                judgeText.text = "Good";
                break;
            case RhythmState.Bad:
                RhythmGameScoreManager.Instance.ResetCombo();
                RhythmGameScoreManager.Instance.bad++;
                break;
            case RhythmState.Miss:
                RhythmGameScoreManager.Instance.ResetCombo();
                RhythmGameScoreManager.Instance.miss++;
                break;
        }

        pixelEdit.FillTexture(state);
    }


    void OnDrawGizmos()
    {

        Vector3 pos = judgePos.position;
    
    // 미스 판정 경계선을 빨간색 선으로 그립니다.
        Gizmos.color = Color.red;
        Vector3 missLinePos = new Vector3(pos.x - 1f, pos.y - badDistance, pos.z);
        Vector3 missLineEnd = new Vector3(pos.x + 1f, pos.y - badDistance, pos.z);
        Gizmos.DrawLine(missLinePos, missLineEnd);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos,badDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos,goodDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos,perfectDistance);
        
        
        
    }


    
}


