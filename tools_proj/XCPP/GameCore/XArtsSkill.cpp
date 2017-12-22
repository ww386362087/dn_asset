#include "XArtsSkill.h"
#include "XEntity.h"

XArtsSkill::XArtsSkill(XEntity* firer) :XSkill(firer)
{
}

XArtsSkill::~XArtsSkill()
{
}

void XArtsSkill::FireEvents()
{
	/*for (size_t i = 0; i < _data->Result.size(); i++)
	{
		_data->Result[i].Token = i;
		float delay = _data->Result[i].At;
		AddedTimerToken(
			XCommon::SetTimer(
				delay * _time_scale,
				CALLBACK(XSkill, OnResult, this),
				reinterpret_cast<XResultData*>(&_data->Result[i]),
				__FILE__,
				__LINE__));
	}

	for (size_t i = 0; i < _data->Charge.size(); i++)
	{
		float charge_at = _data->Charge[i].Using_Curve ? 0 : _data->Charge[i].At;
		AddedTimerToken(
			XCommon::SetTimer(
				charge_at * _time_scale,
				CALLBACK(XArtsSkill, ChargeTo, this),
				reinterpret_cast<void*>(i),
				__FILE__,
				__LINE__));
	}

	for (size_t i = 0; i < _data->Manipulation.size(); i++)
	{
		AddedTimerToken(
			XCommon::SetTimer(
				_data->Manipulation[i].At * _time_scale,
				CALLBACK(XArtsSkill, Manipulate, this),
				reinterpret_cast<void*>(&_data->Manipulation[i]),
				__FILE__,
				__LINE__));
	}

	for (size_t i = 0; i < _data->Warning.size(); i++)
	{
		AddedTimerToken(
			XCommon::SetTimer(
				_data->Warning[i].At * _time_scale,
				CALLBACK(XArtsSkill, Warning, this),
				reinterpret_cast<XWarningData*>(&_data->Warning[i]),
				__FILE__,
				__LINE__));
	}

	for (size_t i = 0; i < _data->Mobs.size(); i++)
	{
		if (_data->Mobs[i].TemplateID > 0)
		{
			AddedTimerToken(
				XCommon::SetTimer(
					_data->Mobs[i].At * _time_scale,
					CALLBACK(XArtsSkill, Mob, this),
					reinterpret_cast<XMobUnitData*>(&_data->Mobs[i]),
					__FILE__, __LINE__));
		}
	}*/
}

//skill do self stoppage here
void XArtsSkill::Stop()
{
	/*if (_data->Manipulation.size() > 0)
	{
		XManipulationOffArgs args;
		args.DenyToken = 0;
		_firer->DispatchEvent(&args);
	}

	for (size_t i = 0; i < _mob_unit.size(); i++)
	{
		Unit* unit = Unit::FindUnit(_mob_unit[i]);
		if (unit && !unit->IsDead())
		{
			XEntity* e = dynamic_cast<XEntity*>(unit->GetXObject());
			if (e && e->IsLifewithinMobbedSkill())
				unit->TriggerDeath();
		}
	}
	_mob_unit.clear();*/
}



bool XArtsSkill::ChargeTo(IArgs*, void* param)
{
	int n = reinterpret_cast<intptr_t>(param);
	int i = n & 0xFF;
	float offset = (n >> 16) / 1000.0f;
	XChargeActionArgs args(&_data->Charge[i]);
	args.TimeGone = offset;
	args.TimeScale = _time_scale;
	args.Target = _data->Charge[i].AimTarget ? _target : 0;
	args.TimeSpan = _data->Charge[i].End - (_data->Charge[i].Using_Curve ? 0 : _data->Charge[i].At);
	if (!_data->Charge[i].Using_Curve && args.TimeSpan <= 0)
	{
		ERROR("Zero Charge TimeSpan of skill " + tostring(_data->Name));
		return false;
	}
	else
		return _firer->DispatchEvent(&args);
}

bool XArtsSkill::Manipulate(IArgs*, void* param)
{
	/*XManipulationData* data = reinterpret_cast<XManipulationData*>(param);

	XManipulationOnArgs args(data);
	int token = (int)(args.GetToken());
	_firer->DispatchAction(&args);

	AddedTimerToken(
		XCommon::SetTimer(
		(data->End - data->At) * _time_scale,
			CALLBACK(XArtsSkill, KillManipulate, this),
			reinterpret_cast<void*>(token),
			__FILE__, __LINE__));*/

	return true;
}

