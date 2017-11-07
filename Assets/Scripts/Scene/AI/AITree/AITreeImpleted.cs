using UnityEngine;
using System.Collections.Generic;
using AI.Runtime;

namespace AI
{
    public class AITreeImpleted 
    {
        public static XEntity Transform2Entity(Transform t)
        {
            uint id = uint.Parse(t.name);
            return XEntityMgr.singleton.GetEntity(id);
        }

        public static AIRuntimeStatus NavToTargetUpdate(XEntity entity,GameObject mAIArgTarget,GameObject mAIArgNavTarget,Vector3 mAIArgNavPos)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            if (mAIArgTarget == null)
            {
                if (mAIArgNavTarget == null)
                {
                    if (mAIArgNavPos == Vector3.zero)
                        return AIRuntimeStatus.Failure;
                    else
                    {
                        if (ActionNav(entity, mAIArgNavPos))
                            return AIRuntimeStatus.Success;
                        else
                            return AIRuntimeStatus.Failure;
                    }
                }
                else
                {
                    if (NavToTarget(entity, mAIArgNavTarget))
                        return AIRuntimeStatus.Success;
                    else
                        return AIRuntimeStatus.Failure;
                }
            }
            else
            {
                if (NavToTarget(entity, mAIArgTarget))
                    return AIRuntimeStatus.Success;
                else
                    return AIRuntimeStatus.Failure;
            }
        }
        
