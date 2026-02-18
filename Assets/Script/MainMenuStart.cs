using PixeLadder.EasyTransition;
using UnityEngine;

public class MainMenuStart : MainMenuText
{
    public override void Context()
    {
        SceneTransitioner.Instance.LoadScene("SongSelectScene");
    }
}
