using System.Collections;

using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(NoteJudge))]
public class NoteJudgeEffect : MonoBehaviour
{
    private LineRenderer lineRenderer;

    public NoteType judgeLine;
    private KeyCode judgeKey;
    private Coroutine effectCoroutine;
    [Range(0f,1f)][SerializeField] private float lineAlpha;
    [SerializeField] private float lineLength;

    private GameObject closet;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPositions(new Vector3[2]{transform.position-new Vector3(0,0.15f,0),transform.position+new Vector3(0,lineLength,0)});
        lineRenderer.startWidth = 0;
        lineRenderer.endWidth = 0;


        judgeKey = judgeLine == NoteType.A ? KeyBindData.NoteA : KeyBindData.NoteB;

        
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.startColor = new Color(0.97f,0.8f,0.15f,lineAlpha);
        lineRenderer.endColor = new Color(1,0.87f,0.53f,0f);
        if (Input.GetKeyDown(judgeKey))
        {
            closet = GetComponent<NoteJudge>().FindClosetNote();
            if(effectCoroutine != null)
            {
                StopCoroutine(effectCoroutine);
            }
            
            effectCoroutine = StartCoroutine(NoteLine());
            
        }
    }

    private IEnumerator NoteLine()
    {
        float duration = 0.2f;
        float time = 0f;
        

        while (time < duration)
        {
            float t = time / duration;
            float width = Mathf.Lerp(2f, 0f, t);
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            time += Time.deltaTime;
            yield return null;
        }

        lineRenderer.startWidth = 0f;
        lineRenderer.endWidth = 0f;
    }

    

    
}
