using UnityEngine;

public class NativeNPC : NativeEntity
{
    protected CharacterController controller;

    protected override void OnInitial()
    {
        base.OnInitial();
        EnableShadow(true);
        //controller = EntityObject.GetComponent<CharacterController>();
        //controller.enabled = false;
    }


    protected override void InitAnim()
    {
        OverrideAnim(Clip.Idle, _present.Idle);
    }

    private void EnableShadow(bool able)
    {
        if (skin == null)
        {
            skin = transfrom.GetComponentInChildren<SkinnedMeshRenderer>();
            skin.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
    }

}

