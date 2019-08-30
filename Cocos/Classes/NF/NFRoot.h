// -------------------------------------------------------------------------
//    @FileName			:    NFRoot.h
//    @Author           :    Johance
//    @Date             :    2016-12-27
//    @Module           :    NFRoot
//
// -------------------------------------------------------------------------

#ifndef __NFRoot_H__
#define __NFRoot_H__

#include "cocos2d.h"
#include "NFComm/NFCore/NFSingleton.hpp"

class NFRoot : public cocos2d::CCNode, public NFSingleton<NFRoot>
{
public:
	enum IndexZOrder
	{
		eIZO_Scene = 0,
        eIZO_UI,
        eIZO_WebView,
	};

	NFRoot();
	~NFRoot();
    virtual bool init();  
    static cocos2d::CCScene* scene();
	virtual void update(float dt);
    CREATE_FUNC(NFRoot);
};

#endif // __HELLOWORLD_SCENE_H__
