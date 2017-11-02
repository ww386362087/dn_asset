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
    public float defaultFov = 45;

    private XEntityPresentation.RowData _present_data = null;
    
    private string trigger = null;
    public Animator ator = null;
    private DummyState _state = DummyState.Idle;
    private XSkillCamera _camera = null;
    private XSkillData _current = null;
    private XSkillAttributes _attribute;
    

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

    public XSkillAttributes Attribute { get { return _attribute; } }

    public XEntityPresentation.RowData Present_data { get { return _present_data; } }

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
        _attribute = new XSkillAttributes(this, transform);
    }

    void OnDrawGizmos()
    {
        DrawManipulationFileds();
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


    private void Execute()
    {
        if (_current == null) return;
        if (_attribute != null) _attribute.Execute();
    }


    void Update()
    {
        if (_state != DummyState.Fire)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (_xData.TypeToken == 1 && ComboSkills.Count > 0) oVerrideController["Art"] = Resources.Load(_xData.ClipName) as AnimationClip;
                _current = _xData;
                Fire();
            }
            _action_framecount = 0;
        }
        else
        {
            _action_framecount += Time.deltaTime;
            if (_action_framecount > _current.Time) StopFire();
        }
    }

    void LateUpdate()
    {
        if (_attribute != null) _attribute.UpdateRotation();
        if (!string.IsNullOrEmpty(trigger) && ator != null && !ator.IsInTransition(0))
        {
            if (trigger != AnimTriger.ToStand &&
                trigger != AnimTriger.ToMove &&
                trigger != AnimTriger.EndSkill)
                Execute(); // is casting

            ator.speed = 1;
            ator.SetTrigger(trigger);
            trigger = null;
        }
    }


    private void Fire()
    {
        _state = DummyState.Fire;
      
        if (_current.TypeToken == 0)
            trigger = XSkillData.JA_Command[_current.SkillPosition];
        else if (_current.TypeToken == 1)
            trigger = AnimTriger.ToArtSkill;

        FocusTarget();
    }

    public void StopFire()
    {
        if (_state != DummyState.Fire) return;
        _state = DummyState.Idle;
        trigger = AnimTriger.EndSkill;

        if (_attribute != null) _attribute.Clear();
        _action_framecount = 0;
        _camera.EndEffect(null);
        nResultForward = Vector3.zero;
        Time.timeScale = 1;
        if (ator != null) ator.speed = 1;
    }


    private void FocusTarget()
    {
        XHitHoster hit = GameObject.FindObjectOfType<XHitHoster>();
        _target = (_current.NeedTarget && hit != null) ? hit.gameObject : null;
        if (_target != null && _current.IsInAttckField(transform.position, transform.forward, _target))
        {
            if (_attribute != null)
            {
                _attribute.PrepareRotation(XCommon.singleton.Horizontal(_target.transform.position - transform.position));
                _attribute.rotate_speed = _xConfigData.RotateSpeed;
            }
        }
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

