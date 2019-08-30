// -------------------------------------------------------------------------
//    @FileName			:    NFLogicPlugin.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-26
//    @Module           :    NFLogicPlugin
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFLogicPlugin.h"

#include "NFLoginLogic.h"
#include "NFPlayerLogic.h"
#include "NFPropertyLogic.h"
#include "NFRecordLogic.h"
#include "NFNetLogic.h"
#include "NFChatLogic.h"

const int NFLogicPlugin::GetPluginVersion()
{
    return 0;
}

const std::string NFLogicPlugin::GetPluginName()
{
	return GET_CLASS_NAME(NFLogicPlugin);
}

void NFLogicPlugin::Install()
{
	REGISTER_MODULE(pPluginManager, NFNetLogic, NFNetLogic);
	REGISTER_MODULE(pPluginManager, NFLoginLogic, NFLoginLogic);
	REGISTER_MODULE(pPluginManager, NFPlayerLogic, NFPlayerLogic);
	REGISTER_MODULE(pPluginManager, NFPropertyLogic, NFPropertyLogic);
	REGISTER_MODULE(pPluginManager, NFRecordLogic, NFRecordLogic);
	REGISTER_MODULE(pPluginManager, NFChatLogic, NFChatLogic);
}

void NFLogicPlugin::Uninstall()
{
	UNREGISTER_MODULE(pPluginManager, NFNetLogic, NFNetLogic);
	UNREGISTER_MODULE(pPluginManager, NFLoginLogic, NFLoginLogic);
	UNREGISTER_MODULE(pPluginManager, NFPlayerLogic, NFPlayerLogic);
	UNREGISTER_MODULE(pPluginManager, NFPropertyLogic, NFPropertyLogic);
	UNREGISTER_MODULE(pPluginManager, NFRecordLogic, NFRecordLogic);
	UNREGISTER_MODULE(pPluginManager, NFChatLogic, NFChatLogic);
}
