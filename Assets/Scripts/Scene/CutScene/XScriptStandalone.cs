using System.Collections.Generic;
using UnityEngine;

public class XScriptStandalone : MonoBehaviour
{
    private XCutSceneCamera _cut_scene_camera = null;
    private List<XActor> _actors = new List<XActor>();

    private uint _token = 0;
    private const float FPS = 30.0f;

    [SerializeField]
    public XCutSceneData _cut_scene_data = null;

    void Start()
    {
        GameEnine.Init(this);

        _cut_scene_camera = new XCutSceneCamera();
        _cut_scene_camera.Initialize();

        XCameraMotionData m = new XCameraMotionData();
        m.AutoSync_At_Begin = false;
        m.Follow_Position = _cut_scene_data.GeneralShow;
        m.LookAt_Target = false;
        m.At = 0;
        m.Motion = _cut_scene_data.CameraClip;

        XCutSceneUI.singleton.Init();
        XCutSceneUI.singleton.SetText("");

        _cut_scene_camera.Effect(m);
        _cut_scene_camera.UnityCamera.fieldOfView = _cut_scene_data.FieldOfView;

        if (!_cut_scene_data.GeneralShow)
        {
            foreach (XActorDataClip clip in _cut_scene_data.Actors)
            {
                XResourceMgr.Load<AnimationClip>(clip.Clip, AssetType.Anim);
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS - 0.016f, BeOnStage, clip);
            }
            foreach (XPlayerDataClip clip in _cut_scene_data.Player)
            {
                XResourceMgr.Load<AnimationClip>(clip.Clip1, AssetType.Anim);
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS - 0.016f, BePlayerOnStage, clip);
            }
            foreach (XFxDataClip clip in _cut_scene_data.Fxs)
            {
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS, Fx, clip);
            }
        }
        foreach (XAudioDataClip clip in _cut_scene_data.Audios)
        {
            XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS, Audio, clip);
        }
        if (_cut_scene_data.AutoEnd)
        {
            XTimerMgr.singleton.SetTimer((_cut_scene_data.TotalFrame - 30) / FPS, EndShow, null);
        }
        if (_cut_scene_data.Mourningborder)
        {
            XCutSceneUI.singleton.SetVisible(true);

            foreach (XSubTitleDataClip clip in _cut_scene_data.SubTitle)
            {
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS, SubTitle, clip);
            }
            foreach (XSlashDataClip clip in _cut_scene_data.Slash)
            {
                XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS, Slash, clip);
            }
        }
    }

    void Update()
    {
        GameEnine.Update(Time.deltaTime);
    }

    void LateUpdate()
    {
        _cut_scene_camera.PostUpdate(Time.deltaTime);
    }

    void BePlayerOnStage(object o)
    {
        XPlayerDataClip clip = o as XPlayerDataClip;
        _actors.Add(new XActor(clip.AppearX, clip.AppearY, clip.AppearZ, clip.Clip1));
    }

    void BeOnStage(object o)
    {
        XActor target = null;
        XActorDataClip clip = o as XActorDataClip;
        if (clip.bUsingID)
            target = new XActor((uint)clip.StatisticsID, clip.AppearX, clip.AppearY, clip.AppearZ, clip.Clip);
        else
            target = new XActor(clip.Prefab, clip.AppearX, clip.AppearY, clip.AppearZ, clip.Clip);

        _actors.Add(target);
    }

    void Fx(object o)
    {
        XFxDataClip clip = o as XFxDataClip;
        Transform trans = (clip.BindIdx < 0) ? null : _actors[clip.BindIdx].Actor.transform;
        if (clip.Bone != null && clip.Bone.Length > 0)
            trans = trans.Find(clip.Bone);
        else
            trans = null;
        XFx fx = XFxMgr.singleton.CreateFx(clip.Fx);
        fx.DelayDestroy = clip.Destroy_Delay;
        if (trans != null)
            fx.Play(trans.gameObject, Vector3.zero, clip.Scale * Vector3.one, 1);
        else
            fx.Play(new Vector3(clip.AppearX, clip.AppearY, clip.AppearZ), XCommon.singleton.FloatToQuaternion(clip.Face), Vector3.one);
    }

    void Audio(object o)
    {
        //XAudioDataClip clip = o as XAudioDataClip;
        //if (clip.BindIdx < 0) return;
        //XFmod fmod = _actors[clip.BindIdx].Actor.GetComponent<XFmod>();
        //if (fmod == null)
        //    fmod = _actors[clip.BindIdx].Actor.AddComponent<XFmod>();
        //fmod.StartEvent("event:/" + clip.Clip, clip.Channel);
    }


    void SubTitle(object o)
    {
        XSubTitleDataClip clip = o as XSubTitleDataClip;
        XCutSceneUI.singleton.SetText(clip.Context);
        XTimerMgr.singleton.RemoveTimer(_token);
        _token = XTimerMgr.singleton.SetTimer(clip.Duration / FPS, EndSubTitle, null);
    }

    void Slash(object o)
    {
        XSlashDataClip clip = o as XSlashDataClip;
        XCutSceneUI.singleton.SetIntroText(true, clip.Name);
        XCutSceneUI.singleton.SetIntroPos(clip.AnchorX, clip.AnchorY);
        XTimerMgr.singleton.SetTimer(clip.Duration, EndSlash, null);
    }

    void EndShow(object o)
    {
        XTimerMgr.singleton.RemoveTimer(_token);
    }

    void EndSlash(object o)
    {
        XCutSceneUI.singleton.SetIntroText(false, "");
    }

    void EndSubTitle(object o)
    {
        XCutSceneUI.singleton.SetText("");
    }
}

