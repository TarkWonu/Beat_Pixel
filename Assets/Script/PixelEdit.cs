using System.Collections;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
public class PixelEdit : MonoBehaviour
{
    
    [SerializeField] int updateSize = 10;
    [SerializeField] float noiseIntensity;
    SpriteRenderer spriteRenderer;
    Texture2D originTexture, texture;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originTexture = spriteRenderer.sprite.texture;
        texture = Instantiate(originTexture);
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        for(int i = 0; i < texture.width; i++)
        {
            for(int k = 0; k < texture.height; k++)
            {
                texture.SetPixel(i, k, Color.black);
            }
        }
        texture.Apply();

        
    }
    int x = 0, y = 0;
    WaitForSeconds wait;

    Color pixelColor;
    public void FillTexture(RhythmState state)
    {
        
        for(int i = 0; i < updateSize && x + i < texture.width; i++)
        {
            for(int k = 0; k < updateSize && y + k < texture.height; k++)
            {
                switch (state)
                {
                    case RhythmState.Perfect:
                        pixelColor =   originTexture.GetPixel(x + i, y + k);
                        break;
                    case RhythmState.Good:
                        pixelColor = GetNoiseTexture(originTexture.GetPixel(x+i,y+k));
                        break;
                    case RhythmState.Bad:
                        pixelColor = GetGrayScale(originTexture.GetPixel(x+i,y+k));
                        break;
                    case RhythmState.Miss:
                        pixelColor = new Color(0,0,0,0);
                        break;
                    default:
                        Debug.LogError("이게 뭐노");
                        break;
                        
                }
                texture.SetPixel(x + i, y + k, pixelColor);
            }
        }
        x += updateSize;
        if (x >= texture.width)
        {
            x = 0; y += updateSize;
        }
        texture.Apply();
    }

    Color GetNoiseTexture(Color pixel)
    {
        float rNoise = Random.Range(-noiseIntensity, noiseIntensity);
        float gNoise = Random.Range(-noiseIntensity, noiseIntensity);
        float bNoise = Random.Range(-noiseIntensity, noiseIntensity);
        return new Color(
            Mathf.Clamp01(pixel.r+rNoise),
            Mathf.Clamp01(pixel.g+gNoise),
            Mathf.Clamp01(pixel.b+bNoise)
            );
    }

    Color GetGrayScale(Color pixel)
    {
        float grayValue = pixel.grayscale;

        return new Color(grayValue,grayValue,grayValue);
    }
}



