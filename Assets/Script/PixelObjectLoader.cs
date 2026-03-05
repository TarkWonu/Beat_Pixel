using UnityEngine;

public class PixelObjectLoader : MonoBehaviour
{
    private ImageFilePath pathList;
    [SerializeField] GameObject prefeb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pathList = DataSaveManager.LoadData<ImageFilePath>("ImagePath.Json");
        foreach(var path in pathList.filePaths)
        {
            GameObject obj = Instantiate(prefeb);
            obj.transform.parent = this.transform;

            Texture2D texture = ImageSaveManager.LoadPNG(path.imagePath);

            obj.GetComponent<ObjectSetting>().Init(texture:texture,title:path.fileName,accuracy:path.accuracy,path.type);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