bool XArtsSkill::KillManipulate(IArgs*, void* param)
{
	/*XManipulationOffArgs args;
	args.DenyToken = reinterpret_cast<intptr_t>(param);
	_firer->DispatchEvent(&args);*/

	return true;
}



bool XArtsSkill::Warning(IArgs*, void* param)
{
	//XWarningData* data = reinterpret_cast<XWarningData*>(param);
	//Scene* pScene = _firer->GetUnit()->GetCurrScene();
	//std::vector<XSkillCore::XSkillWarningPackage>& list = _core->WarningPosAt[data->Index];
	//list.clear();

	//if (data->RandomWarningPos || data->Type == Warning_Multiple)
	//{
	//	uint extra_id = 0;

	//	auto it = _core->WarningRandomItems[data->Index].begin();
	//	while (it != _core->WarningRandomItems[data->Index].end())
	//	{
	//		XEntity* e = (it->ID == 0) ? Target() : XEntity::ValidEntity(it->ID);

	//		auto is = it->Pos.begin();
	//		while (is != it->Pos.end())
	//		{
	//			++extra_id;

	//			if (e)
	//			{
	//				uint m = *is;
	//				float r = (m & 0xFFFF) / 10.0f;
	//				uint d = (m >> 16);
	//				XSkillCore::XSkillWarningPackage pakcage;
	//				pakcage.WarningAt = e->GetPosition_p() + (r * Vector3::HorizontalRotate(Vector3::forward, d));
	//				pakcage.WarningTo = extra_id;
	//				if (pScene) pScene->GetGrid()->TryGetHeight(
	//					pakcage.WarningAt.x,
	//					pakcage.WarningAt.z,
	//					pakcage.WarningAt.y);

	//				list.push_back(pakcage);
	//			}
	//			++is;
	//		}
	//		++it;
	//	}
	//}
	//else
	//{
	//	switch (data->Type)
	//	{
	//	case Warning_None:
	//	{
	//		XSkillCore::XSkillWarningPackage pakcage;
	//		const Vector3 v(data->OffsetX, data->OffsetY, data->OffsetZ);
	//		float magniatude = v.IsZero() ? 0 : Vector3::Magnitude(v);
	//		const Vector3& offset = (magniatude == 0) ? Vector3::zero : Vector3::HorizontalRotate(v, _firer->GetFaceDegree()) * magniatude;

	//		pakcage.WarningAt = Firer()->GetPosition_p() + offset;
	//		pakcage.WarningTo = 0;
	//		if (pScene) pScene->GetGrid()->TryGetHeight(
	//			pakcage.WarningAt.x,
	//			pakcage.WarningAt.z,
	//			pakcage.WarningAt.y);
	//		list.push_back(pakcage);
	//	}break;
	//	case Warning_Target:
	//	{
	//		XSkillCore::XSkillWarningPackage pakcage;
	//		if (HasValidTarget())
	//		{
	//			pakcage.WarningAt = Target()->GetPosition_p();
	//			pakcage.WarningTo = Target()->GetID();
	//			if (pScene) pScene->GetGrid()->TryGetHeight(
	//				pakcage.WarningAt.x,
	//				pakcage.WarningAt.z,
	//				pakcage.WarningAt.y);
	//		}
	//		else
	//		{
	//			const Vector3 v(data->OffsetX, data->OffsetY, data->OffsetZ);
	//			float magniatude = v.IsZero() ? 0 : Vector3::Magnitude(v);
	//			const Vector3& offset = (magniatude == 0) ? Vector3::zero : Vector3::HorizontalRotate(v, _firer->GetFaceDegree()) * magniatude;

	//			pakcage.WarningAt = Firer()->GetPosition_p() + offset;
	//			pakcage.WarningTo = 0;
	//			if (pScene) pScene->GetGrid()->TryGetHeight(
	//				pakcage.WarningAt.x,
	//				pakcage.WarningAt.z,
	//				pakcage.WarningAt.y);
	//		}

	//		list.push_back(pakcage);
	//	}break;
	//	case Warning_All:
	//	{
	//		const std::vector<Unit*>* enemies = pScene->GetOpponents(_firer->GetUnit()->GetFightGroup());
	//		if (NULL == enemies)
	//		{
	//			return true;
	//		}

	//		for (size_t i = 0; i < (*enemies).size(); i++)
	//		{
	//			XEntity* obj = dynamic_cast<XEntity*>((*enemies)[i]->GetXObject());

	//			if (obj)
	//			{
	//				if (obj->MobbedBy() != 0 && !data->Mobs_Inclusived) continue;

	//				XSkillCore::XSkillWarningPackage pakcage;
	//				pakcage.WarningAt = obj->GetPosition_p();
	//				pakcage.WarningTo = obj->GetID();
	//				if (pScene) pScene->GetGrid()->TryGetHeight(
	//					pakcage.WarningAt.x,
	//					pakcage.WarningAt.z,
	//					pakcage.WarningAt.y);

	//				list.push_back(pakcage);
	//			}
	//		}
	//	}break;
	//	case Warning_Multiple:
	//		break;
	//	}
	//}

	return true;
}


