using UnityEngine;

namespace AI
{
    public class XAIGeneralMgr : XSingleton<XAIGeneralMgr>
    {

        public bool FindTargetByDistance(GameObject go, float distance, float angle)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(go.transform.name));
            return entity.GetComponent<XAIComponent>().FindTargetByDistance(distance, angle);
        }


        public bool DoSelectNearest(GameObject go)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(go.transform.name));
            return entity.GetComponent<XAIComponent>().DoSelectNearest();
        }


    }
}
