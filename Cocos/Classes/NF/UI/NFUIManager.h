// -------------------------------------------------------------------------
//    @FileName			:    NFUIManager.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUIManager
// -------------------------------------------------------------------------
#ifndef _NFUIManager_H_
#define _NFUIManager_H_

#include "NFUIDialog.h"
#include "NFComm/NFCore/NFSingleton.hpp"
#include "NFComm/NFPluginModule/NFIModule.h"

class NFUIManager : public NFIModule, public NFSingleton<NFUIManager>
{
public:
	NFUIManager() {}
	~NFUIManager() {}

    NFUIManager(NFIPluginManager* p)
    {
        pPluginManager = p;
    }

    virtual bool Init();
    virtual bool Shut();
    virtual bool ReadyExecute();
    virtual bool Execute();

    virtual bool AfterInit();

	// ---------  ------------
    virtual Node* RootNode();
    
    virtual NFUIDialog* ShowDialog(NFUIDialog *pDialog, bool bPushToHistory = true);
    virtual void CloseDialog(NFUIDialog *pDialog);
	virtual void ChangeBackDialog();
	virtual bool HasBackDialog();
	virtual void CloseDialog();

	virtual void ShowPanel(NFUIDialog *pPanel);
	virtual void ClosePanel(NFUIDialog *pDialog);
	virtual void CloseAllPanel();

public:
	void onBackKeyClicked();
	void onMenuKeyClicked();

private:
    std::vector<NFUIDialog*> m_vecDialog;
	
	CC_SYNTHESIZE_READONLY(Node*, m_pRootNode, RootNode);
	CC_SYNTHESIZE_READONLY(Node*, m_pPanelNode, PanelNode);
	CC_SYNTHESIZE_READONLY(Node*, m_pDialogNode, DialogNode);
	CC_SYNTHESIZE_READONLY(Node*, m_pBoxNode, BoxNode);
	CC_SYNTHESIZE_READONLY(Node*, m_pEffectNode, EffectNode);

private:
    Vector<NFUIDialog*> m_pDialogQueue;
	Vector<NFUIDialog*> m_pPanelQueue;
	NFUIDialog *m_pCurrentDialog;
};

#define g_pNFUIManager (NFUIManager::Instance())

// 与管理器逻辑无关的纯UI逻辑类
template<typename T>
class IUniqueDialog: public NFUIDialog
{	
public:
	// 显示窗口(窗口没创建时创建窗口并显示，已经创建则只显示)
	static void showUI(bool bPushToHistory = true, const void *customData = nullptr) 
	{ 
		IUniqueDialog **p = _getDialog();
		if (!*p) {
			*p = new T;
			(*p)->initLayout();
			(*p)->autorelease();
		}
		
		(*p)->initData((void*)customData);
		g_pNFUIManager->ShowDialog(*p, bPushToHistory);
	}
	static void closeUI(const void *customData = nullptr) 
	{ 
		IUniqueDialog **p = _getDialog();
		if (*p) {
			g_pNFUIManager->CloseDialog(*p);
		}
	}
	IUniqueDialog() { // 不允许外部手动new
		*_getDialog() = this;
	}
	virtual ~IUniqueDialog() { // 不允许外部手动delete
		if(*_getDialog() == this)
			*_getDialog() = nullptr;
	}

private:
	static IUniqueDialog** _getDialog() {
		static T* _instance;
		return (IUniqueDialog**)(&_instance);
	}
};

template<typename T>
class IUniquePanel : public NFUIDialog
{
public:
	// 显示窗口(窗口没创建时创建窗口并显示，已经创建则只显示)
	static void showPanel(const void *customData = nullptr)
	{
		IUniquePanel **p = _getPanel();
		if (!*p) {
			*p = new T;
			(*p)->initLayout();
			(*p)->autorelease();
		}

		(*p)->initData((void*)customData);
		g_pNFUIManager->ShowPanel(*p);
	}
	static void closePanel(const void *customData = nullptr)
	{
		IUniquePanel **p = _getPanel();
		if (*p) {
			g_pNFUIManager->ClosePanel(*p);
		}
	}
	IUniquePanel() { // 不允许外部手动new
		*_getPanel() = this;
	}
	virtual ~IUniquePanel() { // 不允许外部手动delete
		if (*_getPanel() == this)
			*_getPanel() = nullptr;
	}

private:
	static IUniquePanel** _getPanel() {
		static T* _instance;
		return (IUniquePanel**)(&_instance);
	}
};

#endif
