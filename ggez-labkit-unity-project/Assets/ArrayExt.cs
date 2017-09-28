using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace System.Linq
{
	// Taken from https://github.com/Microsoft/referencesource/blob/master/System.Core/System/Linq/Enumerable.cs
	// The MIT License (MIT)
	
	// Copyright (c) Microsoft Corporation
	
	// Permission is hereby granted, free of charge, to any person obtaining a copy 
	// of this software and associated documentation files (the "Software"), to deal 
	// in the Software without restriction, including without limitation the rights 
	// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell 
	// copies of the Software, and to permit persons to whom the Software is 
	// furnished to do so, subject to the following conditions: 
	
	// The above copyright notice and this permission notice shall be included in all 
	// copies or substantial portions of the Software. 
	
	// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
	// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
	// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
	// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
	// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
	// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
	// SOFTWARE.
    public static class Enumerable
    {
        public static IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) {
            if (first == null) throw new System.ArgumentNullException ("first");
            if (second == null) throw new System.ArgumentNullException ("second");
            if (resultSelector == null) throw new System.ArgumentNullException ("resultSelector");
            return ZipIterator(first, second, resultSelector);
        }

        static IEnumerable<TResult> ZipIterator<TFirst, TSecond, TResult>(IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector) {
            using (IEnumerator<TFirst> e1 = first.GetEnumerator())
                using (IEnumerator<TSecond> e2 = second.GetEnumerator())
                    while (e1.MoveNext() && e2.MoveNext())
                        yield return resultSelector(e1.Current, e2.Current);
        }
	
	}
}

public static partial class ArrayExt
{

	// TODO PUT THIS ELSEWHERE

	public static bool ContainsValueEqualTo<TKey,TValue> (this Dictionary<TKey,TValue> self, TKey key, TValue value)
	{
		TValue valueInSelf;
		return self.TryGetValue (key, out valueInSelf) && valueInSelf.Equals (value);
	}


	public static T[] SelectByLottery<T> (
		this T[] self,
		int numberToSelect,
		System.Func<T, int> computeWeight
		)
	{
		T[] retval = new T[numberToSelect];
		int[] weights = new int[self.Length];
		int sumOfWeights = 0;
		for (int i = 0; i < self.Length; ++i)
		{
			int weight = computeWeight (self[i]);
			sumOfWeights += weight;
			weights[i] = weight;
		}
		if (sumOfWeights < 1)
		{
			return new T[] {};
		}
		// if (typeof(T).Equals(typeof(Throne.Character)))
		// {
		// 	Debug.LogFormat ("Selecting {0} from {1} (total = {2})", numberToSelect, Util.ObjectToJsonPretty (weights.Zip(self,(w,c)=>w.ToString()+" "+(c as Throne.Character).FullName).ToArray()), sumOfWeights);
		// }
		int numberSelected = 0;
		while (numberSelected < numberToSelect)
		{
			int weightToSelect = Random.Range (0, sumOfWeights);
			int indexToSelect = 0;
			// Debug.LogFormat ("Select weight {0}", weightToSelect);
			while (weightToSelect >= 0 && indexToSelect < weights.Length)
			{
				weightToSelect -= weights[indexToSelect];
				++indexToSelect;
			}
			--indexToSelect;
			if (indexToSelect < 0 || indexToSelect >= self.Length || weightToSelect >= 0)
			{
				System.Array.Resize (ref retval, numberSelected);
				break;
			}
			retval[numberSelected] = self[indexToSelect];
			if (weights[indexToSelect] == 0)
			{
				throw new System.InvalidOperationException ("Shouldn't be able to select something with 0 weight");
			}
			++numberSelected;
			sumOfWeights -= weights[indexToSelect];
			weights[indexToSelect] = 0;
		}
		// if (typeof(T).Equals(typeof(Throne.Character)))
		// 	Debug.LogFormat (" => {0}", Util.ObjectToJson (retval.Select((e)=>(e as Throne.Character).FullName).ToArray()));
		return retval;
	}

	public static IEnumerable<T> SortByDependencies <T> (this IEnumerable<T> source, System.Func<T, IEnumerable<T>> dependenciesOf)
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
						throw new System.InvalidOperationException ("Cyclic dependency");
					}
					sorted.Add (item);
				}
			}
		}

		return sorted;
	}
}
