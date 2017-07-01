using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace XEditor
{

    public class MaterialEditor
    {
        [MenuItem(@"Assets/Tool/Res/FindMat")]
        private static void FindMat()
        {
            MaterialFindEditor window = (MaterialFindEditor)EditorWindow.GetWindow(typeof(MaterialFindEditor), true, "查找材质");
            window.Show();
        }


        public delegate bool EnumMaterialCallback(Material material, string path);

        public static void EnumMaterial(EnumMaterialCallback cb, string title)
        {
            UnityEngine.Object[] mats = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
            if (mats != null)
            {
                for (int i = 0; i < mats.Length; ++i)
                {
                    Material mat = mats[i] as Material;
                    string path = "";
                    if (mat != null)
                    {
                        path = AssetDatabase.GetAssetPath(mat);
                        if (cb(mat, path))
                        {
                        }
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, mats.Length), path, (float)i / mats.Length);
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }

        private static bool _Cutoff2RMat(Material mat, string path)
        {
            if (mat.shader.name.Contains("CutoutDiffuse") ||
                mat.shader.name.Contains("TransparentDiffuse"))
            {
                Texture2D tex = mat.mainTexture as Texture2D;
                if (tex != null)
                {
                    Texture2D alphaTex = TextureModify.ConvertTexRtex(tex);
                    if (mat.shader.name.Contains("RimLight/CutoutDiffuse"))
                        mat.shader = Shader.Find("Custom/RimLight/CutoutDiffuseMaskR");
                    else if (mat.shader.name.Contains("CutoutDiffuse") && mat.shader.name.Contains("NoLight"))
                        mat.shader = Shader.Find("Custom/Common/CutoutDiffuseMaskRNoLight");
                    else if (mat.shader.name.Contains("CutoutDiffuse"))
                        mat.shader = Shader.Find("Custom/Common/CutoutDiffuseMaskR");
                    else if (mat.shader.name.Contains("Common/TransparentDiffuse") && mat.shader.name.Contains("NoLight"))
                        mat.shader = Shader.Find("Custom/Common/TransparentDiffuseMaskRNoLight");
                    else if (mat.shader.name.Contains("Common/TransparentDiffuse"))
                        mat.shader = Shader.Find("Custom/Common/TransparentDiffuseMaskR");
                    mat.SetTexture("_Mask", alphaTex);

                }
            }
            return true;
        }

        [MenuItem(@"Assets/Tool/Material/Cutoff2RMat2 %2")]
        private static void Cutoff2RMat2()
        {
            EnumMaterial(_Cutoff2RMat, "Cutoff2RMat");
        }

        [MenuItem(@"Assets/Tool/Material/Cutoff2RMat")]
        private static void Cutoff2RMat()
        {
            TextureModify.scaleSize = 1;
            EnumMaterial(_Cutoff2RMat, "Cutoff2RMat");
            TextureModify.scaleSize = 2;
        }


        private static HashSet<string> matName = new HashSet<string>();
        private static Dictionary<string, List<string>> matTexName = new Dictionary<string, List<string>>();
        private static bool _FindSameMat(Material mat, string path)
        {
            if (matName.Contains(mat.name))
            {
                Debug.Log(string.Format("Same Mat:{0}", mat.name));
            }
            else
            {
                matName.Add(mat.name);
            }
            Texture tex = mat.mainTexture;
            if (tex != null)
            {
                List<string> matList = null;
                if (matTexName.TryGetValue(tex.name, out matList))
                {
                    matList.Add(mat.name);
                }
                else
                {
                    matList = new List<string>();
                    matList.Add(mat.name);
                    matTexName.Add(tex.name, matList);
                }
            }
            return true;
        }

        [MenuItem(@"Assets/Tool/Material/FindSameMat")]
        private static void FindSameMat()
        {
            matName.Clear();
            matTexName.Clear();
            EnumMaterial(_FindSameMat, "FindSameMat");
            foreach (KeyValuePair<string, List<string>> kvp in matTexName)
            {
                if (kvp.Value.Count > 1)
                {
                    Debug.Log(string.Format("Tex:{0}----------------------", kvp.Key));
                    foreach (string mat in kvp.Value)
                    {
                        Debug.Log(string.Format("Mat:{0}", mat));
                    }
                }
            }
        }


        private static bool _FindAllMat(Material mat, string path)
        {
            if (!matName.Contains(path))
            {
                matName.Add(path);
            }
            return true;
        }
        private static bool _FindUnusedMat(GameObject go, string path)
        {
            string[] fileName = new string[] { path };

            string[] reses = AssetDatabase.GetDependencies(fileName);

            for (int j = 0; j < reses.Length; ++j)
            {
                string res = reses[j];
                string resLow = res.ToLower();
                if (resLow.EndsWith(".mat"))
                {
                    matName.Remove(res);
                }
            }
            return true;
        }


        [MenuItem(@"Assets/Tool/Material/FindUnusedMat")]
        private static void FindUnusedMat()
        {
            matName.Clear();
            EnumMaterial(_FindAllMat, "FindAllMat");
            PrefabEditor.EnumPrefab(_FindUnusedMat, "FindUnusedMat");
            foreach (string path in matName)
            {
                Debug.Log(path);
            }
        }

        class ShaderValue
        {
            public ShaderValue(string n, ShaderUtil.ShaderPropertyType t)
            {
                name = n;
                type = t;
            }
            public string name = "";
            public ShaderUtil.ShaderPropertyType type = ShaderUtil.ShaderPropertyType.Float;

            public virtual void SetValue(Material mat)
            {

            }
        }

        class ShaderIntValue : ShaderValue
        {
            public ShaderIntValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetInt(n);
            }
            public int value = 0;
            public override void SetValue(Material mat)
            {
                mat.SetInt(name, value);
            }
        }

        class ShaderFloatValue : ShaderValue
        {
            public ShaderFloatValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetFloat(n);
            }
            public float value = 0;
            public override void SetValue(Material mat)
            {
                mat.SetFloat(name, value);
            }
        }

        class ShaderVectorValue : ShaderValue
        {
            public ShaderVectorValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetVector(n);
            }
            public Vector4 value = Vector4.zero;
            public override void SetValue(Material mat)
            {
                mat.SetVector(name, value);
            }
        }
        class ShaderColorValue : ShaderValue
        {
            public ShaderColorValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetColor(n);
            }
            public Color value = Color.black;
            public override void SetValue(Material mat)
            {
                mat.SetColor(name, value);
            }
        }
        class ShaderTexValue : ShaderValue
        {
            public ShaderTexValue(string n, ShaderUtil.ShaderPropertyType t, Material mat)
                : base(n, t)
            {
                value = mat.GetTexture(n);
                offset = mat.GetTextureOffset(n);
                scale = mat.GetTextureScale(n);
            }
            public Texture value = null;
            public Vector2 offset = Vector2.zero;
            public Vector2 scale = Vector2.one;

            public override void SetValue(Material mat)
            {
                mat.SetTexture(name, value);
                mat.SetTextureOffset(name, offset);
                mat.SetTextureScale(name, scale);
            }
        }

        private static List<ShaderValue> shaderValue = new List<ShaderValue>();
        private static bool _ClearMat(Material mat, string path)
        {
            shaderValue.Clear();
            Shader shader = mat.shader;
            int count = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < count; ++i)
            {
                ShaderValue sv = null;
                string name = ShaderUtil.GetPropertyName(shader, i);
                ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
                switch (type)
                {
                    case ShaderUtil.ShaderPropertyType.Color:
                        sv = new ShaderColorValue(name, type, mat);
                        break;
                    case ShaderUtil.ShaderPropertyType.Vector:
                        sv = new ShaderVectorValue(name, type, mat);
                        break;
                    case ShaderUtil.ShaderPropertyType.Float:
                        sv = new ShaderFloatValue(name, type, mat);
                        break;
                    case ShaderUtil.ShaderPropertyType.Range:
                        sv = new ShaderFloatValue(name, type, mat);
                        break;
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        sv = new ShaderTexValue(name, type, mat);
                        break;
                }
                shaderValue.Add(sv);

            }
            Material emptyMat = new Material(shader);
            mat.CopyPropertiesFromMaterial(emptyMat);
            UnityEngine.Object.DestroyImmediate(emptyMat);
            for (int i = 0; i < shaderValue.Count; ++i)
            {
                ShaderValue sv = shaderValue[i];
                sv.SetValue(mat);
            }
            mat.renderQueue = -1;
            return true;
        }


        [MenuItem(@"Assets/Tool/Material/ClearMat")]
        private static void ClearMat()
        {
            EnumMaterial(_ClearMat, "ClearMat");
            AssetDatabase.SaveAssets();
        }

        public static void GetMatTex(Material mat, List<Texture> lst)
        {
            Shader shader = mat.shader;
            int count = ShaderUtil.GetPropertyCount(shader);
            for (int i = 0; i < count; ++i)
            {
                string name = ShaderUtil.GetPropertyName(shader, i);
                ShaderUtil.ShaderPropertyType type = ShaderUtil.GetPropertyType(shader, i);
                switch (type)
                {
                    case ShaderUtil.ShaderPropertyType.TexEnv:
                        Texture tex = mat.GetTexture(name);
                        if (tex != null)
                            lst.Add(tex);
                        break;
                }
            }
        }

    }
}