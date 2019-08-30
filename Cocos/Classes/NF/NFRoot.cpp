// -------------------------------------------------------------------------
//    @FileName			:    NFRoot.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-27
//    @Module           :    NFRoot
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFRoot.h"
#include "NFClientPlugin.h"
#include "NFComm/NFPluginLoader/NFPluginManager.h"

#include "UI/NFUIPlugin.h"
#include "Logic/NFLogicPlugin.h"
#include "Scene/NFScenePlugin.h"

#include "UI/NFUILogin.h"

USING_NS_CC;

#ifdef WIN32
extern "C" int gettimeofday(struct timeval *val, struct timezone *tz)
{
	return cocos2d::gettimeofday(val, tz);
}
#endif

Scene* NFRoot::scene()
{
    Scene *scene = Scene::create();    
    NFRoot *layer = NFRoot::create();
    scene->addChild(layer);
    return scene;
}

NFRoot::NFRoot()
{		
}

NFRoot::~NFRoot()
{
	NFPluginManager::GetSingletonPtr()->BeforeShut();
	NFPluginManager::GetSingletonPtr()->Shut();
	NFPluginManager::GetSingletonPtr()->ReleaseInstance();
}

bool NFRoot::init()
{
    if ( !CCNode::init() )
    {
        return false;
    }

	NFIPluginManager *pPluginManager = NFPluginManager::GetSingletonPtr();

	pPluginManager->SetGetFileContentFunctor([](const std::string &strFileName, std::string &strContent) -> bool
	{
		if (FileUtils::getInstance()->getContents(strFileName, &strContent) != FileUtils::Status::OK || strContent.empty())
		{
			return false;
		}
		return true;
	});

	pPluginManager->SetAppName("GameClient");
	pPluginManager->SetConfigPath("");
	
	CREATE_PLUGIN(pPluginManager, NFClientPlugin);

	CREATE_PLUGIN(pPluginManager, NFLogicPlugin);
	CREATE_PLUGIN(pPluginManager, NFUIPlugin);
	CREATE_PLUGIN(pPluginManager, NFScenePlugin);

	NFPluginManager::GetSingletonPtr()->Awake();
	NFPluginManager::GetSingletonPtr()->Init();
	NFPluginManager::GetSingletonPtr()->AfterInit();
	NFPluginManager::GetSingletonPtr()->CheckConfig();
	NFPluginManager::GetSingletonPtr()->ReadyExecute();

	CCFileUtils::getInstance()->addSearchPath("res");

	NFUILogin::showUI();

	scheduleUpdate();

    return true;
}

void NFRoot::update(float dt)
{
	NFPluginManager::GetSingletonPtr()->Execute();
}