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

// This class was created to allow editor scripts to download files using WWW.
// Using other wait types (e.g. WaitForSeconds) can be supported but is not
// implemented yet. Search "NotImplementedException" for where code to support
// those should be added.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GGEZ
{
public class EditorCoroutineRunner
{
private static readonly List<IEnumerator> coroutines = new List<IEnumerator>();

public static void StartCoroutine (IEnumerator coroutine)
    {
    if (coroutines.Count == 0)
        {
        EditorApplication.update += Update;
        }
    coroutines.Add(Root(coroutine, coroutines.Count));
    }

private static IEnumerator Root (IEnumerator coroutine, int index)
    {
    while (coroutine.MoveNext ())
        {
        yield return coroutine.Current;
        }
    coroutines[index] = null;
    }

private static void Update()
    {
    int activeCoroutines = 0;
    int sentinel = 999999;
    for (int i = coroutines.Count - 1; i >= 0 && sentinel > 0; --i, --sentinel)
        {
        var e = coroutines[i];
        if (e == null)
            {
            continue;
            }
        if (e.MoveNext())
            {
            ++activeCoroutines;
            if (e.Current == null)
                {
                }
            else if (e.Current is WWW)
                {
                coroutines[i] = EditorCoroutineRunner.waitForWWW ((WWW)e.Current, i, e);
                }
            else
                {
                throw new System.NotImplementedException ("EditorCoroutineRunner can't handle " + e.Current.GetType() + " yet. Add this type to EditorCoroutineRunner.cs");
                }
            }
        else
            {
            ++i;
            }
        }
    if (sentinel == 0)
        {
        coroutines.Clear ();
        EditorApplication.update -= Update;
        throw new System.InvalidOperationException ("Infinite loop in EditorCoroutineRunner.cs. Be careful when adding new wait conditions!");
        }
    if (activeCoroutines == 0)
        {
        coroutines.Clear ();
        EditorApplication.update -= Update;
        }
    }

private static IEnumerator waitForWWW (WWW www, int index, IEnumerator parent)
    {
    while (!www.isDone)
        {
        yield return null;
        }
    coroutines[index] = parent;
    }


}
}
