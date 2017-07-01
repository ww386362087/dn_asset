using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using XEditor;
using System.IO;
using XTable;

namespace XEditor
{
    public class SelectBones : EditorWindow
    {
        protected GameObject model = null;
        protected string prefabName = "";

        protected bool checkAll = false;
        protected GameObject newGo = null;

        protected List<string> exposedBones = new List<string>();
        protected List<string> exExposedBones = new List<string>();
        protected ModelBone root = new ModelBone();
        protected ModelImporter modelImporter;
        protected string path = "";
        protected Vector2 scrollPos = Vector2.zero;
        protected string creatorFolderName = "";
        protected bool gameResource = true;
        protected string prefabRootPath = "Assets/Resources/Prefabs/";


        public virtual void Init()
        {
            UnityEngine.Object[] fbxs = Selection.GetFiltered(typeof(GameObject), SelectionMode.DeepAssets);
            if (fbxs != null && fbxs.Length > 0)
            {
                model = fbxs[0] as GameObject;
                path = "";
                if (model != null)
                {
                    path = AssetDatabase.GetAssetPath(model);
                    modelImporter = AssetImporter.GetAtPath(path) as ModelImporter;
                    //revert to default
                    modelImporter.optimizeGameObjects = false;
                    modelImporter.extraExposedTransformPaths = null;
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    if (prefabName == "")
                    {
                        prefabName = model.name.ToLower();
                        int index = prefabName.IndexOf("_bandpose");
                        if (index >= 0)
                        {
                            prefabName = prefabName.Substring(0, index);
                        }
                    }
                    FilterBone();
                }
            }
            AssetDatabase.Refresh();
        }
        public void AddExExposeBone(string bone)
        {
            if (!exExposedBones.Contains(bone))
                exExposedBones.Add(bone);
        }
        protected virtual void FilterBone()
        {
            root.BoneName = model.name;
            FindModelBones(model.transform, "", root);
        }


        private ModelBone FindModelBones(Transform t, string parentPath, ModelBone parent)
        {
            ModelBone modelBone = null;
            if (t != model.transform)
            {
                modelBone = new ModelBone();
            }
            if (modelBone != null)
            {
                //SkinnedMeshRenderer skin = t.GetComponent<SkinnedMeshRenderer>();
                //MeshRenderer mesh = t.GetComponent<MeshRenderer>();
                modelBone.BoneName = t.name;
                modelBone.Path = parentPath + t.name;
                modelBone.Check = exposedBones.Contains(modelBone.Path) || exExposedBones.Contains(t.name);
                modelBone.Parent = parent;
            }
            if (t != model.transform)
                parentPath = parentPath + t.name + "/";
            for (int i = 0; i < t.childCount; ++i)
            {
                ModelBone childModelBone = FindModelBones(t.GetChild(i), parentPath, modelBone);
                if (modelBone != null)
                    modelBone.Child.Add(childModelBone);
                else
                    parent.Child.Add(childModelBone);
            }
            return modelBone;
        }

        private void FindComponentSkinnedMesh(Transform t, List<SkinnedMeshRenderer> components)
        {
            SkinnedMeshRenderer comp = t.GetComponent<SkinnedMeshRenderer>();
            if (comp != null)
            {
                components.Add(comp);
            }
            for (int i = 0; i < t.childCount; ++i)
            {
                FindComponentSkinnedMesh(t.GetChild(i), components);
            }
        }

        private void FindComponentMesh(Transform t, List<MeshRenderer> components)
        {
            MeshRenderer comp = t.GetComponent<MeshRenderer>();
            if (comp != null)
            {
                components.Add(comp);
            }
            for (int i = 0; i < t.childCount; ++i)
            {
                FindComponentMesh(t.GetChild(i), components);
            }
        }

        private void DrawTree(ModelBone modelBone, int depth)
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < depth; ++i)
                GUILayout.Label(" ", GUILayout.ExpandWidth(false));
            modelBone.Check = EditorGUILayout.Toggle(modelBone.Check, GUILayout.Width(30));
            GUILayout.Label(modelBone.BoneName, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            for (int i = 0; i < modelBone.Child.Count; ++i)
            {
                DrawTree(modelBone.Child[i], depth + 1);
            }
        }

        private void ExposedBone(ModelBone modelBone)
        {
            if (modelBone.Check && !exposedBones.Contains(modelBone.Path))
            {
                exposedBones.Add(modelBone.Path);
            }
            for (int i = 0; i < modelBone.Child.Count; ++i)
            {
                ExposedBone(modelBone.Child[i]);
            }
        }

