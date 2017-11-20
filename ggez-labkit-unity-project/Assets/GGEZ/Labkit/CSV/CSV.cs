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

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GGEZ
{
public static class Csv
{
static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
static char[] TRIM_CHARS = { '\"' };

public delegate string GetCsvCell(int index, string label);


public static GetCsvCell ReadCsv(string text, out int length)
    {
    return _readCsv (text, out length, false);
    }
    
public static GetCsvCell ReadCsvTransposed (string text, out int length)
    {
    return _readCsv (text, out length, true);
    }

public static GetCsvCell ReadCsv (string text, out int length, bool transposed)
    {
    return _readCsv (text, out length, transposed);
    }

internal static GetCsvCell _readCsv (string text, out int length, bool transpose)
    {
    var lines = Regex.Split (text, LINE_SPLIT_RE);

    if (lines.Length <= 1)
        {
        length = 0;
        return delegate (int row, string column)
            {
            throw new System.ArgumentOutOfRangeException ("No data in this CSV");
            };
        }

    var firstLine = Regex.Split (lines[0], SPLIT_RE);

    var labelToIndex = new Dictionary<string, int>();
    string[,] data;

    if (transpose)
        {
        data = new string[firstLine.Length - 1, lines.Length];
        }
    else
        {
        data = new string[lines.Length - 1, firstLine.Length];
        for (var i = 0; i < firstLine.Length; ++i)
            {
            labelToIndex[firstLine[i]] = i;
            }
        }

    var lineSplitRegex = new Regex(SPLIT_RE);
    var currentRow = 0;

    for (var i = 0; i < lines.Length; i++)
        {
        if (!transpose && i == 0)
            {
            continue;
            }
        var values = lineSplitRegex.Split(lines[i]);
        if (values.Length == 0 || values[0] == "") continue;
        for (var j = 0; j < firstLine.Length && j < values.Length; ++j)
            {
            string value = values[j];
            if (transpose && j == 0)
                {
                labelToIndex[value] = i;
                continue;
                }
            value = value.Trim (TRIM_CHARS).Replace ("\\", "");
            if (transpose)
                {
                data[j-1, i] = value;
                }
            else
                {
                data[currentRow, j] = value;
                }
            }
        ++currentRow;
        }

    if (transpose)
        {
        length = firstLine.Length - 1;
        }
    else
        {
        length = currentRow;
        }

    int lengthCaptured = length;
    GetCsvCell retval = delegate (int index, string label)
        {
        if (index < 0 || index >= lengthCaptured)
            {
            throw new System.ArgumentOutOfRangeException ("Index out of range");
            }
        int labelIndex;
        if (!labelToIndex.TryGetValue (label, out labelIndex))
            {
            throw new System.ArgumentException ("Unknown label: " + label);
            }
        return data[index, labelIndex];
        };

    return retval;
    }

public static object CsvEntryToObject (int index, GetCsvCell getCsvCell, Type type)
    {
    var retval = Activator.CreateInstance (type);
    var fields = type.GetFields (BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
    foreach (FieldInfo field in fields)
        {
        if (field.FieldType.IsArray)
            {
            ArrayList arrayContents = new ArrayList ();
            try
                {
                int i = 0;
                const int ArrayMaxLength = 9999;
                while (i < ArrayMaxLength)
                    {
                    arrayContents.Add (getCsvCell (index, field.Name + "[" + i + "]"));
                    ++i;
                    }
                }
            catch (Exception)
                {
                }
            if (arrayContents.Count == 0)
                {
                continue;
                }
            Type elementType = field.FieldType.GetElementType ();
            var array = (Array)Array.CreateInstance (elementType, arrayContents.Count);
            for (int i = 0; i < arrayContents.Count; ++i)
                {
                try
                    {
                    array.SetValue (Convert.ChangeType (arrayContents[i], elementType), i);
                    }
                catch (System.Exception)
                    {
                    }
                }
            field.SetValue (retval, array);
            continue;
            }

        try
            {
            string cell = getCsvCell (index, field.Name);
            field.SetValue (retval, Convert.ChangeType (cell, field.FieldType));
            }
        catch (System.Exception)
            {
#if UNITY
            UnityEngine.Debug.LogFormat ("Couldn't set object.{0} = ({1})'{2}'", field.Name, field.FieldType.Name, cell);
#endif
            }
        }
    return retval;
    }
}
}
