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

namespace GGEZ
{
    [
        RequireComponent(typeof(SpriteRenderer))
    ]
    public class ColorSwapSpriteRenderer : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private Texture2D _colorSwapTexture;
        private bool _dirty;

#if UNITY_EDITOR
        public bool Override;
        public Color[] Colors = new Color[256];
#endif

        public void Awake()
        {
            _spriteRenderer = (SpriteRenderer)this.GetComponent(typeof(SpriteRenderer));

            const int kPaletteWidth = 256;

            _colorSwapTexture = new Texture2D(kPaletteWidth, 1, TextureFormat.RGBA32, false, false);
            _colorSwapTexture.filterMode = FilterMode.Point;

            for (int i = 0; i < kPaletteWidth; ++i)
            {
                _colorSwapTexture.SetPixel(i, 0, Color.clear);
            }
            _colorSwapTexture.Apply();

            _spriteRenderer.material.SetTexture("_SwapTex", _colorSwapTexture);
        }

        public void SwapColor(int index, Color color)
        {
            _colorSwapTexture.SetPixel(index, 0, color);
            _dirty = true;
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (this.Override)
            {
                for (int i = 0; i < this.Colors.Length; ++i)
                {
                    this.SwapColor(i, this.Colors[i]);
                }
            }
#endif
            if (_dirty)
            {
                _colorSwapTexture.Apply();
                _dirty = false;
            }
        }
    }
}
