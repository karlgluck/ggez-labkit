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
namespace GGEZ
{

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//----------------------------------------------------------------------
// Add this component into the Unity UI to make a LayoutGroup that
// preserves the aspect-ratio size of its members. Remember to add
// the LayoutElement component to children!
//----------------------------------------------------------------------
[RequireComponent (typeof(RectTransform))]
public class AspectRatioLayoutGroup : UnityEngine.UI.GridLayoutGroup
{
[UnityEngine.SerializeField]
public float AspectRatio = .75f;

new void OnValidate ()
    {
    this.AspectRatio = Mathf.Max (this.AspectRatio, 0f);
    this.updateCellSize ();
    base.OnValidate ();
    }

void updateCellSize ()
    {
    if (constraint == Constraint.FixedColumnCount)
        {
        float cellWidth = (this.rectTransform.rect.width - this.padding.horizontal) / this.constraintCount;
        base.cellSize = new Vector2 (cellWidth, cellWidth / this.AspectRatio);
        }
    else if (constraint == Constraint.FixedRowCount)
        {
        float cellHeight = (this.rectTransform.rect.height - this.padding.vertical) / this.constraintCount;
        base.cellSize = new Vector2 (cellHeight * this.AspectRatio, cellHeight);
        }
    this.SetDirty ();
    }

new void OnRectTransformDimensionsChange ()
    {
    this.updateCellSize ();
    base.OnRectTransformDimensionsChange();
    }

public void ResizePanel (int itemCount)
    {
    var sizeDelta = this.rectTransform.sizeDelta;
    if (constraint == Constraint.FixedColumnCount)
        {
        sizeDelta.y = (cellSize.y * itemCount) / (float)constraintCount;
        }
    else if (constraint == Constraint.FixedRowCount)
        {
        sizeDelta.x = (cellSize.x * itemCount) / (float)constraintCount;
        }
    this.rectTransform.sizeDelta = sizeDelta;
    }

}



}

