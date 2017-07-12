using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleDetail : EditorWindow
{

    [MenuItem("ABSystem/AssetBundle Detail")]
    static void Open()
    {
        GetWindow<AssetBundleDetail>("AssetBundle Detail", true);
    }

    private string _file = null;
    private string _main_asset_name = null;
    private string _main_asset_type = null;

    void OnGUI()
    {
        if (GUILayout.Button("Open"))
        {
            _file = EditorUtility.OpenFilePanel("Select AssetBundle", "Assets/StreamingAssets/update", "ab");

            if (_file.Length != 0)
            {
                AssetBundle _bundle = null;
                _bundle = AssetBundle.LoadFromFile(_file);
                Object o = _bundle.LoadAsset(_bundle.GetAllAssetNames()[0]);
                if (o != null)
                {
                    _main_asset_name = o.name;
                    _main_asset_type = o.GetType().ToString();
                }
                else
                {
                    _main_asset_name = "SCENE";
                }
                _bundle.Unload(false);
            }
        }

        if (_file != null)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Bundle Name: ");
            GUILayout.Label(Path.GetFileName(_file));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MainAsset Name: ");
            GUILayout.Label(_main_asset_name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("MainAsset Type: ");
            GUILayout.Label(_main_asset_type);
            EditorGUILayout.EndHorizontal();
        }
    }
}
