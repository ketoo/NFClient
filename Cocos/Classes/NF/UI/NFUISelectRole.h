// -------------------------------------------------------------------------
//    @FileName			:    NFUISelectRole.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUISelectRole
//
// -------------------------------------------------------------------------
#ifndef NFUISelectRole_H
#define NFUISelectRole_H

#include "NFUIManager.h"

class NFUISelectRole : public IUniqueDialog<NFUISelectRole>
{
public:
	NFUISelectRole();
	~NFUISelectRole();
	virtual const char* getUIName() { return ""; }
	virtual const char* getUIPath() { return ""; }
	virtual int getUIPriority() { return 0; }

    virtual bool initLayout();
	virtual void initData(void *customData);

private:
	int OnRoleListEvent(const int nEventID, const NFDataList& varDataList);

private:
	void OnRoleSelected(Ref* sender);
	void OnRoleCreate(Ref* sender);
	
private:
	ui::ListView *m_pRoleList;

};

#endif // _NFUISelectRole_H__
