using UnityEngine;
using System.IO;

public static class DataSaveManager
{
    // 내부에서 경로를 완성하는 헬퍼 함수를 하나 만듭니다.
    private static string GetFinalPath(string fileName,string folderName = "Json")
    {
        #if UNITY_EDITOR
            // 에디터일 때는 Assets/Json/ 폴더
            string dir = Path.Combine(Application.dataPath, folderName);
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, fileName);
        #else
            // 빌드 후에는 안전한 저장소
            return Path.Combine(Application.persistentDataPath, fileName);
        #endif
    }

    public static void SaveData<T>(string fileName, T data)
    {
        string fullPath = GetFinalPath(fileName);
        string jData = JsonUtility.ToJson(data, true);
        
        File.WriteAllText(fullPath, jData);
        Debug.Log($"저장 완료: {fullPath}");
    }

    public static T LoadData<T>(string fileName)
    {
        string fullPath = GetFinalPath(fileName);

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"파일 없음: {fullPath}");
            return default(T);
        }

        string json = File.ReadAllText(fullPath);
        return JsonUtility.FromJson<T>(json);
    }

    public static void ClearFile(string fileName)
    {
        string fullPath = GetFinalPath(fileName);
        if(File.Exists(fullPath)) File.Delete(fullPath);
    }
}