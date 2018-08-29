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

using UnityEngine;
using UnityEditor;
using System.IO;

namespace GGEZ.Labkit
{
    public static class AssetStoreLinks
    {
        [MenuItem("Asset Store/Go to Store")]
        private static void AssetStore_GoToStore()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma");
        }

        [MenuItem("Asset Store/Free Essentials/Post-Processing Stack")]
        private static void EssentialAssetsFreePostProcessingStack()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/83912");
        }

        [MenuItem("Asset Store/Free Essentials/TextMeshPro")]
        private static void EssentialAssetsFreeTextMeshPro()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/84126");
        }

        [MenuItem("Asset Store/Free Essentials/MoonSharp (Lua Scripting)")]
        private static void EssentialAssetsMoonSharp()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/33776");
        }

        [MenuItem("Asset Store/Free Essentials/Console Enhanced")]
        private static void EssentialAssetsConsoleEnhanced()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/42381");
        }

        [MenuItem("Asset Store/Free Essentials/DOTween")]
        private static void EssentialAssetsDOTween()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/27676");
        }

        [MenuItem("Asset Store/Free Essentials/ProBuilder")]
        private static void EssentialAssetsProBuilder()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/111418");
        }

        [MenuItem("Asset Store/Free Essentials/ProGrids")]
        private static void EssentialAssetsProGrids()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/111425");
        }

        [MenuItem("Asset Store/GGEZ/Perfect Pixel Camera")]
        private static void AssetStore_GGEZ_PerfectPixelCamera()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/100000");
        }

        [MenuItem("Asset Store/GGEZ/Perfect Pixel Sprite")]
        private static void AssetStore_GGEZ_PerfectPixelSprite()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:content/112908");
        }

        [MenuItem("Asset Store/GGEZ/All Assets")]
        private static void AssetStore_GGEZ_AllAssets()
        {
            UnityEditorInternal.AssetStore.Open("com.unity3d.kharma:publisher/31538");
        }
    }
}
