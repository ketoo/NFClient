// -------------------------------------------------------------------------
//    @FileName			:    NFChatLogic.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFChatLogic
//
// -------------------------------------------------------------------------

#include "stdafx.h"
#include "NFChatLogic.h"
#include "NFComm/NFMessageDefine/NFMsgDefine.h"
#include "NFComm/NFMessageDefine/NFProtocolDefine.hpp"
#include "NFNetLogic.h"
#include "NFPlayerLogic.h"

bool NFChatLogic::Init()
{
    return true;
}

bool NFChatLogic::Shut()
{
    return true;
}

bool NFChatLogic::ReadyExecute()
{
    return true;
}

bool NFChatLogic::Execute()
{


    return true;
}

bool NFChatLogic::AfterInit()
{
	NFLogicBase::AfterInit();
	g_pNetLogic->AddReceiveCallBack(NFMsg::EGMI_ACK_CHAT, this, &NFChatLogic::OnChatProcess);
    return true;
}
//--------------------------------------------发消息-------------------------------------------------------------
void NFChatLogic::RequireChatWorld(std::string strContent)
{
	NFMsg::ReqAckPlayerChat xMsg;
	xMsg.set_player_name(g_pPlayerLogic->GetRoleInfo().noob_name());
	*xMsg.mutable_player_id() = NFINetModule::NFToPB(g_pPlayerLogic->GetRoleGuid());
	xMsg.set_chat_info(strContent);
	xMsg.set_chat_channel(NFMsg::ReqAckPlayerChat::EGCC_GLOBAL);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_CHAT, xMsg);
}

void NFChatLogic::RequireChatGuild(std::string strContent)
{
	NFMsg::ReqAckPlayerChat xMsg;
	xMsg.set_player_name(g_pPlayerLogic->GetRoleInfo().noob_name());
	*xMsg.mutable_player_id() = NFINetModule::NFToPB(g_pPlayerLogic->GetRoleGuid());
	xMsg.set_chat_info(strContent);
	xMsg.set_chat_channel(NFMsg::ReqAckPlayerChat::EGCC_CLAN);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_CHAT, xMsg);
}

void NFChatLogic::RequireChatPrivate(const NFGUID &target, std::string strContent)
{
	NFMsg::ReqAckPlayerChat xMsg;
	xMsg.set_player_name(g_pPlayerLogic->GetRoleInfo().noob_name());
	*xMsg.mutable_player_id() = NFINetModule::NFToPB(g_pPlayerLogic->GetRoleGuid());
	*xMsg.mutable_target_id() = NFINetModule::NFToPB(target);
	xMsg.set_chat_info(strContent);
	xMsg.set_chat_channel(NFMsg::ReqAckPlayerChat::EGCC_FRIEND);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_CHAT, xMsg);
}

void NFChatLogic::RequireChatTeam(std::string strContent)
{
	NFMsg::ReqAckPlayerChat xMsg;
	xMsg.set_player_name(g_pPlayerLogic->GetRoleInfo().noob_name());
	*xMsg.mutable_player_id() = NFINetModule::NFToPB(g_pPlayerLogic->GetRoleGuid());
	xMsg.set_chat_info(strContent);
	xMsg.set_chat_channel(NFMsg::ReqAckPlayerChat::EGCC_TEAM);
	g_pNetLogic->SendToServerByPB(NFMsg::EGameMsgID::EGMI_REQ_CHAT, xMsg);
}

//--------------------------------------------收消息-------------------------------------------------------------
void NFChatLogic::OnChatProcess(const NFSOCK nSockIndex, const int nMsgID, const char* msg, const uint32_t nLen)
{
	NFGUID nPlayerID;
	NFMsg::ReqAckPlayerChat xMsg;
	if (!NFINetModule::ReceivePB(nMsgID, msg, nLen, xMsg, nPlayerID))
	{
		return;
	}

	NFDataList var;
	var.AddObject(NFINetModule::PBToNF(xMsg.player_id()));
	var.AddInt(xMsg.chat_type());
	var.AddString(xMsg.player_name());
	var.AddString(xMsg.chat_info());

	switch (xMsg.chat_type())
	{
	case NFMsg::ReqAckPlayerChat::EGCC_GLOBAL:
	{
		DoEvent(E_ChatEvent_ChatGlobal, var);
	}
	break;
	case NFMsg::ReqAckPlayerChat::EGCC_CLAN:
	{
		DoEvent(E_ChatEvent_ChatClan, var);
	}
	break;
	case NFMsg::ReqAckPlayerChat::EGCC_FRIEND:
	{
		DoEvent(E_ChatEvent_ChatFriend, var);
	}
	break;
	case NFMsg::ReqAckPlayerChat::EGCC_BATTLE:
	{
		DoEvent(E_ChatEvent_ChatBattle, var);
	}
	break;
	case NFMsg::ReqAckPlayerChat::EGCC_TEAM:
	{
		DoEvent(E_ChatEvent_ChatTeam, var);
	}
	break;
	case NFMsg::ReqAckPlayerChat::EGCC_ROOM:
	{
		DoEvent(E_ChatEvent_ChatRoom, var);
	}
	break;
	default:
		break;;
	}
}
