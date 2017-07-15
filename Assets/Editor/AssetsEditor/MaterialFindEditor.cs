using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

namespace XEditor
{
    public class MaterialFindEditor : EditorWindow
    {

        public enum EResType
        {
            Fx = 0,
            UI,
            Creaters,
            Sprite,
            Wing,
            Tail,
            Scene
        }

        private Dictionary<Shader, List<Material>> m_Shaders = new Dictionary<Shader, List<Material>>();
        private List<Material> m_Materials = null;
        private Vector2 shaderScrollPos = Vector2.zero;
        private Vector2 materialScrollPos = Vector2.zero;
        private Shader m_SelectShader = null;
        private EResType resType = EResType.Fx;
        private string[] resPaths = new string[] {
            "Assets/Resources/Effects",
            "Assets/Resources/atlas/UI",
            "Assets/Creatures",
            "Assets/Equipment/Spirit",
            "Assets/Equipment/Wing",
            "Assets/Equipment/Tail",
            "Assets/XScene"};

        private void FindMat(string path)
        {
            m_Shaders.Clear();
            m_Materials = null;
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.mat", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string matPath = fi.FullName.Replace("\\", "/");
                int index = matPath.IndexOf(path);
                matPath = matPath.Substring(index);

                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", matPath, i, files.Length), path, (float)i / files.Length);

                Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
                if (mat != null)
                {
                    Shader shader = mat.shader;
                    List<Material> mats = null;
                    if (!m_Shaders.TryGetValue(shader, out mats))
                    {
                        mats = new List<Material>();
                        m_Shaders.Add(shader, mats);
                    }
                    mats.Add(mat);
                }
            }
            EditorUtility.ClearProgressBar();
        }

        private void FindFxUnusedMat(string path)
        {
            m_Shaders.Clear();
            m_Materials = null;
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.mat", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                string matPath = fi.FullName.Replace("\\", "/");
                int index = matPath.IndexOf(path);
                matPath = matPath.Substring(index);

                EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", matPath, i, files.Length), path, (float)i / files.Length);

                Material mat = AssetDatabase.LoadAssetAtPath(matPath, typeof(Material)) as Material;
                if (mat != null)
                {
                    Shader shader = mat.shader;
                    List<Material> mats = null;
                    if (!m_Shaders.TryGetValue(shader, out mats))
                    {
                        mats = new List<Material>();
                        m_Shaders.Add(shader, mats);
                    }
                    mats.Add(mat);
                }
            }
            EditorUtility.ClearProgressBar();
        }


        protected virtual void OnGUI()
        {
            GUILayout.BeginHorizontal();
            resType = (EResType)EditorGUILayout.EnumPopup("资源目录", resType, GUILayout.MaxWidth(250));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            EditorGUILayout.ObjectField("SelectedShader", m_SelectShader, typeof(Shader), true, GUILayout.MaxWidth(450));
            if (GUILayout.Button("Scan", GUILayout.MaxWidth(150)))
            {
                string resPath = resPaths[(int)resType];
                FindMat(resPath);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            shaderScrollPos = GUILayout.BeginScrollView(shaderScrollPos, false, false);

            Dictionary<Shader, List<Material>>.Enumerator it = m_Shaders.GetEnumerator();
            while (it.MoveNext())
            {
                GUILayout.BeginHorizontal();
                Shader shader = it.Current.Key;
                EditorGUILayout.ObjectField(it.Current.Value.Count.ToString(), shader, typeof(Shader), true, GUILayout.MaxWidth(450));
                if (GUILayout.Button("Select", GUILayout.MaxWidth(50)))
                {
                    m_SelectShader = shader;
                    m_Materials = it.Current.Value;
                }
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            materialScrollPos = GUILayout.BeginScrollView(materialScrollPos, false, false);
            if (m_Materials != null)
            {
                for (int i = 0; i < m_Materials.Count; ++i)
                {
                    GUILayout.BeginHorizontal();
                    Material mat = m_Materials[i];
                    EditorGUILayout.ObjectField("", mat, typeof(Material), true, GUILayout.MaxWidth(450));
                    GUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }


    }

}