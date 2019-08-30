// -------------------------------------------------------------------------
//    @FileName			:    NFScenePlugin.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-26
//    @Module           :    NFScenePlugin
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFScenePlugin.h"

#include "NFSceneManager.h"

const int NFScenePlugin::GetPluginVersion()
{
    return 0;
}

const std::string NFScenePlugin::GetPluginName()
{
	return GET_CLASS_NAME(NFScenePlugin);
}

void NFScenePlugin::Install()
{
	REGISTER_MODULE(pPluginManager, NFSceneManager, NFSceneManager)
}

void NFScenePlugin::Uninstall()
{
	UNREGISTER_MODULE(pPluginManager, NFSceneManager, NFSceneManager)
}
