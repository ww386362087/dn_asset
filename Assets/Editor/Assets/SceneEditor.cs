using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

namespace XEditor
{
    public class SceneEditor
    {
        private delegate bool EnumSceneCallback(EditorBuildSettingsScene scene);
        private static void EnumScene(EnumSceneCallback cb, string title)
        {
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

            UnityEngine.Object[] objs = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            if (objs != null)
            {
                for (int i = 0; i < objs.Length; ++i)
                {
                    UnityEngine.Object obj = objs[i];
                    string path = "";
                    if (obj != null && cb != null)
                    {
                        path = AssetDatabase.GetAssetPath(obj);
                        foreach (EditorBuildSettingsScene scene in scenes)
                        {
                            if (scene.path == path)
                            {
                                cb(scene);
                                break;
                            }
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

        private static bool _PrcessLight(EditorBuildSettingsScene scene)
        {
            Scene sc = EditorSceneManager.OpenScene(scene.path);
            Light[] lights = UnityEngine.Object.FindObjectsOfType(typeof(Light)) as Light[];
            if (lights != null)
            {
                foreach (Light light in lights)
                {
                    if (light.gameObject.activeInHierarchy && light.type == LightType.Directional)
                    {
                        lights[0].transform.parent = null;
                        lights[0].transform.localRotation = Quaternion.Euler(new Vector3(45, 180, 0));
                        lights[0].name = "MainLight";
                        LayerMask layerMask = lights[0].cullingMask;
                        layerMask.value = 1 << LayerMask.NameToLayer("Player") |
                             1 << LayerMask.NameToLayer("Role") |
                             1 << LayerMask.NameToLayer("Enemy") |
                             1 << LayerMask.NameToLayer("BigGuy") |
                             1 << LayerMask.NameToLayer("Npc") |
                             1 << LayerMask.NameToLayer("Terrain") |
                             1 << LayerMask.NameToLayer("Dummy");
                        lights[0].cullingMask = layerMask;
                        break;
                    }
                }
                EditorSceneManager.SaveScene(sc, scene.path);
            }
            return true;
        }


        private static bool _PrcessSceneMat(Material mat, string path)
        {
            _PrcessSceneMat(mat, false);
            return true;
        }

        private static HashSet<Material> processedMat = new HashSet<Material>();
        private static void _PrcessSceneMat(Material mat, bool recover)
        {
            if (!processedMat.Contains(mat) && mat.shader != null)
            {
                if (mat.shader.name == "Transparent/Cutout/Diffuse" ||
                mat.shader.name == "Custom/Scene/CutoutDiffuseMaskLM")
                {
                    Texture2D tex = mat.mainTexture as Texture2D;
                    if (tex != null)
                    {
                        if (recover)
                        {
                            string texPath = AssetDatabase.GetAssetPath(tex);
                            int size = tex.width > tex.height ? tex.width : tex.height;
                            TextureImporter texImporter = AssetImporter.GetAtPath(texPath) as TextureImporter;
                            texImporter.textureType = TextureImporterType.Default;
                            texImporter.anisoLevel = 0;
                            texImporter.mipmapEnabled = size > 256;
                            texImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                            texImporter.wrapMode = TextureWrapMode.Repeat;
                            texImporter.filterMode = FilterMode.Bilinear;
                            TextureImporterPlatformSettings pseting = new TextureImporterPlatformSettings();
                            pseting.format = TextureImporterFormat.RGBA16;
                            pseting.name = BuildTarget.Android.ToString();
                            pseting.maxTextureSize = size;
                            texImporter.SetPlatformTextureSettings(pseting);
                            pseting.name = BuildTarget.iOS.ToString();
                            texImporter.SetPlatformTextureSettings(pseting);
                            texImporter.isReadable = false;
                            AssetDatabase.ImportAsset(texPath, ImportAssetOptions.ForceUpdate);
                            mat.shader = Shader.Find("Transparent/Cutout/Diffuse");
                        }
                        else
                        {
                            Texture2D alphaTex = TextureModify.ConvertTexRtex(tex);
                            mat.shader = Shader.Find("Custom/Scene/CutoutDiffuseMaskLM");
                            mat.SetTexture("_Mask", alphaTex);
                        }
                    }
                    processedMat.Add(mat);
                }
            }
        }

        [MenuItem(@"Assets/Tool/Scene/PrcessSceneMat")]
        private static void PrcessSceneMat()
        {
            processedMat.Clear();
            MaterialEditor.EnumMaterial(_PrcessSceneMat, "PrcessSceneMat");
            processedMat.Clear();
        }

        private static bool _FullMatTex(Material mat, string path)
        {
            if (!processedMat.Contains(mat) && mat.shader != null)
            {
                processedMat.Add(mat);
                Texture2D tex = mat.mainTexture as Texture2D;
                string texturePath = AssetDatabase.GetAssetPath(tex);
                texturePath = texturePath.Replace("Half", "");
                Texture2D fullTex = AssetDatabase.LoadAssetAtPath(texturePath, typeof(Texture2D)) as Texture2D;
                if (fullTex != tex) mat.mainTexture = fullTex;
            }
            return false;
        }

        [MenuItem(@"Assets/Tool/Scene/FullMatTex")]
        private static void FullMatTex()
        {
            processedMat.Clear();
            MaterialEditor.EnumMaterial(_FullMatTex, "FullMatTex");
            processedMat.Clear();
        }

        public static void PrcessSceneMat(string[] files)
        {
            int i = 0;
            foreach (string strPath in files)
            {
                string strTempPath = strPath.Replace(@"\", "/");
                strTempPath = strTempPath.Substring(strTempPath.IndexOf("Assets"));
                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(@strTempPath, typeof(UnityEngine.Material));
                Material mat = obj as Material;
                if (mat != null)
                {
                    _PrcessSceneMat(mat, "");
                    EditorUtility.DisplayProgressBar(i.ToString(), mat.name, 1.0f * i / files.Length);
                }
                ++i;
            }
            EditorUtility.ClearProgressBar();
        }

        public static void PrcessSceneTexMat()
        {
            processedMat.Clear();
            string[] arrStrPath = Directory.GetFiles(Application.dataPath + "/XScene/grass/", "*", SearchOption.AllDirectories);
            PrcessSceneMat(arrStrPath);
            arrStrPath = Directory.GetFiles(Application.dataPath + "/XScene/modlelib/", "*", SearchOption.AllDirectories);
            PrcessSceneMat(arrStrPath);
            processedMat.Clear();
        }

        private static bool _PrcessSceneTerrain(EditorBuildSettingsScene scene)
        {
            Scene sc = EditorSceneManager.OpenScene(scene.path);
            Terrain[] terrains = UnityEngine.Object.FindObjectsOfType(typeof(Terrain)) as Terrain[];
            if (terrains != null)
            {
                for (int i = 0, imax = terrains.Length; i < imax; ++i)
                {
                    Terrain terrain = terrains[i];
                    if (terrain.basemapDistance < 1000)
                    {
                        terrain.basemapDistance = 1000;
                    }
                    else if (terrain.basemapDistance > 1000)
                    {
                        terrain.basemapDistance = 1000;
                    }
                    else
                    {
                        terrain.basemapDistance = 1001;
                    }
                    terrain.terrainData.baseMapResolution = 16;
                    terrain.Flush();
                }
                EditorSceneManager.SaveScene(sc, scene.path);
            }
            return true;
        }

        [MenuItem(@"Assets/Tool/Scene/PrcessSceneTerrain")]
        private static void PrcessSceneTerrain()
        {
            EnumScene(_PrcessSceneTerrain, "PrcessSceneTerrain");
        }

        private static void _SeprateScene(string path)
        {
            if (path.EndsWith("_add.unity")) return;
            string rootPath = "Assets/XSeperateScene/";
            string name = path;
            int index = path.LastIndexOf("/");
            if (index >= 0)
            {
                name = name.Substring(index + 1);
            }
            index = name.LastIndexOf(".");
            if (index >= 0)
            {
                name = name.Substring(0, index);
            }
            EditorSceneManager.OpenScene(path);
            GameObject go = GameObject.Find("Scene");
            if (go != null)
            {
                //1.save as new scene
                string addpath = string.Format("{0}{1}_add.unity", rootPath, name);
                Scene addScene = EditorSceneManager.GetSceneByPath(addpath);
                EditorSceneManager.SaveScene(addScene);

                //2.save as old scene
                GameObject.DestroyImmediate(go);
                LightmapSettings.lightmaps = null;
                LightmapSettings.lightProbes = null;
                string origpath = string.Format("{0}{1}.unity", rootPath, name);
                Scene oriScene = EditorSceneManager.GetSceneByPath(origpath);
                EditorSceneManager.SaveScene(oriScene);

                addScene = EditorSceneManager.OpenScene(addpath);
                List<GameObject> gos = new List<GameObject>();
                HierarchyProperty hp = new HierarchyProperty(HierarchyType.GameObjects);
                int[] expanded = new int[0];
                while (hp.Next(expanded))
                {
                    gos.Add(hp.pptrValue as GameObject);
                }
                for (int i = gos.Count - 1; i >= 0; --i)
                {
                    if (gos[i].name != "Scene")
                    {
                        GameObject.DestroyImmediate(gos[i]);
                    }
                }
                StaticOcclusionCulling.Cancel();
                EditorSceneManager.SaveScene(addScene, addpath);
            }
            else
            {
                LightmapSettings.lightmaps = null;
                LightmapSettings.lightProbes = null;
                string origpath = string.Format("{0}{1}.unity", rootPath, name);
                Scene oriScene = EditorSceneManager.GetSceneByPath(origpath);
                EditorSceneManager.SaveScene(oriScene);
            }
        }
        private static bool _SeprateScene(EditorBuildSettingsScene scene)
        {
            _SeprateScene(scene.path);
            return true;
        }

        private static HashSet<Mesh> processedMesh = new HashSet<Mesh>();
        private static void InnerProcessMat(Transform t, bool recover)
        {
            Animator animator = t.GetComponent<Animator>();
            MeshRenderer meshRender = t.GetComponent<MeshRenderer>();
            if (animator != null && meshRender != null)
            {
                UnityEngine.Object.DestroyImmediate(animator);
            }
            if (meshRender != null && meshRender.enabled)
            {
                meshRender.shadowCastingMode = recover ? ShadowCastingMode.On : ShadowCastingMode.Off;
                meshRender.receiveShadows = recover;
                for (int i = 0, imax = meshRender.sharedMaterials.Length; i < imax; ++i)
                {
                    Material mat = meshRender.sharedMaterials[i];
                    if (mat != null)
                        _PrcessSceneMat(mat, recover);
                    else
                    {
                        Debug.LogError("Null mat:" + meshRender.name);
                    }
                }
            }

            MeshFilter mf = t.GetComponent<MeshFilter>();
            if (mf != null && mf.sharedMesh != null)
            {
                Mesh m = mf.sharedMesh;
                if (!processedMesh.Contains(m))
                {
                    string modelPath = AssetDatabase.GetAssetPath(m);
                    ModelImporter modelImporter = AssetImporter.GetAtPath(modelPath) as ModelImporter;
                    if (modelImporter != null)
                    {
                        AssetDatabase.ImportAsset(modelPath, ImportAssetOptions.ForceUpdate);
                    }
                }
            }
            for (int i = t.childCount - 1; i >= 0; --i)
            {
                Transform child = t.GetChild(i);
                InnerProcessMat(child, recover);
            }
        }

        [MenuItem(@"Assets/Tool/Scene/RecoverMaterial")]
        private static void RecoverMaterial()
        {
            XResModelImportEditor.bAccordingSettings = false;
            processedMat.Clear();
            processedMesh.Clear();
            Transform[] allTrans = Selection.transforms;
            for (int i = 0, imax = allTrans.Length; i < imax; ++i)
            {
                Transform t = allTrans[i];
                InnerProcessMat(t, true);
            }
            processedMat.Clear();
            processedMesh.Clear();
            XResModelImportEditor.bAccordingSettings = true;
            EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        }

        private static bool _RecoverFbx(GameObject fbx, ModelImporter modelImporter, string path)
        {
            return false;
        }


        [MenuItem(@"Assets/Tool/Scene/RecoverMaterialInFolder")]
        private static void RecoverMaterialInFolder()
        {
            FbxEditor.EnumFbx(_RecoverFbx, "RecoverFbx");

            XResModelImportEditor.bAccordingSettings = false;
            processedMat.Clear();
            processedMesh.Clear();
            Transform[] allTrans = Selection.transforms;
            for (int i = 0, imax = allTrans.Length; i < imax; ++i)
            {
                Transform t = allTrans[i];
                InnerProcessMat(t, true);
            }
            processedMat.Clear();
            processedMesh.Clear();
            XResModelImportEditor.bAccordingSettings = true;
            EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        }

        [MenuItem(@"Assets/Tool/Scene/ChangeMaterial")]
        private static void ChangeMaterial()
        {
            processedMat.Clear();
            processedMesh.Clear();
            Transform[] allTrans = Selection.transforms;
            for (int i = 0, imax = allTrans.Length; i < imax; ++i)
            {
                Transform t = allTrans[i];
                InnerProcessMat(t, false);
            }
            processedMat.Clear();
            processedMesh.Clear();
            EditorUtility.DisplayDialog("Finish", "processed finish", "OK");
        }
        public static void RemoveLightmapBakeThing(string scenePath)
        {
            Scene sc = EditorSceneManager.OpenScene(scenePath);
            GameObject scene = GameObject.Find(@"Scene");
            if (scene != null)
            {
                List<GameObject> lst = new List<GameObject>();
                for (int i = 0, imax = scene.transform.childCount; i < imax; ++i)
                {
                    Transform t = scene.transform.GetChild(i);
                    if (!t.gameObject.activeSelf)
                    {
                        lst.Add(t.gameObject);
                    }
                }
                for (int i = 0, imax = lst.Count; i < imax; ++i)
                {
                    GameObject.DestroyImmediate(lst[i]);
                }
                if (lst.Count > 0)
                    EditorSceneManager.SaveScene(sc);
            }
        }

        public static void RemoveLightmapBakeThing(string[] scenes)
        {
            foreach (string scenePath in scenes)
            {
                RemoveLightmapBakeThing(scenePath);
            }
        }

    }
}
