using System;
using System.Collections.Generic;
using System.IO;


public class AssetBundleData
{
    public string shortName;
    public string loadName;
    public uint hash;
    public string crc;
    public string assetpath;
    public AssetBundleExportType compositeType;
    public uint[] dependencies;
    public bool isAnalyzed;
    public AssetBundleData[] dependList;
}



/// <summary>
/// 文本文件格式说明
/// *固定一行字符串ABDT
/// 循环 { AssetBundleData
///     *名字(string)
///     *短名字(string)
///     *Hash值(string)
///     *类型(AssetBundleExportType)
///     *依赖文件个数M(int)
///     循环 M {
///         *依赖的AB文件名(string)
///     }
/// }
/// </summary>
public class AssetBundleDataReader
{
    public Dictionary<uint, AssetBundleData> infoMap = new Dictionary<uint, AssetBundleData>();

    protected Dictionary<string, uint> assetpath2hash = new Dictionary<string, uint>();

    public virtual void Read(Stream fs)
    {
        StreamReader sr = new StreamReader(fs);
        char[] fileHeadChars = new char[6];
        sr.Read(fileHeadChars, 0, fileHeadChars.Length);
        //读取文件头判断文件类型，ABDT 意思即 Asset-Bundle-Data-Text
        if (fileHeadChars[0] != 'A' || fileHeadChars[1] != 'B' || fileHeadChars[2] != 'D' || fileHeadChars[3] != 'T')
            return;

        while (true)
        {
            string assetpath = sr.ReadLine();
            if (string.IsNullOrEmpty(assetpath))
                break;

            uint _hash = uint.Parse(sr.ReadLine().Replace(".ab", ""));
            string _short = sr.ReadLine();
            string _crc = sr.ReadLine();
            int _type = Convert.ToInt32(sr.ReadLine());
            int depsCount = Convert.ToInt32(sr.ReadLine());
            uint[] deps = new uint[depsCount];

            if (!assetpath2hash.ContainsKey(assetpath))
                assetpath2hash.Add(assetpath, _hash);
            for (int i = 0; i < depsCount; i++)
            {
                deps[i] = uint.Parse(sr.ReadLine().Replace(".ab", ""));
            }
            sr.ReadLine(); // skip <------------->

            AssetBundleData info = new AssetBundleData();
            info.assetpath = assetpath;
            info.crc = _crc;
            info.hash = _hash;
            info.shortName = _short;
            int index = _short.LastIndexOf(".");
            info.loadName = _short.Substring(0, index);
            info.dependencies = deps;
            info.compositeType = (AssetBundleExportType)_type;
            infoMap[_hash] = info;
        }
        sr.Close();
    }

    /// <summary>
    /// 分析生成依赖树
    /// </summary>
    public void Analyze()
    {
        var e = infoMap.GetEnumerator();
        while (e.MoveNext())
        {
            Analyze(e.Current.Value);
        }
    }

    void Analyze(AssetBundleData abd)
    {
        if (!abd.isAnalyzed)
        {
            abd.isAnalyzed = true;
            abd.dependList = new AssetBundleData[abd.dependencies.Length];
            for (int i = 0; i < abd.dependencies.Length; i++)
            {
                AssetBundleData dep = GetAssetBundleInfo(abd.dependencies[i]);
                abd.dependList[i] = dep;
                Analyze(dep);
            }
        }
    }

    public uint GetFullName(string shortName)
    {
        uint fullName = 0;
        assetpath2hash.TryGetValue(shortName, out fullName);
        return fullName;
    }

    public AssetBundleData GetAssetBundleInfoByAssetpath(string assetpath)
    {
        uint fullName = GetFullName(assetpath);
        if (fullName != 0 && infoMap.ContainsKey(fullName))
            return infoMap[fullName];
        return null;
    }

    public AssetBundleData GetAssetBundleInfo(uint fullName)
    {
        if (fullName != 0)
        {
            if (infoMap.ContainsKey(fullName))
                return infoMap[fullName];
        }
        return null;
    }
}


