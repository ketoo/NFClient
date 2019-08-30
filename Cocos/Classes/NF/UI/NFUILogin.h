// -------------------------------------------------------------------------
//    @FileName			:    NFUILogin.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUILogin
//
// -------------------------------------------------------------------------
#ifndef NFUILogin_H
#define NFUILogin_H

#include "NFUIManager.h"
#include "NFComm/NFPluginModule/NFINetClientModule.h"

class NFUILogin : public IUniqueDialog<NFUILogin>
{
public:
	NFUILogin();
	~NFUILogin();
	virtual const char* getUIName() { return "UILogin"; }
	virtual const char* getUIPath() { return "UILogin.csb"; }
	virtual int getUIPriority() { return 0; }

    virtual bool initLayout();
	virtual void initData(void *customData);

private:
	void OnSocketEvent(const NFSOCK nSockIndex, const NF_NET_EVENT eEvent, NFINet* pNet);
	int OnLoginEvent(const int, const NFDataList&);

private:
	void onLoginTouch(Ref *sender);
	
private:
	ui::Button *m_pLoginButton;
	ui::TextField *m_pUserName;
	ui::TextField *m_pUserPWD;

};

#endif // __HELLOWORLD_SCENE_H__
