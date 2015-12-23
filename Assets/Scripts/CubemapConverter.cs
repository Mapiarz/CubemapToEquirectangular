using UnityEngine;

public static class CubemapConverter
{
    public static byte[] ConvertToEquirectangular(Texture2D sourceTexture, int outputWidth, int outputHeight)
    {
        Texture2D equiTexture = new Texture2D(outputWidth, outputHeight, TextureFormat.ARGB32, false);
        float u, v; //Normalised texture coordinates, from 0 to 1, starting at lower left corner
        float phi, theta; //Polar coordinates
        int cubeFaceWidth, cubeFaceHeight;

        cubeFaceWidth = sourceTexture.width / 4; //4 horizontal faces
        cubeFaceHeight = sourceTexture.height / 3; //3 vertical faces


        for (int j = 0; j < equiTexture.height; j++)
        {
            //Rows start from the bottom
            v = 1 - ((float)j / equiTexture.height);
            theta = v * Mathf.PI;

            for (int i = 0; i < equiTexture.width; i++)
            {
                //Columns start from the left
                u = ((float)i / equiTexture.width);
                phi = u * 2 * Mathf.PI;

                float x, y, z; //Unit vector
                x = Mathf.Sin(phi) * Mathf.Sin(theta) * -1;
                y = Mathf.Cos(theta);
                z = Mathf.Cos(phi) * Mathf.Sin(theta) * -1;

                float xa, ya, za;
                float a;

                a = Mathf.Max(new float[3] { Mathf.Abs(x), Mathf.Abs(y), Mathf.Abs(z) });

                //Vector Parallel to the unit vector that lies on one of the cube faces
                xa = x / a;
                ya = y / a;
                za = z / a;

                Color color;
                int xPixel, yPixel;
                int xOffset, yOffset;

                if (xa == 1)
                {
                    //Right
                    xPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceWidth);
                    xOffset = 2 * cubeFaceWidth; //Offset
                    yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                    yOffset = cubeFaceHeight; //Offset
                }
                else if (xa == -1)
                {
                    //Left
                    xPixel = (int)((((za + 1f) / 2f)) * cubeFaceWidth);
                    xOffset = 0;
                    yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                    yOffset = cubeFaceHeight;
                }
                else if (ya == 1)
                {
                    //Up
                    xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                    xOffset = cubeFaceWidth;
                    yPixel = (int)((((za + 1f) / 2f) - 1f) * cubeFaceHeight);
                    yOffset = 2 * cubeFaceHeight;
                }
                else if (ya == -1)
                {
                    //Down
                    xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                    xOffset = cubeFaceWidth;
                    yPixel = (int)((((za + 1f) / 2f)) * cubeFaceHeight);
                    yOffset = 0;
                }
                else if (za == 1)
                {
                    //Front
                    xPixel = (int)((((xa + 1f) / 2f)) * cubeFaceWidth);
                    xOffset = cubeFaceWidth;
                    yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                    yOffset = cubeFaceHeight;
                }
                else if (za == -1)
                {
                    //Back
                    xPixel = (int)((((xa + 1f) / 2f) - 1f) * cubeFaceWidth);
                    xOffset = 3 * cubeFaceWidth;
                    yPixel = (int)((((ya + 1f) / 2f)) * cubeFaceHeight);
                    yOffset = cubeFaceHeight;
                }
                else
                {
                    Debug.LogWarning("Unknown face, something went wrong");
                    xPixel = 0;
                    yPixel = 0;
                    xOffset = 0;
                    yOffset = 0;
                }

                xPixel = Mathf.Abs(xPixel);
                yPixel = Mathf.Abs(yPixel);

                xPixel += xOffset;
                yPixel += yOffset;

                color = sourceTexture.GetPixel(xPixel, yPixel);
                equiTexture.SetPixel(i, j, color);
            }
        }

        equiTexture.Apply();
        var bytes = equiTexture.EncodeToPNG();
        Object.DestroyImmediate(equiTexture);

        return bytes;
    }
}
