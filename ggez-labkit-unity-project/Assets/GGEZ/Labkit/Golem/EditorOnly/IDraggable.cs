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

#if UNITY_EDITOR
using UnityEditor;

namespace GGEZ.Labkit
{

    //-------------------------------------------------------------------------
    // IDraggable
    //-------------------------------------------------------------------------
    public interface IDraggable
    {
        Vector2 Offset { set; }
    }

    // if drag object is CreateTransitionDragObject, send it OnInspectorGUI
    // all objects overlapping the screen receive OnInspectorGUI
    // selected object receives OnInspectorGUI if it didn't already
    // if dragobject is not CreateTransitionDragObject, send it OnInspectorGUI
    // at the end of Layout, objects in the ObjectsToDelete list are processed

    // public interface IGolemEditorWindow
    // {
    //     BaseDragObject BeginDrag(CanvasPickResult obj);
    //     void ContextMenu(CanvasPickResult obj);
    //     void OnInspectorGUI(CanvasDragObject obj);
    // }

    // public abstract class BaseDragObject
    // {
    //     public abstract void OnInspectorGUI(IGolemEditorWindow window);
    // }

    // public abstract class BaseEditorGraphObject
    // {
    //     public Rect Position;

    //     public abstract void Pick(IGolemEditorWindow window, Vector2 point, List<PickResult> pickResults);
    //     public abstract void OnInspectorGUI(IGolemEditorWindow window);
    //     public abstract void BeginDrag(IGolemEditorWindow window);
    // }

    // public class PickResult
    // {
    //     public int Priority;
    //     public BaseEditorGraphObject GraphObject;

    //     public virtual BaseDragObject BeginDrag (IGolemEditorWindow window) { return null; }
    //     public virtual void ContextMenu (IGolemEditorWindow window) { }
    // }

    // public class CanvasPickResult : PickResult
    // {
    //     public override BaseDragObject BeginDrag(IGolemEditorWindow window)
    //     {
    //         return window.BeginDrag(this);
    //     }

    //     public override void ContextMenu(IGolemEditorWindow window)
    //     {
    //         window.ContextMenu(this);
    //     }
    // }

    // public class CanvasDragObject : BaseDragObject
    // {
    //     public override void OnInspectorGUI(IGolemEditorWindow window)
    //     {
    //         window.OnInspectorGUI(this);
    //     }
    // }

    // public class CreateWireDragObject : BaseDragObject
    // {
    // }

    // public class BaseEditorCircuitGraphObject : BaseEditorGraphObject
    // {
    // }

}

#endif
