#include "AIFactory.h"
#include "runtime/AIRuntimeXAIActionSkill.h"
#include "runtime/AIRuntimeDoSelectNearest.h"
#include "runtime/AIRuntimeFindTargetByDistance.h"
#include "runtime/AIRuntimeLog.h"
#include "runtime/AIRuntimeMoveForward.h"
#include "runtime/AIRuntimeNavToTarget.h"
#include "runtime/AIRuntimeResetTarget.h"
#include "runtime/AIRuntimeRotateSelf.h"
#include "runtime/AIRuntimeRotateToTarget.h"
#include "runtime/AIRuntimeStatusRandom.h"
#include "runtime/AIRuntimeStopNavMove.h"
#include "runtime/AIRuntimeValueDistance.h"
#include "runtime/AIRuntimeValueHP.h"
#include "runtime/AIRuntimeWait.h"

AIFactory::AIFactory(void)
{
}


AIFactory::~AIFactory(void)
{
}


AIBase* AIFactory::MakeRuntime(AITaskData* data, AITree* tree)
{
	AIBase* rst = NULL;
	if (data == NULL)  return 0;
	if (data->type == "Sequence")
	{
		rst = new AISequence();
	}
	else if (data->type == "Selector")
	{
		rst = new AISelector();
	}
	else if (data->type == "Inverter")
	{
		rst = new AIInterval();
	}
	else if (data->type == "DoSelectNearest")
	{
		rst = new AIRuntimeDoSelectNearest();
	}
	else if (data->type == "FindTargetByDistance")
	{
		rst = new AIRuntimeFindTargetByDistance();
	}
	else if (data->type == "Log")
	{
		rst = new AIRuntimeLog();
	}
	else if (data->type == "MoveForward")
	{
		rst = new AIRuntimeMoveForward();
	}
	else if (data->type == "NavToTarget")
	{
		rst = new AIRuntimeNavToTarget();
	}
	else if (data->type == "ResetTarget")
	{
		rst = new AIRuntimeResetTarget();
	}
	else if (data->type == "RotateSelf")
	{
		rst = new AIRuntimeRotateSelf();
	}
	else if (data->type == "RotateToTarget")
	{
		rst = new AIRuntimeRotateToTarget();
	}
	else if (data->type == "StatusRandom")
	{
		rst = new AIRuntimeStatusRandom();
	}
	else if (data->type == "StopNavMove")
	{
		rst = new AIRuntimeStopNavMove();
	}
	else if (data->type == "ValueDistance")
	{
		rst = new AIRuntimeValueDistance();
	}
	else if (data->type == "ValueHP")
	{
		rst = new AIRuntimeValueHP();
	}
	else if (data->type == "Wait")
	{
		rst = new AIRuntimeWait();
	}
	else if (data->type == "XAIActionSkill")
	{
		rst = new AIRuntimeXAIActionSkill();
	}
	if (rst != NULL) 
	{
		rst->SetTree(tree);
		rst->Init(data);
	}
	return rst;
}