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

namespace GGEZ
{
    [CustomEditor(typeof(ExampleMonoBehaviour))]
    public class HandleTesterEditor : Editor
    {
        private Tool _oldTool = Tool.None;

        private string _lastClickedObject = "";

        private void OnDisable()
        {
            this.restoreOldTool();
        }

        private void restoreOldTool()
        {
            if (_oldTool != Tool.None)
            {
                Tools.current = _oldTool;
                _oldTool = Tool.None;
            }
        }

        private void OnSceneGUI()
        {
            bool isDrawingToTheFirstSceneView =
                    SceneView.sceneViews != null
                    && SceneView.sceneViews.Count >= 1
                    && SceneView.currentDrawingSceneView != null
                    && object.ReferenceEquals(SceneView.currentDrawingSceneView, SceneView.sceneViews[0]);
            if (!isDrawingToTheFirstSceneView)
            {
                this.restoreOldTool();
                return;
            }
            if (Tools.current != Tool.None)
            {
                _oldTool = Tools.current;
                Tools.current = Tool.None;
                Tools.viewTool = ViewTool.None;
            }
            Camera sceneCamera;
            Rect sceneCameraScreenRect;
            if (true)
            {
                sceneCamera = SceneView.currentDrawingSceneView.camera;
                var screenSize = sceneCamera.ViewportToScreenPoint(Vector3.one);
                sceneCameraScreenRect = new Rect(0f, 0f, screenSize.x, screenSize.y);
            }

            // SceneView.currentDrawingSceneView.isRotationLocked = true;
            // SceneView.currentDrawingSceneView.in2DMode = true;

            bool shouldPreventClicksFromPassingThrough = Event.current.type == EventType.Layout;
            if (shouldPreventClicksFromPassingThrough)
            {
                HandleUtility.AddDefaultControl(GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive));
            }

            MouseCursor mouseCursorToUse = MouseCursor.Arrow;
            EditorGUIUtility.AddCursorRect(sceneCameraScreenRect, mouseCursorToUse);

            if (Event.current.type == EventType.MouseDown)
            {
                var pickedObject = HandleUtility.PickGameObject(Event.current.mousePosition, false);
                if (pickedObject != null)
                {
                    _lastClickedObject = pickedObject.name;
                    SceneView.currentDrawingSceneView.ShowNotification(new GUIContent("You clicked " + _lastClickedObject));
                }
            }

            Handles.BeginGUI();

            // Use GUI to draw over the scene view

            GUILayout.BeginArea(sceneCameraScreenRect);

            // Can use EditorGUILayout and GUILayout as normal to draw things over the scene view

            GUILayout.EndArea();

            Handles.EndGUI();

            bool shouldStopInputGoingToTheRestOfUnity =
                    Event.current.type != EventType.Repaint
                    && Event.current.type != EventType.Layout
                    && GUIUtility.hotControl == 0;
            if (shouldStopInputGoingToTheRestOfUnity)
            {
                Event.current.Use();
            }
        }
    }
}
