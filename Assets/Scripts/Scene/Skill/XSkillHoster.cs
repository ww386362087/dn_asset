using System.Collections.Generic;
using UnityEngine;
using XEditor;
using XTable;

public class XSkillHoster : MonoBehaviour
{
    [SerializeField]
    private XSkillData _xData = null;
    [SerializeField]
    private XSkillDataExtra _xDataExtra = null;
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

    private XEntityPresentation.RowData _present_data = null;


    private enum DummyState { Idle, Move, Fire };

    private DummyState _state = DummyState.Idle;

    private Animator _ator = null;

     private XSkillCamera _camera = null;

    private XSkillData _current = null;

    public float defaultFov = 45;

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
            if (_xDataExtra == null) _xDataExtra = new XSkillDataExtra();
            return _xDataExtra;
        }
    }

    public XSkillData SkillData
    {
        get
        {
            if (_xData == null) _xData = new XSkillData();
            return _xData;
        }
        set
        {
            _xData = value;
        }
    }

    public XSkillData CurrentSkillData
    {
        get { return _state == DummyState.Fire ? _current : SkillData; }
    }


    void Awake()
    {
        ShownTransform = transform;
        GameEnine.Init(this);
    }

    void Start()
    {
        _state = DummyState.Idle;
        if (oVerrideController == null) BuildOverride();

        _camera = new XSkillCamera(gameObject);
        _camera.Initialize();
        _camera.UnityCamera.fieldOfView = defaultFov;

        Light light = _camera.UnityCamera.gameObject.AddComponent<Light>() as Light;
        light.type = LightType.Directional;
        light.intensity = 0.5f;

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
                        float x = CurrentSkillData.Result[nHotID].Low_Range / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = CurrentSkillData.Result[nHotID].Low_Range / ShownTransform.localScale.y * Mathf.Sin(theta);
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

                    Vector3 beginPoint2 = Vector3.zero;
                    Vector3 firstPoint2 = Vector3.zero;

                    for (float theta = 0; theta < 2 * Mathf.PI; theta += m_Theta)
                    {
                        float x = or / ShownTransform.localScale.y * Mathf.Cos(theta);
                        float z = or / ShownTransform.localScale.y * Mathf.Sin(theta);
                        Vector3 endPoint = new Vector3(x, 0, z);
                        if (theta == 0)
                        {
                            firstPoint2 = endPoint;
                        }
                        else
                        {
                            Gizmos.DrawLine(beginPoint2, endPoint);
                        }
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

    void OnGUI()
    {
        GUI.Label(_rect, "Action Frame: " + _action_framecount);
    }

    private int _comboskill_index = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_comboskill_index < ComboSkills.Count)
            {
                //XSkillData data = ComboSkills[_comboskill_index];
                //if (CanReplacedBy(data))
                //{
                //    _comboskill_index++;
                //    StopFire();

                //    _xOuterData = data;
                //    _current = data;

                //    if (data.TypeToken != 3)
                //    {
                //        oVerrideController["Art"] = XResourceLoaderMgr.singleton.GetSharedResource<AnimationClip>(data.ClipName, ".anim");
                //        _trigger = "ToArtSkill";
                //    }
                //    else
                //    {
                //        _combinedlist.Clear();
                //        for (int i = 0; i < data.Combined.Count; i++)
                //        {
                //            XSkillData x = XResourceLoaderMgr.singleton.GetData<XSkillData>("SkillPackage/" + XAnimationLibrary.AssociatedAnimations((uint)_xConfigData.Player).SkillLocation + data.Combined[i].Name);
                //            AnimationClip c = Resources.Load(x.ClipName) as AnimationClip;
                //            oVerrideController[XSkillData.CombinedOverrideMap[i]] = c;
                //            _combinedlist.Add(x);
                //        }
                //        _trigger = XSkillData.Combined_Command[0];
                //        Combined(0);
                //    }

                //    _state = DummyState.Fire;
                //    _fire_time = Time.time;
                //    _delta = 0;
                //    if (_ator != null) _ator.speed = 0;
                //}
            }
        }
    }

    private void BuildOverride()
    {
        oVerrideController = new AnimatorOverrideController();

        _ator = GetComponent<Animator>();
        if (_ator != null)
        {
            oVerrideController.runtimeAnimatorController = _ator.runtimeAnimatorController;
            _ator.runtimeAnimatorController = oVerrideController;
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
        //else if (SkillData.TypeToken == 3)
        //{
        //    for (int i = 0; i < SkillData.Combined.Count; i++)
        //    {
        //        oVerrideController[XSkillData.CombinedOverrideMap[i]] = SkillDataExtra.CombinedEx[i].Clip;
        //    }
        //}
        else
        {
            oVerrideController["Art"] = clip;
        }

        _present_data = XEntityPresentation.sington.GetItemID((uint)_xConfigData.Player);

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


}

