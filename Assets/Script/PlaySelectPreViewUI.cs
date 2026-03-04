using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlaySelectpreviewUI : MonoBehaviour
{
    [SerializeField] Image previewImage;
    [SerializeField] TMP_Text previewText;
    [SerializeField] GameObject playButton;

    

    void Update()
    {
        if (PlayDataManager.Instance.PlayChart == null)
        {
            previewImage.sprite = null;
            previewText.text = "";
            playButton.SetActive(false);
        }
        else
        {
            //previewImage.sprite = PlayDataManager.Instance.PlayChart.pixelImage;
            previewText.text = PlayDataManager.Instance.PlayChart.infoText;
            playButton.SetActive(true);
        }
    }
}