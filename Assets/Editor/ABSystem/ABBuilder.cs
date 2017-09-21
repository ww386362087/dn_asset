using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ABSystem
{

    public class ABBuilder
    {
        AssetBundleDataWriter dataWriter = new AssetBundleDataBinaryWriter();

        [MenuItem(@"ABSystem/BuildABImmediate")]
        public static void BuildBundle()
        {
            AssetBundleBuildPanel.Save();
            AssetBundleBuildPanel.BuildAssetBundles();
        }


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

        public void Analyze()
        {
            var all = AssetBundleUtil.GetAll();
            int total = all.Count;
            int count = 0;
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


        public void AddRootTargets(FileInfo file)
        {
            AssetTarget target = AssetBundleUtil.Load(file);
            if (target == null)  XDebug.LogError(file);
            target.exportType = AssetBundleExportType.Root;
        }

        public void AddRootTargets(DirectoryInfo bundleDir, string[] partterns)
        {
            for (int i = 0; i < partterns.Length; i++)
            {
                FileInfo[] files = bundleDir.GetFiles(partterns[i], SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    AddRootTargets(file);
                }
            }
        }

        protected void SaveDepAll(List<AssetTarget> all)
        {
            string path = Path.Combine(AssetBundleUtil.GetBundleDir(), AssetBundlePathResolver.DependFileName);
            if (File.Exists(path)) File.Delete(path);
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
            dataWriter = w;
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
                     XDebug.Log("Remove unused AB : " , fi.Name);
                    fi.Delete();
                    File.Delete(fi.FullName + ".manifest");
                }
            }
        }
    }
}
