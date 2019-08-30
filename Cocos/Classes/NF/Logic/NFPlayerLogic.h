// -------------------------------------------------------------------------
//    @FileName			:    NFPlayerLogic.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFPlayerLogic
//
// -------------------------------------------------------------------------

#ifndef NF_PLAYERLOGIC_H
#define NF_PLAYERLOGIC_H

#include "NFLogicBase.h"

enum PlayerLogicEvent
{
	E_PlayerEvent_RoleList,
	E_PlayerEvent_PlayerMove,
	E_PlayerEvent_PlayerJump,
};

class NFPlayerLogic
    : public NFLogicBase, public NFSingleton<NFPlayerLogic>
{
public:
	NFPlayerLogic() {};
	virtual ~NFPlayerLogic() {};
    NFPlayerLogic(NFIPluginManager* p)
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
	void RequireRoleList();
	void RequireCreateRole(string strRoleName, int byCareer, int bySex);
	void RequireEnterGameServer(int nRoleIndex);
	void RequireMove(NFVector3 pos);

	// 接收消息
private:
	void OnRoleList(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	// 进入和离开
	void OnObjectEntry(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	void OnObjectLeave(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	// 移动
	void OnObjectMove(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);
	void OnObjectJump(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen);

private:
	void AddRecord(const NF_SHARE_PTR<NFIObject>& object, const std::string &strRecordName, const NFMsg::RecordAddRowStruct &data);

public:
	std::vector<NFMsg::RoleLiteInfo> GetRoleList() { return m_RoleList; }
	const NFMsg::RoleLiteInfo& GetRoleInfo() { return m_RoleList[m_nRoleIndex]; }
	const NFGUID& GetRoleGuid() { return m_RoleGuid; }

private:
	std::vector<NFMsg::RoleLiteInfo> m_RoleList;
	int m_nRoleIndex;
	NFGUID m_RoleGuid;
};

#define g_pPlayerLogic (NFPlayerLogic::Instance())

#endif