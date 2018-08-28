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
using System.Collections.Generic;
using System;
using GGEZ.FullSerializer;

namespace GGEZ.Omnibus
{
    [fsSerializeEnumAsInteger]
    public enum EditorStateIndex : int { Invalid = int.MaxValue }
    [fsSerializeEnumAsInteger]
    public enum EditorLayerIndex : int { Invalid = int.MaxValue }
    [fsSerializeEnumAsInteger]
    public enum EditorTransitionIndex : int { Invalid = int.MaxValue }

    //-------------------------------------------------------------------------
    // EditorSpecialStateType
    //-------------------------------------------------------------------------
    public enum EditorSpecialStateType
    {
        Normal,
        LayerEnter,
        LayerExit,
        LayerAny,
    }

    //-------------------------------------------------------------------------
    // EditorState
    //-------------------------------------------------------------------------
    public class EditorState
    {
        public string Name;
        public EditorLayerIndex Layer = EditorLayerIndex.Invalid;
        public EditorStateIndex Index = EditorStateIndex.Invalid;
        public StateIndex CompiledIndex = StateIndex.Invalid;
        public EditorSpecialStateType SpecialState;
        public Rect Position;
        public List<EditorScript> Scripts = new List<EditorScript>();
        public List<EditorTransitionIndex> TransitionsIn = new List<EditorTransitionIndex>();
        public List<EditorTransitionIndex> TransitionsOut = new List<EditorTransitionIndex>();

        public OmnibusEditorUtility.NodeStyleColor NodeColor
        {
            get
            {
                switch (SpecialState)
                {
                    default:
                    case EditorSpecialStateType.Normal: return OmnibusEditorUtility.NodeStyleColor.Gray;
                    case EditorSpecialStateType.LayerEnter: return OmnibusEditorUtility.NodeStyleColor.Green;
                    case EditorSpecialStateType.LayerExit: return OmnibusEditorUtility.NodeStyleColor.Red;
                    case EditorSpecialStateType.LayerAny: return OmnibusEditorUtility.NodeStyleColor.Yellow;
                }
            }
        }

        public IDraggable DragPosition ()
        {
            return new DraggablePosition() { EditorState = this, Position = Position.position };
        }
        class DraggablePosition : IDraggable
        {
            public EditorState EditorState;
            public Vector2 Position;
            public Vector2 Offset
            {
                set { EditorState.Position.position = OmnibusEditorUtility.SnapToGrid(Position + value); }
            }
        }

        public IDraggable DragSize ()
        {
            return new DraggableSize() { EditorState = this, Size = Position.size };
        }
        class DraggableSize : IDraggable
        {
            public EditorState EditorState;
            public Vector2 Size;
            public Vector2 Offset
            {
                set { EditorState.Position.size = OmnibusEditorUtility.SnapToGrid(Size + value); }
            }
        }
    }

    //-------------------------------------------------------------------------
    // EditorScript
    //-------------------------------------------------------------------------
    public class EditorScript
    {
        // TODO: could add an enabled flag here to allow us to turn on/off
        //       individual scripts while editing!

        public Script Script;
    }

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

        public IDraggable DragExpressionAnchor ()
        {
            return new DraggablePosition() { EditorTransition = this, ExpressionAnchor = ExpressionAnchor };
        }

        class DraggablePosition : IDraggable
        {
            public EditorTransition EditorTransition;
            public Vector2 ExpressionAnchor;
            public Vector2 Offset
            {
                set { EditorTransition.ExpressionAnchor = OmnibusEditorUtility.SnapToGrid(ExpressionAnchor + value); }
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
