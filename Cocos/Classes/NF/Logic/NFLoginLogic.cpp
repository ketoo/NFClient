// -------------------------------------------------------------------------
//    @FileName			:    NFLoginLogic.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFLoginLogic
//
// -------------------------------------------------------------------------
#include "stdafx.h"
#include "NFLoginLogic.h"
#include "NFComm/NFMessageDefine/NFMsgDefine.h"
#include "NFComm/NFMessageDefine/NFProtocolDefine.hpp"
#include "NFNetLogic.h"

bool NFLoginLogic::Init()
{
    return true;
}

bool NFLoginLogic::Shut()
{
    return true;
}

bool NFLoginLogic::ReadyExecute()
{
    return true;
}

bool NFLoginLogic::Execute()
{


    return true;
}

bool NFLoginLogic::AfterInit()
{
	NFLogicBase::AfterInit();
	g_pNetLogic->AddReceiveCallBack(NFMsg::EGMI_ACK_LOGIN, this, &NFLoginLogic::OnLoginProcess);
	g_pNetLogic->AddReceiveCallBack(NFMsg::EGMI_ACK_WORLD_LIST, this, &NFLoginLogic::OnWorldList);
	g_pNetLogic->AddReceiveCallBack(NFMsg::EGMI_ACK_CONNECT_WORLD, this, &NFLoginLogic::OnConnectWorld);
	g_pNetLogic->AddReceiveCallBack(NFMsg::EGMI_ACK_CONNECT_KEY, this, &NFLoginLogic::OnConnectKey);
	g_pNetLogic->AddReceiveCallBack(NFMsg::EGMI_ACK_SELECT_SERVER, this, &NFLoginLogic::OnSelectServer);
    return true;
}
//--------------------------------------------发消息-------------------------------------------------------------
void NFLoginLogic::LoginPB(const std::string &strAccount, const std::string &strPwd, const std::string &strKey)
{
	NFMsg::ReqAccountLogin xMsg;
	xMsg.set_security_code("");
	xMsg.set_signbuff("");
	xMsg.set_clientversion(1);
	xMsg.set_loginmode(NFMsg::ELoginMode::ELM_AUTO_REGISTER_LOGIN);
	xMsg.set_clientip(0);
	xMsg.set_clientip(0);
	xMsg.set_clientmac(0);
	xMsg.set_device_info("");
	xMsg.set_extra_info("");

	m_strAccount = strAccount;
	m_strKey = strKey;

	xMsg.set_account(strAccount);
	xMsg.set_password(strPwd);

	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_LOGIN, xMsg);
}

void NFLoginLogic::RequireWorldList()
{
	NFMsg::ReqServerList xMsg;
	xMsg.set_type(NFMsg::RSLT_WORLD_SERVER);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_WORLD_LIST, xMsg);
}

void NFLoginLogic::RequireConnectWorld(int nWorldID)
{
	m_nServerID = nWorldID;
	NFMsg::ReqConnectWorld xMsg;
	xMsg.set_world_id(nWorldID);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_CONNECT_WORLD, xMsg);
}

void NFLoginLogic::RequireVerifyWorldKey(const std::string &strAccount, const std::string &strKey)
{
	NFMsg::ReqAccountLogin xMsg;
	xMsg.set_signbuff("");
	xMsg.set_clientversion(1);
	xMsg.set_loginmode(NFMsg::ELoginMode::ELM_AUTO_REGISTER_LOGIN);
	xMsg.set_clientip(0);
	xMsg.set_clientip(0);
	xMsg.set_clientmac(0);
	xMsg.set_device_info("");
	xMsg.set_extra_info("");
	xMsg.set_password("");

	xMsg.set_account(strAccount);
	xMsg.set_security_code(strKey);

	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_CONNECT_KEY, xMsg);
}

void NFLoginLogic::RequireServerList()
{
	NFMsg::ReqServerList xMsg;
	xMsg.set_type(NFMsg::RSLT_GAMES_ERVER);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_WORLD_LIST, xMsg);
}

void NFLoginLogic::RequireSelectServer(int nServerID)
{
	m_nServerID = nServerID;
	NFMsg::ReqSelectServer xMsg;
	xMsg.set_world_id(nServerID);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_SELECT_SERVER, xMsg);
}
//--------------------------------------------收消息-------------------------------------------------------------
void NFLoginLogic::OnLoginProcess(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen)
{
	NFGUID nPlayerID;
	NFMsg::AckEventResult xMsg;
	if (!NFINetModule::ReceivePB(nMsgID, msg, nLen, xMsg, nPlayerID))
	{
		return;
	}

	if(NFMsg::EGEC_ACCOUNT_SUCCESS == xMsg.event_code())
	{
		CCLOG("Login Success");
		DoEvent(E_LoginEvent_LoginSuccess, NFDataList());
	}
	else
	{
		CCLOG("Login Failure(%d)", xMsg.event_code());
	}
}

void NFLoginLogic::OnWorldList(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen)
{
	NFGUID nPlayerID;
	NFMsg::AckServerList xMsg;
	if (!NFINetModule::ReceivePB(nMsgID, msg, nLen, xMsg, nPlayerID))
	{
		return;
	}

	if(xMsg.type() == NFMsg::RSLT_WORLD_SERVER)
	{
		m_WorldServerList.clear();
		m_WorldServerList.resize(xMsg.info_size());
		for(int i = 0; i < xMsg.info_size(); i++)
		{
			m_WorldServerList[i] = xMsg.info(i);
		}
		DoEvent(E_LoginEvent_WorldList, NFDataList());
	}
	else if(xMsg.type() == NFMsg::RSLT_GAMES_ERVER)
	{
		m_GameServerList.clear();
		m_GameServerList.resize(xMsg.info_size());
		for(int i = 0; i < xMsg.info_size(); i++)
		{
			m_GameServerList[i] = xMsg.info(i);
		}
		DoEvent(E_LoginEvent_ServerList, NFDataList());
	}
}

void NFLoginLogic::OnConnectWorld(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen)
{
	NFGUID nPlayerID;
	NFMsg::AckConnectWorldResult xMsg;
	if (!NFINetModule::ReceivePB(nMsgID, msg, nLen, xMsg, nPlayerID))
	{
		return;
	}

	m_strKey = xMsg.world_key();
	g_pNetLogic->ConnectServer(xMsg.world_ip(), xMsg.world_port());	

	RequireVerifyWorldKey(m_strAccount, m_strKey);
}

void NFLoginLogic::OnConnectKey(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen)
{
	NFGUID nPlayerID;
	NFMsg::AckEventResult xMsg;
	if (!NFINetModule::ReceivePB(nMsgID, msg, nLen, xMsg, nPlayerID))
	{
		return;
	}

	if(xMsg.event_code() == NFMsg::EGEC_VERIFY_KEY_SUCCESS)
	{
		RequireServerList();
	}
}

void NFLoginLogic::OnSelectServer(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen)
{
	NFGUID nPlayerID;
	NFMsg::AckEventResult xMsg;
	if (!NFINetModule::ReceivePB(nMsgID, msg, nLen, xMsg, nPlayerID))
	{
		return;
	}

	if(xMsg.event_code() == NFMsg::EGEC_SELECTSERVER_SUCCESS)
	{
		CCLOG("SelectGame SUCCESS");
	}
}