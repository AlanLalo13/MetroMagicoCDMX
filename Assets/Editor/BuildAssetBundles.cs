using UnityEditor;
using System.IO;

public class BuildAssetBundles
{
    [MenuItem("Herramientas/Build Asset Bundles")]
    static void BuildAllAssetBundles()
    {
        string exportPath = "Assets/AssetBundlesExport";
        if (!Directory.Exists(exportPath))
            Directory.CreateDirectory(exportPath);

        BuildPipeline.BuildAssetBundles(exportPath, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}
