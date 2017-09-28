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
using System.Collections;
using System.IO;

namespace GGEZ
{

public class BytesBlob
{
public byte[] Bytes = null;

public string GetBytesAsString()
    {
    return System.Text.Encoding.UTF8.GetString (this.Bytes);
    }

public Texture2D GetBytesAsTexture ()
    {
    var texture = new Texture2D (2, 2);
    texture.LoadImage (this.Bytes, false);
    return texture;
    }
}

public static partial class Util
{
private enum DownloadCacheUsage
    {
    DownloadAlways,
    DownloadOrReadCache,
    ReadCacheOrDownload,
    }

public static IEnumerator DownloadAsync (string url, BytesBlob result)
    {
    return downloadImplementation (url, DownloadCacheUsage.DownloadAlways, result);
    }

public static IEnumerator DownloadOrReadCacheAsync (string url, BytesBlob result)
    {
    return downloadImplementation (url, DownloadCacheUsage.DownloadOrReadCache, result);
    }

public static IEnumerator ReadCacheOrDownloadAsync (string url, BytesBlob result)
    {
    return downloadImplementation (url, DownloadCacheUsage.ReadCacheOrDownload, result);
    }

private static IEnumerator downloadImplementation (string url, DownloadCacheUsage cacheUsage, BytesBlob result)
    {
    var filename = Application.persistentDataPath + "/dl" + Util.ToBase62String (url.GetJenkinsHash ());
    bool existsOnDisk = File.Exists (filename);
    if (existsOnDisk && cacheUsage == DownloadCacheUsage.ReadCacheOrDownload)
        {
        yield return null;
        result.Bytes = File.ReadAllBytes (filename);
        yield break;
        }

        {
        var www = new WWW (url);
        yield return www;
        if (!string.IsNullOrEmpty (www.error))
            {
            if (existsOnDisk && cacheUsage == DownloadCacheUsage.DownloadOrReadCache)
                {
                result.Bytes = File.ReadAllBytes (filename);
                }
            yield break;
            }
        result.Bytes = www.bytes;
        try
            {
            File.WriteAllBytes (filename, result.Bytes);
            }
        catch (IOException)
            {
            }
        }
    }

private static IEnumerator DownloadSpriteAsync (string url, object objectWithSpriteField, string spriteFieldToSet)
    {
    BytesBlob data = new BytesBlob ();
        {
        var enumerator = Util.ReadCacheOrDownloadAsync (url, data);
        while (enumerator.MoveNext ())
            {
            yield return enumerator.Current;
            }
        }
    Sprite sprite = null;
    if (null != data.Bytes)
        {
        var texture = data.GetBytesAsTexture ();
        sprite = Sprite.Create (texture, new Rect (0f, 0f, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
        }
    var fieldInfo = objectWithSpriteField.GetType ().GetField (spriteFieldToSet);
    fieldInfo.SetValue (objectWithSpriteField, sprite);
    }
}
}
