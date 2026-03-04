using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;


[Serializable]
public class ImageFilePath
{
    public List<FileContext> filePaths;
}

[Serializable]
public class FileContext
{
    public string fileName;
    public string imagePath;
    public float accuracy;

     public FileContext() { } 

    public FileContext(string fileName,string imagePath,float accuracy)
    {
        this.fileName = fileName;
        this.imagePath = imagePath;
        this.accuracy = accuracy;
    }
}
public static class ImageSaveManager
{
    
    public static string pathFileName = "ImagePath.json";

    private static string GetFinalPath(string fileName,string folderName = "Image")
    {
        #if UNITY_EDITOR
            
            string dir = Path.Combine(Application.dataPath, folderName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, fileName);
        #else
           
            return Path.Combine(Application.persistentDataPath, fileName);
        #endif
    }


    
    public static string SavePNG(Texture2D tex, string fileName)
    {
       

        byte[] pngData = tex.EncodeToPNG();

        int cnt = 0;
        string finalPath;
        string newFileName;
        

        do
        {
            newFileName = $"{fileName}_{cnt}.png";
            finalPath = GetFinalPath(newFileName);

            cnt++;
        }
        while (File.Exists(finalPath));

        File.WriteAllBytes(finalPath, pngData);

        Debug.Log("저장 완료: " + finalPath);
        return newFileName;
    }

    public static Texture2D LoadPNG(string fileName){
        string path = GetFinalPath(fileName);
         if (!File.Exists(path))
        {
            Debug.Log("파일 없음");
            return null;
        }

        byte[] pngData = File.ReadAllBytes(path);

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(pngData);


        return tex;


    }
}