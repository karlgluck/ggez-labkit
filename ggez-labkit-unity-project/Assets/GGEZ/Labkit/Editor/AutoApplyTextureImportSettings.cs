﻿using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;

namespace GGEZ
{
namespace Labkit
{



//---------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------
public class AutoApplyTextureImportSettings : AssetPostprocessor
{

//---------------------------------------------------------------------------------------
//---------------------------------------------------------------------------------------
void OnPreprocessTexture ()
    {
    TextureImporter textureImporter  = (TextureImporter)this.assetImporter;
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
void OnPostprocessTexture (Texture2D texture)
    {
    TextureImporter textureImporter  = (TextureImporter)this.assetImporter;
    bool isSprite = textureImporter.textureType == TextureImporterType.Sprite && !string.IsNullOrEmpty (textureImporter.spritePackingTag);
    bool ignoreNpotTextures = !LabkitProjectSettings.Instance.BreakNonPowerOfTwoTextures;
    if (isSprite || ignoreNpotTextures)
        {
        return;
        }

    if (Mathf.IsPowerOfTwo (texture.width)
            && Mathf.IsPowerOfTwo (texture.height))
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
}
 
