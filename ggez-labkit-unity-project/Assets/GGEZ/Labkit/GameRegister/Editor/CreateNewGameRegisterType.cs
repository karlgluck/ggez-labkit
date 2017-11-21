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

namespace GGEZ
{
namespace Labkit
{


public static class CreateNewGameRegisterType
{

[MenuItem ("Labkit/Create/Game Register Type")]
static void LabkitNewGameRegisterType ()
    {

    string path = EditorUtility.SaveFilePanelInProject (
            "New Game Register Type",
            "type.cs",
            "cs",
            "Name the source file <type>.cs to make the Game Register"
            );
    if (string.IsNullOrEmpty (path))
        {
        return;
        }


    string type = System.IO.Path.GetFileNameWithoutExtension (path);
    string upperType = type.Substring (0, 1).ToUpper () + type.Substring (1);
    string lowerType = type.Substring (0, 1).ToLower () + type.Substring (1);
    Debug.LogFormat ("Creating {0}Register for data of type {1}", upperType, type);

    string directory = Path.GetDirectoryName (path);

    LabkitEditorUtility.WriteFileUsingTemplate (
            "GameRegister_So",
            Path.Combine (directory, upperType + "Register.cs"),
            "%TYPE% " + type,
            "%UPPERTYPE% " + upperType,
            "%LOWERTYPE% " + lowerType
            );

    LabkitEditorUtility.WriteFileUsingTemplate (
            "GameRegisterListener_Mb",
            Path.Combine (directory, upperType + "RegisterListener.cs"),
            "%TYPE% " + type,
            "%UPPERTYPE% " + upperType,
            "%LOWERTYPE% " + lowerType
            );
    }
}

}
}
