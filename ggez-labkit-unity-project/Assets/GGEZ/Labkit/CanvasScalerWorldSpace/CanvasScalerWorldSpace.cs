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
namespace GGEZ
{

//----------------------------------------------------------------------
// Replaces the normal CanvasScaler 
//----------------------------------------------------------------------
[ExecuteInEditMode, RequireComponent (typeof(Canvas)), RequireComponent (typeof(RectTransform))]
public class CanvasScalerWorldSpace : MonoBehaviour
{

public Camera Camera;
public float Depth;
public float Width, Height;

[Range(0f,1f)]
public float MatchWidthOrHeight;

[Header ("Note: Set Sprite Pixels Per Unit to match this!")]
public float ReferencePixelsPerUnit = 200f;


void Update ()
    {

    if (this.Camera == null)
        {
        return;
        }

    this.enabled = false;

    Vector2 halfIntendedSize = new Vector2 (0.5f * this.Width, 0.5f * this.Height);

    this.transform.localScale = Vector3.one;
    this.Depth = Mathf.Clamp (
            this.Depth,
            this.Camera.nearClipPlane,
            this.Camera.farClipPlane
            );
    var bl = this.Camera.ViewportToWorldPoint (new Vector3 (0f, 0f, this.Depth));
    var br = this.Camera.ViewportToWorldPoint (new Vector3 (1f, 0f, this.Depth));
    var tr = this.Camera.ViewportToWorldPoint (new Vector3 (1f, 1f, this.Depth));
    this.transform.SetPositionAndRotation (
            (bl + tr) * 0.5f,
            Quaternion.LookRotation (Vector3.Cross (tr - br, bl - br))
            );
    var halfWidthAndHeight = 0.5f * (this.transform.InverseTransformPoint (tr) - this.transform.InverseTransformPoint (bl));

    var rectTransform = (RectTransform)this.transform;
    rectTransform.anchorMin = rectTransform.anchorMax = Vector2.zero;
    rectTransform.pivot = Vector2.one * 0.5f;
    rectTransform.offsetMax = halfIntendedSize;
    rectTransform.offsetMin = -halfIntendedSize;

    float scale = Mathf.Lerp (halfWidthAndHeight.x / halfIntendedSize.x, halfWidthAndHeight.y / halfIntendedSize.y, this.MatchWidthOrHeight);
    this.transform.localScale = new Vector3(scale, scale, 1f);

    var canvas = (Canvas)this.GetComponent (typeof(Canvas));
    canvas.scaleFactor = 1f;
    canvas.referencePixelsPerUnit = this.ReferencePixelsPerUnit;
    }
}


}
