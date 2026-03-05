using UnityEngine;


public class GridTile : MonoBehaviour
{
    [SerializeField]Vector2Int gridSize;

    [SerializeField] Sprite sprite;


    void GenerateTile()
    {
        Vector2 spawnPoint = transform.position - new Vector3(gridSize.x/2f,gridSize.y/2f)+new Vector3(0.5f,0.5f);

        float imageSize = 1f/sprite.bounds.size.x;
        
        for(int x = 0; x < gridSize.x; x++)
        {
            for(int y = 0; y < gridSize.y; y++)
            {
                GameObject obj = new GameObject($"Tile{x},{y}");

                obj.transform.parent = this.transform;

                obj.transform.position = spawnPoint+new Vector2(x,y);
                obj.transform.localScale = new Vector2(imageSize,imageSize);

                SpriteRenderer tileRenderer = obj.AddComponent<SpriteRenderer>();
                tileRenderer.sprite = sprite;
            }
        }
    }

    public void ChangeTile(Sprite newSprite)
    {
        Transform[] myChildren = this.GetComponentsInChildren<Transform>();
        
        float imageSize = 1f/newSprite.bounds.size.x;

        foreach(var child in myChildren)
        {
            child.localScale = new Vector3(imageSize,imageSize);
            child.gameObject.GetComponent<SpriteRenderer>().sprite = newSprite;
        }
    }

    void Start()
    {
        GenerateTile();
    }



    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position,new Vector3(gridSize.x,gridSize.y));
    }
}