using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GGEZ
{
[CustomEditor (typeof(AssetPackageConfig))]
public class AssetPackageConfigEditor : Editor
{
public override void OnInspectorGUI()
	{
	var t = this.target as AssetPackageConfig;
	t.AssetPackageName = EditorGUILayout.TextField ("Name", t.AssetPackageName);
	EditorGUILayout.Space ();
	EditorGUI.BeginDisabledGroup (t.Assets == null || t.Assets.Length < 1);
	if (GUILayout.Button ("Build"))
		{
		t.Build ();
		}
	EditorGUI.EndDisabledGroup ();
	EditorGUILayout.Space ();
	GUILayout.Label ("Assets ", EditorStyles.boldLabel);
	EditorGUILayout.Space ();
    if (t.Assets == null)
        {
        t.Assets = new Object[0];
        }
	for (int i = t.Assets.Length - 1; i >= 0; --i)
		{
		var assetObject = t.Assets[i];
		bool shouldDelete = assetObject == null;
		if (!shouldDelete)
			{
			EditorGUILayout.BeginHorizontal ();
			shouldDelete = GUILayout.Button ("x", EditorStyles.miniButton, GUILayout.Width(17f));
			bool isAnotherAssetPackage = assetObject as AssetPackageConfig != null;
			Color oldBackgroundColor = GUI.backgroundColor;
			if (isAnotherAssetPackage)
				{
				GUI.backgroundColor = Color.green;
				}
			assetObject = EditorGUILayout.ObjectField (GUIContent.none, assetObject, typeof(Object), false);
			GUI.backgroundColor = oldBackgroundColor;
			shouldDelete = shouldDelete || assetObject == null;
			EditorGUILayout.EndHorizontal ();
			}
		if (shouldDelete)
			{
			for (int a = i; a < t.Assets.Length - 1; ++a)
				{
				t.Assets[a] = t.Assets[a + 1];
				}
			System.Array.Resize(ref t.Assets, t.Assets.Length - 1);
			}
		else
			{
			t.Assets[i] = assetObject;
			}
		}
	EditorGUILayout.Space ();
	var newAssets = AssetPackageConfigEditor.dropAssetsArea ("Drop New Assets Here", "Add {0} Asset(s)");
	if (newAssets != null)
		{
		int oldLength = t.Assets.Length;
		System.Array.Resize (ref t.Assets, oldLength + newAssets.Length);
		newAssets.CopyTo (t.Assets, oldLength);
		}

	}


private static Object[] dropAssetsArea (string idlePrompt, string dropPrompt)
    {
	Event e = Event.current;
	Rect drop_area = GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
	bool isDragPerformEvent = e.type == EventType.DragPerform;
	bool isAnyDragEvent = e.type == EventType.DragUpdated || isDragPerformEvent;
	bool isMouseInArea = drop_area.Contains (e.mousePosition);
	bool dragAndDropHasSomething = DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0;

    var centeredStyle = GUI.skin.GetStyle("Box");
    centeredStyle.alignment = TextAnchor.MiddleCenter;
	GUI.Box (
			drop_area,
			isMouseInArea && dragAndDropHasSomething
					? string.Format (dropPrompt, DragAndDrop.objectReferences.Length)
					: idlePrompt,
			centeredStyle
			);

	if (isAnyDragEvent && isMouseInArea)
		{
		DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
		if (isDragPerformEvent)
			{
			DragAndDrop.AcceptDrag();
			return DragAndDrop.objectReferences;
			}
		}
	
	return null;
    }
}
}

