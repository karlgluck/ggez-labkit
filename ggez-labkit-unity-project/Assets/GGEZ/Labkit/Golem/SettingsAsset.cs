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
using System;
using System.Reflection;
using GGEZ.FullSerializer;
using System.Collections.Generic;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using UnityEditor;

namespace GGEZ.Labkit
{
    //-----------------------------------------------------------------------------
    // SettingsAsset
    //-----------------------------------------------------------------------------
    [CreateAssetMenu(menuName = "GGEZ/Omnibus/Settings", fileName = "New Settings.asset")]
    public class SettingsAsset : ScriptableObject, IHasSettings
    {

        [SerializeField]
        private Settings _settings;
        public Settings Settings { get { return _settings; } }

        /// <summary>
        ///     Settings asset that settings are inherited from
        /// </summary>
        [Tooltip("Another SettingsAsset to be queried if this one is missing a value")]
        public SettingsAsset InheritFrom;

        /// <summary>
        ///     Returns the settings asset that settings are inherited from
        /// </summary>
        public IHasSettings InheritsSettingsFrom { get { return InheritFrom; } }


        void Reset()
        {
            _settings = new Settings(this);
        }

        void OnEnable()
        {
            name = System.IO.Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(this));
        }

        void OnValidate()
        {
            _settings = _settings ?? new Settings(this);
        }
    }
}
