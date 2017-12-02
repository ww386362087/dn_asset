using UnityEngine;
using UnityEditor.Callbacks;

namespace UnityEditor.XBuild
{
    public sealed class PostProcessBuildEditor
    {

        [PostProcessBuild]
        static void OnBuildingEnd(BuildTarget target, string path)
        {
            ProjectSettingIOS ps = null;
#if UNITY_IOS //tencent
            ps = new ProjectSettingIOS_Tencent();
#endif

            if (ps == null)
            {
                Debug.LogError("No platform matched, please check it!");
                return;
            }
            ps.PostProcessBuild(target, path);
            Debug.Log("Build Task over !");
        }
    }

}