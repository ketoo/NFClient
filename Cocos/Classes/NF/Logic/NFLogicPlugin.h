// -------------------------------------------------------------------------
//    @FileName			:    NFLogicPlugin.h
//    @Author           :    Johance
//    @Date             :    2016-12-26
//    @Module           :    NFLogicPlugin
//
// -------------------------------------------------------------------------

#ifndef NF_LOGIC_PLUGIN_H
#define NF_LOGIC_PLUGIN_H

#include "NFComm/NFPluginModule/NFIPlugin.h"
#include "NFComm/NFPluginModule/NFIPluginManager.h"


class NFLogicPlugin : public NFIPlugin
{
public:
    NFLogicPlugin(NFIPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual const int GetPluginVersion();

    virtual const std::string GetPluginName();

    virtual void Install();

    virtual void Uninstall();
};

#endif