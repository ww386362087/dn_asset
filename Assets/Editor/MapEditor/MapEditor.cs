using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace XEditor
{
    public class MapEditor : EditorWindow
    {

        private static GUIContent
               ButtonContent = new GUIContent("Generate", "generate map data");

        private static GUIContent
            LoadButtonContent = new GUIContent("Load", "load mapheight file");

        MapGenerator _map_generate = new MapGenerator();

        [MenuItem(@"XEditor/Map Editor %k")]
        static void Init()
        {
            GetWindow(typeof(MapEditor));
        }

        void OnDestroy()
        {
            _map_generate.Reset();
        }

        public void OnGUI()
        {
            EditorGUILayout.Space();

            _map_generate._grid_size = EditorGUILayout.Slider("Grid Size", _map_generate._grid_size, 0.1f, 1);
            _map_generate._inaccuracy = EditorGUILayout.Slider("Height Inaccuracy", _map_generate._inaccuracy, 0.01f, 0.1f);

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(ButtonContent))
            {
                GenerateMapData();
            }
            if (GUILayout.Button(LoadButtonContent))
            {
                LoadMapHeight();
            }
            if (GUILayout.Button("Reset"))
            {
                _map_generate.Reset();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void GenerateMapData()
        {
            UnityEngine.SceneManagement.Scene scene = EditorSceneManager.GetActiveScene();
            //Application.loadedLevelName
            string path = EditorUtility.SaveFilePanel("Select a file to save", XEditorLibrary.Sc, scene.name + ".bytes", "bytes");
            if (!string.IsNullOrEmpty(path)) _map_generate.Generate(path);
        }

        private void LoadMapHeight()
        {
            string path = EditorUtility.OpenFilePanel("Select a file to load", XEditorLibrary.Sc, "bytes");
            if (!string.IsNullOrEmpty(path)) _map_generate.LoadFromFile(path);
        }

    }

}