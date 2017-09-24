using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestVector2Ext
{
[Test]
public void TestMagnitudeFast ()
	{
	float maxError = 0f;
	for (int i = 0; i < 100000; ++i )
		{
		var v = Random.onUnitSphere.ToVector2 ();
		var errorMagnitude = Mathf.Abs (v.magnitude - v.MagnitudeFast ());
		if (errorMagnitude > maxError)
			{
			maxError = errorMagnitude;
			}
		}
	Assert.IsTrue (maxError < 0.05f);
	}
}