        public static AIRuntimeStatus StopNavMoveUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && StopNavMove(entity))
                return AIRuntimeStatus.Success;
            return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus RotateToTargetUpdate(XEntity entity, GameObject mAIArgTarget, float ang)
        {
            if (XEntity.Valide(entity) && RotateToTarget(entity.EntityTransfer, mAIArgTarget, ang))
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus RotateSelfUpdate(XEntity entity, float max,float min)
        {
            if (XEntity.Valide(entity))
            {
                float ang = Random.Range(min, max);
                Vector3 dir = XCommon.singleton.HorizontalRotateVetor3(entity.Forward, ang);
                entity.EntityTransfer.forward = dir;
                return AIRuntimeStatus.Success;
            }
            return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus MoveForwardUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity))
            {
                entity.MoveForward(entity.EntityTransfer.forward);
                return AIRuntimeStatus.Success;
            }
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus DetectEnemyInSightUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && DetectEnemyInSight(entity))
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus FindTargetByDistanceUpdate(XEntity entity,float mAIArgDistance,float mAIArgAngle)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().FindTargetByDistance(mAIArgDistance, mAIArgAngle))
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus ResetTargetUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().ResetTarget())
                return AIRuntimeStatus.Success;
            return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus DoSelectNearestUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().DoSelectNearest())
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus DoSelectFarthestUpdate(XEntity entity)
        {
            if (XEntity.Valide(entity) && entity.GetComponent<XAIComponent>().DoSelectFarthest())
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus DoSelectRandomTargetUpdate(XEntity entity)
        {
            if (entity != null && entity.GetComponent<XAIComponent>().DoSelectRandom())
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus CalDistanceUpdate(XEntity entity, Transform mAIArgObject, float mAIArgDistance, Vector3 mAIArgDestPoint)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            if (mAIArgObject != null)
            {
                mAIArgDistance = (entity.Position - mAIArgObject.position).magnitude;
            }
            else
            {
                mAIArgDistance = (entity.Position - mAIArgDestPoint).magnitude;
            }
            return AIRuntimeStatus.Success;
        }

        public static AIRuntimeStatus SelectMoveTargetByIdUpdate(XEntity entity, Transform mAIArgMoveTarget, int mAIArgObjectId)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            Transform moveTarget = SelectMoveTargetById(entity, mAIArgObjectId);
            if (moveTarget == null)
                return AIRuntimeStatus.Failure;
            else
            {
                mAIArgMoveTarget = moveTarget;
                return AIRuntimeStatus.Success;
            }
        }

        public static AIRuntimeStatus ValueHPUpdate(XEntity entity, int mAIArgMaxHP,int mAIArgMinHP)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            double hp = entity.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentHP_Basic);
            if (hp >= mAIArgMinHP && hp <= mAIArgMaxHP)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus ValueMPUpdate(XEntity entity, int mAIArgMaxMP, int mAIArgMinMP)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            double hp = entity.Attributes.GetAttr(XAttributeDefine.XAttr_CurrentMP_Basic);
            if (hp >= mAIArgMinMP && hp <= mAIArgMaxMP)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus ValueTargetUpdate(XEntity entity, Transform mAIArgTarget)
        {
            if (XEntity.Valide(mAIArgTarget))
                return AIRuntimeStatus.Success;
            else
            {
                if (XEntity.Valide(entity))
                {
                    entity.GetComponent<XAIComponent>().SetTarget(null);
                }
                return AIRuntimeStatus.Failure;
            }
        }

        public static AIRuntimeStatus ValueDistanceUpdate(XEntity entity, GameObject mAIArgTarget, float mAIArgMaxDistance)
        {
            if (mAIArgTarget == null)
                return AIRuntimeStatus.Failure;

            float dis = (entity.Position - mAIArgTarget.transform.position).sqrMagnitude;
            if (dis <= mAIArgMaxDistance * mAIArgMaxDistance)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus StatusRandomUpdate(XEntity e, int prob)
        {
            if (prob < 0 || prob > 100)
            {
                return AIRuntimeStatus.Failure;
            }
            else
            {
                int rand = Random.Range(0, 100);
                if (rand >= prob)
                    return AIRuntimeStatus.Success;
                else return AIRuntimeStatus.Failure;
            }
        }

        public static AIRuntimeStatus IsOppoCastingSkillUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsOppoCastingSkill)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus IsHurtOppoUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsHurtOppo)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus IsFixedInCdUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsFixedInCd)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus IsCastingSkillUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;
            if (entity.GetComponent<XAIComponent>().IsCastingSkill)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus StatusIdleUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Idle)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus StatusMoveUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Move)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus StatusBehitUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;

            if (entity.CurState == XStateDefine.XState_BeHit)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus StatusDeathUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Death)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus StatusFreezeUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Freeze)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus StatusSkillUpdate(XEntity entity)
        {
            if (!XEntity.Valide(entity)) return AIRuntimeStatus.Failure;

            if (entity.CurState == XStateDefine.XState_Skill)
                return AIRuntimeStatus.Success;
            else
                return AIRuntimeStatus.Failure;
        }

        public static AIRuntimeStatus LogUpdate(XEntity entity, string str, bool b)
        {
            if (b) XDebug.LogError(str);
            else XDebug.Log(str);
            return AIRuntimeStatus.Success;
        }

        public static AIRuntimeStatus WaitUpdate(XEntity entity, float wait, bool rand, float min, float max)
        {
            if (entity.SetTimer(wait)) return AIRuntimeStatus.Success;
            return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus XAIActionSkillUpdate(XEntity entity, string scr, GameObject target)
        {
            var skill = entity.GetComponent<XSkillComponent>();
            if (skill != null && target != null)
            {
                if (!skill.IsCasting)
                    skill.CastSkill(scr);
                else
                    return AIRuntimeStatus.Running;
            }
            return AIRuntimeStatus.Failure;
        }


        public static AIRuntimeStatus RandomCompareUpdate(XEntity entity, int prob)
        {
            int rand = Random.Range(0, 100);
            if (prob > rand)
            {
                return AIRuntimeStatus.Success;
            }
            else
            {
                return AIRuntimeStatus.Failure;
            }
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

        public static bool RotateToTarget(Transform src, GameObject target,float ang)
        {
            if (src != null && target != null)
            {
                Vector3 dir = target.transform.position - src.position;
                dir = XCommon.singleton.HorizontalRotateVetor3(dir, ang);
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
