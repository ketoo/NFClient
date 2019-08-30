// -------------------------------------------------------------------------
//    @FileName			:    NFLogicBase.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFLogicBase
//
// -------------------------------------------------------------------------

#include "NFLogicBase.h"

#include "NFComm/NFPluginModule/NFILogModule.h"
#include "NFComm/NFPluginModule/NFIKernelModule.h"
#include "NFComm/NFPluginModule/NFIClassModule.h"
#include "NFComm/NFPluginModule/NFIElementModule.h"
#include "NFComm/NFPluginModule/NFIScheduleModule.h"
#include "NFComm/NFPluginModule/NFIEventModule.h"
#include "NFComm/NFPluginModule/NFIKernelModule.h"
#include "NFComm/NFPluginModule/NFINetClientModule.h"

NFIClassModule *g_pClassModule = NULL;
NFIElementModule *g_pElementModule = NULL;
NFILogModule *g_pLogModule = NULL;
NFIKernelModule *g_pKernelModule = NULL;
NFIEventModule *g_pEventModule = NULL;
NFIScheduleModule *g_pScheduleModule = NULL;
NFINetClientModule *g_pNetClientModule = NULL;

bool NFLogicBase::Init()
{
    return true;
}

bool NFLogicBase::Shut()
{
    return true;
}

bool NFLogicBase::ReadyExecute()
{
    return true;
}

bool NFLogicBase::Execute()
{


    return true;
}

bool NFLogicBase::AfterInit()
{
	static bool bInitGlobal = false;
	if(!bInitGlobal)
	{
		g_pClassModule = pPluginManager->FindModule<NFIClassModule>();
		g_pElementModule = pPluginManager->FindModule<NFIElementModule>();
		g_pKernelModule = pPluginManager->FindModule<NFIKernelModule>();
		g_pLogModule = pPluginManager->FindModule<NFILogModule>();
		g_pScheduleModule = pPluginManager->FindModule<NFIScheduleModule>();
		g_pEventModule = pPluginManager->FindModule<NFIEventModule>();
		g_pNetClientModule = pPluginManager->FindModule<NFINetClientModule>();

		bInitGlobal = true;
	}

    return true;
}


bool NFLogicBase::DoEvent(const int nEventID, const NFDataList & valueList)
{
	auto listElement = mModuleEventInfoMapEx[nEventID];
	for (auto itr = listElement.begin(); itr != listElement.end(); ++itr)
	{
		if (mModuleRemoveSet.find(*itr) == mModuleRemoveSet.end())
		{
			MODULE_EVENT_FUNCTOR* pFunc = (*itr).get();
			(*pFunc)(nEventID, valueList);
		}
	}

	return true;
}

bool NFLogicBase::RemoveEventCallBack(const int nEventID, void *pTarget)
{
	auto &listElement = mModuleEventInfoMapEx[nEventID];
	for (auto itr = listElement.begin(); itr != listElement.end(); )
	{
		auto pFunPtr = mModuleEventPrtMap[(*itr).get()];
		if (pTarget == pFunPtr)
		{
			mModuleRemoveSet.insert(*itr);
			itr = listElement.erase(itr);
			continue;
		}
		++itr;
	}

	return true;
}

bool NFLogicBase::AddEventCallBack(const int nEventID, const MODULE_EVENT_FUNCTOR_PTR cb)
{
	auto &listElement = mModuleEventInfoMapEx[nEventID];
	listElement.push_back(cb);
	return false;
}