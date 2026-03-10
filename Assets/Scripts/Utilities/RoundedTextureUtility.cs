using UnityEngine;

public static class RoundedTextureUtility
{
    public static void ApplyRoundedCorners(Texture2D texture, float radiusDp)
    {
        if (texture == null)
            return;

        int width = texture.width;
        int height = texture.height;
        if (width <= 1 || height <= 1)
            return;

        float dpi = Screen.dpi > 0f ? Screen.dpi : 160f;
        int radiusPx = Mathf.RoundToInt(radiusDp * (dpi / 160f));
        radiusPx = Mathf.Clamp(radiusPx, 0, Mathf.Min(width, height) / 2);
        if (radiusPx <= 0)
            return;

        Color32[] pixels = texture.GetPixels32();
        int r = radiusPx;
        int rr = r * r;

        // Top-left
        ApplyCornerMask(pixels, width, height, 0, height - r, r - 1, height - r, rr, true, true);
        // Top-right
        ApplyCornerMask(pixels, width, height, width - r, height - r, width - r, height - r, rr, false, true);
        // Bottom-left
        ApplyCornerMask(pixels, width, height, 0, 0, r - 1, r - 1, rr, true, false);
        // Bottom-right
        ApplyCornerMask(pixels, width, height, width - r, 0, width - r, r - 1, rr, false, false);

        texture.SetPixels32(pixels);
        texture.Apply();
    }

    private static void ApplyCornerMask(
        Color32[] pixels,
        int width,
        int height,
        int startX,
        int startY,
        int centerX,
        int centerY,
        int rr,
        bool leftCorner,
        bool topCorner)
    {
        int endX = startX + (leftCorner ? centerX + 1 : width - centerX);
        int endY = startY + (topCorner ? height - centerY : centerY + 1);

        for (int y = startY; y < endY; y++)
        {
            for (int x = startX; x < endX; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                if ((dx * dx + dy * dy) > rr)
                {
                    int index = y * width + x;
                    Color32 c = pixels[index];
                    c.a = 0;
                    pixels[index] = c;
                }
            }
        }
    }
}
