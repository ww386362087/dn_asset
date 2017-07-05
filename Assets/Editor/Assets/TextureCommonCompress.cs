using UnityEngine;
using System.Collections;
using UnityEditor;
using XEditor;
using System.IO;


namespace XEditor
{
    public class TextureCommonCompress : EditorWindow
    {

        public enum ETextureSize
        {
            X32 = 32,
            X64 = 64,
            X128 = 128,
            X256 = 256,
            X512 = 512,
            X1024 = 1024,
        }

        public enum ETextureCompress
        {
            ECompress,
            E16,
            E32,
            EA8
        }


        protected ETextureSize compressSize = ETextureSize.X64;
        protected TextureImporterFormat iosFormat = TextureImporterFormat.PVRTC_RGB4;
        protected TextureImporterFormat androidFormat = TextureImporterFormat.ETC_RGB4;
        protected ETextureCompress compressType = ETextureCompress.ECompress;
        protected bool genMipmap = false;
        protected TextureWrapMode wrapMode = TextureWrapMode.Repeat;
        protected bool genAlpha = false;
        protected bool genRAlpha = true;
        protected ETextureSize alphaSize = ETextureSize.X64;

        private bool _TextureCompress(Texture2D tex, TextureImporter textureImporter, string path)
        {
            textureImporter.textureType = TextureImporterType.Advanced;
            textureImporter.anisoLevel = 0;
            textureImporter.filterMode = FilterMode.Bilinear;
            textureImporter.isReadable = false;
            textureImporter.wrapMode = wrapMode;
            textureImporter.mipmapEnabled = genMipmap;
            switch (compressType)
            {
                case ETextureCompress.ECompress:
                    iosFormat = TextureImporterFormat.PVRTC_RGB4;
                    androidFormat = TextureImporterFormat.ETC_RGB4;
                    break;
                case ETextureCompress.E16:
                    if (textureImporter.DoesSourceTextureHaveAlpha())
                    {
                        iosFormat = TextureImporterFormat.RGBA32;
                        androidFormat = TextureImporterFormat.RGBA16;
                    }
                    else
                    {
                        iosFormat = TextureImporterFormat.RGB24;
                        androidFormat = TextureImporterFormat.RGB16;
                    }
                    break;
                case ETextureCompress.E32:
                    if (textureImporter.DoesSourceTextureHaveAlpha())
                    {
                        iosFormat = TextureImporterFormat.RGBA32;
                        androidFormat = TextureImporterFormat.RGBA32;
                    }
                    else
                    {
                        iosFormat = TextureImporterFormat.RGB24;
                        androidFormat = TextureImporterFormat.RGB24;
                    }
                    break;
                case ETextureCompress.EA8:
                    iosFormat = TextureImporterFormat.Alpha8;
                    androidFormat = TextureImporterFormat.Alpha8;
                    break;
            }

            textureImporter.SetPlatformTextureSettings("Standalone", (int)compressSize, genAlpha ? TextureImporterFormat.DXT5 : TextureImporterFormat.DXT1);

            if (genAlpha)
            {
                int extIndex = path.LastIndexOf(".");
                if (extIndex >= 0)
                {
                    string alphaTexPath = path.Substring(0, extIndex) + "_A.png";

                    if (genRAlpha)
                    {
                        textureImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), (int)compressSize, TextureImporterFormat.RGBA32);
                        textureImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), (int)compressSize, TextureImporterFormat.RGBA32);
                        textureImporter.isReadable = true;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        TextureModify.MakeAlphaRTex(alphaTexPath, (int)alphaSize, tex);
                        textureImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), (int)compressSize, iosFormat);
                        textureImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), (int)compressSize, androidFormat);
                        textureImporter.isReadable = false;
                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    }
                    else
                    {
                        File.Copy(path, alphaTexPath, true);
                        AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
                        TextureImporter alphaTextureImporter = AssetImporter.GetAtPath(alphaTexPath) as TextureImporter;
                        if (alphaTextureImporter != null)
                        {
                            alphaTextureImporter.textureType = TextureImporterType.Advanced;
                            alphaTextureImporter.anisoLevel = 0;
                            alphaTextureImporter.mipmapEnabled = false;
                            alphaTextureImporter.isReadable = false;
                            alphaTextureImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                            alphaTextureImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), (int)alphaSize, TextureImporterFormat.Alpha8);
                            alphaTextureImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), (int)alphaSize, TextureImporterFormat.Alpha8);
                            AssetDatabase.ImportAsset(alphaTexPath, ImportAssetOptions.ForceUpdate);
                        }
                    }
                }
            }
            return true;

        }
        private void Compress()
        {
            TextureModify.EnumTextures(_TextureCompress, "TextureCompress");
        }
        private void OnGUI()
        {
            //GUILayout.BeginHorizontal();

            if (GUILayout.Button("Compress", GUILayout.MaxWidth(150)))
            {
                Compress();
            }

            compressSize = (ETextureSize)EditorGUILayout.EnumPopup("缩放", compressSize);
            compressType = (ETextureCompress)EditorGUILayout.EnumPopup("压缩格式", compressType);
            wrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("采样模式", wrapMode);
            genMipmap = EditorGUILayout.ToggleLeft("GenMipmap", genMipmap);
            genAlpha = EditorGUILayout.ToggleLeft("GenAlpha", genAlpha);
            if (genAlpha)
            {
                genRAlpha = EditorGUILayout.ToggleLeft("Gen R Channel Alpha", genRAlpha);
                alphaSize = (ETextureSize)EditorGUILayout.EnumPopup("alpha缩放", alphaSize);
            }
        }
    }

}