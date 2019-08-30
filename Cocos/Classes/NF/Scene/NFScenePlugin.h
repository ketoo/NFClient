// -------------------------------------------------------------------------
//    @FileName			:    NFScenePlugin.h
//    @Author           :    Johance
//    @Date             :    2016-12-26
//    @Module           :    NFScenePlugin
//
// -------------------------------------------------------------------------

#ifndef NFScenePlugin_H
#define NFScenePlugin_H

#include "NFComm/NFPluginModule/NFIPlugin.h"
#include "NFComm/NFPluginModule/NFIPluginManager.h"


class NFScenePlugin : public NFIPlugin
{
public:
    NFScenePlugin(NFIPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual const int GetPluginVersion();

    virtual const std::string GetPluginName();

    virtual void Install();

    virtual void Uninstall();
};

#endif