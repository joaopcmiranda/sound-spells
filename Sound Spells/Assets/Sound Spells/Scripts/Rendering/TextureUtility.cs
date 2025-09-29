using UnityEngine;

public static class TextureUtility
{
    public static Texture2D CreatePixelatedTexture(Sprite sourceSprite, int targetPixelSize)
    {
        Texture2D sourceTexture = sourceSprite.texture;

        Rect spriteRect = sourceSprite.textureRect;
        Vector2 scale = new Vector2(spriteRect.width / sourceTexture.width, spriteRect.height / sourceTexture.height);
        Vector2 offset = new Vector2(spriteRect.x / sourceTexture.width, spriteRect.y / sourceTexture.height);

        // 1. Create a temporary RenderTexture.
        RenderTexture tempRT = RenderTexture.GetTemporary(targetPixelSize, targetPixelSize, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        tempRT.filterMode = FilterMode.Point;
        RenderTexture.active = tempRT;

        // 2. Clear the render texture to prevent artifacts from previous frames.
        GL.Clear(true, true, Color.clear);

        // 3. Blit using the overload that takes scale and offset.
        Graphics.Blit(sourceTexture, tempRT, scale, offset);

        // 4. Create a new Texture2D to hold the result.
        Texture2D resultTexture = new Texture2D(targetPixelSize, targetPixelSize, TextureFormat.RGBA32, false);
        resultTexture.filterMode = FilterMode.Point;
        
        // 5. Read the pixels from the active RenderTexture.
        resultTexture.ReadPixels(new Rect(0, 0, targetPixelSize, targetPixelSize), 0, 0);
        resultTexture.Apply();

        // 6. Clean up.
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(tempRT);

        return resultTexture;
    }
}