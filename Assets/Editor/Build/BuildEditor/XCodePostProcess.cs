using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;

public static class XCodePostProcess
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
        {
            Debug.LogWarning("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        XCProject project = new XCProject(path);
        string basePath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
        string file = basePath + "/Mods/game.projmods";
        project.ApplyMod(file);
        project.Save();
    }

}