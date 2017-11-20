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
using System.Collections.Generic;


namespace System.Linq
{
public static partial class LinqExt
{
public static IEnumerable<TFirst> PickWeighted <TFirst> (
        this IEnumerable<TFirst> first,
        Random random,
        int numberToSelect,
        Func<TFirst, int> computeWeight
        )
    {
    if (first == null) throw new System.ArgumentNullException ("first");
    if (random == null) throw new System.ArgumentNullException ("random");
    if (computeWeight == null) throw new System.ArgumentNullException ("computeWeight");
    if (numberToSelect <= 0)
        {
        yield break;
        }
    List<TFirst> objects = new List<TFirst> ();
    List<int> weights = new List<int> ();
    int sumOfWeights = 0;
    using (IEnumerator<TFirst> e1 = first.GetEnumerator ())
        {
        while (e1.MoveNext ())
            {
            TFirst o1 = e1.Current;
            int weight = computeWeight (o1);
            sumOfWeights += weight;
            objects.Add (o1);
            weights.Add (weight);
            }
        }
    if (sumOfWeights < 1)
        {
        yield break;
        }
    int numberSelected = 0;
    while (numberSelected < numberToSelect)
        {
        int weightToSelect = random.Next (0, sumOfWeights);
        int indexToSelect = 0;
        while (weightToSelect >= 0 && indexToSelect < weights.Count)
            {
            weightToSelect -= weights[indexToSelect];
            ++indexToSelect;
            }
        --indexToSelect;
        if (indexToSelect < 0
                || indexToSelect >= objects.Count
                || weightToSelect >= 0)
            {
                break;
            }
        yield return objects[indexToSelect];
        if (weights[indexToSelect] == 0)
            {
            throw new System.InvalidOperationException ("Shouldn't be able to select something with 0 weight");
            }
        ++numberSelected;
        sumOfWeights -= weights[indexToSelect];
        weights[indexToSelect] = 0;
        }
    }


public static IEnumerable<T> SortByDependencies <T> (
        this IEnumerable<T> source,
        System.Func<T, IEnumerable<T>> dependenciesOf
        )
    {
    var sorted = new List<T>();
    var visited = new HashSet<T>();
    var worklist = new Stack<T> ();

    foreach (var inputItem in source)
        {
        if (sorted.Contains (inputItem))
            {
            continue;
            }
        worklist.Push (inputItem);
        while (worklist.Count > 0)
            {
            var item = worklist.Pop ();
            if (!visited.Contains (item))
                {
                visited.Add (item);
                worklist.Push(item);
                var dependencies = dependenciesOf (item);
                if (dependencies != null)
                    {
                    foreach (var dependency in dependencies)
                        {
                        worklist.Push (dependency);
                        }
                    }
                }
            else
                {
                if (worklist.Contains (item))
                    {
                    throw new System.InvalidOperationException ("cyclic dependency");
                    }
                sorted.Add (item);
                }
            }
        }
    return sorted;
    }
}
}
