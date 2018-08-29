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

namespace Examples
{
    internal class ExampleModalEditorWindow : EditorWindow
    {
        public static EditorWindow CreateAndShow()
        {
            var window = ExampleModalEditorWindow.CreateInstance<ExampleModalEditorWindow>();
            Vector2 size = new Vector2(400f, 180f);
            window.ShowAsDropDown(new Rect(Screen.width / 2f - size.x / 2f, Screen.height / 2 - size.y / 2f, 1f, 1f), size);
            window.titleContent = new GUIContent("Window Title");
            return window;
        }

        protected virtual void OnLostFocus()
        {
            this.Close();
        }

        private void OnGUI()
        {
            const float titleHeight = 35f;

            GUILayout.BeginArea(new Rect(0, 0, this.position.width, this.position.height));
            GUIStyle titleStyle = new GUIStyle("Toolbar");
            titleStyle.fontSize = 12;
            titleStyle.fontStyle = FontStyle.Bold;

            titleStyle.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginHorizontal(titleStyle, GUILayout.Height(titleHeight));
            GUILayout.Label(this.titleContent, titleStyle, GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.Space(16f);

            if (GUILayout.Button("OK"))
            {
                this.Close();
            }

            GUILayout.FlexibleSpace();

            GUILayout.EndArea();
        }
    }
}
