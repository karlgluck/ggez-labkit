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
using UnityEditorInternal;
using System;
using System.IO;

namespace GGEZ
{
    public static class EditorMenuForScriptTemplates
    {
#if UNITY_EDITOR_WIN
        static readonly string s_windowsScriptPath = @"C:\Program Files\Unity\Editor\Data\Resources\ScriptTemplates";
        static readonly string s_windowsScriptPathHub = @"C:\Program Files\Unity\Hub\Editor\{0}\Editor\Data\Resources\ScriptTemplates";
#elif UNITY_EDITOR_OSX
        static readonly string s_osxScriptPath = @"/Applications/Unity/Unity.app/Contents/Resources/ScriptTemplates";
#endif

        private static readonly string s_csharpScriptFileName = @"81-C# Script-NewBehaviourScript.cs.txt";

        private static string getScriptPath()
        {
            string version = InternalEditorUtility.GetFullUnityVersion().Split(' ')[0];
#if UNITY_EDITOR_WIN
            string path = string.Format(s_windowsScriptPathHub, version, s_csharpScriptFileName);
            if (Directory.Exists(path))
            {
                return path;
            }
            else if (Directory.Exists(s_windowsScriptPath))
            {
                return s_windowsScriptPath;
            }
#elif UNITY_EDITOR_OSX
            Debug.LogError("This code is untested!! Script path might be wrong for OSX, especially when installed using Unity Hub.");
            if (Directory.Exists(s_osxScriptPath))
            {
                return s_osxScriptPath;
            }
#endif
            throw new NotImplementedException();
        }

        [MenuItem("Edit/Script Templates/C# Script Template")]
        public static void Edit_ScriptTemplates_CsharpScript()
        {
            InternalEditorUtility.OpenFileAtLineExternal(Path.Combine(getScriptPath(), s_csharpScriptFileName), 0);
        }
    }
}