bool XArtsSkill::Mob(IArgs*, void* param)
{
	//XMobUnitData* data = reinterpret_cast<XMobUnitData*>(param);

	//Vector3 offset(data->Offset_At_X, data->Offset_At_Y, data->Offset_At_Z);

	//float yRot = Vector3::Angle(Vector3::forward, _firer->GetFace_p());
	//if (!Vector3::Clockwise(Vector3::forward, _firer->GetFace_p())) yRot = -yRot;

	//offset = Vector3::HorizontalRotate(offset, yRot, false);

	//Vector3 mobat = _firer->GetPosition_p() + offset;
	//Scene* pScene = _firer->GetUnit()->GetCurrScene();
	//if (pScene)
	//{
	//	if (!pScene->GetGrid()->TryGetHeight(mobat.x, mobat.z, mobat.y) || mobat.y < 0)
	//	{
	//		mobat = _firer->GetPosition_p();
	//	}

	//	Enemy* enemy = EnemyManager::Instance()->CreateEnemyByCaller(
	//		_firer->GetUnit(), data->TemplateID, mobat, _firer->GetFaceDegree(), data->Shield);

	//	if (enemy)
	//	{
	//		XEntity* pent = dynamic_cast<XEntity*>(enemy->GetXObject());
	//		if (pent)
	//		{
	//			pent->SetMobber(_firer->GetID());
	//			pent->SetLifewithinMobbedSkill(data->LifewithinSkill);

	//			if (_firer->SkillComponent()->AddSkillMob(pent))
	//			{
	//				XSkillEffectMgr::Instance()->SetMobProperty(enemy, _firer->GetUnit(), Core()->ID());

	//				//log security
	//				if (_firer->GetUnit()->GetSecurityStatistics() && _firer->GetUnit()->GetSecurityStatistics()->_AIInfo)
	//					_firer->GetUnit()->GetSecurityStatistics()->_AIInfo->OnExternalCallMonster();

	//				_mob_unit.push_back(enemy->GetID());
	//			}
	//			else
	//			{
	//				/*
	//				* enemy already be triggered death during AddSkillMob or
	//				* already in skill mobs set
	//				*/
	//			}
	//		}
	//	}
	//}

	return true;
}

XBullet* XArtsSkill::GenerateBullet(XResultData* data, XEntity* target, int additional, int loop, int group, int wid, bool extrainfo)
{
	int diviation = data->LongAttackData.FireAngle + additional;

	//XBullet* pb = new XBullet(
	//	false,
	//	(((uint)(loop)) << 56) | (((uint)(group)) << 48) | (((uint)(GetCombinedId())) << 40) | (((uint)(data->Index)) << 32) | (uint)_token,
	//	new XBulletCore(
	//		_token,
	//		_firer->GetID(),
	//		target,
	//		_core,
	//		data->Index,
	//		MainCore()->ID(),
	//		diviation,
	//		wid));

	//if (extrainfo) pb->SetExtraID(target ? target->GetID() : _core->WarningPosAt[data->Warning_Idx][wid].WarningTo);

	//return pb;
	return 0;
}