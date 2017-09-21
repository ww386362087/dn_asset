using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;


namespace XEditor
{
    public class PrefabEditor
    {

        public delegate bool EnumEnumPrefabCallback(GameObject go, string path);

        public static void EnumPrefab(EnumEnumPrefabCallback cb, string title)
        {
            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; ++i)
                {
                    GameObject go = objs[i] as GameObject;
                    string path = "";
                    if (go != null)
                    {
                        path = AssetDatabase.GetAssetPath(go);
                        if (cb(go, path))
                        {
                        }
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, objs.Length), path, (float)i / objs.Length);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }


        private static bool _AddStaticRigdbody(GameObject go, string path)
        {
            BoxCollider box = go.GetComponent<BoxCollider>();
            Animator anim = go.GetComponent<Animator>();
            if (box != null && anim != null)
            {
                Rigidbody rb = go.GetComponent<Rigidbody>();
                if (rb == null)
                {
                    rb = go.AddComponent<Rigidbody>();
                }
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            return true;
        }

        [MenuItem(@"Assets/Tool/Prefab/AddStaticRigdbody")]
        private static void AddStaticRigdbody()
        {
            EnumPrefab(_AddStaticRigdbody, "AddStaticRigdbody");
        }

        static string str = "";
        private static bool _Check(GameObject go, string path)
        {
            GameObject prefab = GameObject.Instantiate(go) as GameObject;
            List<Renderer> renderLst = new List<Renderer>();
            prefab.GetComponentsInChildren<Renderer>(renderLst);

            int renderCount = 0;
            for (int i = 0; i < renderLst.Count; ++i)
            {
                Renderer render = renderLst[i];
                if (render != null)
                {
                    if (render.receiveShadows || render.shadowCastingMode== UnityEngine.Rendering.ShadowCastingMode.On)
                    {
                         XDebug.LogError(string.Format("Error Shadow cast:{0} render:{1}", go.name, render.name));
                    }
                    if (render.sharedMaterials.Length > 1)
                    {
                        str += string.Format("Too many materials:{0} render:{1} count:{2}\r\n", go.name, render.name, render.sharedMaterials.Length);
                    }
                    if (!(render is ParticleSystemRenderer))
                    {
                        renderCount++;
                    }
                }
            }
            if (renderCount > 1)
            {
                str += string.Format("Too many renders:{0} count:{1}\r\n", go.name, renderCount);
            }
            GameObject.DestroyImmediate(prefab);
            return true;
        }


        [MenuItem(@"Assets/Tool/Prefab/CheckEquip")]
        private static void CheckEquip()
        {
            str = "";
            EnumPrefab(_Check, "Check");
             XDebug.LogError(str);
        }

        static Dictionary<string, List<string>> fxPath = new Dictionary<string, List<string>>();
        static string currentFxPath = "";
        private static void _CheckFx(Transform t, ref string path)
        {
            for (int i = 0; i < t.childCount; ++i)
            {
                Transform child = t.GetChild(i);
                path += child.name;
                ParticleSystem ps = child.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    if (ps.GetComponent<Renderer>() == null || ps.GetComponent<Renderer>().sharedMaterial == null)
                    {
                         XDebug.LogError("ps render error:" , currentFxPath);
                    }
                    else
                    {
                        path += ps.GetComponent<Renderer>().sharedMaterial.name;
                    }
                }
                _CheckFx(child, ref path);
            }
        }


        private static bool _CheckFx(GameObject go, string path)
        {
            GameObject prefab = GameObject.Instantiate(go) as GameObject;
            string pathKey = "";
            currentFxPath = path;
            _CheckFx(prefab.transform, ref pathKey);
            if (pathKey != "")
            {
                List<string> prefabs = null;
                if (!fxPath.TryGetValue(pathKey, out prefabs))
                {
                    prefabs = new List<string>();
                    fxPath.Add(pathKey, prefabs);
                }
                prefabs.Add(path);
            }

            GameObject.DestroyImmediate(prefab);
            return true;
        }


        [MenuItem(@"Assets/Tool/Prefab/CheckFx")]
        private static void CheckFx()
        {
            fxPath.Clear();

            EnumPrefab(_CheckFx, "CheckFx");
            string full = "";
            foreach (List<string> prefabs in fxPath.Values)
            {
                if (prefabs.Count > 1)
                {
                    string str = "";
                    foreach (string prefab in prefabs)
                    {
                        str += prefab + "\r\n";
                    }
                    full += str + "\r\n";
                     XDebug.LogWarning(str);
                }
            }
            File.WriteAllText("Assets/Resources/Effects/Fx.txt", full);
        }


        public static Dictionary<string, List<string>> CheckSameFx(string path = "Assets/Resources/Effects")
        {
            fxPath.Clear();
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] files = di.GetFiles("*.prefab", SearchOption.AllDirectories);
            foreach (FileInfo fi in files)
            {
                string filePath = fi.FullName;
                filePath = filePath.Replace("\\", "/");
                int index = filePath.IndexOf("Assets/Resources/");
                filePath = filePath.Substring(index);
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(filePath, typeof(UnityEngine.Object));
                if (obj != null)
                {
                    GameObject prefab = GameObject.Instantiate(obj) as GameObject;
                    string pathKey = "";
                    _CheckFx(prefab.transform, ref pathKey);
                    if (pathKey != "")
                    {
                        List<string> prefabs = null;
                        if (!fxPath.TryGetValue(pathKey, out prefabs))
                        {
                            prefabs = new List<string>();
                            fxPath.Add(pathKey, prefabs);
                        }
                        prefabs.Add(filePath);
                    }

                    GameObject.DestroyImmediate(prefab);
                }

            }
            return fxPath;
        }

    }

}