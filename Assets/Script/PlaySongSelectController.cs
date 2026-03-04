using UnityEngine;

public class PlaySongSelectController : MonoBehaviour
{
    [SerializeField] private MusicPlayData[] playlist;
    [SerializeField] private PanelShowController expandFsm;
    

    private int playIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        if(playIndex == -1)
        {
            PlayDataManager.Instance.PlayChart = null;
        }
        else
        {
            PlayDataManager.Instance.PlayChart = playlist[playIndex];
        }
    }

    public void SelectSong(int index)
    {
        if(expandFsm.expandStateMachine.currentState is FullBackGround)
        {
            playIndex = index;
        }
        
    }
}
