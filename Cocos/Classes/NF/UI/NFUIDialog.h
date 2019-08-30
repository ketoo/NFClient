// -------------------------------------------------------------------------
//    @FileName			:    NFUIDialog.h
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUIDialog
//
// -------------------------------------------------------------------------
#ifndef NFUIDialog_H
#define NFUIDialog_H

class NFUIDialog : public cocos2d::Layer
{
public:
	NFUIDialog();
	~NFUIDialog();
		
    Ref *GetObjectByName(const char *pName);
    template <typename T>
    T GetObjectByName(T *ppControl, const char *pName)
    {
        *ppControl = dynamic_cast<T>(GetObjectByName(pName));
        return *ppControl;
    }

	virtual const char* getUIName() { return ""; }
	virtual const char* getUIPath() { return ""; }
	virtual int getUIPriority() { return 0; }

	virtual bool init();
    virtual bool initLayout();
	virtual void initData(void *customData) {};
	
    virtual void onEnter();
    virtual void onExit();

protected:
	std::string m_strFileName;
	Node *m_pContent;
};

#endif // _NFUIDialog_H__
