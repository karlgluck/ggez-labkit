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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GGEZ
{
    [CreateAssetMenu(fileName = "New Asset Package.asset", menuName = "GGEZ/Labkit/Asset Package Config")]
    public class AssetPackageConfig : ScriptableObject
    {
        public string AssetPackageName
        {
            get { return this.name; }
            set { this.name = value; }
        }

        public Object[] Assets;

        public string[] CollectAssetPaths()
        {
            Object[] collectedAssets = this.Assets.Where(assetObject => assetObject != null).ToArray();
            for (int i = 0; i < 100; ++i)
            {
                var expanded = collectedAssets
                        .SelectMany(
                                delegate (Object assetObject)
                                    {
                                        if (assetObject is AssetPackageConfig)
                                        {
                                            return (assetObject as AssetPackageConfig).Assets;
                                        }
                                        else
                                        {
                                            return new Object[] { assetObject };
                                        }
                                    }
                                );
                bool canReturn = !expanded.Any(assetObject => assetObject is AssetPackageConfig);
                if (canReturn)
                {
                    return expanded.Select(o => AssetDatabase.GetAssetPath(o)).ToArray();
                }
                collectedAssets = expanded.ToArray();
            }
            Debug.LogErrorFormat("Too many iteration attempts while expanding asset references. Is there a recursive AssetPackageConfig reference?");
            return null;
        }

        public void Build()
        {
            string targetDirectory = System.IO.Path.Combine("../packages/", Application.unityVersion);
            System.IO.Directory.CreateDirectory(targetDirectory);
            var packagePath = System.IO.Path.Combine(targetDirectory, this.AssetPackageName + ".unitypackage");
            try
            {
                Debug.LogFormat("Building {0}", packagePath);
                AssetDatabase.ExportPackage(
                        this.CollectAssetPaths(),
                        packagePath,
                        ExportPackageOptions.Recurse
                        );
                Debug.LogFormat("Built {0}", System.IO.Path.GetFullPath(packagePath));
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat("BUILD FAILED\n{0}", e);
            }
        }
    }
}
