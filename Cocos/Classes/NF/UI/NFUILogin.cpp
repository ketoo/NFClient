// -------------------------------------------------------------------------
//    @FileName			:    NFUILogin.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUILogin
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFUILogin.h"
#include "Logic/NFLoginLogic.h"
#include "Logic/NFNetLogic.h"

#include "NFUISelectServer.h"

NFUILogin::NFUILogin()
{
}

NFUILogin::~NFUILogin()
{
	g_pLoginLogic->RemoveEventCallBack(E_LoginEvent_LoginSuccess, this);
}

bool NFUILogin::initLayout()
{	
	if(!NFUIDialog::initLayout())
		return false;
		
	GetObjectByName(&m_pLoginButton, "login_Button");
	GetObjectByName(&m_pUserName, "name_TextField");
	GetObjectByName(&m_pUserPWD, "password_TextField");

	m_pUserName->setString("test1");
	m_pUserPWD->setString("123456");

	m_pLoginButton->addClickEventListener(CC_CALLBACK_1(NFUILogin::onLoginTouch, this));
		
	g_pNetLogic->ConnectServer("104.160.35.67", 14001);
	//g_pNetLogic->ConnectServer("127.0.0.1", 14001);
	g_pLoginLogic->AddEventCallBack(E_LoginEvent_LoginSuccess, this, &NFUILogin::OnLoginEvent);
	return true;
}

void NFUILogin::initData(void *customData)
{
}


void NFUILogin::onLoginTouch(Ref *sender)
{
	g_pLoginLogic->LoginPB(m_pUserName->getString(), m_pUserPWD->getString(), "");
}

int NFUILogin::OnLoginEvent(const int nEventID, const NFDataList& varDataList)
{
	NFUISelectServer::showUI();
	return 0;
}
