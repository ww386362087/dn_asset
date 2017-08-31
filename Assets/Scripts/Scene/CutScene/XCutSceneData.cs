using System;
using System.Collections.Generic;
using UnityEngine;


public enum XClipType
{
    Actor = 0,
    Player,
    Fx,
    Audio,
    SubTitle,
    Slash
}


[Serializable]
public class XCutSceneData
{
    [SerializeField]
    public float TotalFrame = 0;
    [SerializeField]
    public string CameraClip = null;
    [SerializeField]
    public string Name = null;
    [SerializeField]
    public string Script = null;
    [SerializeField]
    public string Scene = null;
    [SerializeField]
    public int TypeMask = -1;
    [SerializeField]
    public bool OverrideBGM = true;
    [SerializeField]
    public bool Mourningborder = true;
    [SerializeField]
    public bool AutoEnd = true;
    [SerializeField]
    public float Length = 0;
    [SerializeField]
    public float FieldOfView = 45.0f;
    [SerializeField]
    public string Trigger = "ToEffect";

    [SerializeField]
    public bool GeneralShow = false;
    [SerializeField]
    public bool GeneralBigGuy = false;

    [SerializeField]
    public List<XActorDataClip> Actors = new List<XActorDataClip>();
    [SerializeField]
    public List<XPlayerDataClip> Player = new List<XPlayerDataClip>();
    [SerializeField]
    public List<XFxDataClip> Fxs = new List<XFxDataClip>();
    [SerializeField]
    public List<XAudioDataClip> Audios = new List<XAudioDataClip>();
    [SerializeField]
    public List<XSubTitleDataClip> SubTitle = new List<XSubTitleDataClip>();
    [SerializeField]
    public List<XSlashDataClip> Slash = new List<XSlashDataClip>();
}


[Serializable]
public abstract class XCutSceneClip
{
    [SerializeField]
    public XClipType Type = XClipType.Actor;
    [SerializeField]
    public float TimeLineAt = 0;
}

[Serializable]
public class XActorDataClip : XCutSceneClip
{
    [SerializeField]
    public string Prefab = null;
    [SerializeField]
    public string Clip = null;
    [SerializeField]
    public float AppearX = 0;
    [SerializeField]
    public float AppearY = 0;
    [SerializeField]
    public float AppearZ = 0;
    [SerializeField]
    public int StatisticsID = 0;
    [SerializeField]
    public bool bUsingID = false;
    [SerializeField]
    public bool bToCommonPool = false;
    [SerializeField]
    public string Tag = null;
}

[Serializable]
public class XPlayerDataClip : XCutSceneClip
{
    [SerializeField]
    public string Clip1 = null;
    [SerializeField]
    public string Clip2 = null;
    [SerializeField]
    public string Clip3 = null;
    [SerializeField]
    public string Clip4 = null;
    [SerializeField]
    public string Clip5 = null;
    [SerializeField]
    public string Clip6 = null;
    [SerializeField]
    public float AppearX = 0;
    [SerializeField]
    public float AppearY = 0;
    [SerializeField]
    public float AppearZ = 0;
}

[Serializable]
public class XFxDataClip : XCutSceneClip
{
    [SerializeField]
    public string Fx = null;
    [SerializeField]
    public int BindIdx = 0;
    [SerializeField]
    public string Bone = null;
    [SerializeField]
    public float Scale = 1;
    [SerializeField]
    public bool Follow = true;
    [SerializeField]
    public float Destroy_Delay = 0;
    [SerializeField]
    public float AppearX = 0;
    [SerializeField]
    public float AppearY = 0;
    [SerializeField]
    public float AppearZ = 0;
    [SerializeField]
    public float Face = 0;
}

[Serializable]
public class XAudioDataClip : XCutSceneClip
{
    [SerializeField]
    public string Clip = null;
    [SerializeField]
    public int BindIdx = 0;
    [SerializeField]
    public AudioChannel Channel = AudioChannel.Skill;
}

[Serializable]
public class XSubTitleDataClip : XCutSceneClip
{
    [SerializeField]
    public string Context = null;
    [SerializeField]
    public float Duration = 45;
}

[Serializable]
public class XSlashDataClip : XCutSceneClip
{
    [SerializeField]
    public float Duration = 1;
    [SerializeField]
    public string Name;
    [SerializeField]
    public string Discription;
    [SerializeField]
    public float AnchorX;
    [SerializeField]
    public float AnchorY;
}

public enum AudioChannel
{
    Motion = 1,
    Action = 1 << 1,
    Skill = 1 << 2,
    Behit = 1 << 3,
    SkillCombine = 1 << 4,
}