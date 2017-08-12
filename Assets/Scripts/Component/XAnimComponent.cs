using UnityEngine;
using XTable;

public class XAnimComponent : XComponent
{
    private Animator m_Animator;
    private AnimatorOverrideController m_overrideController = null;
    private string m_stateName = "";
    private int m_playLayer = 0;
    private bool m_crossFade = false;
    //表示一个常数持有负无穷大
    private float m_normalizedTime = float.NegativeInfinity;
    private string m_triggerName = "";
    private float m_speed = 1;
    private float m_value = 0;
    private bool m_enable = true;

    static XEntityPresentation.RowData epRow;
    public override void OnInitial(XEntity _entity)
    {
        base.OnInitial(_entity);
        m_Animator = _entity.EntityObject.GetComponent<Animator>();
        if (m_Animator.runtimeAnimatorController is AnimatorOverrideController)
        {
            m_overrideController = m_Animator.runtimeAnimatorController as AnimatorOverrideController;
        }
        else
        {
            m_overrideController = new AnimatorOverrideController();
            m_overrideController.runtimeAnimatorController = m_Animator.runtimeAnimatorController;
            m_Animator.runtimeAnimatorController = m_overrideController;
        }
        m_Animator.Rebind();
        if (epRow == null)
        {
            uint archerid = 2;
            XEntityPresentation p = new XEntityPresentation();
            epRow = p.GetItemID(archerid);
        }
        OverrideAnims();
    }

    public override void OnUninit()
    {
        Reset();
        base.OnUninit();
    }


    public void OverrideAnims()
    {
        OverrideAnim("A", epRow.AnimLocation + epRow.A);
        OverrideAnim("AA", epRow.AnimLocation + epRow.AA);
        OverrideAnim("AAA", epRow.AnimLocation + "Player_archer_attack_run");
        OverrideAnim("AAAA", epRow.AnimLocation + "Player_archer_attack_run");
        OverrideAnim("AAAAA", epRow.AnimLocation + epRow.AAAAA);
        OverrideAnim("Walk", epRow.AnimLocation + epRow.Walk);
        OverrideAnim("Idle", epRow.AnimLocation + epRow.Idle);
        OverrideAnim("Death", epRow.AnimLocation + epRow.Death);
        OverrideAnim("Run", epRow.AnimLocation + epRow.Run);
        OverrideAnim("RunLeft", epRow.AnimLocation + epRow.RunLeft);
        OverrideAnim("RunRight", epRow.AnimLocation + epRow.RunRight);
        OverrideAnim("Freezed", epRow.AnimLocation + epRow.Freeze);
        OverrideAnim("HitLanding", epRow.AnimLocation + epRow.Hit_l);
        OverrideAnim("Phase0", epRow.AnimLocation + "Player_archer_jump");
        OverrideAnim("Phase1", epRow.AnimLocation + "Player_archer_attack_lifttwinshot");
        OverrideAnim("Phase2", epRow.AnimLocation + "Player_archer_attack_aerialchainshot");
        OverrideAnim("Art", epRow.AnimLocation + "Player_archer_victory");
    }

    public void SyncSpeed(float speed)
    {
        if (m_Animator != null)
        {
            m_speed = speed;
            m_Animator.speed = m_speed;
        }
    }

    public void CrossFade(string stateName, float transitionDuration, int layer, float normalizedTime)
    {
        m_stateName = stateName;
        m_value = transitionDuration;
        m_playLayer = layer;
        m_normalizedTime = normalizedTime;
        m_crossFade = true;
        if (IsAnimStateValid())
        {
            RealPlay();
        }
    }

    public void SetTrigger(string name)
    {
        m_triggerName = name;
        if (m_Animator != null && m_Animator != null)
        {
            m_Animator.SetTrigger(m_triggerName);
        }
    }

    public void SyncEnable(bool enable)
    {
        if (m_Animator != null)
        {
            m_enable = enable;
            m_Animator.enabled = m_enable;
        }
    }

    public void Play(string stateName, int layer, float normalizedTime)
    {
        m_stateName = stateName;
        m_playLayer = layer;
        m_normalizedTime = normalizedTime;
        m_crossFade = false;
        if (IsAnimStateValid())
        {
            RealPlay();
        }
    }

    public void Play(string stateName, int layer)
    {
        m_stateName = stateName;
        m_playLayer = layer;
        m_normalizedTime = float.NegativeInfinity;
        m_crossFade = false;
        if (IsAnimStateValid())
        {
            RealPlay();
        }
    }


    public void RealPlay()
    {
        if (m_Animator != null)
        {
            if (m_crossFade)
            {
                m_Animator.CrossFade(m_stateName, m_value, m_playLayer, m_normalizedTime);
            }
            else
            {
                m_Animator.Play(m_stateName, m_playLayer, m_normalizedTime);
            }
        }
    }

    public bool IsAnimStateValid()
    {
        if (m_Animator == null) return false;
        if (string.IsNullOrEmpty(m_stateName))
            return false;
        return true;
    }


    public void OverrideAnim(string key, string clippath)
    {
        if (string.IsNullOrEmpty(clippath) || m_Animator == null || m_overrideController == null)
            return;
        m_overrideController[key] = XResourceMgr.Load<AnimationClip>("Animation/" + clippath, AssetType.Anim);
    }

    public void Reset()
    {
        if (m_Animator != null)
        {
            m_Animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            m_Animator.enabled = false;
            m_Animator = null;
        }
        ResetClips();
        m_overrideController = null;
        m_stateName = "";
        m_value = -1;
        m_playLayer = -1;
        m_normalizedTime = float.NegativeInfinity;
        m_triggerName = "";
    }


    private void ResetClips()
    {
        if (m_overrideController != null)
        {
            AnimationClipPair[] clips = m_overrideController.clips;
            for (int i = 0; i < clips.Length; ++i)
            {
                AnimationClipPair clip = clips[i];
                if (clip.overrideClip != null)
                {
                    m_overrideController[clip.originalClip.name] = null;
                }
            }
        }
    }

}
