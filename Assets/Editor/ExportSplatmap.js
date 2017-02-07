// This script is for saving splat maps as PNG files

import System.IO;

@MenuItem("Terrain/Export Splatmap")
static function Apply ()
{
    var texture : Texture2D = Selection.activeObject as Texture2D;

    if (texture == null)
    {
        EditorUtility.DisplayDialog("Select Splat Map", "You Must Select a Splat Map first!\nGo to the project tab, find your terrain and open it's foldout. Then select either SplatAlpha0 or SplatAlpha1.", "Ok");
        return;
    }

    //var bytes = texture.EncodeToPNG();
    //File.WriteAllBytes(Application.dataPath + "/Exported_Splatmap.png", bytes);

    var bytes = texture.EncodeToJPG();
    File.WriteAllBytes(Application.dataPath + "/Exported_Splatmap.jpg", bytes);

    AssetDatabase.Refresh();
}

