using UnityEditor;
using UnityEngine;

public class PaintSprite : MonoBehaviour
{
    public Texture2D texture, OriginalTexture; // Assign the texture in the inspector
    public Color paintColor = Color.green;
    public float brushSize = 10f;
    public LayerMask paintLayer;

    private void Start()
    {
        texture= new Texture2D(OriginalTexture.width, OriginalTexture.height);
       
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, OriginalTexture.width, OriginalTexture.height), new Vector2(.5f, .5f));
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = Color.white;

    }

    
    void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            // Cast a 2D ray from the mouse position
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector3.forward,paintLayer);

            if (hit.collider != null && hit.collider.CompareTag("paintable"))
            {
                // Get the local hit point relative to the collider's bounds
                Vector2 localHitPoint = hit.collider.transform.InverseTransformPoint(hit.point);

                // Convert local position to UV coordinates
                Vector2 uv = new Vector2(
                    Mathf.InverseLerp(hit.collider.bounds.min.x, hit.collider.bounds.max.x, localHitPoint.x),
                    Mathf.InverseLerp(hit.collider.bounds.min.y, hit.collider.bounds.max.y, localHitPoint.y)
                );

                // Convert UV coordinates to pixel coordinates on the texture
                int centerX = Mathf.RoundToInt(uv.x * texture.width);
                int centerY = Mathf.RoundToInt(uv.y * texture.height);

                // Paint the pixels in a circular brush area around the center
                for (int x = Mathf.Max(0, centerX - Mathf.RoundToInt(brushSize / 2)); x < Mathf.Min(texture.width, centerX + Mathf.RoundToInt(brushSize / 2)); x++)
                {
                    for (int y = Mathf.Max(0, centerY - Mathf.RoundToInt(brushSize / 2)); y < Mathf.Min(texture.height, centerY + Mathf.RoundToInt(brushSize / 2)); y++)
                    {
                        // Calculate the distance from the current pixel to the center
                        float distance = Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY));

                        // Check if the pixel is within the circular brush area
                        if (distance <= brushSize / 2)
                        {
                            bool isvisible = IsPixelVisible(new Vector2Int(x, y));
                            if (!isvisible)
                            texture.SetPixel(x, y, paintColor);
                        }
                    }
                }


                // Apply changes to the texture
                texture.Apply();
            }
        }
    }
    bool IsPixelVisible(Vector2Int pixel)
    {
        // Convert the pixel coordinates to UV coordinates
        Vector2 uv = new Vector2(
            (float)pixel.x / texture.width,
            (float)pixel.y / texture.height
        );

        // Convert UV coordinates to screen coordinates
        Vector3 screenPoint = GetComponent<SpriteRenderer>().transform.TransformPoint(new Vector3(uv.x, uv.y, 0));
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(screenPoint);

        // Check if the screen coordinates are within the camera's viewport
        if (viewportPoint.x >= 0 && viewportPoint.x <= 1 && viewportPoint.y >= 0 && viewportPoint.y <= 1 && viewportPoint.z > 0)
        {
            // Cast a ray from the camera through the screen coordinates
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(screenPoint.x, screenPoint.y, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the ray hits the object with the SpriteRenderer component
                if (hit.collider.gameObject == gameObject && hit.textureCoord == uv)
                {
                    return true;
                }
            }
        }

        return false;
    }
}

