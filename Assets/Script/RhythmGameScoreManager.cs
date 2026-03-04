using UnityEngine;

public class RhythmGameScoreManager : Singleton<RhythmGameScoreManager>
{
    public int perfect = 0;
    public int good = 0;
    public int bad = 0;
    public int miss = 0;

    public int noteCount;
    public int combo{get;private set;} = 0;

    public int maxCombo{get;private set;} = 0;

    public void init()
    {
        perfect = 0;
        good =0;
        bad = 0;
        miss = 0;

        combo = 0;
        maxCombo = 0;
    }

    public void AddCombo()
    {
        combo++;
        if (combo > maxCombo)
        {
            maxCombo = combo;
        }
    }

    public void ResetCombo()
    {
        combo = 0;
    }

    public Sprite sprite;
}