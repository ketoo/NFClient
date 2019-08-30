// -------------------------------------------------------------------------
//    @FileName			:    NFUIDialog.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUIDialog
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFUIDialog.h"

NFUIDialog::NFUIDialog()
	: m_pContent(NULL)
{
}

NFUIDialog::~NFUIDialog()
{
}

Ref *NFUIDialog::GetObjectByName(const char *pName)
{
	return ui::Helper::seekWidgetByName((ui::Widget*)m_pContent, pName);
}

bool NFUIDialog::init()
{
	Layer::init();
	return initLayout();
}

bool NFUIDialog::initLayout()
{	
	if(strcmp(getUIPath(), "") != 0)
	{
		m_strFileName = FileUtils::getInstance()->fullPathForFilename(getUIPath());
		m_pContent = CSLoader::createNode(m_strFileName.c_str());
	}
	else
	{
		Size winSize = Director::getInstance()->getWinSize();
		ui::Layout *pLayout = ui::Layout::create();
		pLayout->setContentSize(winSize);
		m_pContent = pLayout;
	}

	if(!m_pContent)
	{
		return false;
	}

	addChild(m_pContent);

	return true;
}

void NFUIDialog::onEnter()
{
	CCLayer::onEnter();
}

void NFUIDialog::onExit()
{
	CCLayer::onExit();
}