        private void SetLayer(Transform t, int layer)
        {
            t.gameObject.layer = layer;
            for (int i = 0; i < t.childCount; ++i)
            {
                SetLayer(t.GetChild(i), layer);
            }
        }

        private bool CopyValue<T>(GameObject srcGO, GameObject destGO) where T : Component
        {
            T srcCC = srcGO.GetComponent<T>();
            if (srcCC == null)
                return false;

            T destCC = destGO.GetComponent<T>();
            if (destCC == null)
                destCC = destGO.AddComponent<T>();

            UnityEditorInternal.ComponentUtility.CopyComponent(srcCC);
            UnityEditorInternal.ComponentUtility.PasteComponentValues(destCC);

            return true;
        }

        private void CheckAll(ModelBone modelBone)
        {
            modelBone.Check = checkAll;
            for (int i = 0; i < modelBone.Child.Count; ++i)
            {
                CheckAll(modelBone.Child[i]);
            }
        }
        protected virtual void MakeGameObject()
        {
            ExposedBone(root);
            modelImporter.optimizeGameObjects = true;
            modelImporter.isReadable = false;
            modelImporter.extraExposedTransformPaths = exposedBones.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            List<SkinnedMeshRenderer> srcSkins = new List<SkinnedMeshRenderer>();
            List<MeshRenderer> srcMeshs = new List<MeshRenderer>();

            string prefabPath = prefabRootPath + prefabName + ".prefab";

            GameObject srcPrefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
            if (srcPrefab != null)
            {
                FindComponentSkinnedMesh(srcPrefab.transform, srcSkins);
                FindComponentMesh(srcPrefab.transform, srcMeshs);
            }

            newGo = UnityEngine.Object.Instantiate(model) as GameObject;
            if (newGo != null)
            {
                List<SkinnedMeshRenderer> skins = new List<SkinnedMeshRenderer>();
                FindComponentSkinnedMesh(newGo.transform, skins);
                foreach (SkinnedMeshRenderer skin in skins)
                {
                    skin.castShadows = false;
                    skin.receiveShadows = false;
                    skin.useLightProbes = true;
                    foreach (SkinnedMeshRenderer srcSkin in srcSkins)
                    {
                        if (srcSkin.name == skin.name)
                        {
                            skin.useLightProbes = srcSkin.useLightProbes;
                            break;
                        }
                    }
                }
                List<MeshRenderer> meshs = new List<MeshRenderer>();
                FindComponentMesh(newGo.transform, meshs);
                foreach (MeshRenderer mesh in meshs)
                {
                    mesh.castShadows = false;
                    mesh.receiveShadows = false;
                    mesh.useLightProbes = true;
                    foreach (MeshRenderer srcMesh in srcMeshs)
                    {
                        if (srcMesh.name == mesh.name)
                        {
                            mesh.useLightProbes = srcMesh.useLightProbes;
                            break;
                        }
                    }
                }
                Animator srcAni = null;
                if (srcPrefab != null)
                {
                    CopyValue<CharacterController>(srcPrefab, newGo);
                    CopyValue<BoxCollider>(srcPrefab, newGo);
                    SetLayer(newGo.transform, srcPrefab.layer);
                    srcAni = srcPrefab.GetComponent<Animator>();
                    for (int i = 0; i < srcPrefab.transform.childCount; ++i)
                    {
                        Transform child = srcPrefab.transform.GetChild(i);
                        if (child.name.ToLower() == "hugemonstercolliders")
                        {
                            GameObject childGo = GameObject.Instantiate(child.gameObject) as GameObject;
                            childGo.name = "HugeMonsterColliders";
                            childGo.SetActive(false);
                            childGo.transform.parent = newGo.transform;
                        }
                        else if (child.name.StartsWith("Ty_"))
                        {
                            GameObject childGo = GameObject.Instantiate(child.gameObject) as GameObject;
                            childGo.name = child.name;
                            childGo.transform.parent = newGo.transform;
                        }
                    }
                    UnityEditor.Selection.activeObject = srcPrefab;
                }
                Animator desAni = newGo.GetComponent<Animator>();
                if (desAni != null)
                {
                    if (srcAni != null && srcAni.runtimeAnimatorController != null)
                    {
                        desAni.runtimeAnimatorController = Resources.Load("Controller/" + srcAni.runtimeAnimatorController.name) as RuntimeAnimatorController;
                    }
                    else
                    {
                        desAni.runtimeAnimatorController = Resources.Load("Controller/XAnimator") as RuntimeAnimatorController;
                    }
                }
            }
        }

