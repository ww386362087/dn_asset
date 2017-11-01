using System.Collections.Generic;
using UnityEngine;
using XTable;
using System.Collections;

public class XSkillHoster : MonoBehaviour, ISkillHoster
{
    [SerializeField]
    private XSkillData _xData = null;
    [SerializeField]
    public XSkillDataExtra xDataExtra = null;
    [SerializeField]
    private XEditorData _xEditorData = null;
    [SerializeField]
    private XConfigData _xConfigData = null;

    GameObject _target = null;

    [HideInInspector]
    public GameObject Target { get { return _target; } }
    [HideInInspector]
    public static XSerialized<XSkillData> sData = new XSerialized<XSkillData>();
    [HideInInspector]
    public static XSerialized<XEditorData> sEditorData = new XSerialized<XEditorData>();
    [HideInInspector]
    public static XSerialized<XConfigData> sConfigData = new XSerialized<XConfigData>();
    [HideInInspector]
    public List<XSkillData> ComboSkills = new List<XSkillData>();
    [HideInInspector]
    public int nHotID = 0;
    [HideInInspector]
    public Vector3 nResultForward = Vector3.zero;
    [HideInInspector]
    public Transform ShownTransform { get; set; }
    [HideInInspector]
    public AnimatorOverrideController oVerrideController = null;
    [HideInInspector]
    public float ir { get; set; }
    [HideInInspector]
    public float or { get; set; }
    [HideInInspector]
    public float defaultFov = 45;

    private XEntityPresentation.RowData _present_data = null;
    public XSkillData xOuterData { get; set; }
    private float _to = 0;
    private float _from = 0;
    private float fire_time = 0;
    private string trigger = null;
    public Animator ator = null;

    private DummyState _state = DummyState.Idle;
    private XSkillCamera _camera = null;
    private XSkillData _current = null;

    private bool _execute = false;
    private bool _anim_init = false;
    private bool _skill_when_move = false;

    private List<uint> _combinedToken = new List<uint>();
    private List<uint> _presentToken = new List<uint>();
    private List<uint> _logicalToken = new List<uint>();

    public List<Vector3>[] warningPosAt { get; set; }
    public XSkillResult skillResult { get; set; }
    public XSkillMob skillMob { get; set; }
    public XSkillFx skillFx { get; set; }
    public XSkillManipulate skillManip { get; set; }
    public XSkillWarning skillWarning { get; set; }

    public List<XSkill> skills = new List<XSkill>();

    public XConfigData ConfigData
    {
        get
        {
            if (_xConfigData == null) _xConfigData = new XConfigData();
            return _xConfigData;
        }
        set { _xConfigData = value; }
    }

    public Transform Transform { get { return transform; } }

    public XEditorData EditorData
    {
        get
        {
            if (_xEditorData == null) _xEditorData = new XEditorData();
            return _xEditorData;
        }
    }

    public XSkillDataExtra SkillDataExtra
    {
        get
        {
            if (xDataExtra == null) xDataExtra = new XSkillDataExtra();
            return xDataExtra;
        }
    }

    public XSkillData SkillData
    {
        get
        {
            if (_xData == null) _xData = new XSkillData();
            return _xData;
        }
        set { _xData = value; }
    }

    public XSkillData CurrentSkillData
    {
        get { return _state == DummyState.Fire ? _current : SkillData; }
    }

    void Awake()
    {
        ShownTransform = transform;
        gameObject.AddComponent<GameEntrance>();
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.4f);
        _state = DummyState.Idle;
        if (oVerrideController == null) BuildOverride();
        _camera = new XSkillCamera(gameObject);
        _camera.Initialize();
        _camera.UnityCamera.fieldOfView = defaultFov;

        Light light = _camera.UnityCamera.gameObject.AddComponent<Light>() as Light;
        light.type = LightType.Directional;
        light.intensity = 0.5f;

