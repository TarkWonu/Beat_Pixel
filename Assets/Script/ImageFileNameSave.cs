using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



[RequireComponent(typeof(SpriteRenderer))]
public class ImageFileSave : MonoBehaviour
{
    private string fileName;
    private string pathListName = ImageSaveManager.pathFileName;
    private SpriteRenderer spriteRenderer;
    private ImageFilePath list;
    private objectType type;

    void Awake()
    {
        fileName = PlayDataManager.Instance.PlayChart.title;
        type  = PlayDataManager.Instance.PlayChart.type;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SaveImage()
    {
        string savedFilename = ImageSaveManager.SavePNG(spriteRenderer.sprite.texture,fileName);
        SavePath(savedFilename);
        RhythmGameScoreManager.Instance.sprite = spriteRenderer.sprite;
        
    }


    private void SavePath(string imageName)
    {
        
        list = DataSaveManager.LoadData<ImageFilePath>("ImagePath.json");
        
        list.filePaths.Add(new FileContext(fileName:fileName,imageName,accuracy:GetAccuracy(),objectType:type));
        
        DataSaveManager.SaveData<ImageFilePath>(pathListName,list);

    }

    float GetAccuracy()
    {
        float score = RhythmGameScoreManager.Instance.perfect * 1 + RhythmGameScoreManager.Instance.good *0.75f + RhythmGameScoreManager.Instance.bad *0.5f;

        float count = RhythmGameScoreManager.Instance.perfect + RhythmGameScoreManager.Instance.good + RhythmGameScoreManager.Instance.bad + RhythmGameScoreManager.Instance.miss;

        return Mathf.Round(score/count*1000)*0.1f;
    }
}