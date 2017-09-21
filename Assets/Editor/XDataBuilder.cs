using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using XTable;

namespace XEditor
{
    public class XDataBuilder : XSingleton<XDataBuilder>
    {
        public static GameObject hoster = null;
        public static DateTime Time;
        public static string prefixPath = "";

        public void Load(string pathwithname)
        {
            try
            {
                XSkillHoster.Quit = false;
                string path = XEditorPath.GetCfgFromSkp(pathwithname);
                XConfigData conf = XDataIO<XConfigData>.singleton.DeserializeData(path);
                GameObject prefab = XEditorLibrary.GetDummy((uint)conf.Player);
                if (prefab == null) { XDebug.LogError("Prefab not found by id: ", conf.Player); return; }
                ColdBuild(prefab, conf);
                prefixPath = pathwithname.Substring(0, pathwithname.IndexOf("/Skill"));
                Time = File.GetLastWriteTime(pathwithname);
            }
            catch (Exception e)
            {
                XDebug.LogError("Error occurred during loading config file: " , pathwithname , " with error: " , e.Message);
            }
        }

        public void HotBuild(XSkillHoster hoster, XConfigData conf)
        {
            hoster.SkillDataExtra.JaEx.Clear();
            if (conf.Ja != null)
            {
                foreach (XJADataExtra ja in conf.Ja)
                {
                    XJADataExtraEx jaex = new XJADataExtraEx();
                    if (ja.Next_Skill_PathWithName != null && ja.Next_Skill_PathWithName.Length > 0)
                    {
                        XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData("Assets/Resources/" + ja.Next_Skill_PathWithName);
                        jaex.Next = skill;
                    }
                    if (ja.JA_Skill_PathWithName != null && ja.JA_Skill_PathWithName.Length > 0)
                    {
                        XSkillData skill = XDataIO<XSkillData>.singleton.DeserializeData("Assets/Resources/" + ja.JA_Skill_PathWithName);
                        jaex.Ja = skill;
                    }
                    hoster.SkillDataExtra.JaEx.Add(jaex);
                }
            }
        }

       /// <summary>
       /// 刷内存数据
       /// </summary>
        public void HotBuildEx(XSkillHoster hoster, XConfigData conf)
        {
            XSkillDataExtra edata = hoster.SkillDataExtra;
            XSkillData data = hoster.SkillData;

            edata.ResultEx.Clear();
            edata.Fx.Clear();
            edata.HitEx.Clear();

            if (data.Result != null)
            {
                foreach (XResultData result in data.Result)
                {
                    XResultDataExtraEx rdee = new XResultDataExtraEx();
                    if (result.LongAttackEffect)
                    {
                        rdee.BulletPrefab = Resources.Load(result.LongAttackData.Prefab) as GameObject;
                        rdee.BulletEndFx = Resources.Load(result.LongAttackData.End_Fx) as GameObject;
                        rdee.BulletHitGroundFx = Resources.Load(result.LongAttackData.HitGround_Fx) as GameObject;
                    }
                    edata.ResultEx.Add(rdee);
                }
            }
            if (data.Hit != null)
            {
                foreach (XHitData hit in data.Hit)
                {
                    XHitDataExtraEx hee = new XHitDataExtraEx();
                    hee.Fx = Resources.Load(hit.Fx) as GameObject;
                    edata.HitEx.Add(hee);
                }
            }
            if (data.Fx != null)
            {
                foreach (XFxData fx in data.Fx)
                {
                    XFxDataExtra fxe = new XFxDataExtra();
                    fxe.Fx = Resources.Load(fx.Fx) as GameObject;
                    if (fx.Bone != null && fx.Bone.Length > 0)
                    {
                        Transform attachPoint = hoster.gameObject.transform.Find(fx.Bone);
                        if (attachPoint != null)
                        {
                            fxe.BindTo = attachPoint.gameObject;
                        }
                        else
                        {
                            int index = fx.Bone.LastIndexOf("/");
                            if (index >= 0)
                            {
                                string bone = fx.Bone.Substring(index + 1);
                                attachPoint = hoster.gameObject.transform.Find(bone);
                                if (attachPoint != null)
                                {
                                    fxe.BindTo = attachPoint.gameObject;
                                }
                            }
                        }
                    }
                    fxe.Ratio = fx.At / data.Time;
                    edata.Fx.Add(fxe);
                }
            }

            if (data.Manipulation != null)
            {
                foreach (XManipulationData manipulation in data.Manipulation)
                {
                    XManipulationDataExtra me = new XManipulationDataExtra();
                    edata.ManipulationEx.Add(me);
                }
            }
            if (data.Warning != null)
            {
                foreach (XWarningData warning in data.Warning)
                {
                    XWarningDataExtra we = new XWarningDataExtra();
                    we.Fx = Resources.Load(warning.Fx) as GameObject;
                    we.Ratio = warning.At / data.Time;
                    edata.Warning.Add(we);
                }
            }
            if (data.Mob != null)
            {
                foreach (XMobUnitData mob in data.Mob)
                {
                    XMobUnitDataExtra me = new XMobUnitDataExtra();
                    me.Ratio = mob.At / data.Time;
                    edata.Mob.Add(me);
                }
            }
        }

