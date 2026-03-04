using UnityEngine;

public class NoteCount : MonoBehaviour
{
    private RhythmChart chart;



    void Start()
    {
        int cnt = 0;

        chart = PlayDataManager.Instance.PlayChart.rhythmChart;
        
        for(int j = 0; j < chart.notes.Count;j++)
        {
            if (chart.notes[j].isLongNote)
            {
                cnt+=(int)(chart.notes[j].longNoteSize/(1.0f/chart.snapDiv))+1;
            }
            else
            {
                cnt++;
            }
        }

        RhythmGameScoreManager.Instance.noteCount = cnt;

        
    }
}
