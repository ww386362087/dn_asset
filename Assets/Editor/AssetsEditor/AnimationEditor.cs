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
                        //Debug.Log("path" + data.path);
                        //Debug.Log("propertyName" + data.propertyName);
                        //Debug.Log("type" + data.type);

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
                    Debug.Log("clip not found:" + controllerPath);
                }
            }
            return true;
        }

        [MenuItem(@"Assets/Tool/Animation/ReduceKeyFrame")]
        private static void ReduceKeyFrame()
        {
            Object[] clips = Selection.GetFiltered(typeof(AnimationClip), SelectionMode.DeepAssets);
            if (clips != null)
            {
                for (int i = 0; i < clips.Length; ++i)
                {
                    AnimationClip clip = clips[i] as AnimationClip;
                    if (clip != null)
                    {
                        AnimationClipCurveData[] accds = AnimationUtility.GetAllCurves(clip, true);
                        foreach (AnimationClipCurveData accd in accds)
                        {
                            EditorCurveBinding ecb = new EditorCurveBinding();
                            ecb.path = accd.path;
                            ecb.propertyName = accd.propertyName;
                            ecb.type = accd.type;
                            AnimationCurve ac = AnimationUtility.GetEditorCurve(clip, ecb);
                            Keyframe[] keys = ac.keys;
                            if (keys.Length > 2)
                            {
                                Keyframe key0 = keys[0];
                                int firstIn = (int)(key0.inTangent * 1000.0f);
                                int firstOut = (int)(key0.outTangent * 1000.0f);
                                int firstValue = (int)(key0.value * 1000.0f);
                                bool same = true;
                                for (int j = 1, jmax = keys.Length; j < jmax; ++j)
                                {
                                    Keyframe key = keys[j];
                                    int inTangent = (int)(key.inTangent * 1000.0f);
                                    int outTangent = (int)(key.outTangent * 1000.0f);
                                    int value = (int)(key.value * 1000.0f);
                                    if (inTangent != firstIn || outTangent != firstOut || value != firstValue)
                                    {
                                        same = false;
                                    }
                                }
                                if (same)
                                {
                                    Keyframe keyn = keys[keys.Length - 1];
                                    AnimationUtility.SetEditorCurve(clip,
                                        EditorCurveBinding.FloatCurve(accd.path, accd.type, accd.propertyName),
                                        AnimationCurve.Linear(key0.time, key0.value, keyn.time - key0.time, key0.value));
                                    //Debug.Log(string.Format("Clip:{0} path:{1} propertyName:{2}", clip.name, ecb.path, ecb.propertyName));                                    
                                }
                            }
                        }
                    }
                    //EditorUtility.DisplayProgressBar(string.Format("{0}-{1}/{2}", title, i, prefabs.Length), path, (float)i / prefabs.Length);
                }
            }
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Finish", "All Clips process finish", "OK");
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
                    Debug.Log("state name: "+state.name+" cotroll: "+controller.name);
                    if (state.name != controller.name)
                    {
                        Debug.LogError(string.Format("Animator name error controller name:{0} state name:{1} path:{2}", controller.name, state.name, path));
                    }
                }
            }
            else
            {
                Debug.LogError(string.Format("Not 1 layer:{0} Count:{1}", controller.name, controller.layers.Length));
            }
            return true;
        }
        

    }

}