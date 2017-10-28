using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using System.Collections.Generic;

namespace AI
{
    public class AITreeImpleted 
    {
        public static XEntity Transform2Entity(Transform t)
        {
            uint id = uint.Parse(t.name);
            return XEntityMgr.singleton.GetEntity(id);
        }

        public static TaskStatus NavToTargetUpdate(XEntity entity,GameObject mAIArgTarget,GameObject mAIArgNavTarget,Vector3 mAIArgNavPos)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (mAIArgTarget == null)
            {
                if (mAIArgNavTarget == null)
                {
                    if (mAIArgNavPos == Vector3.zero)
                        return TaskStatus.Failure;
                    else
                    {
                        if (ActionNav(entity, mAIArgNavPos))
                            return TaskStatus.Success;
                        else
                            return TaskStatus.Failure;
                    }
                }
                else
                {
                    if (NavToTarget(entity, mAIArgNavTarget))
                        return TaskStatus.Success;
                    else
                        return TaskStatus.Failure;
                }
            }
            else
            {
                if (NavToTarget(entity, mAIArgTarget))
                    return TaskStatus.Success;
                else
                    return TaskStatus.Failure;
            }
        }

        public static TaskStatus StopNavMoveUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && StopNavMove(entity))
                return TaskStatus.Success;
            return TaskStatus.Failure;
        }

        public static TaskStatus RotateToTargetUpdate(XEntity entity, GameObject mAIArgTarget)
        {
            if (XEntity.Valide(entity) && RotateToTarget(entity.EntityTransfer, mAIArgTarget))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus DetectEnemyInSightUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && DetectEnemyInSight(entity))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }


        public static TaskStatus FindTargetByDistanceUpdate(XEntity entity,float mAIArgDistance,float mAIArgAngle)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().FindTargetByDistance(mAIArgDistance, mAIArgAngle))
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus DoSelectNearestUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().DoSelectNearest())
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }


        public static TaskStatus DoSelectFarthestUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().DoSelectFarthest())
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus DoSelectRandomTargetUpdate(XEntity entity)
        {
            if (entity != null && entity.GetComponent<XAIComponent>().DoSelectRandom())
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }


        public static TaskStatus CalDistanceUpdate(XEntity entity, Transform mAIArgObject, float mAIArgDistance, Vector3 mAIArgDestPoint)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (mAIArgObject != null)
            {
                mAIArgDistance = (entity.Position - mAIArgObject.position).magnitude;
            }
            else
            {
                mAIArgDistance = (entity.Position - mAIArgDestPoint).magnitude;
            }
            return TaskStatus.Success;
        }

        public static TaskStatus SelectMoveTargetByIdUpdate(XEntity entity, Transform mAIArgMoveTarget, int mAIArgObjectId)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            Transform moveTarget = SelectMoveTargetById(entity, mAIArgObjectId);
            if (moveTarget == null)
                return TaskStatus.Failure;
            else
            {
                mAIArgMoveTarget = moveTarget;
                return TaskStatus.Success;
            }
        }

        public static TaskStatus ValueHPUpdate(XEntity entity, int mAIArgMaxHP,int mAIArgMinHP)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            double hp = entity.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentHP_Basic);
            if (hp >= mAIArgMinHP && hp <= mAIArgMaxHP)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus ValueMPUpdate(XEntity entity, int mAIArgMaxMP, int mAIArgMinMP)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            double hp = entity.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentMP_Basic);
            if (hp >= mAIArgMinMP && hp <= mAIArgMaxMP)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus ValueTargetUpdate(XEntity entity, Transform mAIArgTarget)
        {
            if (XEntity.Valide(mAIArgTarget))
                return TaskStatus.Success;
            else
            {
                if (XEntity.Valide(entity))
                {
                    entity.GetComponent<XAIComponent>().SetTarget(null);
                }
                return TaskStatus.Failure;
            }
        }

        public static TaskStatus ValueDistanceUpdate(XEntity entity, GameObject mAIArgTarget,float mAIArgMaxDistance)
        {
            if (mAIArgTarget == null)
                return TaskStatus.Failure;

            float dis = (entity.Position - mAIArgTarget.transform.position).magnitude;
            if (dis <= mAIArgMaxDistance)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus IsOppoCastingSkillUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsOppoCastingSkill)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }


        public static TaskStatus IsHurtOppoUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsHurtOppo)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus IsFixedInCdUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsFixedInCd)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus IsCastingSkillUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsCastingSkill)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus StatusIdleUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Idle)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus StatusMoveUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Move)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus StatusBehitUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;

            if (entity.CurState == XStateDefine.XState_BeHit)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }


        public static TaskStatus StatusDeathUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Death)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus StatusFreezeUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Freeze)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }

        public static TaskStatus StatusSkillUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return TaskStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Skill)
                return TaskStatus.Success;
            else
                return TaskStatus.Failure;
        }









        public static Transform SelectMoveTargetById(XEntity entity, int objectid)
        {
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

        public static bool ActionNav(XEntity entity, Vector3 dest)
        {
            XNavComponent nav = entity.GetComponent<XNavComponent>();
            if (nav != null)
            {
                nav.Navigate(dest);
                entity.MoveForward(dest - entity.Position);
                return true;
            }
            return false;
        }


        public static bool NavToTarget(XEntity entity, GameObject target)
        {
            if (entity == null) return false;
            if (target != null)
            {
                XNavComponent nav = entity.GetComponent<XNavComponent>();
                if (nav != null)
                {
                    nav.Navigate(target.transform.position);
                    entity.MoveForward(target.transform.position - entity.Position);
                    return true;
                }
            }
            return false;
        }

        public static bool StopNavMove(XEntity entity)
        {
            if (entity == null) return false;
            XNavComponent nav = entity.GetComponent<XNavComponent>();
            if (nav == null) return false;
            nav.NavEnd();
            return true;
        }

        public static bool RotateToTarget(Transform src, GameObject target)
        {
            if (src != null && target != null)
            {
                Vector3 dir = target.transform.position - src.position;
                src.forward = dir;
                return true;
            }
            return false;
        }

        public static bool DetectEnemyInSight(XEntity e)
        {
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
