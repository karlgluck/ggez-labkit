using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class RectExt
{
public static Vector2[] GetVertices (this Rect self)
	{
    return new Vector2[] {
			new Vector2 (self.xMin, self.yMin),
			new Vector2 (self.xMin, self.yMax),
			new Vector2 (self.xMax, self.yMax),
			new Vector2 (self.xMax, self.yMin),
	    	};
	}
}
