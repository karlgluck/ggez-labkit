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

namespace GGEZ.Labkit
{
    public class Chart : MonoBehaviour
    {
        private static Material s_chartMaterial;

        public static Chart Create(int maxPoints, Rect screenRect, Color lowColor, Color highColor)
        {
            var retval = Camera.main.gameObject.AddComponent<Chart>();
            retval._screenRect = screenRect;
            retval._data = new float[maxPoints];
            retval._lowColor = lowColor;
            retval._highColor = highColor;
            return retval;
        }

        public void AddDataPoint(float f)
        {
            _data[_end] = f;

            _min = _min < f ? _min : f;
            _max = _max > f ? _max : f;

            int nextEnd = (_end + 1) % _data.Length;
            if (nextEnd == _begin)
            {
                _begin = (_begin + 1) % _data.Length;
            }
            _end = nextEnd;
        }

        private Rect _screenRect;
        public Rect ScreenRect { get { return _screenRect; } set { _screenRect = value; } }
        private float[] _data;
        public int MaxPoints { get { return _data.Length; } set { _data = new float[value]; } }
        private float _min = float.MaxValue;
        private float _max = float.MinValue;
        private int _begin, _end;
        private Color _lowColor = new Color(0f / 255f, 158f / 255f, 115 / 255f);
        public Color LowColor { get { return _lowColor; } set { _lowColor = value; } }
        private Color _highColor = new Color(230 / 255f, 159 / 255f, 0f / 255f);
        public Color HighColor { get { return _highColor; } set { _highColor = value; } }

        [ContextMenu("Clear Chart")]
        public void Clear()
        {
            _min = float.MaxValue;
            _max = float.MinValue;
            _begin = 0;
            _end = 0;
        }

        public void RecalculateMinMax()
        {
            _min = float.MaxValue;
            _max = float.MinValue;
            if (_end < _begin)
            {
                int dataLength = _data.Length;
                for (int i = _begin; i < dataLength; ++i)
                {
                    float f = _data[i];
                    _min = _min < f ? _min : f;
                    _max = _max > f ? _max : f;
                }
                for (int i = 0; i < _end; ++i)
                {
                    float f = _data[i];
                    _min = _min < f ? _min : f;
                    _max = _max > f ? _max : f;
                }
            }
            else
            {
                for (int i = _begin; i < _end; ++i)
                {
                    float f = _data[i];
                    _min = _min < f ? _min : f;
                    _max = _max > f ? _max : f;
                }
            }
        }

        void OnPostRender()
        {
            GL.PushMatrix();
            GL.LoadPixelMatrix();
            GL.Begin(GL.LINE_STRIP);
            if (s_chartMaterial == null)
            {
                s_chartMaterial = new Material(Shader.Find("Hidden/Internal-Colored"));
                s_chartMaterial.hideFlags = HideFlags.HideAndDontSave;
                s_chartMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                s_chartMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                s_chartMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                s_chartMaterial.SetInt("_ZWrite", 0);
            }
            s_chartMaterial.SetPass(0);

            float xMin = _screenRect.xMin, xMax = _screenRect.xMax;
            float yMin = _screenRect.yMin, yMax = _screenRect.yMax;
            float xAxisScale = 1f / _data.Length;
            if (_end < _begin)
            {
                int dataLength = _data.Length;
                int j = 0;
                for (int i = _begin; i < dataLength; ++i, ++j)
                {
                    float y = Mathf.InverseLerp(_min, _max, _data[i]);
                    GL.Color(Color.Lerp(_lowColor, _highColor, y));
                    GL.Vertex(new Vector3 (Mathf.Lerp(xMin, xMax, j * xAxisScale), Mathf.Lerp(yMin, yMax, y), 0f));
                }
                for (int i = 0; i < _end; ++i, ++j)
                {
                    float y = Mathf.InverseLerp(_min, _max, _data[i]);
                    GL.Color(Color.Lerp(_lowColor, _highColor, y));
                    GL.Vertex(new Vector3 (Mathf.Lerp(xMin, xMax, j * xAxisScale), Mathf.Lerp(yMin, yMax, y), 0f));
                }
            }
            else
            {
                for (int i = _begin; i < _end; ++i)
                {
                    float y = Mathf.InverseLerp(_min, _max, _data[i]);
                    GL.Color(Color.Lerp(_lowColor, _highColor, y));
                    GL.Vertex(new Vector3 (Mathf.Lerp(xMin, xMax, i * xAxisScale), Mathf.Lerp(yMin, yMax, y), 0f));
                }
            }

            GL.End();
            GL.PopMatrix();
        }
    }
}
