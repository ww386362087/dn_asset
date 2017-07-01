using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using XEditor;


public enum ETexChanel
{
    R,
    G,
    B,
    A
}


public struct TexureImportInfo
{
    public TextureImporterFormat format;
    public int texSize;
    public TextureWrapMode wrapMode;
    public TextureImporter textureImporter;
    public string path;
}



public class TextureCombine : EditorWindow
{
    private Texture2D[] texs = new Texture2D[4];
    private ETexChanel[] texChanels = new ETexChanel[4];
    private TexureImportInfo[] importInfo = new TexureImportInfo[4];
    private TextureImporterFormat desAndroidFormat = TextureImporterFormat.RGBA16;
    private TextureImporterFormat desIOSFormat = TextureImporterFormat.RGBA16;
    private int width = 256;
    private int height = 256;
    private string namepath = "";
    private Vector2 scrollPosition = Vector2.zero;

    private float GetChanel(Texture2D tex, ETexChanel chanel, int x, int y)
    {
        if (tex == null)
            return 0.0f;
        Color c = tex.GetPixel(x, y);
        if (chanel == ETexChanel.R)
            return c.r;
        if (chanel == ETexChanel.G)
            return c.g;
        if (chanel == ETexChanel.B)
            return c.b;
        if (chanel == ETexChanel.A)
            return c.a;
        return 0.0f;
    }

    private void Combine()
    {
        if (namepath != "")
        {
            Texture2D[] tmpTex = new Texture2D[4];
            for (int i = 0, imax = texs.Length; i < imax; ++i)
            {
                Texture2D tex = texs[i];
                if (tex != null)
                {
                    string path = AssetDatabase.GetAssetPath(tex);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (textureImporter != null)
                    {
                        TexureImportInfo tii;

                        tii.wrapMode = textureImporter.wrapMode;
                        tii.textureImporter = textureImporter;
                        tii.path = path;
                        int texSize = 1024;
                        TextureImporterFormat format;
                        textureImporter.wrapMode = TextureWrapMode.Clamp;
                        textureImporter.GetPlatformTextureSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), out texSize, out format);
                        textureImporter.SetPlatformTextureSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), texSize, TextureImporterFormat.RGBA16);

                        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        tii.format = format;
                        tii.texSize = texSize;
                        importInfo[i] = tii;
                    }
                }
            }
            Material mat = new Material(Shader.Find("Custom/Effect/CombineTex"));
            for (int i = 0, imax = texs.Length; i < imax; ++i)
            {
                mat.SetTexture("_Tex" + i.ToString(), texs[i]);
                ETexChanel c = texChanels[i];
                Vector4 chanelMask = Vector4.zero;
                if (c == ETexChanel.R)
                {
                    chanelMask = new Vector4(1, 0, 0, 0);
                }
                else if (c == ETexChanel.G)
                {
                    chanelMask = new Vector4(0, 1, 0, 0);
                }
                else if (c == ETexChanel.B)
                {
                    chanelMask = new Vector4(0, 0, 1, 0);
                }
                else if (c == ETexChanel.A)
                {
                    chanelMask = new Vector4(0, 0, 0, 1);
                }
                mat.SetVector("_ChanelMask" + i.ToString(), chanelMask);
            }
            RenderTexture current = RenderTexture.active;
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            Graphics.SetRenderTarget(rt);
            // Set up the simple Matrix
            GL.PushMatrix();
            GL.LoadOrtho();
            mat.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
            GL.End();
            GL.PopMatrix();

            Texture2D des = new Texture2D(width, height);
            des.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            Graphics.SetRenderTarget(current);
            rt.Release();
            GameObject.DestroyImmediate(mat);
            byte[] bytes = des.EncodeToPNG();

            File.WriteAllBytes(namepath, bytes);
            AssetDatabase.ImportAsset(namepath, ImportAssetOptions.ForceUpdate);

            TextureImporter desTexImporter = AssetImporter.GetAtPath(namepath) as TextureImporter;
            if (desTexImporter != null)
            {
                int size = width > height ? width : height;
                desTexImporter.textureType = TextureImporterType.Advanced;
                desTexImporter.anisoLevel = 0;
                desTexImporter.mipmapEnabled = false;
                desTexImporter.isReadable = false;
                desTexImporter.npotScale = TextureImporterNPOTScale.ToNearest;
                desTexImporter.SetPlatformTextureSettings(BuildTarget.Android.ToString(), size, desAndroidFormat);
                desTexImporter.SetPlatformTextureSettings(BuildTarget.iPhone.ToString(), size, desIOSFormat);
                AssetDatabase.ImportAsset(namepath, ImportAssetOptions.ForceUpdate);
            }
            for (int i = 0, imax = texs.Length; i < imax; ++i)
            {
                Texture2D tex = texs[i];
                if (tex != null)
                {
                    TexureImportInfo tii = importInfo[i];
                    tii.textureImporter.SetPlatformTextureSettings(EditorUserBuildSettings.activeBuildTarget.ToString(), tii.texSize, tii.format);
                    tii.textureImporter.wrapMode = tii.wrapMode;
                    AssetDatabase.ImportAsset(tii.path, ImportAssetOptions.ForceUpdate);
                }
            }
            EditorUtility.DisplayDialog("Finish", "Combine Finish", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Error", "Empty file path", "OK");
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Combine", GUILayout.MaxWidth(150)))
        {
            Combine();
        }
        GUILayout.EndHorizontal();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
        for (int i = 0, imax = texs.Length; i < imax; ++i)
        {
            Texture2D tex = texs[i];
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tex != null ? tex.name : "Empty");
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            ETexChanel c = (ETexChanel)i;

            texs[i] = EditorGUILayout.ObjectField("Chanel" + c, tex, typeof(Texture2D), true, GUILayout.MaxWidth(250)) as Texture2D;
            if (texs[i] != null && namepath == "")
            {
                namepath = AssetDatabase.GetAssetPath(texs[i]);
                int index = namepath.LastIndexOf(".");
                if (index > 0)
                    namepath = namepath.Substring(0, index);
                namepath += ".png";
            }
            texChanels[i] = (ETexChanel)EditorGUILayout.EnumPopup("通道", texChanels[i]);
            GUILayout.EndHorizontal();
        }
        GUILayout.BeginHorizontal();
        namepath = EditorGUILayout.TextField("Asset Path", namepath);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        width = EditorGUILayout.IntField("Width", width);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        height = EditorGUILayout.IntField("Height", height);
        GUILayout.EndHorizontal();
        EditorGUILayout.EndScrollView();

    }

    public static void ScaleTexture(Texture src, string despath, int width, int height, string shaderName)
    {
        Material mat = new Material(Shader.Find(shaderName));
        mat.mainTexture = src;
        RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        RenderTexture current = RenderTexture.active;
        Graphics.SetRenderTarget(rt);
        // Set up the simple Matrix
        GL.PushMatrix();
        GL.LoadOrtho();
        mat.SetPass(0);
        GL.Begin(GL.QUADS);
        GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
        GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
        GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
        GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
        GL.End();
        GL.PopMatrix();

        Texture2D des = new Texture2D(width, height);
        des.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        Graphics.SetRenderTarget(current);
        rt.Release();
        GameObject.DestroyImmediate(mat);
        if (File.Exists(despath))
        {
            File.Delete(despath);
            AssetDatabase.Refresh();
        }
        byte[] bytes = des.EncodeToPNG();

        File.WriteAllBytes(despath, bytes);
        AssetDatabase.ImportAsset(despath, ImportAssetOptions.ForceUpdate);
    }
}
