using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace ABSystem
{
    public class AssetTarget : System.IComparable<AssetTarget>
    {
        /// <summary>
        /// 目标Object
        /// </summary>
        public Object asset;
        /// <summary>
        /// 文件路径
        /// </summary>
        public FileInfo file;
        /// <summary>
        /// 路径： Assets\Equipment\Archer\ar_lotus_a01_leg.tga
        /// </summary>
        public string assetPath;
        /// <summary>
        /// 此文件是否已导出
        /// </summary>
        public bool isExported;
        /// <summary>
        /// 素材类型
        /// </summary>
        public AssetType type = AssetType.Asset;
        /// <summary>
        /// 导出类型
        /// </summary>
        public AssetBundleExportType exportType = AssetBundleExportType.Asset;
        /// <summary>
        /// 保存地址
        /// </summary>
        public string bundleSavePath;
        /// <summary>
        /// BundleName 如:3975040047.ab
        /// </summary>
        public string bundleName;
        /// <summary>
        /// 短名：mobilediffuse.shader
        /// </summary>
        public string bundleShortName;

        public int level = -1;
        public List<AssetTarget> levelList;

        //目标文件是否已改变
        private bool _isFileChanged = false;
        //是否已分析过依赖
        private bool _isAnalyzed = false;
        //依赖树是否改变（用于增量打包）
        private bool _isDepTreeChanged = false;
        //上次打包的信息（用于增量打包）
        private AssetCacheInfo _cacheInfo;
        //.meta 文件的Hash
        private string _metaHash;
        //上次打好的AB的CRC值（用于增量打包）
        private string _bundleCrc;
        //是否是新打包的
        private bool _isNewBuild;
        /// <summary>
        /// 我要依赖的项
        /// </summary>
        private HashSet<AssetTarget> _dependencies = new HashSet<AssetTarget>();
        /// <summary>
        /// 依赖我的项
        /// </summary>
        private HashSet<AssetTarget> _dependsChildren = new HashSet<AssetTarget>();


        public AssetTarget(Object o, FileInfo f, string ap)
        {
            asset = o;
            file = f;
            assetPath = ap;
            bundleShortName = file.Name.ToLower();
            bundleName = XCommon.singleton.XHash(AssetBundleUtil.ConvertToABName(assetPath)) + ".ab";
            bundleSavePath = Path.Combine(AssetBundleUtil.GetBundleDir(), bundleName);
            _isFileChanged = true;
            _metaHash = "0";
        }


        public void Analyze()
        {
            if (_isAnalyzed) return;
            _isAnalyzed = true;
            _cacheInfo = AssetBundleUtil.GetCacheInfo(assetPath);
            _isFileChanged = _cacheInfo == null || !_cacheInfo.fileHash.Equals(GetHash()) || !_cacheInfo.metaHash.Equals(_metaHash);
            if (_cacheInfo != null)
            {
                _bundleCrc = _cacheInfo.bundleCrc;
                if (_isFileChanged) Debug.Log("File was changed : " + assetPath);
            }

            Object[] deps = EditorUtility.CollectDependencies(new Object[] { asset });
            var res = from s in deps
                      let path = AssetDatabase.GetAssetPath(s)
                      where !path.StartsWith("Assets/Resources") && !(s is MonoScript)
                      select path;

            var paths = res.Distinct().ToArray();
            for (int i = 0; i < paths.Length; i++)
            {
                if (!File.Exists(paths[i]) && !paths[i].Contains("unity_builtin_extra"))
                {
                    Debug.Log("invalid:" + paths[i]);
                    continue;
                }
                FileInfo fi = new FileInfo(paths[i]);
                AssetTarget target = AssetBundleUtil.Load(fi);
                if (target == null) continue;

                AddDepend(target);
                target.Analyze();
            }
        }

        public void Merge()
        {
            if (NeedExportStandalone())
            {
                var children = new List<AssetTarget>(_dependsChildren);
                RemoveDependsChildren();
                foreach (AssetTarget child in children)
                {
                    child.AddDepend(this);
                }
            }
        }


        private bool beforeExportProcess;

        /// <summary>
        /// 在导出之前执行 计算被多个素材依赖
        /// </summary>
        public void BeforeExport()
        {
            if (beforeExportProcess) return;
            beforeExportProcess = true;

            foreach (AssetTarget item in _dependsChildren)
            {
                item.BeforeExport();
            }
            if (exportType == AssetBundleExportType.Asset)
            {
                HashSet<AssetTarget> rootSet = new HashSet<AssetTarget>();
                GetRoot(rootSet);
                if (rootSet.Count > 1) exportType = AssetBundleExportType.Standalone;
            }
        }


        private void GetRoot(HashSet<AssetTarget> rootSet)
        {
            switch (exportType)
            {
                case AssetBundleExportType.Standalone:
                case AssetBundleExportType.Root:
                    rootSet.Add(this);
                    break;
                default:
                    foreach (AssetTarget item in _dependsChildren)
                    {
                        item.GetRoot(rootSet);
                    }
                    break;
            }
        }

        /// <summary>
        /// 获取所有依赖项
        /// </summary>
        public void GetDependencies(HashSet<AssetTarget> list)
        {
            var ie = _dependencies.GetEnumerator();
            while (ie.MoveNext())
            {
                AssetTarget target = ie.Current;
                if (target.needSelfExport)
                {
                    list.Add(target);
                }
                else
                {
                    target.GetDependencies(list);
                }
            }
        }


        public AssetBundleExportType compositeType
        {
            get
            {
                AssetBundleExportType type = exportType;
                if (type == AssetBundleExportType.Root && _dependsChildren.Count > 0)
                    type |= AssetBundleExportType.Asset;
                return type;
            }
        }


        public string bundleCrc
        {
            get { return _bundleCrc; }
            set
            {
                _isNewBuild = value != _bundleCrc;
                if (_isNewBuild) Debug.Log("Export AB : " + bundleShortName);
                _bundleCrc = value;
            }
        }

        /// <summary>
        /// 是不是需要重编
        /// </summary>
        public bool needRebuild
        {
            get
            {
                if (_isFileChanged || _isDepTreeChanged) return true;
                foreach (AssetTarget child in _dependsChildren)
                {
                    if (child.needRebuild) return true;
                }
                return false;
            }
        }


        /// <summary>
        /// 是不是自身的原因需要导出如指定的类型prefab等，有些情况下是因为依赖树原因需要单独导出
        /// </summary>
        public bool needSelfExport
        {
            get
            {
                if (type == AssetType.Builtin) return false;
                bool v = exportType == AssetBundleExportType.Root || exportType == AssetBundleExportType.Standalone;
                return v;
            }
        }

        /// <summary>
        /// (作为AssetType.Asset时)是否需要单独导出
        /// </summary>
        private bool NeedExportStandalone()
        {
            return _dependsChildren.Count > 1;
        }

        /// <summary>
        /// 增加依赖项
        /// </summary>
        /// <param name="target"></param>
        private void AddDepend(AssetTarget target)
        {
            if (target == this || ContainsDepend(target)) return;
            _dependencies.Add(target);
            target.AddDependChild(this);
            ClearParentDepend(target);
        }

        /// <summary>
        /// 是否已经包含了这个依赖（检查子子孙孙）
        /// </summary>
        private bool ContainsDepend(AssetTarget target, bool recursive = true)
        {
            if (_dependencies.Contains(target)) return true;
            if (recursive)
            {
                var e = _dependencies.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.ContainsDepend(target, true)) return true;
                }
            }
            return false;
        }


        private void AddDependChild(AssetTarget parent)
        {
            _dependsChildren.Add(parent);
        }

        /// <summary>
        /// 我依赖了这个项，那么依赖我的项不需要直接依赖这个项了
        /// </summary>
        private void ClearParentDepend(AssetTarget target = null)
        {
            IEnumerable<AssetTarget> cols = _dependencies;
            if (target != null) cols = new AssetTarget[] { target };
            foreach (AssetTarget at in cols)
            {
                var e = _dependsChildren.GetEnumerator();
                while (e.MoveNext())
                {
                    AssetTarget dc = e.Current;
                    dc.RemoveDepend(at);
                }
            }
        }

        /// <summary>
        /// 移除依赖项
        /// </summary>
        /// <param name="target"></param>
        /// <param name="recursive"></param>
        private void RemoveDepend(AssetTarget target, bool recursive = true)
        {
            _dependencies.Remove(target);
            target._dependsChildren.Remove(this);

            //recursive
            var dcc = new HashSet<AssetTarget>(_dependsChildren);
            var e = dcc.GetEnumerator();
            while (e.MoveNext())
            {
                AssetTarget dc = e.Current;
                dc.RemoveDepend(target);
            }
        }

        private void RemoveDependsChildren()
        {
            var all = new List<AssetTarget>(_dependsChildren);
            _dependsChildren.Clear();
            foreach (AssetTarget child in all)
            {
                child._dependencies.Remove(this);
            }
        }

        int System.IComparable<AssetTarget>.CompareTo(AssetTarget other)
        {
            return other._dependsChildren.Count.CompareTo(_dependsChildren.Count);
        }

        public string GetHash()
        {
            if (type == AssetType.Builtin)
                return "0000000000";
            else
                return AssetBundleUtil.GetFileHash(file.FullName);
        }

        public void WriteCache(StreamWriter sw)
        {
            sw.WriteLine(this.assetPath);
            sw.WriteLine(GetHash());
            sw.WriteLine(_metaHash);
            sw.WriteLine(this._bundleCrc);
            HashSet<AssetTarget> deps = new HashSet<AssetTarget>();
            GetDependencies(deps);
            sw.WriteLine(deps.Count.ToString());
            foreach (AssetTarget at in deps)
            {
                sw.WriteLine(at.assetPath);
            }
            sw.WriteLine("***************************************");
        }
    }
}