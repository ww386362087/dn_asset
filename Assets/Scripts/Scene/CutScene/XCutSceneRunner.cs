using System.Collections.Generic;
using UnityEngine;

public class XCutSceneRunner : MonoBehaviour
{

    private XCutSceneCamera _cut_scene_camera = null;

    private List<XActor> _actors = new List<XActor>();
    private List<XFx> _fxs = new List<XFx>();
    private List<uint> _times = new List<uint>();
    private uint _token = 0;
    
    const float FPS = 30.0f;
    float start_play_time = 0;
    public bool is_start_by_editor = false;
    public XCutSceneData cut_scene_data = null;


    public bool IsPlaying
    {
        get { return cut_scene_data != null ? Time.time - start_play_time < cut_scene_data.TotalFrame : false; }
    }

    void Start()
    {
        if (is_start_by_editor)
        {
            GameEnine.Init(this);
            _cut_scene_camera = new XCutSceneCamera();
            _cut_scene_camera.Initialize();
            _cut_scene_camera.Effect(cut_scene_data.CameraClip, cut_scene_data.Trigger);
            _cut_scene_camera.UnityCamera.fieldOfView = cut_scene_data.FieldOfView;
            XLoading.Show(false);
        }
        else
        {
            XScene.singleton.GameCamera.Effect(cut_scene_data.CameraClip, cut_scene_data.Trigger);
            XScene.singleton.GameCamera.UnityCamera.fieldOfView = cut_scene_data.FieldOfView;
        }
        start_play_time = Time.time;
        XCutSceneUI.singleton.Initial();

        if (!cut_scene_data.GeneralShow)
        {
            for (int i = 0, max = cut_scene_data.Actors.Count; i < max; i++)
            {
                XActorDataClip clip = cut_scene_data.Actors[i];
                XResources.Load<AnimationClip>(clip.Clip, AssetType.Anim);
                _times.Add(XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS - 0.016f, BeOnStage, clip));
            }
            for (int i = 0, max = cut_scene_data.Player.Count; i < max; i++)
            {
                XPlayerDataClip clip = cut_scene_data.Player[i];
                XResources.Load<AnimationClip>(clip.Clip1, AssetType.Anim);
                _times.Add(XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS - 0.016f, BePlayerOnStage, clip));
            }
            for (int i = 0, max = cut_scene_data.Fxs.Count; i < max; i++)
            {
                _times.Add(XTimerMgr.singleton.SetTimer(cut_scene_data.Fxs[i].TimeLineAt / FPS, Fx, cut_scene_data.Fxs[i]));
            }
        }
        for (int i = 0, max = cut_scene_data.Audios.Count; i < max; i++)
        {
            XAudioDataClip clip = cut_scene_data.Audios[i];
            _times.Add(XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS, Audio, clip));
        }
        if (cut_scene_data.AutoEnd)
        {
            _times.Add(XTimerMgr.singleton.SetTimer((cut_scene_data.TotalFrame - 30) / FPS, EndShow, null));
        }
        if (cut_scene_data.Mourningborder)
        {
            XCutSceneUI.singleton.SetVisible(true);
            for (int i = 0, max = cut_scene_data.SubTitle.Count; i < max; i++)
            {
                XSubTitleDataClip clip = cut_scene_data.SubTitle[i];
                _times.Add(XTimerMgr.singleton.SetTimer(clip.TimeLineAt / FPS, SubTitle, clip));
            }
            for (int i = 0, max = cut_scene_data.Slash.Count; i < max; i++)
            {
                _times.Add(XTimerMgr.singleton.SetTimer(cut_scene_data.Slash[i].TimeLineAt / FPS, Slash, cut_scene_data.Slash[i]));
            }
        }
    }

    void Update()
    {
        if (is_start_by_editor) GameEnine.Update(Time.deltaTime);
    }


    void LateUpdate()
    {
        if (is_start_by_editor)
        {
            _cut_scene_camera.PostUpdate(Time.deltaTime);
        }
        else
            XScene.singleton.GameCamera.TriggerEffect();
    }

    public void UnLoad()
    {
        XTimerMgr.singleton.RemoveTimer(_token);
        XCutSceneUI.singleton.SetVisible(false);
        for (int i = 0, max = _actors.Count; i < max; i++)
        {
            Destroy(_actors[i].Actor.gameObject);
        }
        _actors.Clear();

        for (int i = 0, max = _fxs.Count; i < max; i++)
        {
            XFxMgr.singleton.DestroyFx(_fxs[i], true);
        }
        _fxs.Clear();

        for(int i=0,max=_times.Count;i<_times.Count;i++)
        {
            XTimerMgr.singleton.RemoveTimer(_times[i]);
        }
        _times.Clear();
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

        _fxs.Add(fx);
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

