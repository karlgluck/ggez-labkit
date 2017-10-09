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
using System.Linq;
using System.IO;

namespace GGEZ
{
public static partial class EditorExt
{

[MenuItem("Labkit/Export/Project as .unitypackage...")]
public static void BuildAssetPackage ()
    {
    var path = EditorUtility.SaveFilePanel ("Export Unity Asset Package", "", "Project.unitypackage", "unitypackage");
    if (string.IsNullOrEmpty (path))
        {
        return;
        }
    UnityEditor.AssetDatabase.ExportPackage (
        UnityEditor.AssetDatabase.FindAssets ("").Select ((guid) => UnityEditor.AssetDatabase.GUIDToAssetPath (guid)).ToArray (),
        path,
        ExportPackageOptions.IncludeLibraryAssets
        );
    string fileName = Path.GetFileName (path); 
#if UNITY_EDITOR_WIN
    string sevenZipPath = @"""C:\Program Files\7-Zip\7z.exe""";
    File.WriteAllText (
            Path.Combine (Path.GetDirectoryName (path), "win32_package_cleanup.bat"),
            string.Format (
                    @"
                    {1} x ""{0}""
                    {1} x archtemp.tar
                    
                    for /F ""tokens=1 delims=\"" %%i in ('findstr /s /n /i /p /m /c:""C:/Program Files/Unity/"" pathname') do ({1} d archtemp.tar %%i)
                    for /F ""tokens=1 delims=\"" %%i in ('findstr /s /n /i /p /m /c:""Library/BuildPlayer.prefs"" pathname') do ({1} d archtemp.tar %%i)

                    {1} u ""{0}"" archtemp.tar

                    for /F ""delims="" %%i in ('dir /b /a:D') do (rmdir ""%%i"" /s/q)
                    del archtemp.tar /s/q
                    ".Replace ("                        ", ""),
                    fileName,
                    sevenZipPath)
            );
#endif
#if UNITY_EDITOR_OSX
    File.WriteAllText (
            Path.Combine (Path.GetDirectoryName (path), "macOS_package_cleanup"),
            string.Format (
                    @"# Usage (from terminal): $  source ./macOS_package_cleanup

                    tar xopf '{0}'
                    grep --include=pathname -rnl '.' -e '/Applications/Unity/Unity.app/' -e 'Library/BuildPlayer.prefs' | xargs -n1 dirname | xargs rm -rf
                    tar -cvzf tmp.unitypackage */
                    rm -R -- */
                    mv -f tmp.unitypackage '{0}'
                    ".Replace ("                     ", ""),
                    fileName
                    )
            );
#endif
    }
}
}
