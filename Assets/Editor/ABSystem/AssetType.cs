using System;

namespace ABSystem
{
    public enum AssetType
    {
        Asset,
        Builtin
    }



    class AssetCacheInfo
    {
        /// <summary>
        /// 源文件的hash，比较变化
        /// </summary>
        public string fileHash;
        /// <summary>
        /// 源文件meta文件的hash，部分类型的素材需要结合这个来判断变化
        /// 如：Texture
        /// </summary>
        public string metaHash;
        /// <summary>
        /// 上次打好的AB的CRC值，用于增量判断
        /// </summary>
        public string bundleCrc;
        /// <summary>
        /// 所依赖的那些文件
        /// </summary>
        public string[] depNames;
    }



    [Serializable]
    public class XMetaResPackage
    {
        //location in bundle
        public string download;
        //location
        public string buildinpath;
        //bundle id
        public string bundle;
        //Size in byte
        public uint Size;
    }

}