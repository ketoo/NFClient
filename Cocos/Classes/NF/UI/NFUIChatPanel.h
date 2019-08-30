// -------------------------------------------------------------------------
//    @FileName			:    NFUIChatPanel.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUIChatPanel
//
// -------------------------------------------------------------------------
#ifndef NFUIChatPanel_H
#define NFUIChatPanel_H

#include "NFUIManager.h"

class NFUIChatPanel : public IUniquePanel<NFUIChatPanel>
{
public:
	NFUIChatPanel();
	~NFUIChatPanel();
	virtual const char* getUIName() { return "UIChatPanel"; }
	virtual const char* getUIPath() { return "UIChatPanel.csb"; }
	virtual int getUIPriority() { return 0; }

    virtual bool initLayout();
	virtual void initData(void *customData);

private:

private:
	void onLoginTouch(Ref *sender);
	
private:
	ui::Button *m_pSend;
	ui::TextField *m_pInputText;

};

#endif // __HELLOWORLD_SCENE_H__
