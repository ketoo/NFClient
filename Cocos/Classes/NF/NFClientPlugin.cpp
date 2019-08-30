// -------------------------------------------------------------------------
//    @FileName			:    NFClientPlugin.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-27
//    @Module           :    NFClientPlugin
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFClientPlugin.h"

#include "NFComm/NFConfigPlugin/NFClassModule.h"
#include "NFComm/NFConfigPlugin/NFElementModule.h"
#include "NFComm/NFLogPlugin/NFLogModule.h"
#include "NFComm/NFLogPlugin/NFLogModule.h"

#include "NFComm/NFKernelPlugin/NFKernelModule.h"
#include "NFComm/NFKernelPlugin/NFEventModule.h"
#include "NFComm/NFKernelPlugin/NFScheduleModule.h"

#include "NFComm/NFNetPlugin/NFNetClientModule.h"

const int NFClientPlugin::GetPluginVersion()
{
    return 0;
}

const std::string NFClientPlugin::GetPluginName()
{
	return GET_CLASS_NAME(NFClientPlugin);
}

void NFClientPlugin::Install()
{
    REGISTER_MODULE(pPluginManager, NFIClassModule, NFClassModule)
    REGISTER_MODULE(pPluginManager, NFIElementModule, NFElementModule)
    REGISTER_MODULE(pPluginManager, NFILogModule, NFLogModule)
	REGISTER_MODULE(pPluginManager, NFIKernelModule, NFKernelModule)
	REGISTER_MODULE(pPluginManager, NFIEventModule, NFEventModule)
	REGISTER_MODULE(pPluginManager, NFIScheduleModule, NFScheduleModule)
	
    REGISTER_MODULE(pPluginManager, NFINetClientModule, NFNetClientModule)
}

void NFClientPlugin::Uninstall()
{
    UNREGISTER_MODULE(pPluginManager, NFIElementModule, NFElementModule)
    UNREGISTER_MODULE(pPluginManager, NFIClassModule, NFClassModule)
    UNREGISTER_MODULE(pPluginManager, NFILogModule, NFLogModule)
	UNREGISTER_MODULE(pPluginManager, NFIEventModule, NFEventModule)
	UNREGISTER_MODULE(pPluginManager, NFIKernelModule, NFKernelModule)
	UNREGISTER_MODULE(pPluginManager, NFIScheduleModule, NFScheduleModule)

    UNREGISTER_MODULE(pPluginManager, NFINetClientModule, NFNetClientModule)
}
