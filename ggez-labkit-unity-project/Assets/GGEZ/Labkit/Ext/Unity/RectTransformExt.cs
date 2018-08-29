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

public static partial class RectTransformExt
{
    public static Canvas FindParentCanvas(this RectTransform self)
    {
        var transform = (Transform)self;
        Canvas canvas = null;
        int sentinel = 9999;
        while (transform != null && --sentinel > 0 && canvas == null)
        {
            canvas = (Canvas)transform.GetComponent(typeof(Canvas));
            transform = transform.parent;
        }
        return canvas;
    }

    public static Rect GetRectInLocalSpace(this RectTransform self, RectTransform localSpace)
    {
        Vector2 size = Vector2.Scale(self.rect.size, self.lossyScale);
        Rect rect = new Rect(self.position.x, self.position.y, size.x, size.y);
        rect.x -= (self.pivot.x * size.x);
        rect.y -= (self.pivot.y * size.y);
        var min = localSpace.InverseTransformPoint(rect.min);
        var max = localSpace.InverseTransformPoint(rect.max);
        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
}
