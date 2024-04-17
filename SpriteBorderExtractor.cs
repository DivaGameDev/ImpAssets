using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteBorderExtractor : MonoBehaviour
{
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer component
    public float threshold = 0.5f; // Threshold to determine if a pixel is considered part of the border

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogWarning("SpriteRenderer component is not assigned!");
            return;
        }

        // Get the sprite texture
        Texture2D texture = spriteRenderer.sprite.texture;

        // Create a new texture to store the border pixels
        Texture2D borderTexture = new Texture2D(texture.width, texture.height);

        // Loop through each pixel of the sprite texture
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                // Get the color of the pixel
                Color pixelColor = texture.GetPixel(x, y);

                // Calculate the grayscale value of the pixel
                float grayscale = pixelColor.grayscale;

                // Check if the grayscale value is below the threshold
                if (grayscale < threshold)
                {
                    // If so, set the color of the corresponding pixel in the border texture to black
                    borderTexture.SetPixel(x, y, Color.black);
                }
                else
                {
                    // Otherwise, set it to transparent
                    borderTexture.SetPixel(x, y, Color.clear);
                }
            }
        }

        // Apply changes to the border texture
        borderTexture.Apply();

        // Create a new sprite using the border texture
        Sprite borderSprite = Sprite.Create(borderTexture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        // Create a new GameObject to display the border sprite
        GameObject borderObject = new GameObject("BorderSprite");
        SpriteRenderer borderRenderer = borderObject.AddComponent<SpriteRenderer>();
        borderRenderer.sortingOrder = 2;
        borderRenderer.sprite = borderSprite;

        // Set the position and scale of the border GameObject to match the original sprite
        borderObject.transform.position = spriteRenderer.transform.position;
        borderObject.transform.localScale = spriteRenderer.transform.localScale;
    }
}

