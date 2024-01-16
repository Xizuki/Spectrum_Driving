using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace XizukiMethods
{
    namespace Textures
    {
        public static class Xi_Helper_Texture
        {


            // CHATGPT'ed, NEED TO READ UP
            #region Contrast Methods
            public static Texture2D AdjustContrast(Texture2D texture, float contrast)
            {
                // Clone the original texture to avoid modifying it directly
                Texture2D adjustedTexture = new Texture2D(texture.width, texture.height);

                for (int y = 0; y < texture.height; y++)
                {
                    for (int x = 0; x < texture.width; x++)
                    {
                        Color originalColor = texture.GetPixel(x, y);
                        Color adjustedColor = AdjustPixelColorContrast(originalColor, contrast);
                        adjustedTexture.SetPixel(x, y, adjustedColor);
                    }
                }

                // Apply changes and update the target image
                adjustedTexture.Apply();
                return adjustedTexture;
            }

            public static Color AdjustPixelColorContrast(Color originalColor, float contrast)
            {
                // Adjust contrast using the contrast factor
                Color adjustedColor = new Color(
                    (originalColor.r - 0.5f) * contrast + 0.5f,
                    (originalColor.g - 0.5f) * contrast + 0.5f,
                    (originalColor.b - 0.5f) * contrast + 0.5f,
                    originalColor.a
                );

                // Clamp the color values to the valid range (0 to 1)
                adjustedColor.r = Mathf.Clamp01(adjustedColor.r);
                adjustedColor.g = Mathf.Clamp01(adjustedColor.g);
                adjustedColor.b = Mathf.Clamp01(adjustedColor.b);

                return adjustedColor;
            }
            #endregion




            public static Texture CombineTexturesIntoSpriteSheet(Texture2D[] textures, int maxDimension, int padding)
            {
                Texture2D result = null;



                // Calculate the width and height of the combined sprite sheet
                int spriteDimension = 0;
                int width = 0;
                int height = 0;
                foreach (Texture2D texture in textures)
                {
                    width += texture.width + padding;
                    height = Mathf.Max(height, texture.height);
                }



                // Create a new Texture2D to hold the combined sprite sheet
                Texture2D combinedTexture = new Texture2D(maxDimension, maxDimension);



                int iterations = maxDimension / textures[0].width;

                // Combine the sprites into the new Texture2D
                int x = 0;
                foreach (Texture2D texture in textures)
                {
                    Color[] pixels = texture.GetPixels();
                    for (int y = 0; y < texture.height; y++)
                    {
                        for (int px = 0; px < texture.width; px++)
                        {
                            combinedTexture.SetPixel(x + px, y, pixels[px + y * texture.width]);
                        }
                    }
                    x += texture.width + padding;
                }
                combinedTexture.Apply();




                result = combinedTexture;

                return result;
            }




            public static Texture CombineTexturesIntoSpriteSheetFixedSquareSize(Texture2D[] textures, int maxDimension, int padding)
            {
                Texture2D result = null;


                // Create a new Texture2D to hold the combined sprite sheet
                //Texture2D combinedTexture = new Texture2D(maxDimension, maxDimension);


                // Calculate the width and height of the combined sprite sheet
                int spriteDimension = 0;
                int width = 0;
                int height = 0;
                foreach (Texture2D texture in textures)
                {
                    width += texture.width + padding;
                    height = Mathf.Max(height, texture.height);
                }



                Texture2D combinedTexture = new Texture2D(maxDimension, maxDimension, TextureFormat.ARGB32, false);

                Color32[] transparentColors = new Color32[maxDimension * maxDimension];

                for (int i = 0; i < transparentColors.Length; i++)
                {
                    transparentColors[i] = new Color32(0, 0, 0, 0);
                }

                combinedTexture.SetPixels32(transparentColors);


                int xPos = 0;
                int yPos = 0;

                int index = 0;

                foreach (Texture2D texture in textures)
                {
                    Color32[] pixels = texture.GetPixels32();

                    int xOffSet = (112 - texture.width) / 2;
                    int yOffSet = (112 - texture.height) / 2;

                    xPos += xOffSet;
                    yPos += yOffSet;


                    for (int x = 0; x < texture.width; x++)
                    {
                        for (int y = 0; y < texture.height; y++)
                        {
                            if (x + (y * texture.width) > 112 * 112) { continue; }

                            //Debug.Log("pixels.Length = " + pixels.Length);

                            combinedTexture.SetPixel(x + xPos, y + yPos, pixels[x + (y * texture.width)]);
                        }
                    }


                    //xPos += xOffSet;
                    //yPos += yOffSet;

                    xPos += 112 - xOffSet;
                    yPos -= yOffSet;

                    if (xPos >= maxDimension)
                    {
                        yPos += 112 - yOffSet;
                        xPos = 0;

                        index++;

                    }

                }
                combinedTexture.Apply();



                result = combinedTexture;

                return result;
            }


            public static void OverrideStaticTextureObjectWithDyanamicTexture(ref Texture staticTexture, Texture2D overrideTexture)
            {
                Texture2D staticTexture2D = (Texture2D)staticTexture;

                staticTexture2D.Reinitialize(overrideTexture.width, overrideTexture.height);

                Color[] pixels = overrideTexture.GetPixels();

                staticTexture2D.SetPixels(pixels);

                staticTexture2D.Apply();

                staticTexture = staticTexture2D;
            }


        }
    }
}