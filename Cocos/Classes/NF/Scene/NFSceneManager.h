// -------------------------------------------------------------------------
//    @FileName			:    NFSceneManager.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFSceneManager
//
// -------------------------------------------------------------------------
#ifndef _NFSceneManager_H_
#define _NFSceneManager_H_

#include "NFComm/NFCore/NFSingleton.hpp"
#include "NFComm/NFPluginModule/NFIModule.h"

class NFSceneManager : public NFIModule, public NFSingleton<NFSceneManager>
{
public:
	NFSceneManager() {}
	~NFSceneManager() {}

    NFSceneManager(NFIPluginManager* p)
    {
        pPluginManager = p;
    }

    virtual bool Init();
    virtual bool Shut();
    virtual bool ReadyExecute();
    virtual bool Execute();

    virtual bool AfterInit();

public:
    virtual Node* RootNode();

	void ShowScene(Node *pScene);
	void CloseScene(Node *pScene);

private:
    // 场景的根节点
	CC_SYNTHESIZE_READONLY(Node*, m_pRootNode, RootNode);
	CC_SYNTHESIZE_READONLY(Node*, m_pCurrentScene, CurrentScene);
};

#define g_pNFSceneManager (NFSceneManager::Instance())

// 与管理器逻辑无关的纯UI逻辑类
template<typename T>
class IUniqueScene: public Layer
{	
public:
	// 显示窗口(窗口没创建时创建窗口并显示，已经创建则只显示)
	static void showScene(const void *customData = nullptr) 
	{ 
		IUniqueScene **p = _getScene();
		if (!*p) {
			*p = new T;
			(*p)->initLayout();
			(*p)->autorelease();
		}
		
		(*p)->initData((void*)customData);
		g_pNFSceneManager->ShowScene(*p);
	}
	static void closeUI(const void *customData = nullptr) 
	{ 
		IUniqueScene **p = _getScene();
		if (*p) {
			g_pNFSceneManager->CloseScene(*p);
		}
	}
	IUniqueScene() { // 不允许外部手动new
		*_getScene() = this;
	}
	virtual ~IUniqueScene() { // 不允许外部手动delete
		if(*_getScene() == this)
			*_getScene() = nullptr;
	}

	virtual bool initLayout() { return true; };
	virtual void initData(void *) {};

private:
	static IUniqueScene** _getScene() {
		static T* _instance;
		return (IUniqueScene**)(&_instance);
	}
};

#endif
