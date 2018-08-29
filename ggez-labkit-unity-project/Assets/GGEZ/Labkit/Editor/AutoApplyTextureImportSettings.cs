// This is free and unencumbered software released into the public domain.
// 
// Anyone is free to copy, modify, publish, use, compile, sell, or
// distribute this software, either in source code form or as a compiled
// binary, for any purpose, commercial or non-commercial, and by any
// means.
// 
// In jurisdictions that recognize copyright laws, the author or authors
// of this software dedicate any and all copyright interest in the
// software to the public domain. We make this dedication for the benefit
// of the public at large and to the detriment of our heirs and
// successors. We intend this dedication to be an overt act of
// relinquishment in perpetuity of all present and future rights to this
// software under copyright law.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS BE LIABLE FOR ANY CLAIM, DAMAGES OR
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE,
// ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// 
// For more information, please refer to <http://unlicense.org/>

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

namespace GGEZ.Labkit
{
    //---------------------------------------------------------------------------------------
    //---------------------------------------------------------------------------------------
    public class AutoApplyTextureImportSettings : AssetPostprocessor
    {
        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        private void OnPreprocessTexture()
        {
            TextureImporter textureImporter = (TextureImporter)this.assetImporter;
            if (LabkitProjectSettings.Instance.TextureDefaults == LabkitProjectSettings_TextureDefaults.PixelPerfect2D)
            {
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                textureImporter.mipmapEnabled = false;
                textureImporter.filterMode = FilterMode.Point;
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                textureImporter.spritePixelsPerUnit = LabkitProjectSettings.Instance.PixelsPerUnit;
                textureImporter.spritePivot = Vector2.zero;
            }
        }

        //---------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------
        private void OnPostprocessTexture(Texture2D texture)
        {
            TextureImporter textureImporter = (TextureImporter)this.assetImporter;
            bool isSprite = textureImporter.textureType == TextureImporterType.Sprite && !string.IsNullOrEmpty(textureImporter.spritePackingTag);
            bool ignoreNpotTextures = !LabkitProjectSettings.Instance.BreakNonPowerOfTwoTextures;
            if (isSprite || ignoreNpotTextures)
            {
                return;
            }

            if (Mathf.IsPowerOfTwo(texture.width)
                    && Mathf.IsPowerOfTwo(texture.height))
            {
                return;
            }

            for (int m = 0; m < texture.mipmapCount; m++)
            {
                Color[] c = texture.GetPixels(m);
                for (int i = 0; i < c.Length; i += 3)
                {
                    c[i].r = 1;
                    c[i].g = 0;
                    c[i].b = 1;
                }
                texture.SetPixels(c, m);
            }
        }
    }
}