using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using ABSystem;

public class XBuild : EditorWindow
{
    static bool _init = true;
    static string _targetDir = "";
    static string _macro = "TEST;";
    static BuildTarget _target;
    static BuildOptions _build = BuildOptions.None;
    static string _identifier = "com.yunstudio.dnasset";
    static string _product = "谷基";
    static string _version = "0.0.0";
    static TPlatform _platform = TPlatform.None;
    static TPlatform _lastPlatform = TPlatform.None;
    static string[] _scenes = null;

    static bool isRelease = true;

    public static string Macro
    {
        get
        {
            string str = "TEST";
            string path = typeof(AssetDatabase).Module.FullyQualifiedName;
            string backup = path.Replace("UnityEditor.dll", "UnityEditor-backup.dll");
            if (File.Exists(backup)) str += ";Inject";
            return str;
        }
    }

    enum TPlatform
    {
        None,
        Win32,
        iOS,
        Android,
    }

    [MenuItem("XBuild/Build/IOS")]
    public static void BuildIOS()
    {
        SwitchPlatForm(TPlatform.iOS);
        Build();
        AssetDatabase.Refresh();
    }


    [MenuItem("XBuild/Build/Android")]
    public static void BuildAndroid()
    {
        SwitchPlatForm(TPlatform.Android);
        Build();
        AssetDatabase.Refresh();
    }


    [MenuItem("XBuild/Build/Win32")]
    public static void BuildWin32()
    {
        SwitchPlatForm(TPlatform.Win32);
        Build();
        AssetDatabase.Refresh();
    }

    [MenuItem(@"XBuild/BuildPanel")]
    private static void BuildWindow()
    {
        EditorWindow.GetWindow(typeof(XBuild));
    }

    void OnGUI()
    {
        if (_init) InitPlatform();
        _lastPlatform = _platform;
        _platform = (TPlatform)EditorGUILayout.EnumPopup("Platform Type", _platform);
        if (_lastPlatform != _platform)
        {
            if (EditorUtility.DisplayDialog("Switch Platform Or Not",
                        "Do you want to Switch Platform?",
                        "Yes", "No"))
            {
                SwitchPlatForm(_platform);
            }
            else
            {
                _platform = _lastPlatform;
            }
        }

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Product Name: ");
        GUILayout.Label(_product);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bundle Identifier: ");
        GUILayout.Label(_identifier);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Bundle Version: ");
        GUILayout.Label(_version);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("=========================ReleaseOption=========================");
        EditorGUILayout.BeginHorizontal();
        isRelease = EditorGUILayout.ToggleLeft("Is Release", isRelease);
        EditorGUILayout.EndHorizontal();

        _build = BuildOptions.None;
        if (_platform != TPlatform.None)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("BuildBundle", GUILayout.MaxWidth(80)))
            {
                if (EditorUtility.DisplayDialog("AB Build Or Not",
                        "Do you want to Build AssetsBundle)?",
                        "Yes", "No"))
                {
                    AssetBundleBuildPanel.BuildAssetBundles();
                }
            }
            EditorGUILayout.Separator();
            if (GUILayout.Button("Build", GUILayout.MaxWidth(80)))
            {
                if (EditorUtility.DisplayDialog("Build Or Not",
                        "Do you want to Build?",
                        "Yes", "No"))
                {
                    SwitchPlatForm(_platform);
                    Build();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }
    }

