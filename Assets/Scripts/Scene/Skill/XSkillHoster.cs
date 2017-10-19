using System.Collections.Generic;
using UnityEngine;
using XTable;
using System.Collections;

public class XSkillHoster : MonoBehaviour
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
    public static bool Quit { get; set; }
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
    public Transform ShownTransform = null;
    [HideInInspector]
    public AnimatorOverrideController oVerrideController = null;
    [HideInInspector]
    public float ir = 0;
    [HideInInspector]
    public float or = 0;

    public float defaultFov = 45;

    private XEntityPresentation.RowData _present_data = null;
    public XSkillData xOuterData = null;
    private float _to = 0;
    private float _from = 0;
    private float _time_offset = 0;
    public float fire_time = 0;
    public string trigger = null;
    public Animator ator = null;
    public enum DummyState { Idle, Move, Fire };

    private DummyState _state = DummyState.Idle;
    private XSkillCamera _camera = null;
    private XSkillData _current = null;
    
    private bool _execute = false;
    private bool _anim_init = false;
    private bool _skill_when_move = false;

    private List<uint> _combinedToken = new List<uint>();
    private List<uint> _presentToken = new List<uint>();
    private List<uint> _logicalToken = new List<uint>();
    public List<Vector3>[] warningPosAt = null;

    public XSkillResult skillResult;
    public XSkillMob skillMob;
    public XSkillFx skillFx;
    public XSkillManipulate skillManip;
    public XSkillJA skillJa;
    public XSkillWarning skillWarning;
    public List<XSkill> skills = new List<XSkill>();

    public XConfigData ConfigData
    {
        get
        {
            if (_xConfigData == null) _xConfigData = new XConfigData();
            return _xConfigData;
        }
        set
        {
            _xConfigData = value;
        }
    }

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

    public DummyState state { get { return _state; } set { _state = value; } }
    
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
    private Rect _rect;

    void OnGUI()
    {
#if UNITY_EDITOR
        GUI.Label(_rect, "Action Frame: " + _action_framecount);
#endif
    }
    
    private void InitHost()
    {
        _rect = new Rect(10, 10, 150, 20);
        skills.Clear();
        skillResult = new XSkillResult(this);
        skillMob = new XSkillMob(this);
        skillFx = new XSkillFx(this);
        skillManip = new XSkillManipulate(this);
        skillJa = new XSkillJA(this);
        skillWarning = new XSkillWarning(this);
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
            if (_execute || xOuterData.TypeToken == 3)
            {
                if (nh != 0 || nv != 0)
                {
                    Vector3 MoveDir = h * nh;
                    if (CanAct(MoveDir))
                    {
                        Move(MoveDir);
                    }
                }
                else if (_skill_when_move)
                {
                    trigger = "ToStand";
                    _skill_when_move = false;
                }
            }
            if (_anim_init) Execute();
            _anim_init = false;
        }
    }

    void LateUpdate()
    {
        //face to
        UpdateRotation();
        if (!string.IsNullOrEmpty(trigger) && ator != null && !ator.IsInTransition(0))
        {
            if (trigger != "ToStand" &&
                trigger != "ToMove" &&
                trigger != "EndSkill" &&
                trigger != "ToUltraShow")
                _anim_init = true; // is casting

            ator.speed = 1;
            if (SkillData.TypeToken == 3)
            {
                int i = 0;
                for (; i < XSkillData.Combined_Command.Length; i++)
                {
                    if (trigger == XSkillData.Combined_Command[i]) break;
                }
                if (i < XSkillData.Combined_Command.Length)
                    ator.Play(XSkillData.CombinedOverrideMap[i], 1, _time_offset);
                else
                    ator.SetTrigger(trigger);
            }
            else
            {
                ator.SetTrigger(trigger);
            }
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
            trigger = xOuterData.SkillPosition > 0 ? XSkillData.JA_Command[xOuterData.SkillPosition] : "ToSkill";
        else if (xOuterData.TypeToken == 1)
            trigger = "ToArtSkill";
        else if (xOuterData.TypeToken == 3)
            Combined(0);
        else
            trigger = "ToUltraShow";

        FocusTarget();
        _anim_init = false;
    }

    public void StopFire()
    {
        if (_state != DummyState.Fire) return;
        _state = DummyState.Idle;
        trigger = "EndSkill";
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

    private void Combined(object param)
    {
    }

    private void FocusTarget()
    {
        XHitHoster hit = GameObject.FindObjectOfType<XHitHoster>();
        _target = (xOuterData.NeedTarget && hit != null) ? hit.gameObject : null;
        if (_target != null && IsInAttckField(xOuterData, transform.position, transform.forward, _target))
        {
            PrepareRotation(XCommon.singleton.Horizontal(_target.transform.position - transform.position), _xConfigData.RotateSpeed);
        }
    }

    public bool IsPickedInRange(int n, int d)
    {
        if (n >= d) return true;
        int i = Random.Range(0, d);
        return i < n;
    }

    public bool IsInField(XSkillData data, int triggerTime, Vector3 pos, Vector3 forward, Vector3 target, float angle, float distance)
    {
        bool log = true;
        if (data.Warning != null && data.Warning.Count > 0)
        {
            for (int i = 0; i < data.Warning.Count; i++)
            {
                if (data.Warning[i].RandomWarningPos || data.Warning[i].Type == XWarningType.Warning_Multiple)
                {
                    log = false; break;
                }
            }
        }
        if (data.Result[triggerTime].Sector_Type)
        {
            if (!(distance >= data.Result[triggerTime].Low_Range &&
                   distance < data.Result[triggerTime].Range &&
                    angle <= data.Result[triggerTime].Scope * 0.5f))
            {
                if (log)
                {
                    XDebug.Log("-----------------------------------");
                    XDebug.Log("At " + triggerTime, " Hit missing: distance is " + distance.ToString("F3"), " ( >= " + data.Result[triggerTime].Low_Range.ToString("F3") + ")");
                    XDebug.Log("At " + triggerTime, " Hit missing: distance is " + distance.ToString("F3"), " ( < " + data.Result[triggerTime].Range.ToString("F3") + ")");
                    XDebug.Log("At " + triggerTime, " Hit missing: dir is " + angle.ToString("F3"), " ( < " + (data.Result[triggerTime].Scope * 0.5f).ToString("F3") + ")");
                }
                return false;
            }
        }
        else
        {
            if (!IsInAttackRect(target, pos, forward, data.Result[triggerTime].Range, data.Result[triggerTime].Scope, data.Result[triggerTime].Rect_HalfEffect, data.Result[triggerTime].None_Sector_Angle_Shift))
            {
                float d = data.Result[triggerTime].Range;
                float w = data.Result[triggerTime].Scope;

                Vector3[] vecs = new Vector3[4];
                vecs[0] = new Vector3(-w / 2.0f, 0, data.Result[triggerTime].Rect_HalfEffect ? 0 : (-d / 2.0f));
                vecs[1] = new Vector3(-w / 2.0f, 0, d / 2.0f);
                vecs[2] = new Vector3(w / 2.0f, 0, d / 2.0f);
                vecs[3] = new Vector3(w / 2.0f, 0, data.Result[triggerTime].Rect_HalfEffect ? 0 : (-d / 2.0f));

                if (log)
                {
                    XDebug.Log("-----------------------------------");
                    XDebug.Log("Not in rect " + vecs[0], " " + vecs[1], " " + vecs[2], " " + vecs[3]);
                }
                return false;
            }
        }
        return true;
    }

    private bool IsInAttckField(XSkillData data, Vector3 pos, Vector3 forward, GameObject target)
    {
        forward = XCommon.singleton.HorizontalRotateVetor3(forward, data.Cast_Scope_Shift);
        Vector3 targetPos = target.transform.position;
        if (data.Cast_Range_Rect)
        {
            pos.x += data.Cast_Offset_X;
            pos.z += data.Cast_Offset_Z;
            return IsInAttackRect(targetPos, pos, forward, data.Cast_Range_Upper, data.Cast_Scope, false, 0);
        }
        else
        {
            Vector3 dir = targetPos - pos;
            dir.y = 0;
            float distance = dir.magnitude;
            //normalize
            dir.Normalize();
            float angle = (distance == 0) ? 0 : Vector3.Angle(forward, dir);
            return distance <= data.Cast_Range_Upper &&
                distance >= data.Cast_Range_Lower &&
                angle <= data.Cast_Scope * 0.5f;
        }
    }

    private bool IsInAttackRect(Vector3 target, Vector3 anchor, Vector3 forward, float d, float w, bool half, float shift)
    {
        Quaternion rotation = XCommon.singleton.VectorToQuaternion(XCommon.singleton.HorizontalRotateVetor3(forward, shift));
        Rect rect = new Rect();
        rect.xMin = -w / 2.0f;
        rect.xMax = w / 2.0f;
        rect.yMin = half ? 0 : (-d / 2.0f);
        rect.yMax = d / 2.0f;
        return XCommon.singleton.IsInRect(target - anchor, rect, Vector3.zero, rotation);
    }

    private bool CanAct(Vector3 dir)
    {
        bool can = false;
        float now = Time.time - fire_time;
        XLogicalData logic = (SkillData.TypeToken == 3) ? SkillData.Logical : _current.Logical;
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
        AnimationClip clip = Resources.Load(SkillData.ClipName) as AnimationClip;
        if (oVerrideController == null) BuildOverride();
        if (SkillData.TypeToken == 0)
        {
            string motion = XSkillData.JaOverrideMap[SkillData.SkillPosition];
            oVerrideController[motion] = clip;

            foreach (XJADataExtraEx ja in SkillDataExtra.JaEx)
            {
                if (SkillData.SkillPosition == 15)  //ToJA_QTE
                    continue;

                if (ja.Next != null && ja.Next.Name.Length > 0) oVerrideController[XSkillData.JaOverrideMap[ja.Next.SkillPosition]] = Resources.Load(ja.Next.ClipName) as AnimationClip;
                if (ja.Ja != null && ja.Ja.Name.Length > 0) oVerrideController[XSkillData.JaOverrideMap[ja.Ja.SkillPosition]] = Resources.Load(ja.Ja.ClipName) as AnimationClip;
            }
        }
        else if (SkillData.TypeToken == 3) { }
        else
        {
            oVerrideController["Art"] = clip;
        }

        _present_data = XTableMgr.GetTable<XEntityPresentation>().GetItemID((uint)_xConfigData.Player);
        oVerrideController["Idle"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackIdle) as AnimationClip;
        oVerrideController["Run"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackRun) as AnimationClip;
        oVerrideController["Walk"] = Resources.Load("Animation/" + _present_data.AnimLocation + _present_data.AttackWalk) as AnimationClip;
    }

    public void FetchDataBack()
    {
        _xData = sData.Get();
        _xEditorData = sEditorData.Get();
        _xConfigData = sConfigData.Get();

        //XDataBuilder.singleton.HotBuild(this, _xConfigData);
        //XDataBuilder.singleton.HotBuildEx(this, _xConfigData);
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

    public void SetCurrData(XSkillData data)
    {
        _current = data;
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

