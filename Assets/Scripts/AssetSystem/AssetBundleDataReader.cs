using System;
using System.Collections.Generic;
using System.IO;


public class AssetBundleData
{
    public string shortName;
    public uint fullName;
    public string hash;
    public string debugName;
    public AssetBundleExportType compositeType;
    public uint[] dependencies;
    public bool isAnalyzed;
    public AssetBundleData[] dependList;
}

public enum AssetBundleExportType
{
    /// <summary>
    /// 普通素材，被根素材依赖的
    /// </summary>
    Asset = 1,
    /// <summary>
    /// 根
    /// </summary>
    Root = 1 << 1,
    /// <summary>
    /// 需要单独打包，说明这个素材是被两个或以上的素材依赖的
    /// </summary>
    Standalone = 1 << 2,
    /// <summary>
    /// 既是根又是被别人依赖的素材
    /// </summary>
    RootAsset = Asset | Root
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

    protected Dictionary<string, uint> shortName2FullName = new Dictionary<string, uint>();

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
            string debugName = sr.ReadLine();
            if (string.IsNullOrEmpty(debugName))
                break;

            uint name = uint.Parse(sr.ReadLine().Replace(".ab", ""));
            string shortFileName = sr.ReadLine();
            string hash = sr.ReadLine();
            int typeData = Convert.ToInt32(sr.ReadLine());
            int depsCount = Convert.ToInt32(sr.ReadLine());
            uint[] deps = new uint[depsCount];

            if (!shortName2FullName.ContainsKey(shortFileName))
                shortName2FullName.Add(shortFileName, name);
            for (int i = 0; i < depsCount; i++)
            {
                deps[i] = uint.Parse(sr.ReadLine().Replace(".ab", ""));
            }
            sr.ReadLine(); // skip <------------->

            AssetBundleData info = new AssetBundleData();
            info.debugName = debugName;
            info.hash = hash;
            info.fullName = name;
            info.shortName = shortFileName;
            info.dependencies = deps;
            info.compositeType = (AssetBundleExportType)typeData;
            infoMap[name] = info;
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
                AssetBundleData dep = this.GetAssetBundleInfo(abd.dependencies[i]);
                abd.dependList[i] = dep;
                this.Analyze(dep);
            }
        }
    }

    public uint GetFullName(string shortName)
    {
        uint fullName = 0;
        shortName2FullName.TryGetValue(shortName, out fullName);
        return fullName;
    }

    public AssetBundleData GetAssetBundleInfoByShortName(string shortName)
    {
        uint fullName = GetFullName(shortName);
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


/// <summary>
/// 二进制文件格式说明
/// *固定四个字节ABDB
/// *namesCount 字符串池中字符串的个数
/// 循环 namesCount {
///     *读取字符串到池中(string)
/// }
/// 循环 {
///     *名字在字符串池中的索引(int)
///     *短名字在字符串池中的索引(int)
///     *Hash在字符串池中的索引(int)
///     *类型(AssetBundleExportType)
///     *依赖文件个数M(int)
///     循环 M {
///         *依赖的AB文件名在字符串池中的索引(int)
///     }
/// }
/// </summary>
class AssetBundleDataBinaryReader : AssetBundleDataReader
{
    public override void Read(Stream fs)
    {
        if (fs.Length < 4) return;

        BinaryReader sr = new BinaryReader(fs);
        char[] fileHeadChars = sr.ReadChars(4);
        //读取文件头判断文件类型，ABDB 意思即 Asset-Bundle-Data-Binary
        if (fileHeadChars[0] != 'A' || fileHeadChars[1] != 'B' || fileHeadChars[2] != 'D' || fileHeadChars[3] != 'B')
            return;

        int namesCount = sr.ReadInt32();
        uint[] names = new uint[namesCount];
        for (int i = 0; i < namesCount; i++)
        {
            names[i] = uint.Parse(sr.ReadString().Replace(".ab", ""));
        }

        while (true)
        {
            if (fs.Position == fs.Length)
                break;

            string debugName = sr.ReadString();
            uint name = names[sr.ReadInt32()];
            string shortFileName = sr.ReadString();
            string hash = sr.ReadString();
            int typeData = sr.ReadInt32();
            int depsCount = sr.ReadInt32();
            uint[] deps = new uint[depsCount];

            if (!shortName2FullName.ContainsKey(shortFileName))
                shortName2FullName.Add(shortFileName, name);
            for (int i = 0; i < depsCount; i++)
            {
                deps[i] = names[sr.ReadInt32()];
            }

            AssetBundleData info = new AssetBundleData();
            info.hash = hash;
            info.fullName = name;
            info.shortName = shortFileName;
            info.debugName = debugName;
            info.dependencies = deps;
            info.compositeType = (AssetBundleExportType)typeData;
            infoMap[name] = info;
        }
        sr.Close();
    }
}