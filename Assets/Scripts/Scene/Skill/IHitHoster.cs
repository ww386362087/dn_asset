
using UnityEngine;

public interface IHitHoster
{
    GameObject HitObject { get; }

    Vector3 Pos { get; }

    Vector3 RadiusCenter { get; }

    XHitAttribute Attr { get; }

    void Begin(ISkillHoster hoster, XHitData data, Vector3 dir, bool bAttackOnHitDown);


}
