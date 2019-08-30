// -------------------------------------------------------------------------
//    @FileName			:    NFUIPlugin.h
//    @Author           :    Johance
//    @Date             :    2016-12-26
//    @Module           :    NFUIPlugin
//
// -------------------------------------------------------------------------

#ifndef UI_PLUGIN_H
#define UI_PLUGIN_H

#include "NFComm/NFPluginModule/NFIPlugin.h"
#include "NFComm/NFPluginModule/NFIPluginManager.h"


class NFUIPlugin : public NFIPlugin
{
public:
    NFUIPlugin(NFIPluginManager* p)
    {
        pPluginManager = p;
    }
    virtual const int GetPluginVersion();

    virtual const std::string GetPluginName();

    virtual void Install();

    virtual void Uninstall();
};

#endif