using UnityEngine;
using System.IO;

public class PixelCanvas : MonoBehaviour
{
    [Header("설정")]
    public int canvasSize = 32; // 32x32 해상도
    public Color brushColor = Color.black;
    public FilterMode filterMode = FilterMode.Point; // 도트 느낌을 위해 Point 권장

    private Texture2D drawingTexture;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 1. 새로운 텍스처 생성 (RGBA32 비트)
        drawingTexture = new Texture2D(canvasSize, canvasSize);
        drawingTexture.filterMode = filterMode; // 픽셀이 뭉개지지 않게 설정

        // 2. 캔버스 초기화 (흰색으로 채우기)
        Color[] pixels = new Color[canvasSize * canvasSize];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        drawingTexture.SetPixels(pixels);
        drawingTexture.Apply();

        // 3. 스프라이트로 만들어 적용
        UpdateSprite();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) // 마우스 왼쪽 클릭 시
        {
            Draw();
        }
    }

    void Draw()
    {
        // 마우스 위치를 월드 좌표 -> 로컬 좌표 -> 픽셀 좌표로 변환
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 localPos = transform.InverseTransformPoint(mousePos);

        // 픽셀 좌표 계산 (0~1 사이 값을 텍스처 크기에 곱함)
        int x = Mathf.FloorToInt((localPos.x + 0.5f) * canvasSize);
        int y = Mathf.FloorToInt((localPos.y + 0.5f) * canvasSize);

        if (x >= 0 && x < canvasSize && y >= 0 && y < canvasSize)
        {
            drawingTexture.SetPixel(x, y, brushColor);
            drawingTexture.Apply(); // 변경사항 적용
            UpdateSprite();
        }
    }

    void UpdateSprite()
    {
        // 텍스처를 스프라이트로 변환하여 렌더러에 할당
        spriteRenderer.sprite = Sprite.Create(drawingTexture, new Rect(0, 0, canvasSize, canvasSize), new Vector2(0.5f, 0.5f));
    }

    // PNG 파일로 저장하는 기능
    public void SaveAsPNG(string fileName)
    {
        byte[] bytes = drawingTexture.EncodeToPNG();
        string path = Path.Combine(Application.persistentDataPath, fileName + ".png");
        File.WriteAllBytes(path, bytes);
        Debug.Log("저장 완료: " + path);
    }
}