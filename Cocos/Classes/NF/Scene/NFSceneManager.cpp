// -------------------------------------------------------------------------
//    @FileName			:    NFSceneManager.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFSceneManager
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFSceneManager.h"
#include "NFRoot.h"

bool NFSceneManager::Init()
{
    m_pRootNode = CCNode::create();
    m_pRootNode->retain();
	m_pCurrentScene = NULL;
	NFRoot::Instance()->addChild(m_pRootNode, NFRoot::eIZO_Scene);
    
	return true;
}
bool NFSceneManager::Shut()
{
	if (NULL == m_pRootNode)
		return true;

	m_pRootNode->removeFromParentAndCleanup(true);
	CC_SAFE_RELEASE_NULL(m_pRootNode);

	return true;
}

bool NFSceneManager::ReadyExecute()
{
	return true;
}

bool NFSceneManager::Execute()
{
	return true;
}

bool NFSceneManager::AfterInit()
{
	return true;
}

Node* NFSceneManager::RootNode()
{
    return m_pRootNode;
}

void NFSceneManager::ShowScene(Node *pScene)
{
	if(!pScene)
		return ;

	if(m_pCurrentScene == pScene)
		return ;

	if(m_pCurrentScene)
	{
		CloseScene(m_pCurrentScene);
	}
	m_pCurrentScene = pScene;	
	m_pRootNode->addChild(pScene);
	m_pCurrentScene->retain();
}

void NFSceneManager::CloseScene(Node *pScene)
{
	if(!pScene)
		return ;

	if(m_pCurrentScene == pScene)
	{
		m_pCurrentScene->autorelease();
		m_pCurrentScene->release();
		m_pCurrentScene = NULL;
	}

	m_pRootNode->removeChild(pScene);
}