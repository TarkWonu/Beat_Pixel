using UnityEngine.UI;
using TMPro;
using UnityEngine;

using PixeLadder.EasyTransition;

public class RhythmResultUiController : MonoBehaviour
{
    [Header("노트 상태 표시")]
    [SerializeField] TMP_Text perfect;
    [SerializeField] TMP_Text good;
    [SerializeField] TMP_Text bad;
    [SerializeField] TMP_Text miss;


    [Header("결과 스프라이트")]
    [SerializeField] Image image;

    [Header("맥콤")]
    [SerializeField] TMP_Text maxCombo;
    [Header("정확도")]
    [SerializeField] TMP_Text accuracy;

    void Start()
    {
        perfect.text = RhythmGameScoreManager.Instance.perfect.ToString();
        good.text = RhythmGameScoreManager.Instance.good.ToString();
        bad.text = RhythmGameScoreManager.Instance.bad.ToString();
        miss.text = RhythmGameScoreManager.Instance.miss.ToString();

        image.sprite = RhythmGameScoreManager.Instance.sprite;

        maxCombo.text = RhythmGameScoreManager.Instance.maxCombo.ToString();
        accuracy.text = GetAccuracy().ToString() + "%";



    }
    float GetAccuracy()
    {
        float score = RhythmGameScoreManager.Instance.perfect * 1 + RhythmGameScoreManager.Instance.good *0.75f + RhythmGameScoreManager.Instance.bad *0.5f;

        float count = RhythmGameScoreManager.Instance.perfect + RhythmGameScoreManager.Instance.good + RhythmGameScoreManager.Instance.bad + RhythmGameScoreManager.Instance.miss;

        return Mathf.Round((score/count)*1000)*0.1f;
    }


    public void Retry()
    {
        SceneTransitioner.Instance.LoadScene("InGame");
    }

    public void Next()
    {
        SceneTransitioner.Instance.LoadScene("SongSelectScene");
    }
}
