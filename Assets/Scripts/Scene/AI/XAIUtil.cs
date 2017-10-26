using UnityEngine;
using System.Collections.Generic;

namespace AI
{
    public class XAIUtil
    {

        public static bool FindTargetByDistance(Transform trans, float distance, float angle)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(trans.name));
            return entity.GetComponent<XAIComponent>().FindTargetByDistance(distance, angle);
        }

        public static bool DoSelectNearest(Transform tran)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(tran.name));
            return entity.GetComponent<XAIComponent>().DoSelectNearest();
        }

        public static bool DoSelectFarthest(Transform tran)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(tran.name));
            return entity.GetComponent<XAIComponent>().DoSelectFarthest();
        }

        public static bool DoSelectRandomTarget(Transform tran)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(tran.name));
            return entity.GetComponent<XAIComponent>().DoSelectRandom();
        }

        public static Transform SelectMoveTargetById(Transform transf, int objectid)
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
        public static bool ActionNav(Transform tr, Vector3 dest)
        {
            return ActionNav(uint.Parse(tr.name), dest);
        }

        public static bool ActionNav(uint id, Vector3 dest)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(id);
            XNavComponent nav = entity.GetComponent<XNavComponent>();
            if (nav != null)
            {
                nav.Navigate(dest);
                return true;
            }
            return false;
        }

        public static bool NavToTarget(Transform tr, GameObject target)
        {
            return NavToTarget(uint.Parse(tr.name), target);
        }

        public static bool NavToTarget(uint id, GameObject target)
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


        public static bool RotateToTarget(Transform go)
        {
            XEntity entity = XEntityMgr.singleton.GetEntity(uint.Parse(go.name));
            XDebug.Log(entity.Attributes.Name);
            return true;
        }

        public static bool DetectEnemyInSight(Transform transf)
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