    private static void PlayerSetting_Common()
    {
        PlayerSettings.companyName = "yunstudio";
        PlayerSettings.productName = _product;
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.useAnimatedAutorotation = true;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.bundleIdentifier = _identifier;
        PlayerSettings.bundleVersion = _version;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    private static void PlayerSetting_Win32()
    {
        _target = BuildTarget.StandaloneWindows;
        EditorUserBuildSettings.SwitchActiveBuildTarget(_target);
        PlayerSetting_Common();
        PlayerSettings.defaultIsFullScreen = false;
        PlayerSettings.defaultScreenWidth = 1136;
        PlayerSettings.defaultScreenHeight = 640;
        _targetDir = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Win32");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, _macro);
        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0;
        PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
    }


    private static void PlayerSetting_iOS()
    {
        _target = BuildTarget.iOS;
        EditorUserBuildSettings.SwitchActiveBuildTarget(_target);
        PlayerSetting_Common();
        _targetDir = Path.Combine(Application.dataPath.Replace("/Assets", ""), "IOS");
        PlayerSettings.iOS.buildNumber = _version;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.accelerometerFrequency = 0;
        PlayerSettings.iOS.locationUsageDescription = "";
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, _macro);
        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
        PlayerSettings.aotOptions = "nrgctx-trampolines=4096,nimt-trampolines=4096,ntrampolines=4096";
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetOSVersionString = "7.1";
        PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
        PlayerSettings.iOS.scriptCallOptimization = ScriptCallOptimizationLevel.FastButNoExceptions;
    }

    private static void PlayerSetting_Android()
    {
        _target = BuildTarget.Android;
        EditorUserBuildSettings.SwitchActiveBuildTarget(_target);
        PlayerSetting_Common();
        _targetDir = Path.Combine(Application.dataPath.Replace("/Assets", ""), "Android");
        int bundleVersionCode = int.Parse(System.DateTime.Now.ToString("yyMMdd"));
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel14;
        PlayerSettings.Android.targetDevice = AndroidTargetDevice.FAT;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;
        PlayerSettings.Android.forceSDCardPermission = true;
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, _macro);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
        PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
        PlayerSettings.strippingLevel = StrippingLevel.Disabled;
        PlayerSettings.Android.keystoreName = Application.dataPath + "/Editor/Build/yunstudio.keystore";
        PlayerSettings.Android.keystorePass = "XCvis8RGbw";
        PlayerSettings.Android.keyaliasName = "yunstudio";
        PlayerSettings.Android.keyaliasPass = "XCvis8RGbw";
        PlayerSettings.Android.splashScreenScale = AndroidSplashScreenScale.ScaleToFill;
    }


    private static void Build()
    {
        _scenes = FindEnabledEditorScenes();
        EditorUserBuildSettings.SwitchActiveBuildTarget(_target);
        if (Directory.Exists(_targetDir))
        {
            Directory.Delete(_targetDir, true);
        }
        Directory.CreateDirectory(_targetDir);

        BeforeBuild(EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        string lastName = "";
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) lastName = ".apk";
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows) lastName = ".exe";
        string dest = Path.Combine(_targetDir, "dnasset" + lastName);
        string res = BuildPipeline.BuildPlayer(_scenes, dest, _target, _build);

        AfterBuild(EditorUserBuildSettings.activeBuildTarget);
        string macro = Macro;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, macro);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, macro);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, macro);

        EditorUtility.DisplayDialog("Package Build Finish", "Package Build Finish!(" + res + ")", "OK");
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.iOS)
        {
            System.Diagnostics.Process.Start(_targetDir);
        }
    }


    private static string[] FindEnabledEditorScenes()
    {
        List<string> EditorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled && !EditorScenes.Contains(scene.path))
                EditorScenes.Add(scene.path);
        }
        return EditorScenes.ToArray();
    }


    private static void SwitchPlatForm(TPlatform platformType)
    {
        SetScriptDefine();
        switch (platformType)
        {
            case TPlatform.Win32:
                PlayerSetting_Win32();
                break;
            case TPlatform.iOS:
                PlayerSetting_iOS();
                break;
            case TPlatform.Android:
                PlayerSetting_Android();
                break;
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void InitPlatform()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.iOS:
                _platform = TPlatform.iOS;
                break;
            case BuildTarget.Android:
                _platform = TPlatform.Android;
                break;
            case BuildTarget.StandaloneWindows:
                _platform = TPlatform.Win32;
                break;
            default:
                _platform = TPlatform.None;
                break;
        }
        _init = false;
    }

    private static void SetScriptDefine()
    {
        string res = isRelease ? "Release" : "Debug";
        _macro += res;
    }

    static private bool BeforeBuild(BuildTarget target)
    {
        //TO-DO Before Build

        return true;
    }


    static private bool AfterBuild(BuildTarget target)
    {
        //TO-DO After Build

        return true;
    }

}
