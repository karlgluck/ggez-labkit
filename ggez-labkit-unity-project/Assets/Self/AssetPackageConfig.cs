using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GGEZ
{
[CreateAssetMenu(fileName = "New Asset Package.asset", menuName="Asset Package Config")]
public class AssetPackageConfig : ScriptableObject
{
public string AssetPackageName
	{
	get { return this.name; }
	set { this.name = value; }
	}

public Object[] Assets;

public string[] CollectAssetPaths ()
	{
	Object[] collectedAssets = this.Assets.Where (assetObject => assetObject != null).ToArray ();
	for (int i = 0; i < 100; ++i)
		{
		var expanded = collectedAssets
				.SelectMany (
						delegate (Object assetObject)
							{
							if (assetObject is AssetPackageConfig)
								{
								return (assetObject as AssetPackageConfig).Assets;
								}
							else
								{
								return new Object[] {assetObject};
								}
							}
						);
		bool canReturn = !expanded.Any (assetObject => assetObject is AssetPackageConfig);
		if (canReturn)
			{
			return expanded.Select (o => AssetDatabase.GetAssetPath (o)).ToArray ();
			}
		collectedAssets = expanded.ToArray ();
		}
	Debug.LogErrorFormat ("Too many iteration attempts while expanding asset references. Is there a recursive AssetPackageConfig reference?");
	return null;
	}

public void Build ()
	{
	string targetDirectory = System.IO.Path.Combine ("../packages/", Application.unityVersion);
	System.IO.Directory.CreateDirectory (targetDirectory);
	var packagePath = System.IO.Path.Combine (targetDirectory, this.AssetPackageName + ".unitypackage");
	try
		{
		AssetDatabase.ExportPackage (
				this.CollectAssetPaths (),
				packagePath,
				ExportPackageOptions.Default
				);
		Debug.LogFormat ("Built {0}", System.IO.Path.GetFullPath (packagePath));
		}
	catch (System.Exception e)
		{
		Debug.LogErrorFormat ("BUILD FAILED\n{0}", e);
		}
	}
}	
}