using UnityEngine;
using UnityEditor;

public class FullPackageExporter
{
    [MenuItem("Assets/Export Package With Settings")]
    static void Export ()
    {
        AssetDatabase.ExportPackage
        (
            "Assets",
            "Full Project.unitypackage",
            ExportPackageOptions.Interactive | ExportPackageOptions.Recurse | ExportPackageOptions.IncludeLibraryAssets | ExportPackageOptions.IncludeDependencies
        );
    }
}

