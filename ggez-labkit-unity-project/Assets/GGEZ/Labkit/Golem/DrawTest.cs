using UnityEngine;

public class DrawTest : MonoBehaviour
{
    public GUIStyle Style;
    public GUIStyle NodeStyle;
    public GUIStyle NodeBlockStyle;

    public bool isHover, isActive, on, keyboard;

    public void DrawNode(Rect position, GUIContent content)
    {
        NodeStyle.Draw(position, content, isHover, isActive, on, keyboard);
    }

    public void DrawBlock(Rect position)
    {
        NodeBlockStyle.Draw(position, isHover, isActive, on, keyboard);
    }
}
