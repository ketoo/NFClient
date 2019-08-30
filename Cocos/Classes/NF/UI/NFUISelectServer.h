// -------------------------------------------------------------------------
//    @FileName			:    NFUISelectServer.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUISelectServer
//
// -------------------------------------------------------------------------
#ifndef NFUISelectServer_H
#define NFUISelectServer_H

#include "NFUIManager.h"

class NFUISelectServer : public IUniqueDialog<NFUISelectServer>
{
public:
	NFUISelectServer();
	~NFUISelectServer();
	virtual const char* getUIName() { return ""; }
	virtual const char* getUIPath() { return ""; }
	virtual int getUIPriority() { return 0; }

    virtual bool initLayout();
	virtual void initData(void *customData);

private:
	int OnWorldListEvent(const int nEventID, const NFDataList& varDataList);
	int OnServerListEvent(const int nEventID, const NFDataList& varDataList);

private:
	void OnWorldSelected(Ref *sender);
	void OnServerSelected(Ref *sender);
	
private:
	ui::ListView *m_pServerList;

};

#endif // _NFUISelectServer_H__
