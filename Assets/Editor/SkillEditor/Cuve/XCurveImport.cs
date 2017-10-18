using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;


public class XCurveImport
{

    [MenuItem("Assets/ImportCurve")]
    private static void Import()
    {
        Object[] objs = Selection.GetFiltered(typeof(XCurve), SelectionMode.DeepAssets);
        foreach (var obj in objs)
        {
            XCurve curve = obj as XCurve;
            Parse(curve);
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        XDebug.Log("import finish");
    }

    public static void Parse(XCurve curve)
    {
        string path = @"D:\u5\res\XProject\Assets\Resources\Curve\Monster_hopgoblin\" + curve.name + ".prefab";
        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            float Max_Value = 0f;
            float Land_Value = 0f;
            List<float> times = new List<float>();
            List<float> values = new List<float>();
            List<float> ins = new List<float>();
            List<float> outs = new List<float>();
            List<int> modes = new List<int>();
            for (int i = 0, max = lines.Length; i < max; i++)
            {
                if (lines[i].TrimStart().StartsWith("Max_Value:")) Max_Value = float.Parse(lines[i].Split(':')[1]);
                if (lines[i].TrimStart().StartsWith("Land_Value:")) Land_Value = float.Parse(lines[i].Split(':')[1]);
                if (lines[i].TrimStart().StartsWith("- time:")) times.Add(float.Parse(lines[i].Split(':')[1]));
                if (lines[i].TrimStart().StartsWith("value:")) values.Add(float.Parse(lines[i].Split(':')[1]));
                if (lines[i].TrimStart().StartsWith("inSlope:")) ins.Add(float.Parse(lines[i].Split(':')[1]));
                if (lines[i].TrimStart().StartsWith("outSlope:")) outs.Add(float.Parse(lines[i].Split(':')[1]));
                if (lines[i].TrimStart().StartsWith("tangentMode:")) modes.Add(int.Parse(lines[i].Split(':')[1]));
            }
            Debug.Log(curve.name + " max_value: " + Max_Value + " land_value:" + Land_Value + " times:" + times.Count+" values: "+values.Count);

            curve.Max_Value = Max_Value;
            curve.Land_Value = Land_Value;
            AnimationCurve ac = new AnimationCurve();
            for (int i = 0; i < times.Count; i++)
            {
                Keyframe frame = new Keyframe(times[i], values[i]);
                frame.tangentMode = modes[i];
                frame.inTangent = ins[i];
                frame.outTangent = outs[i];
                ac.AddKey(frame);
            }
            curve.Curve = ac;
        }
        else
        {
            XDebug.LogError(path);
        }
    }

}
