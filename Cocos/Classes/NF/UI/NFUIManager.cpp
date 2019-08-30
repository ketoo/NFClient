// -------------------------------------------------------------------------
//    @FileName			:    NFUIManager.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUIManager
//
// -------------------------------------------------------------------------

#include "stdafx.h"

#include "NFUIManager.h"

#include "NFRoot.h"
bool NFUIManager::Init()
{
	m_pCurrentDialog = NULL;
    m_pRootNode = CCNode::create();
    m_pRootNode->retain();

	NFRoot::Instance()->addChild(m_pRootNode, NFRoot::eIZO_UI);
    
    m_pPanelNode = CCNode::create();
    m_pRootNode->addChild(m_pPanelNode);

    m_pDialogNode = CCNode::create();
    m_pRootNode->addChild(m_pDialogNode);

    m_pBoxNode = CCNode::create();
    m_pRootNode->addChild(m_pBoxNode);

	m_pEffectNode = CCNode::create(); 
	m_pRootNode->addChild(m_pEffectNode);

	return true;
}
bool NFUIManager::Shut()
{
	if (NULL == m_pRootNode)
		return true;

	m_pRootNode->removeFromParentAndCleanup(true);
	CC_SAFE_RELEASE_NULL(m_pRootNode);
	
	m_pRootNode = NULL;
	m_pDialogNode = NULL;
	m_pPanelNode = NULL;
	m_pEffectNode = NULL;
	m_pBoxNode = NULL;

	return true;
}

bool NFUIManager::ReadyExecute()
{
	return true;
}

bool NFUIManager::Execute()
{
	return true;
}

bool NFUIManager::AfterInit()
{
	return true;
}

Node* NFUIManager::RootNode()
{
    return m_pRootNode;
}

NFUIDialog* NFUIManager::ShowDialog(NFUIDialog *pDialog, bool bPushToHistory/* = true*/)
{
	NFUIDialog *pOldDialog = m_pCurrentDialog;

    m_pCurrentDialog = pDialog;
    
    if(pOldDialog)
    {
        if (bPushToHistory)
            m_pDialogQueue.pushBack(pOldDialog);

        m_pDialogNode->removeChild(pOldDialog);
    }

    if(m_pCurrentDialog)
    {
        m_pDialogNode->addChild(m_pCurrentDialog);
    }

    return m_pCurrentDialog;
}

void NFUIManager::CloseDialog(NFUIDialog *pDialog)
{
	if(m_pCurrentDialog == pDialog)
	{
		ChangeBackDialog();
		return ;
	}

	int nIndex = m_pDialogQueue.getIndex(pDialog);
	if(nIndex != CC_INVALID_INDEX)
	{
		m_pDialogQueue.erase(nIndex);
	}
	m_pDialogNode->removeChild(pDialog);
}

void NFUIManager::ChangeBackDialog()
{
    int nDialogs = m_pDialogQueue.size();

    if (m_pCurrentDialog)
    {
        if (nDialogs == 0)
        {
			m_pCurrentDialog->runAction(Sequence::create(DelayTime::create(0.3f), RemoveSelf::create(), NULL));
        }
        else
        {
            m_pDialogNode->removeChild(m_pCurrentDialog);
        }
        m_pCurrentDialog = NULL;
    }

    if(nDialogs > 0)
    {
        m_pCurrentDialog = (NFUIDialog*)m_pDialogQueue.at(nDialogs-1);
        if(m_pCurrentDialog)
        {
            m_pDialogNode->addChild(m_pCurrentDialog);
        }
        m_pDialogQueue.popBack();
    }
}

bool NFUIManager::HasBackDialog()
{
    return m_pDialogQueue.size() > 0;
}

void NFUIManager::CloseDialog()
{
    if(m_pCurrentDialog)
    {
        m_pCurrentDialog->removeFromParentAndCleanup(true);
        m_pCurrentDialog = NULL;
    }
    m_pDialogQueue.clear();
}


void NFUIManager::ShowPanel(NFUIDialog *pPanel)
{
	int nIndex = m_pPanelQueue.getIndex(pPanel);
	if (nIndex == CC_INVALID_INDEX)
	{
		m_pPanelQueue.pushBack(pPanel);
		m_pPanelNode->addChild(pPanel);
	}
}

void NFUIManager::ClosePanel(NFUIDialog *pPanel)
{
	int nIndex = m_pPanelQueue.getIndex(pPanel);
	if (nIndex != CC_INVALID_INDEX)
	{
		m_pPanelQueue.erase(nIndex);
		m_pPanelNode->removeChild(pPanel);
	}
}

void NFUIManager::CloseAllPanel()
{
	m_pPanelQueue.clear();
	m_pPanelNode->removeAllChildren();
}

void NFUIManager::onBackKeyClicked()
{
	CCLOG("%s", __FUNCTION__);
}

void NFUIManager::onMenuKeyClicked()
{

}