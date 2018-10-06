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
using UnityEditor;
using UnityObject = UnityEngine.Object;
using UnityObjectList = System.Collections.Generic.List<UnityEngine.Object>;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR

namespace GGEZ.Labkit
{


    //-------------------------------------------------------------------------
    // EditorTransition
    //-------------------------------------------------------------------------
    public class EditorTransition
    {
        public string Name;
        public EditorTransitionIndex Index;
        public EditorTransitionExpression Expression;
        public Vector2 ExpressionAnchor;
        public Rect Position;
        public EditorStateIndex From;
        public EditorStateIndex To;

        public IDraggable DragExpressionAnchor()
        {
            return new DraggablePosition() { EditorTransition = this, ExpressionAnchor = ExpressionAnchor };
        }

        private class DraggablePosition : IDraggable
        {
            public EditorTransition EditorTransition;
            public Vector2 ExpressionAnchor;
            public Vector2 Offset
            {
                set { EditorTransition.ExpressionAnchor = GolemEditorUtility.SnapToGrid(ExpressionAnchor + value); }
            }
        }
    }


    //---------------------------------------------
    // EditorTransitionExpression
    //
    // Used to build the transition between states
    //---------------------------------------------
    public class EditorTransitionExpression
    {
        public EditorTransitionExpressionType Type;

        public Rect Position; // this needs to be updated every time the expression or the expression anchor changes

        // Used for Type == EditorTransitionExpressionType.Trigger
        public Trigger Trigger;

        // Used for Type == EditorTransitionExpressionType.And / ...Or
        public List<EditorTransitionExpression> Subexpressions = new List<EditorTransitionExpression>();
    }

    //--------------------------------
    // EditorTransitionExpressionType
    //--------------------------------
    public enum EditorTransitionExpressionType
    {
        False,
        True,
        Trigger,

        [HasSubexpressions]
        And,

        [HasSubexpressions]
        Or,
    }


    //-----------------------------------------------------------------------------
    // HasSubexpressionsAttribute
    //-----------------------------------------------------------------------------
    [AttributeUsage(AttributeTargets.Field)]
    public class HasSubexpressionsAttribute : Attribute
    {
        public static bool IsFoundOn(EditorTransitionExpressionType type)
        {
            var member = typeof(EditorTransitionExpressionType).GetMember(type.ToString())[0];
            return member.GetCustomAttributes(typeof(HasSubexpressionsAttribute), false).Length > 0;
        }
    }

}

#endif
