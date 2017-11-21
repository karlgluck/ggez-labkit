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
using System;
using System.Collections;
using System.Collections.Generic;

namespace GGEZ
{

[
CreateAssetMenu(fileName = "New Google Spreadsheet.asset", menuName="GGEZ/Google Spreadsheet")
]
public class GoogleSpreadsheetDataSource : ScriptableObject
{
public string Url;

public bool DataIsColumns = false;

[HideInInspector]
public string LocalAssetCsvText;


private int entries;
private Csv.GetCsvCell getCellCallback;



public bool makeSureDataExists ()
    {
    if (this.entries > 0 && this.getCellCallback != null)
        {
        return true;
        }
    if (this.LocalAssetCsvText == null)
        {
        return false;
        }
    this.getCellCallback = Csv.ReadCsv (this.LocalAssetCsvText, out this.entries, this.DataIsColumns);
    return (this.entries > 0 && this.getCellCallback != null);
    }




public IEnumerator<Func<string, string>> GetObjects ()
    {
    if (!this.makeSureDataExists ())
        {
        throw new InvalidOperationException ("No data exists for " + this.name);
        }
    for (int i = 0; i < this.entries; ++i)
        {
        yield return delegate (string property)
            {
            return this.getCellCallback (i, property);
            };
        }
    }




public IEnumerator<object> GetObjects (Type objectType)
    {
    if (!this.makeSureDataExists ())
        {
        throw new InvalidOperationException ("No data exists for " + this.name);
        }
    for (int i = 0; i < this.entries; ++i)
        {
        yield return Csv.CsvEntryToObject (i, this.getCellCallback, objectType);
        }
    }





public IEnumerator LoadFromWebOrCache ()
    {
#if UNITY_EDITOR
    UnityEditor.EditorUtility.SetDirty (this);
#endif

    BytesBlob downloadedBytes = new BytesBlob ();
    IEnumerator downloadEnumerator = Util.DownloadOrReadCacheAsync (this.Url, downloadedBytes);
    while (downloadEnumerator.MoveNext ())
        {
        yield return downloadEnumerator.Current;
        }

    if (downloadedBytes.Bytes != null)
        {
        var csv = downloadedBytes.GetBytesAsString ();
        if (!Application.isPlaying)
            {
            this.LocalAssetCsvText = csv;
            }
        this.getCellCallback = Csv.ReadCsv (csv, out this.entries, this.DataIsColumns);
        }

    }
}

}
