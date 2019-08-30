// -------------------------------------------------------------------------
//    @FileName			:    NFUIPlugin.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-26
//    @Module           :    NFUIPlugin
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFUIPlugin.h"

#include "NFUIManager.h"

const int NFUIPlugin::GetPluginVersion()
{
    return 0;
}

const std::string NFUIPlugin::GetPluginName()
{
	return GET_CLASS_NAME(NFUIPlugin);
}

void NFUIPlugin::Install()
{
	REGISTER_MODULE(pPluginManager, NFUIManager, NFUIManager)
}

void NFUIPlugin::Uninstall()
{
	UNREGISTER_MODULE(pPluginManager, NFUIManager, NFUIManager)
}