        public void ColdBuild(GameObject prefab, XConfigData conf)
        {
            if (hoster != null) GameObject.DestroyImmediate(hoster);

            hoster = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            hoster.transform.localScale = Vector3.one * XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)conf.Player).Scale;
            hoster.AddComponent<XSkillHoster>();
            hoster.GetComponent<CharacterController>().enabled = false;
            UnityEngine.AI.NavMeshAgent agent = hoster.GetComponent<UnityEngine.AI.NavMeshAgent>();
            if (agent != null) agent.enabled = false;

            XSkillHoster component = hoster.GetComponent<XSkillHoster>();
            string directory = conf.Directory[conf.Directory.Length - 1] == '/' ? conf.Directory.Substring(0, conf.Directory.Length - 1) : conf.Directory;
            string path = XEditorPath.GetPath("Skill" + "/" + directory);

            component.ConfigData = conf;
            component.SkillData = XDataIO<XSkillData>.singleton.DeserializeData(path + conf.SkillName + ".txt");
            component.SkillDataExtra.ScriptPath = path;
            component.SkillDataExtra.ScriptFile = conf.SkillName;
            component.SkillDataExtra.SkillClip = RestoreClip(conf.SkillClip, conf.SkillClipName);
            if (component.SkillData.TypeToken != 3)
            {
                if (component.SkillData.Time == 0)
                    component.SkillData.Time = component.SkillDataExtra.SkillClip.length;
            }
            HotBuild(component, conf);
            HotBuildEx(component, conf);

            EditorGUIUtility.PingObject(hoster);
            Selection.activeObject = hoster;
        }

        public void Update(XSkillHoster hoster)
        {
            string pathwithname = hoster.SkillDataExtra.ScriptPath + hoster.ConfigData.SkillName + ".txt";
            DateTime time = File.GetLastWriteTime(pathwithname);
            if (Time == default(DateTime)) Time = time;

            if (time != Time)
            {
                Time = time;
              if (EditorUtility.DisplayDialog("WARNING!",
                                            "Skill has been Modified outside, Press 'OK' to reload file or 'Ignore' to maintain your change. (Make sure the '.config' file for skill script has been well synchronized)",
                                            "Ok", "Ignore"))
                {
                    hoster.ConfigData = XDataIO<XConfigData>.singleton.DeserializeData(XEditorPath.GetCfgFromSkp(pathwithname));
                    hoster.SkillData = XDataIO<XSkillData>.singleton.DeserializeData(pathwithname);

                    HotBuild(hoster, hoster.ConfigData);
                    HotBuildEx(hoster, hoster.ConfigData);
                }
            }
        }

        private AnimationClip RestoreClip(string path, string name)
        {
            if (path == null || name == null || path == "" || name == "") return null;
            int last = path.LastIndexOf('.');
            string subfix = path.Substring(last, path.Length - last).ToLower();
            if (subfix == AssetType.Fbx)
            {
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
                foreach (UnityEngine.Object obj in objs)
                {
                    AnimationClip clip = obj as AnimationClip;
                    if (clip != null && clip.name == name) return clip;
                }
            }
            else if (subfix == AssetType.Anim)
            {
                return AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip)) as AnimationClip;
            }
            else
                return null;

            return null;
        }
    }

}
