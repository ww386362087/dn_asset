using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ABManager
{

    private Action _initCallback;

    private AssetBundleDataReader _depInfoReader;

    void LoadDepInfo()
    {
        string depFile = string.Format("{0}/{1}", AssetBundlePathResolver.BundleCacheDir, AssetBundlePathResolver.DependFileName);

        if (File.Exists(depFile))
        {
            FileStream fs = new FileStream(depFile, FileMode.Open, FileAccess.Read);
            Init(fs, null);
            fs.Close();
        }
        else
        {
            TextAsset o = Resources.Load<TextAsset>("dep");

            if (o != null)
            {
                Init(new MemoryStream(o.bytes), null);
            }
            else
            {
                Debug.LogError("depFile not exist!");
            }
        }

        this.InitComplete();
    }

    public void Init(Action callback)
    {
        _initCallback = callback;

        LoadDepInfo();
    }

    public void Init(Stream depStream, Action callback)
    {
        if (depStream.Length > 4)
        {
            BinaryReader br = new BinaryReader(depStream);
            if (br.ReadChar() == 'A' && br.ReadChar() == 'B' && br.ReadChar() == 'D')
            {
                if (br.ReadChar() == 'T')
                    _depInfoReader = new AssetBundleDataReader();
                else
                    _depInfoReader = new AssetBundleDataBinaryReader();

                depStream.Position = 0;
                _depInfoReader.Read(depStream);
            }
        }

        depStream.Close();

        if (callback != null)
            callback();
    }

    void InitComplete()
    {
        if (_initCallback != null)
            _initCallback();
        _initCallback = null;
    }
}
