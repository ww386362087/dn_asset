using UnityEngine;
using System.Collections.Generic;

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


        public Transform SelectMoveTargetById(Transform transf, int objectid)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(transf.name));
            List<XEntity> ens = XEntityMgr.singleton.GetAllEnemy(entity);
            for (int i = 0, max = ens.Count; i < max; i++)
            {
                if (XEntity.Valide(ens[i]) && ens[i].Attributes.TypeID == objectid)
                {
                    return ens[i].EntityObject.transform;
                }
            }
            return null;
        }

        //only editor use
        public bool ActionNav(GameObject go, Vector3 dest)
        {
            return ActionNav(uint.Parse(go.transform.name), dest);
        }

        public bool ActionNav(uint id, Vector3 dest)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            XNavComponent nav = entity.GetComponent<XNavComponent>();
            if (nav != null) { nav.Navigate(dest); return true; }
            return false;
        }

        public bool NavToTarget(GameObject go, GameObject target)
        {
            return NavToTarget(uint.Parse(go.transform.name), target);
        }

        public bool NavToTarget(uint id, GameObject target)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            if (entity == null) return false;
            if (target != null)
            {
                XNavComponent nav = entity.GetComponent<XNavComponent>();
                if (nav != null)
                {
                    nav.Navigate(target.transform.position);
                    return true;
                }
            }
            return false;
        }


        public bool RotateToTarget(Transform go)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(go.name));
            XDebug.Log(entity.Attributes.Name);
            return true;
        }

        public bool DetectEnemyInSight(Transform transf)
        {
            XEntity e = XEntityMgr.singleton.GetEntity(uint.Parse(transf.name));
            List<XEntity> opponent = XEntityMgr.singleton.GetAllEnemy(e);
            for (int i = 0; i < opponent.Count; i++)
            {
                if (!XEntity.Valide(opponent[i])) continue;
                Vector3 dir = opponent[i].Position - e.Position;
                float distance = dir.sqrMagnitude;
                //一旦在视野范围，就激活仇恨列表
                if (distance < e.Attributes.EnterFightRange * e.Attributes.EnterFightRange)
                {
                    XAIComponent ai = opponent[i].GetComponent<XAIComponent>();
                    ai.SetTarget(e);
                    return true;
                }
            }
            return false;
        }


    }
}
