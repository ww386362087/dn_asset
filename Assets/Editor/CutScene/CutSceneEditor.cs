using UnityEditor;
using UnityEngine;

namespace XEditor
{

    public class CutSceneEditor
    {
        [MenuItem(@"XEditor/Cut Scene")]
        static void CutScene()
        {
            EditorWindow.GetWindowWithRect(typeof(CutSceneWindow), new Rect(0, 0, 600, 800), true, @"CutScene");
        }
    }

}