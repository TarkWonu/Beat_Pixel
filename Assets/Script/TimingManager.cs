using UnityEngine;
using System.Collections.Generic;

public class TimingManager : MonoBehaviour
{
    public List<GameObject> NoteList = new List<GameObject>();

    [SerializeField] Transform center = null;
    [SerializeField] RectTransform[] timingRect = null; 
    Vector2[] timingBoxes = null;//x = 왼쪽 y = 오른쪽
    void Start()
    {
        timingBoxes = new Vector2[timingRect.Length];

        for (int i = 0; i < timingRect.Length; i++)
        {
            timingBoxes[i].Set(center.localPosition.x - timingRect[i].rect.width / 2,
                              center.localPosition.x + timingRect[i].rect.width / 2);
        }
        PlayerController.Instance.PressSpace += CheckTiming;
    }

    public void CheckTiming()
    {
        for(int i = 0; i < NoteList.Count; i++)
        {
            float notePosX = NoteList[i].transform.localPosition.x;

            for(int j = 0; j < timingBoxes.Length; j++)
            {
                if (timingBoxes[j].x <= notePosX && notePosX <= timingBoxes[j].y)
                {
                    NoteList[i].GetComponent<Note>().HideNote();
                    Destroy(NoteList[i]);
                    NoteList.RemoveAt(i);

                    switch (j)
                    {
                        case 0:
                            Debug.Log("Perfect");
                            break;
                        case 1:
                            Debug.Log("Awesome");
                            break;
                        case 2:
                            Debug.Log("Good");
                            break;
                        case 3:
                            Debug.Log("Bad");
                            break;
                    }
                    
                    return;
                }
            }
            Debug.Log("Miss");
        }
        
    }

}