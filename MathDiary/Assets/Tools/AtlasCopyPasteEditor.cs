using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

//Copy and paste atlas settings to another atlas editor
public class AtlasCopyPasteEditor : EditorWindow
{
    public Texture2D copyFrom;           //Sprite Atlas to copy from settings
    public Texture2D pasteTo;           //Sprite atlas where to paste settings

    [MenuItem("Window/Atlas CopyPaste Editor")]
    static void Init()
    {
        // Window Set-Up
        AtlasCopyPasteEditor window = EditorWindow.GetWindow(typeof(AtlasCopyPasteEditor), false, "Atlas Editor", true) as AtlasCopyPasteEditor;
        window.minSize = new Vector2(260, 170); window.maxSize = new Vector2(260, 170);
        window.Show();
    }

    //Show UI
    void OnGUI()
    {
        copyFrom = (Texture2D)EditorGUILayout.ObjectField("Copy From", copyFrom, typeof(Texture2D), true);
        pasteTo = (Texture2D)EditorGUILayout.ObjectField("Paste To", pasteTo, typeof(Texture2D), true);

        EditorGUILayout.Space();

        if (GUILayout.Button("Copy Paste"))
        {
            if (copyFrom != null && pasteTo != null)
                CopyPaste();
            else
                Debug.LogWarning("Forgot to set the textures?");
        }

        Repaint();
    }

    //Do the copy paste
    private void CopyPaste()
    {
        if (copyFrom.width != pasteTo.width || copyFrom.height != pasteTo.height)
        {
            //Better a warning if textures doesn't match than a crash or error
            Debug.LogWarning("Unable to proceed, textures size doesn't match.");
            return;
        }

        if (!IsAtlas(copyFrom))
        {
            Debug.LogWarning("Unable to proceed, the source texture is not a sprite atlas.");
            return;
        }

        //Proceed to read all sprites from CopyFrom texture and reassign to a TextureImporter for the end result
        string pathFrom = AssetDatabase.GetAssetPath(copyFrom);
        TextureImporter _importerFrom = AssetImporter.GetAtPath(pathFrom) as TextureImporter;

        //Get Texture Importer of pasteTo texture for assigning sprite variables
        string pathTo = AssetDatabase.GetAssetPath(pasteTo);
        TextureImporter _importer = AssetImporter.GetAtPath(pathTo) as TextureImporter;

        EditorUtility.CopySerialized(_importerFrom, _importer);

        //Rebuild asset
        AssetDatabase.ImportAsset(pathTo, ImportAssetOptions.ForceUpdate);
    }

    //Check that the texture is an actual atlas and not a normal texture
    private bool IsAtlas(Texture2D tex)
    {
        string _path = AssetDatabase.GetAssetPath(tex);
        TextureImporter _importer = AssetImporter.GetAtPath(_path) as TextureImporter;

        return _importer.textureType == TextureImporterType.Sprite && _importer.spriteImportMode == SpriteImportMode.Multiple;
    }
}