using UnityEngine;
using UnityEditor;
using System.Collections;

public class AssetbundlesMenuItems
{
	const string kSimulateAssetBundlesMenu = "AssetBundles/Simulate AssetBundles";

	[MenuItem(kSimulateAssetBundlesMenu)]
	public static void ToggleSimulateAssetBundle ()
	{
		AssetBundleManager.SimulateAssetBundleInEditor = !AssetBundleManager.SimulateAssetBundleInEditor;
	}

	[MenuItem(kSimulateAssetBundlesMenu, true)]
	public static bool ToggleSimulateAssetBundleValidate ()
	{
		Menu.SetChecked(kSimulateAssetBundlesMenu, AssetBundleManager.SimulateAssetBundleInEditor);
		return true;
	}

    [MenuItem("AssetBundles/Build AssetBundles Using LZMA (Mobile, Standalone)")]
    static public void BuildAssetBundles_LZMA()
    {
        BuildScript.BuildAssetBundles(BuildAssetBundleOptions.None);
    }

    [MenuItem("AssetBundles/Build AssetBundles Using LZ4 (WebGL, WebPlayer)")]
    static public void BuildAssetBundles_LZ4()
    {
        BuildScript.BuildAssetBundles(BuildAssetBundleOptions.ChunkBasedCompression);
    }

    [MenuItem("AssetBundles/Build Player Using LZMA (Mobile, Standalone)")]
    static void BuildPlayer_LZMA()
    {
        BuildScript.BuildPlayer(BuildAssetBundleOptions.None);
    }

    [MenuItem("AssetBundles/Build Player Using LZ4 (WebGL, WebPlayer)")]
    static void BuildPlayer_LZ4()
    {
        BuildScript.BuildPlayer(BuildAssetBundleOptions.ChunkBasedCompression);
    }
}
