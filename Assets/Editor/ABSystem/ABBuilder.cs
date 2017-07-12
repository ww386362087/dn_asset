using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ABSystem
{

    public class ABBuilder 
    {
        AssetBundleDataWriter dataWriter = new AssetBundleDataBinaryWriter();

        public ABBuilder()
        {
            InitDirs();
        }

        void InitDirs()
        {
            new DirectoryInfo(AssetBundleUtil.GetBundleDir()).Create();
            new FileInfo(AssetBundleUtil.GetCacheFile()).Directory.Create();
        }

        public void Begin()
        {
            EditorUtility.DisplayProgressBar("Loading", "Loading...", 0.1f);
            AssetBundleUtil.Init();
        }

        public void End()
        {
            AssetBundleUtil.SaveCache();
            AssetBundleUtil.ClearCache();
            EditorUtility.ClearProgressBar();
        }

        public virtual void Analyze()
        {
            var all = AssetBundleUtil.GetAll();
            float total = all.Count;
            float count = 0;
            foreach (AssetTarget target in all)
            {
                target.Analyze();
                EditorUtility.DisplayProgressBar(string.Format("Analyze...({0}/{1})", count, total), target.assetPath, ++count / total);
            }
            all = AssetBundleUtil.GetAll();
            total = all.Count;
            count = 0;
            foreach (AssetTarget target in all)
            {
                target.Merge();
                EditorUtility.DisplayProgressBar(string.Format("Merge...({0}/{1})", count, total), target.assetPath, ++count / total);
            }
            all = AssetBundleUtil.GetAll();
            total = all.Count;
            count = 0;
            foreach (AssetTarget target in all)
            {
                target.BeforeExport();
                EditorUtility.DisplayProgressBar(string.Format("BeforeExport...({0}/{1})", count, total), target.assetPath, ++count / total);
            }
        }

        public void Export()
        {
            Analyze();

            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            //标记所有 asset bundle name
            var all = AssetBundleUtil.GetAll();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                {
                    AssetBundleBuild build = new AssetBundleBuild();
                    build.assetBundleName = target.bundleName;
                    build.assetNames = new string[] { target.assetPath };
                    list.Add(build);
                }
            }

            string bundleSavePath = AssetBundleUtil.GetBundleDir();

            //开始打包
            BuildPipeline.BuildAssetBundles(bundleSavePath, list.ToArray(), BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);

            AssetBundle ab = AssetBundle.LoadFromFile(bundleSavePath + "/AssetBundles");

            AssetBundleManifest manifest = ab.LoadAsset("AssetBundleManifest") as AssetBundleManifest;
            //hash
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                {
                    Hash128 hash = manifest.GetAssetBundleHash(target.bundleName);
                    if (target.bundleCrc != hash.ToString())
                    {
                        if (AssetBundleBuildPanel.ABUpdateOpen)
                        {
                            string savePath = Path.Combine(Path.GetTempPath(), target.bundleName);
                            string downloadPath = Path.Combine(Path.Combine(AssetBundleBuildPanel.TargetFolder, "AssetBundles"), target.bundleName);
                            File.Copy(savePath, downloadPath, true);
                            XMetaResPackage pack = new XMetaResPackage();
                            pack.buildinpath = target.bundleShortName;
                            pack.download = string.Format("AssetBundles/{0}", target.bundleName);
                            pack.Size = (uint)(new FileInfo(downloadPath)).Length;
                            if (pack.Size != (new FileInfo(downloadPath)).Length)
                            {
                                EditorUtility.DisplayDialog("Bundle ", target.bundleShortName + " is lager than UINTMAX!!!", "OK");
                            }
                            AssetBundleBuildPanel.assetBundleUpdate.Add(pack);
                            File.Copy(savePath, bundleSavePath, true);
                        }
                    }
                    target.bundleCrc = hash.ToString();
                }
            }
            SaveDepAll(all);
            ab.Unload(true);
            RemoveUnused(all);

            AssetDatabase.RemoveUnusedAssetBundleNames();
            AssetDatabase.Refresh();
        }

        private void AddAssetUIDependencies(Object asset)
        {
            Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });

            for (int i = 0; i < deps.Length; i++)
            {
                Object dep = deps[i];
                string path = AssetDatabase.GetAssetPath(dep);
                if (path.StartsWith("Assets/Resources/atlas") || path.StartsWith("Assets/Resources/StaticUI"))
                {
                    AddRootTargets(path);
                }
            }
        }

        public void AddRootTargets(string path)
        {
            if (path.Contains("atlas/UI"))
            {
                if (path.Contains("_A.png") && File.Exists(path.Replace("_A.png", ".prefab")))
                {
                    return;
                }
                else if (path.Contains(".png") && File.Exists(path.Replace(".png", ".prefab")))
                {
                    return;
                }
                else if (path.Contains(".mat") && File.Exists(path.Replace(".mat", ".prefab")))
                {
                    return;
                }
            }

            if (path.Contains("Resources/UI"))
            {
                AddAssetUIDependencies(AssetDatabase.LoadMainAssetAtPath(path));
            }

            FileInfo file = new FileInfo(path);
            AssetTarget target = AssetBundleUtil.Load(file);
            if (target == null)
                Debug.LogError(file);
            target.exportType = AssetBundleExportType.Root;
        }

        public void AddRootTargets(DirectoryInfo bundleDir, string[] partterns = null, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (partterns == null)
                partterns = new string[] { "*.*" };
            for (int i = 0; i < partterns.Length; i++)
            {
                FileInfo[] prefabs = bundleDir.GetFiles(partterns[i], searchOption);
                foreach (FileInfo file in prefabs)
                {
                    if (file.FullName.Contains("atlas/UI"))
                    {
                        if (file.FullName.Contains("_A.png") && File.Exists(file.FullName.Replace("_A.png", ".prefab")))
                        {
                            continue;
                        }
                        else if (file.FullName.Contains(".png") && File.Exists(file.FullName.Replace(".png", ".prefab")))
                        {
                            continue;
                        }
                        else if (file.FullName.Contains(".mat") && File.Exists(file.FullName.Replace(".mat", ".prefab")))
                        {
                            continue;
                        }
                    }
                    if (file.FullName.Contains("Resources/UI"))
                    {
                        AddAssetUIDependencies(AssetDatabase.LoadMainAssetAtPath(file.FullName));
                    }

                    AssetTarget target = AssetBundleUtil.Load(file);
                    if (target == null) Debug.LogError(file);
                    target.exportType = AssetBundleExportType.Root;
                }
            }
        }

        protected void SaveDepAll(List<AssetTarget> all)
        {
            string path = AssetBundleUtil.GetCacheFile();

            if (File.Exists(path))
                File.Delete(path);

            List<AssetTarget> exportList = new List<AssetTarget>();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                    exportList.Add(target);
            }
            AssetBundleDataWriter writer = dataWriter;
            writer.Save(path, exportList.ToArray());
        }

        public void SetDataWriter(AssetBundleDataWriter w)
        {
            this.dataWriter = w;
        }

        /// <summary>
        /// 删除未使用的AB，可能是上次打包出来的，而这一次没生成的
        /// </summary>
        /// <param name="all"></param>
        protected void RemoveUnused(List<AssetTarget> all)
        {
            HashSet<string> usedSet = new HashSet<string>();
            for (int i = 0; i < all.Count; i++)
            {
                AssetTarget target = all[i];
                if (target.needSelfExport)
                    usedSet.Add(target.bundleName);
            }

            DirectoryInfo di = new DirectoryInfo(AssetBundleUtil.GetBundleDir());

            FileInfo[] abFiles = di.GetFiles("*.ab");
            for (int i = 0; i < abFiles.Length; i++)
            {
                FileInfo fi = abFiles[i];
                if (usedSet.Add(fi.Name))
                {
                    Debug.Log("Remove unused AB : " + fi.Name);

                    fi.Delete();
                    //for U5X
                    File.Delete(fi.FullName + ".manifest");
                }
            }
        }
    }
}
