using UnityEngine;

public static partial class RectTransformExt
{
public static Canvas FindParentCanvas (this RectTransform self)
    {
    var transform = (Transform)self;
    Canvas canvas = null;
    int sentinel = 9999;
    while (transform != null && --sentinel > 0 && canvas == null)
        {
        canvas = (Canvas)transform.GetComponent (typeof (Canvas));
        transform = transform.parent;
        }
    return canvas;
    }

public static Rect GetRectInLocalSpace (this RectTransform self, RectTransform localSpace)
    {
    Vector2 size = Vector2.Scale (self.rect.size, self.lossyScale);
    Rect rect = new Rect (self.position.x, self.position.y, size.x, size.y);
    rect.x -= (self.pivot.x * size.x);
    rect.y -= (self.pivot.y * size.y);
    var min = localSpace.InverseTransformPoint (rect.min);
    var max = localSpace.InverseTransformPoint (rect.max);
    return Rect.MinMaxRect (min.x, min.y, max.x, max.y);
    }

}
