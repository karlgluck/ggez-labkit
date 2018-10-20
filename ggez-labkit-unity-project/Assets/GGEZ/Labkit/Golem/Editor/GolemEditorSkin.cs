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

using System;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using System.Reflection;
using GGEZ.FullSerializer;

#if UNITY_EDITOR
using UnityEditor;


namespace GGEZ.Labkit
{
    [CreateAssetMenu(menuName = "GGEZ/Golem/Editor/Skin")]
    public class GolemEditorSkin : ScriptableObject
    {
        private static GolemEditorSkin _current;
        public static GolemEditorSkin Current
        {
            get
            {
                if (_current == null)
                {
                    const string key = "GolemEditorSkin.Current";
                    string guid = null;
                    if (PlayerPrefs.HasKey(key))
                    {
                        guid = PlayerPrefs.GetString(key);
                        _current = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as GolemEditorSkin;
                    }
                    if (_current == null)
                    {
                        var guids = AssetDatabase.FindAssets("t:"+typeof(GolemEditorSkin).Name);
                        if (guids.Length == 0)
                        {
                            Debug.LogError("No GolemEditorSkin found! Making one...");
                            _current = ScriptableObject.CreateInstance<GolemEditorSkin>();
                            var path = AssetDatabase.GenerateUniqueAssetPath("Assets/Golem Editor Skin.asset");
                            AssetDatabase.CreateAsset(_current, path);
                            guid = AssetDatabase.AssetPathToGUID(path);
                        }
                        else
                        {
                            guid = guids[0];
                            _current = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as GolemEditorSkin;
                        }
                        PlayerPrefs.SetString(key, guid);
                    }
                }
                return _current;
            }
        }

        public GUIStyle CellStyle;
        public GUIStyle CellBodyStyle;
        public GUIStyle CellWithoutBodyStyle;
        public GUIStyle InputLabelStyle;
        public GUIStyle OutputLabelStyle;

        public GUIStyle PortStyle;

        public GUIStyle ScriptStyle;

        void Reset()
        {
        }

        void OnValidate()
        {
            _current = this;
        }
    }
}


#endif
