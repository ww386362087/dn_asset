using System;
using System.Collections.Generic;
using System.IO;

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
            if (fs.Position == fs.Length) break;
            string assetpath = sr.ReadString();
            uint hash = names[sr.ReadInt32()];
            string _short = sr.ReadString();
            string _crc = sr.ReadString();
            int _type = sr.ReadInt32();
            int depsCount = sr.ReadInt32();
            uint[] deps = new uint[depsCount];

            if (!assetpath2hash.ContainsKey(_short))
                assetpath2hash.Add(_short, hash);
            for (int i = 0; i < depsCount; i++)
            {
                deps[i] = names[sr.ReadInt32()];
            }

            AssetBundleData info = new AssetBundleData();
            info.crc = _crc;
            info.hash = hash;
            info.shortName = _short;
            int index = _short.LastIndexOf(".");
            info.loadName = _short.Substring(0,index);
            info.assetpath = assetpath;
            info.dependencies = deps;
            info.compositeType = (AssetBundleExportType)_type;
            infoMap[hash] = info;
        }
        sr.Close();
    }
}