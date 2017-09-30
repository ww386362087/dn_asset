using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace XEditor
{

    public class FbxEditor
    {

        [MenuItem(@"Assets/Tool/Fbx/InitCombineConfig")]
        private static void InitCombineConfig()
        {
            GameObject go = new GameObject("CombineConfig");
            go.AddComponent<CombineConfig>();
            PrefabUtility.CreatePrefab(XEditorLibrary.Comb, go, ReplacePrefabOptions.ReplaceNameBased);
            GameObject.DestroyImmediate(go);
        }

        [MenuItem(@"Assets/Tool/Fbx/OptmizeCreatures")]
        private static void OptmizeGameObject()
        {
            Rect wr = new Rect(0, 0, 600, 800);
            SelectBonesEditor window = (SelectBonesEditor)EditorWindow.GetWindowWithRect(typeof(SelectBonesEditor), wr, true, "隐藏骨骼");
            window.Init();
            window.Show();
        }

        [MenuItem(@"Assets/Tool/Fbx/OptmizeEquipment")]
        private static void OptmizeEquipGameObject()
        {
            Rect wr = new Rect(0, 0, 600, 800);
            SelectEquipBones window = (SelectEquipBones)EditorWindow.GetWindowWithRect(typeof(SelectEquipBones), wr, true, "隐藏骨骼");
            window.Init();
            window.Show();
        }

        [MenuItem(@"Assets/Tool/Fbx/SaveSkinAsset %3")]
        private static void SaveSkinAsset()
        {
            s_CombineConfig = GetConfig();
            EnumFbx(_SaveSkinAsset, "SaveSkinAsset");
        }

        [MenuItem(@"Assets/Tool/Fbx/SaveMountAsset %4")]
        private static void SaveMountAsset()
        {
            EnumFbx(_SaveMountAssett, "SaveMountAsset");
        }

        [MenuItem(@"Assets/Tool/Fbx/MakeFbxReadOnly")]
        private static void MakeFbxReadOnly()
        {
            EnumFbx(_MakeFbxReadOnly, "MakeFbxReadOnly");
        }

        [MenuItem(@"Assets/Tool/Fbx/MakeExtraEquip")]
        private static void MakeExtraEquip()
        {
            Rect wr = new Rect(0, 0, 600, 500);
            MakeEquip window = (MakeEquip)EditorWindow.GetWindowWithRect(typeof(MakeEquip), wr, true, "制作装备");
            window.model = Selection.activeObject;
            window.Show();
        }

        [MenuItem(@"Assets/Tool/Fbx/Preview %5")]
        private static void PreviewEquip()
        {
            EquipPreviewEditor window = (EquipPreviewEditor)EditorWindow.GetWindow(typeof(EquipPreviewEditor), true, "套装预览");
            window.Show();
        }


        public delegate bool EnumFbxCallback(GameObject fbx, ModelImporter modelImporter, string path);
        public static void EnumFbx(EnumFbxCallback cb, string title)
        {
            Object[] fbxs = Selection.GetFiltered(typeof(Object), SelectionMode.DeepAssets);
            if (fbxs != null)
            {
                for (int i = 0; i < fbxs.Length; ++i)
                {
                    GameObject fbx = fbxs[i] as GameObject;
                    string path = "";
                    if (fbx != null)
                    {
                        path = AssetDatabase.GetAssetPath(fbx);
                        ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                        if (modelImporter != null && cb(fbx, modelImporter, path))
                        {
                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        }
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, fbxs.Length), path, (float)i / fbxs.Length);
                }
            }
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }

        private static int GetUVOffset(int profession, string meshName, CombineConfig config)
        {
            if (meshName.ToLower().EndsWith(config.BodyString))
            {
                return (int)EPartType.EUpperBody;
            }
            if (meshName.ToLower().EndsWith(config.LegString))
            {
                return (int)EPartType.ELowerBody;
            }
            if (meshName.ToLower().EndsWith(config.GloveString))
            {
                return (int)EPartType.EGloves;
            }
            if (meshName.ToLower().EndsWith(config.BootString))
            {
                return (int)EPartType.EBoots;
            }
            if (meshName.ToLower().EndsWith(config.HeadString) ||
                meshName.ToLower().EndsWith(config.FaceString))
            {
                return (int)EPartType.EFace;
            }
            if (meshName.ToLower().EndsWith(config.HairString))
            {
                return (int)EPartType.EHair;
            }
            if (meshName.ToLower().EndsWith(config.HelmetString) ||
                meshName.ToLower().EndsWith("_helmat"))
            {
                return (int)EPartType.EHeadgear;
            }
            if (meshName.ToLower().EndsWith(config.SecondaryWeapon[profession]))
            {
                return (int)EPartType.ESecondaryWeapon;
            }
            return -1;
        }

        private static void ReCalculateUV(Mesh mesh, int uvOffsetX)
        {
            if (uvOffsetX >= 0)
            {
                Vector2[] uv = mesh.uv;
                for (int i = 0, imax = mesh.uv.Length; i < imax; ++i)
                {
                    //计算新uv
                    Vector2 tmp = uv[i];
                    tmp.x = tmp.x - Mathf.Floor(tmp.x);
                    tmp.x += uvOffsetX;
                    tmp.y = tmp.y - Mathf.Floor(tmp.y);
                    uv[i] = tmp;
                }
                mesh.uv = uv;
            }
        }

        public static void CleanMesh(Mesh mesh)
        {
            mesh.uv2 = null;
            mesh.uv3 = null;
            mesh.uv4 = null;
            mesh.colors = null;
            mesh.colors32 = null;
            mesh.tangents = null;
        }

        private static void SaveMeshAsset(Mesh mesh, Texture2D tex, int profession, string path)
        {
            int uvOffsetX = GetUVOffset(profession, mesh.name, s_CombineConfig);

            if (uvOffsetX >= 0) ReCalculateUV(mesh, uvOffsetX);
            else XDebug.LogError("Find UV Error:" , mesh.name);

            CleanMesh(mesh);
            string meshPath = path + mesh.name + ".asset";
            AssetDatabase.CreateAsset(mesh, meshPath);
            if (tex != null)
            {
                string srcTexPath = AssetDatabase.GetAssetPath(tex);
                string destTexPath = "Assets/Resources/Equipments/" + tex.name + ".tga";
                AssetDatabase.CopyAsset(srcTexPath, destTexPath);
            }
            AssetDatabase.SaveAssets();
        }

        private static CombineConfig s_CombineConfig = null;
        private static bool _SaveSkinAsset(GameObject fbx, ModelImporter modelImporter, string path)
        {
            int profession = -1;
            for (int i = 0; i < s_CombineConfig.EquipFolderName.Length; ++i)
            {
                if (path.Contains(s_CombineConfig.EquipFolderName[i]))
                {
                    profession = i;
                    break;
                }
            }
            if (profession < 0)
            {
                modelImporter.isReadable = false;
                return true;
            }
            string saveRootPath = "Assets/Resources/Equipments/";
            modelImporter.isReadable = true;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            GameObject go = GameObject.Instantiate(fbx) as GameObject;

            SkinnedMeshRenderer[] smrs = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer smr in smrs)
            {
                Mesh mesh = Object.Instantiate(smr.sharedMesh) as Mesh;
                mesh.name = smr.sharedMesh.name;
                mesh.UploadMeshData(false);
                SaveMeshAsset(mesh, smr.sharedMaterial.mainTexture as Texture2D, profession, saveRootPath);
            }
            modelImporter.isReadable = false;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            saveRootPath = "Assets/Resources/Equipments/weapon/";
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>();
            foreach (MeshFilter mf in mfs)
            {
                Mesh mesh = Object.Instantiate(mf.sharedMesh) as Mesh;
                mesh.name = mf.sharedMesh.name;
                mesh.UploadMeshData(true);
                CleanMesh(mesh);
                string meshPath = saveRootPath + mesh.name + ".asset";
                AssetDatabase.CreateAsset(mesh, meshPath);
                AssetDatabase.SaveAssets();
                Mesh loadMesh = AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh)) as Mesh;
                mf.sharedMesh = loadMesh;
                MeshRenderer mr = mf.transform.GetComponent<MeshRenderer>();
                mr.lightProbeUsage = LightProbeUsage.BlendProbes;
                mr.shadowCastingMode = ShadowCastingMode.Off;
                mr.receiveShadows = false;
                mr.gameObject.layer = LayerMask.NameToLayer("Role");
                PrefabUtility.CreatePrefab(saveRootPath + mesh.name + ".prefab", mr.gameObject, ReplacePrefabOptions.ReplaceNameBased);
            }
            GameObject.DestroyImmediate(go);
            return false;
        }

        private static bool _MakeFbxReadOnly(GameObject fbx, ModelImporter modelImporter, string path)
        {
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            MeshFilter[] mfs = go.GetComponentsInChildren<MeshFilter>();
            bool readable = true;
            bool blendShape = false;
            bool animation = false;
            if (mfs != null)
            {
                foreach (MeshFilter mf in mfs)
                {
                    Mesh mesh = mf.sharedMesh;
                    if (mesh != null)
                    {
                        if (mesh.vertexCount > 1500)
                        {
                            readable = false;
                            break;
                        }
                    }
                }
            }
            SkinnedMeshRenderer[] smr = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (smr != null && smr.Length > 0)
            {
                blendShape = true;
                readable = false;
                animation = true;
            }
            else
            {
                animation = false;
            }
            GameObject.DestroyImmediate(go);
            bool change = modelImporter.isReadable != readable || modelImporter.importBlendShapes != blendShape || modelImporter.importAnimation != animation;
            modelImporter.isReadable = readable;
            modelImporter.importBlendShapes = blendShape;
            modelImporter.importAnimation = animation;
            return change;
        }


        private static bool _SaveMountAssett(GameObject fbx, ModelImporter modelImporter, string path)
        {
            string saveRootPath = "Assets/Resources/Prefabs/Equipment/";
            GameObject go = GameObject.Instantiate(fbx) as GameObject;
            Renderer[] renders = go.GetComponentsInChildren<Renderer>();
            Shader shader = Shader.Find("Custom/Common/MobileDiffuse");
            foreach (Renderer r in renders)
            {
                r.lightProbeUsage = LightProbeUsage.BlendProbes;
                r.shadowCastingMode = ShadowCastingMode.Off;
                r.receiveShadows = false;
                foreach (Material mat in r.sharedMaterials)
                {
                    mat.shader = shader;
                }
            }

            Animator animator = go.GetComponent<Animator>();
            animator.runtimeAnimatorController = XResources.Load("Controller/XMinorAnimator", AssetType.Controller) as RuntimeAnimatorController;
            go.layer = LayerMask.NameToLayer("Role");
            PrefabUtility.CreatePrefab(saveRootPath + fbx.name + ".prefab", go, ReplacePrefabOptions.ReplaceNameBased);
            GameObject.DestroyImmediate(go);
            modelImporter.isReadable = false;
            return true;
        }

        public static CombineConfig GetConfig()
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(XEditorLibrary.Comb, typeof(GameObject)) as GameObject;
            return go.GetComponent<CombineConfig>();
        }
    }
}