        RebuildSkillAniamtion();
        Application.targetFrameRate = 30;
        InitHost();
    }

    void OnDrawGizmos()
    {
        if (nHotID < 0 || CurrentSkillData.Result == null || nHotID >= CurrentSkillData.Result.Count) return;
        if (ShownTransform == null) ShownTransform = transform;
        float offset_x = CurrentSkillData.Result[nHotID].LongAttackEffect ? CurrentSkillData.Result[nHotID].LongAttackData.At_X : CurrentSkillData.Result[nHotID].Offset_X;
        float offset_z = CurrentSkillData.Result[nHotID].LongAttackEffect ? CurrentSkillData.Result[nHotID].LongAttackData.At_Z : CurrentSkillData.Result[nHotID].Offset_Z;
        Vector3 offset = ShownTransform.rotation * new Vector3(offset_x, 0, offset_z);
        Color defaultColor = Gizmos.color;
        Gizmos.color = Color.red;
        Matrix4x4 defaultMatrix = Gizmos.matrix;
        if (ShownTransform == transform)
        {
            ShownTransform.position += offset;
            Gizmos.matrix = ShownTransform.localToWorldMatrix;
            ShownTransform.position -= offset;
        }
        else    //bullet
            Gizmos.matrix = ShownTransform.localToWorldMatrix;

        if (CurrentSkillData.Result[nHotID].LongAttackEffect)
        {
            if (CurrentSkillData.Result[nHotID].LongAttackData.TriggerAtEnd)
            {
                float m_Theta = 0.01f;
                Vector3 beginPoint = Vector3.zero;
                Vector3 firstPoint = Vector3.zero;
                for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                {
                    float x = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                    float z = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                    Vector3 endPoint = new Vector3(x, 0, z);
                    if (theta == 0) firstPoint = endPoint;
                    else Gizmos.DrawLine(beginPoint, endPoint);
                    beginPoint = endPoint;
                }
                Gizmos.DrawLine(firstPoint, beginPoint);

                if (CurrentSkillData.Result[nHotID].Low_Range > 0)
                {
                    m_Theta = 0.01f;
                    beginPoint = Vector3.zero;
                    firstPoint = Vector3.zero;
                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = CurrentSkillData.Result[nHotID].Low_Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = CurrentSkillData.Result[nHotID].Low_Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0) firstPoint = endPoint;
                        else Gizmos.DrawLine(beginPoint, endPoint);
                        beginPoint = endPoint;
                    }
                    Gizmos.DrawLine(firstPoint, beginPoint);
                }
            }
            else
            {
                if (CurrentSkillData.Result[nHotID].LongAttackData.Type == XResultBulletType.Ring)
                {
                    float m_Theta = 0.01f;
                    Vector3 beginPoint = Vector3.zero;
                    Vector3 firstPoint = Vector3.zero;
                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = ir / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = ir / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0) firstPoint = endPoint;
                        else Gizmos.DrawLine(beginPoint, endPoint);
                        beginPoint = endPoint;
                    }

                    Gizmos.DrawLine(firstPoint, beginPoint);
                    Vector3 beginPoint2 = Vector3.zero;
                    Vector3 firstPoint2 = Vector3.zero;
                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = or / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = or / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0) firstPoint2 = endPoint;
                        else Gizmos.DrawLine(beginPoint2, endPoint);
                        beginPoint2 = endPoint;
                    }
                    Gizmos.DrawLine(firstPoint2, beginPoint2);
                }
                else
                {
                    float m_Theta = 0.01f;
                    Vector3 beginPoint = Vector3.zero;
                    Vector3 firstPoint = Vector3.zero;
                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = CurrentSkillData.Result[nHotID].LongAttackData.Radius / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = CurrentSkillData.Result[nHotID].LongAttackData.Radius / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0)
                        {
                            firstPoint = endPoint;
                        }
                        else
                        {
                            Gizmos.DrawLine(beginPoint, endPoint);
                        }
                        beginPoint = endPoint;
                    }
                    Gizmos.DrawLine(firstPoint, beginPoint);
                }
            }
        }
        else
        {
            if (CurrentSkillData.Result[nHotID].Sector_Type)
            {
                float m_Theta = 0.01f;
                Vector3 beginPoint = Vector3.zero;
                Vector3 firstPoint = Vector3.zero;

                for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                {
                    float x = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                    float z = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                    Vector3 endPoint = new Vector3(x, 0, z);
                    if (theta == 0)
                    {
                        firstPoint = endPoint;
                    }
                    else
                    {
                        Gizmos.DrawLine(beginPoint, endPoint);
                    }
                    beginPoint = endPoint;
                }

                Gizmos.DrawLine(firstPoint, beginPoint);

                if (CurrentSkillData.Result[nHotID].Low_Range > 0)
                {
                    m_Theta = 0.01f;
                    beginPoint = Vector3.zero;
                    firstPoint = Vector3.zero;
                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = CurrentSkillData.Result[nHotID].Range / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0)
                        {
                            firstPoint = endPoint;
                        }
                        else
                        {
                            Gizmos.DrawLine(beginPoint, endPoint);
                        }
                        beginPoint = endPoint;
                    }
                    Gizmos.DrawLine(firstPoint, beginPoint);
                }
            }
            else
            {
                Vector3 fr = new Vector3(CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Range / 2.0f);
                Vector3 fl = new Vector3(CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Rect_HalfEffect ? 0 : (-CurrentSkillData.Result[nHotID].Range / 2.0f));
                Vector3 br = new Vector3(-CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Range / 2.0f);
                Vector3 bl = new Vector3(-CurrentSkillData.Result[nHotID].Scope / 2.0f, 0, CurrentSkillData.Result[nHotID].Rect_HalfEffect ? 0 : (-CurrentSkillData.Result[nHotID].Range / 2.0f));

                Gizmos.DrawLine(fr, fl);
                Gizmos.DrawLine(fl, bl);
                Gizmos.DrawLine(bl, br);
                Gizmos.DrawLine(br, fr);
            }
        }
        Gizmos.matrix = defaultMatrix;
        Gizmos.color = defaultColor;
    }

    private float _action_framecount = 0;
    private Rect _rect = new Rect(10, 10, 150, 20);
    void OnGUI() { GUI.Label(_rect, "Action Frame: " + _action_framecount); }

    private void InitHost()
    {
        skills.Clear();
        skillResult = new XSkillResult(this);
        skillMob = new XSkillMob(this);
        skillFx = new XSkillFx(this);
        skillManip = new XSkillManipulate(this);
        skillWarning = new XSkillWarning(this);
        skills.Add(skillResult);
        skills.Add(skillMob);
        skills.Add(skillFx);
        skills.Add(skillManip);
        skills.Add(skillWarning);
    }

    private void Execute()
    {
        _execute = true;
        if (_current == null) return;
        for (int i = 0, max = skills.Count; i < max; i++)
        {
            skills[i].Execute();
        }
    }


    void Update()
    {
        int nh = 0; int nv = 0;
        Vector3 h = Vector3.right;
        Vector3 up = Vector3.up;
        if (_state != DummyState.Fire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_xData.TypeToken == 1 && ComboSkills.Count > 0) oVerrideController["Art"] = Resources.Load(_xData.ClipName) as AnimationClip;
                xOuterData = _xData;
                Fire();
            }
            _action_framecount = 0;
        }
        else
        {
            _action_framecount += Time.deltaTime;
            if (_action_framecount > _current.Time) StopFire();
            if (_execute || xOuterData.TypeToken == 2)
            {
                if (nh != 0 || nv != 0)
                {
                    Vector3 MoveDir = h * nh;
                    if (CanAct(MoveDir)) Move(MoveDir);
                }
                else if (_skill_when_move)
                {
                    trigger = AnimTriger.ToStand;
                    _skill_when_move = false;
                }
            }
            if (_anim_init) Execute();
            _anim_init = false;
        }
    }

    void LateUpdate()
    {
        UpdateRotation();
        if (!string.IsNullOrEmpty(trigger) && ator != null && !ator.IsInTransition(0))
        {
            if (trigger != AnimTriger.ToStand &&
                trigger != AnimTriger.ToMove &&
                trigger != AnimTriger.EndSkill)
                _anim_init = true; // is casting

            ator.speed = 1;
            ator.SetTrigger(trigger);
            trigger = null;
        }
    }

    private float rotate_speed = 0;
    private void UpdateRotation()
    {
        if (_from != _to)
        {
            _from += (_to - _from) * Mathf.Min(1.0f, Time.deltaTime * rotate_speed);
            transform.rotation = Quaternion.Euler(0, _from, 0);
        }
    }

    private void Fire()
    {
        _current = xOuterData;
        _skill_when_move = _state == DummyState.Move;
        _state = DummyState.Fire;
        fire_time = Time.time;

        if (xOuterData.TypeToken == 0)
            trigger = XSkillData.JA_Command[xOuterData.SkillPosition];
        else if (xOuterData.TypeToken == 1)
            trigger = AnimTriger.ToArtSkill;

        FocusTarget();
        _anim_init = false;
    }

    public void StopFire()
    {
        if (_state != DummyState.Fire) return;
        _state = DummyState.Idle;
        trigger = AnimTriger.EndSkill;
        _execute = false;

        for (int i = 0, max = skills.Count; i < max; i++)
        {
            skills[i].Clear();
        }
        _action_framecount = 0;
        _camera.EndEffect(null);
        CleanTokens();
        nResultForward = Vector3.zero;
        Time.timeScale = 1;
        if (ator != null) ator.speed = 1;
        _current = null;
    }


    private void FocusTarget()
    {
        XHitHoster hit = GameObject.FindObjectOfType<XHitHoster>();
        _target = (xOuterData.NeedTarget && hit != null) ? hit.gameObject : null;
        if (_target != null && xOuterData.IsInAttckField(transform.position, transform.forward, _target))
        {
            PrepareRotation(XCommon.singleton.Horizontal(_target.transform.position - transform.position), _xConfigData.RotateSpeed);
        }
    }
    

    private bool CanAct(Vector3 dir)
    {
        bool can = false;
        float now = Time.time - fire_time;
        XLogicalData logic = (SkillData.TypeToken == 2) ? SkillData.Logical : _current.Logical;
        can = true;
        if (now < logic.Not_Move_End && now > logic.Not_Move_At)
        {
            can = false;
        }
        if (can) StopFire();
        else
        {
            if (now < logic.Rotate_End && now > logic.Rotate_At)
            {
                PrepareRotation(XCommon.singleton.Horizontal(dir), logic.Rotate_Speed > 0 ? logic.Rotate_Speed : _xConfigData.RotateSpeed);
            }
        }
        return can;
    }

    private void Move(Vector3 dir)
    {
        PrepareRotation(dir, _xConfigData.RotateSpeed);
        transform.Translate(dir * Time.deltaTime * ConfigData.Speed, Space.World);
    }

    public void PrepareRotation(Vector3 targetDir, float speed)
    {
        Vector3 from = transform.forward;
        _from = YRotation(from);
        float angle = Vector3.Angle(from, targetDir);
        bool clockwise = XCommon.singleton.Clockwise(from, targetDir);
        _to = clockwise ? _from + angle : _from - angle;
        rotate_speed = speed;
    }

    private float YRotation(Vector3 dir)
    {
        float r = Vector3.Angle(Vector3.forward, dir);
        bool clockwise = XCommon.singleton.Clockwise(Vector3.forward, dir);
        return clockwise ? r : 360.0f - r;
    }

    private void BuildOverride()
    {
        oVerrideController = new AnimatorOverrideController();
        ator = GetComponent<Animator>();
        if (ator != null)
        {
            oVerrideController.runtimeAnimatorController = ator.runtimeAnimatorController;
            ator.runtimeAnimatorController = oVerrideController;
        }
    }

    public void RebuildSkillAniamtion()
    {
        AnimationClip clip = XResources.Load<AnimationClip>(SkillData.ClipName, AssetType.Anim);
        if (oVerrideController == null) BuildOverride();
        if (SkillData.TypeToken == 0)
        {
            string motion = XSkillData.JaOverrideMap[SkillData.SkillPosition];
            oVerrideController[motion] = clip;
        }
        else if (SkillData.TypeToken == 1) { oVerrideController["Art"] = clip; }

        _present_data = XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)_xConfigData.Player);
        oVerrideController["Idle"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackIdle) as AnimationClip;
        oVerrideController["Run"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackRun) as AnimationClip;
        oVerrideController["Walk"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackWalk) as AnimationClip;
    }

    public Vector3 GetRotateTo()
    {
        return XCommon.singleton.FloatToAngle(_to);
    }

    public void AddedTimerToken(uint token, bool logical)
    {
        if (logical)
            _logicalToken.Add(token);
        else
            _presentToken.Add(token);
    }

    public void AddedCombinedToken(uint token)
    {
        _combinedToken.Add(token);
    }

    private void CleanTokens()
    {
        foreach (uint token in _combinedToken)
        {
            XTimerMgr.singleton.RemoveTimer(token);
        }
        _combinedToken.Clear();
        foreach (uint token in _presentToken)
        {
            XTimerMgr.singleton.RemoveTimer(token);
        }
        _presentToken.Clear();
        foreach (uint token in _logicalToken)
        {
            XTimerMgr.singleton.RemoveTimer(token);
        }
        _logicalToken.Clear();
    }


    /// <summary>
    /// 绘制攻击范围
    /// </summary>
    private void DrawManipulationFileds()
    {
        if (_xData.Manipulation != null)
        {
            foreach (XManipulationData data in _xData.Manipulation)
            {
                if (data.Radius <= 0 || !xDataExtra.ManipulationEx[data.Index].Present) continue;

                Vector3 offset = transform.rotation * new Vector3(data.OffsetX, 0, data.OffsetZ);

                Color defaultColor = Gizmos.color;
                Gizmos.color = Color.red;

                Matrix4x4 defaultMatrix = Gizmos.matrix;
                transform.position += offset;
                Gizmos.matrix = transform.localToWorldMatrix;
                transform.position -= offset;

                float m_Theta = 0.01f;
                Vector3 beginPoint = Vector3.zero;
                Vector3 firstPoint = Vector3.zero;
                for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                {
                    float x = data.Radius / transform.localScale.y * Mathf.Cos(theta);
                    float z = data.Radius / transform.localScale.y * Mathf.Sin(theta);
                    Vector3 endPoint = new Vector3(x, 0, z);
                    if (theta == 0)
                    {
                        firstPoint = endPoint;
                    }
                    else
                    {
                        if (Vector3.Angle(endPoint, transform.forward) < data.Degree * 0.5f)
                            Gizmos.DrawLine(beginPoint, endPoint);
                    }
                    beginPoint = endPoint;
                }

                if (data.Degree == 360)
                    Gizmos.DrawLine(firstPoint, beginPoint);
                else
                {
                    Gizmos.DrawLine(Vector3.zero, XCommon.singleton.HorizontalRotateVetor3(transform.forward, data.Degree * 0.5f, true) * (data.Radius / transform.localScale.y));
                    Gizmos.DrawLine(Vector3.zero, XCommon.singleton.HorizontalRotateVetor3(transform.forward, -data.Degree * 0.5f, true) * (data.Radius / transform.localScale.y));
                }

                Gizmos.matrix = defaultMatrix;
                Gizmos.color = defaultColor;
            }
        }
    }
}