        protected virtual void OnGUI()
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
            DrawTree(root, 0);
            GUILayout.EndScrollView();
            prefabName = EditorGUILayout.TextField("Prefab Name", prefabName);
            bool beforeCheck = checkAll;
            checkAll = EditorGUILayout.ToggleLeft("Check All", checkAll);
            if (beforeCheck != checkAll)
            {
                CheckAll(root);
            }
            gameResource = EditorGUILayout.ToggleLeft("Game Resource", gameResource);
            if (GUILayout.Button("OK", GUILayout.ExpandWidth(false)))
            {
                MakeGameObject();

                this.Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.ExpandWidth(false)))
            {
                this.Close();
            }
        }
    }


    public class ModelBone
    {
        public bool Check = false;
        public string BoneName = "";
        public string Path = "";
        public ModelBone Parent = null;
        public List<ModelBone> Child = new List<ModelBone>();
    }



    public class SelectEquipBones : SelectBones
    {
        private string spritePoint = "sprite";
        private CombineConfig combineConfig = null;
        public override void Init()
        {
            combineConfig = CombineConfig.GetConfig();
            base.Init();
        }
        protected override void FilterBone()
        {
            if (model != null)
            {
                TextAsset ta = Resources.Load<TextAsset>(@"Table/DefaultEquip");
                if (ta != null)
                {
                    DefaultEquip de = new DefaultEquip();
                    using (MemoryStream ms = new System.IO.MemoryStream(ta.bytes))
                    {
                        de.ReadFile(ms);
                        int id = 0;
                        for (int i = 0; i < combineConfig.BandposeName.Length; ++i)
                        {
                            if (model.name.ToLower() == combineConfig.BandposeName[i])
                            {
                                id = i + 1;
                                prefabName = combineConfig.PrefabName[i];
                                creatorFolderName = combineConfig.SkillFolderName[i];
                            }
                        }
                        if (id > 0)
                        {
                            //DefaultEquip.RowData data = de.GetByProfID(id);
                            //if (data != null)
                            //{
                            //    AddExExposeBone(data.WingPoint);
                            //    AddExExposeBone(data.TailPoint);
                            //    if (data.WeaponPoint != null && data.WeaponPoint.Length > 0)
                            //        AddExExposeBone(data.WeaponPoint[0]);
                            //    if (data.WeaponPoint != null && data.WeaponPoint.Length > 1)
                            //        AddExExposeBone(data.WeaponPoint[1]);
                            //    AddExExposeBone(data.FishingPoint);
                            //    AddExExposeBone(data.FishingPoint);
                            //}
                        }
                    }
                }
            }
            //isReadable = false;
            base.FilterBone();
        }

        protected override void MakeGameObject()
        {
            if (!gameResource)
                prefabRootPath = "Assets/Editor/EditorResources/Prefabs/";
            base.MakeGameObject();
            if (newGo != null)
            {
                GameObject spriteGo = new GameObject(spritePoint);
                spriteGo.transform.parent = newGo.transform;
                spriteGo.layer = newGo.layer;
                if (gameResource)
                {
                    int count = newGo.transform.childCount;
                    for (int i = count - 1; i >= 0; --i)
                    {
                        Transform child = newGo.transform.GetChild(i);
                        if (child.GetComponent<SkinnedMeshRenderer>() != null ||
                            child.GetComponent<MeshRenderer>() != null)
                        {
                            GameObject.DestroyImmediate(child.gameObject);
                        }
                    }
                    GameObject skinMesh = new GameObject("CombinedMesh");
                    skinMesh.transform.parent = newGo.transform;
                    SkinnedMeshRenderer smr = skinMesh.AddComponent<SkinnedMeshRenderer>();
                    smr.receiveShadows = false;
                    smr.useLightProbes = true;
                    smr.castShadows = false;
                    smr.updateWhenOffscreen = false;
                    smr.rootBone = newGo.transform;
                    smr.localBounds = new Bounds(new Vector3(0, 0.5f, 0), new Vector3(0.5f, 1.0f, 0.5f));
                    skinMesh.layer = newGo.layer;
                }
            }
        }

    }

}