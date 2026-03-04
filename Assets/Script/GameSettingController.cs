using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameSettingController : MonoBehaviour
{
    public Slider OffsetSlider;
    public Slider SpeedSlider;

    public TMP_InputField OffsetField;
    public TMP_InputField SpeedField;


    public void UiUpdate()
    {
        OffsetField.text = GameSettingData.NoteOffset.ToString();
        OffsetSlider.value = GameSettingData.NoteOffset;
        SpeedField.text = GameSettingData.NoteSpeed.ToString();
        SpeedSlider.value = GameSettingData.NoteSpeed;
    }

    private void Start()
    {
        UiUpdate();
    }

    public void OffsetSilderChanged()
    {
        GameSettingData.NoteOffset = OffsetSlider.value;
        OffsetField.text = GameSettingData.NoteOffset.ToString();
    }

    public void OffsetFieldChanged()
    {
        
        float.TryParse(OffsetField.text,out float FieldValue);
        
        GameSettingData.NoteOffset = Mathf.Clamp(FieldValue,OffsetSlider.minValue,OffsetSlider.maxValue);
        OffsetField.text = GameSettingData.NoteOffset.ToString();
        OffsetSlider.value = GameSettingData.NoteOffset;
    }


    public void SpeedSliderChanged()
    {
        GameSettingData.NoteSpeed = SpeedSlider.value;
        SpeedField.text = GameSettingData.NoteSpeed.ToString();
    }

    public void SpeedFieldChanged()
    {
        float.TryParse(SpeedField.text,out float SpeedValue);

        GameSettingData.NoteSpeed = Mathf.Clamp(SpeedValue,SpeedSlider.minValue,SpeedSlider.maxValue);
        SpeedField.text = GameSettingData.NoteSpeed.ToString();
        SpeedSlider.value = GameSettingData.NoteSpeed;
    }



}