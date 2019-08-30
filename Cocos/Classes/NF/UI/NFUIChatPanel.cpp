// -------------------------------------------------------------------------
//    @FileName			:    NFUIChatPanel.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUIChatPanel
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFUIChatPanel.h"
#include "Logic/NFChatLogic.h"
#include "Logic/NFNetLogic.h"

#include "NFUISelectServer.h"

NFUIChatPanel::NFUIChatPanel()
{
}

NFUIChatPanel::~NFUIChatPanel()
{
}

bool NFUIChatPanel::initLayout()
{	
	if(!NFUIDialog::initLayout())
		return false;
		
	GetObjectByName(&m_pSend, "m_pSend");
	GetObjectByName(&m_pInputText, "m_pInputText");

	m_pSend->addClickEventListener(CC_CALLBACK_1(NFUIChatPanel::onLoginTouch, this));
	return true;
}

void NFUIChatPanel::initData(void *customData)
{
}


void NFUIChatPanel::onLoginTouch(Ref *sender)
{
	g_pChatLogic->RequireChatWorld(m_pInputText->getString());
	m_pInputText->setString("");
}
