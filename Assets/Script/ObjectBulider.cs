using UnityEngine;

public class Builder : MonoBehaviour
{
    public Grid grid;
    public Sprite buildSprite;
    public LayerMask blockLayer;

    GameObject preview;
    SpriteRenderer previewRenderer;

    void Start()
    {
        float targetSize = 1f;
        preview = new GameObject("Preview");
        previewRenderer = preview.AddComponent<SpriteRenderer>();
        previewRenderer.sprite = buildSprite;
        previewRenderer.color = new Color(1,1,1,0.5f);

        float width = previewRenderer.sprite.bounds.size.x;
        float scale = targetSize / width;

        preview.transform.localScale = new Vector3(scale, scale, 1f);
    }

    void Update()
    {
        if (buildSprite != null)
        {
            UpdatePreview();

            if (Input.GetMouseButtonDown(0))
            {
                TryPlace();
            }

            
        }
        
    }

    void UpdatePreview()
    {
        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        world.z = 0;

        Vector3Int cell = grid.WorldToCell(world);
        Vector3 center = grid.GetCellCenterWorld(cell);

        preview.transform.position = center;

        UpdateSorting(previewRenderer);

        bool blocked = Physics2D.OverlapPoint(center, blockLayer) != null;
        previewRenderer.color = blocked
            ? new Color(1,0,0,0.5f)
            : new Color(1,1,1,0.5f);
    }

    public void TryPlace()
    {
        Vector3 pos = preview.transform.position;

        if (Physics2D.OverlapPoint(pos, blockLayer) != null)
            return;

        GameObject obj = new GameObject("Building");
        obj.transform.position = pos;

        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = buildSprite;

        UpdateSorting(sr);

        obj.layer = LayerMask.NameToLayer("Builded");
        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();

        obj.transform .localScale = preview.transform.localScale;
        float width = sr.sprite.bounds.size.x;
        col.size = new Vector2(width,width);
        
    }

    void UpdateSorting(SpriteRenderer sr)
    {
        sr.sortingOrder = -(int)(sr.transform.position.y * 100);
    }
}
