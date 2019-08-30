// -------------------------------------------------------------------------
//    @FileName			:    NFPropertyLogic.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFPropertyLogic
//
// -------------------------------------------------------------------------

#ifndef NF_PropertyLogic_MODULE_H
#define NF_PropertyLogic_MODULE_H

#include "NFLogicBase.h"

enum PropertyLogicEvent
{
};

class NFPropertyLogic
    : public NFLogicBase, public NFSingleton<NFPropertyLogic>
{
public:
	NFPropertyLogic() {};
	virtual ~NFPropertyLogic() {};
    NFPropertyLogic(NFIPluginManager* p)
    {
        pPluginManager = p;
    }

    virtual bool Init();
    virtual bool Shut();
    virtual bool ReadyExecute();
    virtual bool Execute();

    virtual bool AfterInit();

	// 发送消息
public:
	// 接收消息
private:
	// 属性
	void OnPropertyInt(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	void OnPropertyFloat(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	void OnPropertyString(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	void OnPropertyObject(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	void OnObjectPropertyEntry(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
};

#define g_pPropertyLogic (NFPropertyLogic::Instance())

#endif