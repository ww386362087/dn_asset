using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEditor.Animations;

namespace XEditor
{
    public class AnimationEditor
    {
        private delegate bool EnumAnimatorCallback(AnimatorController controller, string path);
        private static void EnumAnimatorController(EnumAnimatorCallback cb, string title)
        {
            UnityEngine.Object[] objects = Selection.GetFiltered(typeof(AnimatorController), SelectionMode.DeepAssets);
            if (objects != null)
            {
                for (int i = 0; i < objects.Length; ++i)
                {
                    AnimatorController controller = objects[i] as AnimatorController;
                    string path = "";
                    if (controller != null)
                    {
                        path = AssetDatabase.GetAssetPath(controller);
                        if (cb != null) cb(controller, path);
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, objects.Length), path, (float)i / objects.Length);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All gameobjects processed finish", "OK");
        }

        private delegate void EnumAnimationCallback(AnimationClip clip, string path);
        private static void EnumAnimation(EnumAnimationCallback cb, string title)
        {
            UnityEngine.Object[] animationClips = Selection.GetFiltered(typeof(UnityEngine.AnimationClip), SelectionMode.DeepAssets);
            if (animationClips != null)
            {
                for (int i = 0; i < animationClips.Length; ++i)
                {
                    AnimationClip clip = animationClips[i] as AnimationClip;
                    string path = "";
                    if (clip != null)
                    {
                        path = AssetDatabase.GetAssetPath(clip);
                        if (cb != null) cb(clip, path);
                    }
                    EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, animationClips.Length), path, (float)i / animationClips.Length);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All objects processed finish", "OK");
        }

        private static bool _ConvertToLegacy(GameObject go, Animator animator, string path)
        {
            RuntimeAnimatorController rac = animator.runtimeAnimatorController;
            if (rac != null)
            {
                string controllerPath = AssetDatabase.GetAssetPath(rac);
                string clipPath = controllerPath.Replace(".controller", ".anim");
                AnimationClip clip = (AnimationClip)AssetDatabase.LoadAssetAtPath(clipPath, typeof(AnimationClip));
                if (clip != null)
                {
                    SerializedObject serializedObject = new SerializedObject(clip);
                    SerializedProperty animationType = serializedObject.FindProperty("m_AnimationType");
                    animationType.intValue = 1;
                    serializedObject.ApplyModifiedProperties();

                    EditorCurveBinding[] allCurves = AnimationUtility.GetCurveBindings(clip);
                    int processCount = 0;
                    foreach (var data in allCurves)
                    {
                        // XDebug.Log("path" + data.path);
                        // XDebug.Log("propertyName" + data.propertyName);
                        // XDebug.Log("type" + data.type);

                        if (data.type == typeof(UnityEngine.MeshRenderer))
                        {
                            AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, data);
                            EditorCurveBinding newBindings = new EditorCurveBinding();
                            newBindings.path = data.path;
                            newBindings.propertyName = data.propertyName.Replace("material.", "");
                            newBindings.type = typeof(UnityEngine.Material);
                            AnimationUtility.SetEditorCurve(clip, data, null);
                            AnimationUtility.SetEditorCurve(clip, newBindings, curve);
                            processCount++;
                        }
                    }
                    GameObject parent = animator.gameObject;
                    GameObject.DestroyImmediate(animator);
                    Animation ani = parent.AddComponent<Animation>();
                    ani.AddClip(clip, clip.name);
                    ani.clip = clip;
                }
                else
                {
                    XDebug.Log("clip not found:", controllerPath);
                }
            }
            return true;
        }


        [MenuItem(@"Assets/Tool/Animation/ClearTxt")]
        private static void ClearTxt()
        {
            DirectoryInfo di = new DirectoryInfo("Assets/Resources/Animation");
            FileInfo[] files = di.GetFiles("*.txt", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fi = files[i];
                fi.Delete();
            }
        }

        [MenuItem(@"Assets/Tool/Animation/CheckAnimator")]
        private static void CheckAnimator()
        {
            EnumAnimatorController(_CheckAnimator, "CheckAnimator");
        }

        private static bool _CheckAnimator(AnimatorController controller, string path)
        {
            if (controller.layers.Length == 1)
            {
                AnimatorControllerLayer layer = controller.layers[0];
                AnimatorStateMachine asm = layer.stateMachine;
                if (asm.states.Length == 1)
                {
                    AnimatorState state = asm.defaultState;
                    XDebug.Log("state name: ", state.name, " cotroll: ", controller.name);
                    if (state.name != controller.name)
                    {
                        XDebug.LogError(string.Format("Animator name error controller name:{0} state name:{1} path:{2}", controller.name, state.name, path));
                    }
                }
            }
            else
            {
                XDebug.LogError(string.Format("Not 1 layer:{0} Count:{1}", controller.name, controller.layers.Length));
            }
            return true;
        }


    }

}