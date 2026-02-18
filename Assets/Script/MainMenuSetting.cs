using PixeLadder.EasyTransition;
using UnityEngine;

public class MainMenuSetting : MainMenuText
{
    public override void Context()
    {
        SceneTransitioner.Instance.LoadScene("SettingMenu");
    }
}