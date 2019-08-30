// -------------------------------------------------------------------------
//    @FileName			:    NFUISelectRole.cpp
//    @Author           :    Johance
//    @Date             :    2016-12-28
//    @Module           :    NFUISelectRole
//
// -------------------------------------------------------------------------
#include "stdafx.h"

#include "NFUISelectRole.h"
#include "Logic/NFPlayerLogic.h"
#include "Scene/NFGameScene.h"

NFUISelectRole::NFUISelectRole()
{
}

NFUISelectRole::~NFUISelectRole()
{
	g_pPlayerLogic->RemoveEventCallBack(E_PlayerEvent_RoleList, this);
}

bool NFUISelectRole::initLayout()
{	
	if(!NFUIDialog::initLayout())
		return false;

	m_pRoleList = ui::ListView::create();
	m_pRoleList->setSizeType(ui::SizeType::PERCENT);
	m_pRoleList->setSizePercent(Vec2(0.1f, 0.25f));
	m_pRoleList->setPositionType(ui::PositionType::PERCENT);
	m_pRoleList->setPositionPercent(Vec2(0.5f, 0.5f));
	m_pRoleList->setBackGroundColorType(ui::LayoutBackGroundColorType::SOLID);
	m_pRoleList->setBackGroundColor(Color3B(150, 200, 255));
	m_pRoleList->setAnchorPoint(Vec2(0.5, 0.5));
	m_pContent->addChild(m_pRoleList);

	auto button = ui::Button::create();
	button->loadTextureNormal("Default/Button_Normal.png",ui::TextureResType::LOCAL);
	button->setScale9Enabled(true);
	button->setSizePercent(Vec2(1.0f, 0.2f));
	button->setSizeType(ui::SizeType::PERCENT);
	m_pRoleList->setItemModel(button);

	button = ui::Button::create();
	button->loadTextureNormal("Default/Button_Normal.png",ui::TextureResType::LOCAL);
	button->setScale9Enabled(true);
	button->setSizePercent(Vec2(0.1f, 0.1f));
	button->setSizeType(ui::SizeType::PERCENT);
	button->setTitleText("CreateRole");
	button->setPositionType(ui::PositionType::PERCENT);
	button->setPositionPercent(Vec2(0.5f, 0.2f));
	button->setTitleColor(Color3B(0, 0, 0));
	button->addClickEventListener(CC_CALLBACK_1(NFUISelectRole::OnRoleCreate, this));
	m_pContent->addChild(button);

	g_pPlayerLogic->AddEventCallBack(E_PlayerEvent_RoleList, this, &NFUISelectRole::OnRoleListEvent);
	return true;
}

void NFUISelectRole::initData(void *customData)
{
	g_pPlayerLogic->RequireRoleList();
	m_pRoleList->removeAllItems();
}

int NFUISelectRole::OnRoleListEvent(const int nEventID, const NFDataList& varDataList)
{
	auto roleList = g_pPlayerLogic->GetRoleList();
	m_pRoleList->removeAllItems();
	for(size_t i = 0; i < roleList.size(); i++)
	{
		const NFMsg::RoleLiteInfo &kRoleInfo =  roleList[i];
		m_pRoleList->pushBackDefaultItem();
		ui::Button *pButton = (ui::Button *)m_pRoleList->getItem(m_pRoleList->getItems().size()-1);
		pButton->addClickEventListener(CC_CALLBACK_1(NFUISelectRole::OnRoleSelected, this));
		pButton->setTitleText(kRoleInfo.noob_name());
		pButton->setTitleColor(Color3B(0, 0, 0));
	}

	return 0;
}

void NFUISelectRole::OnRoleSelected(Ref* sender)
{
	auto serverList = g_pPlayerLogic->GetRoleList();
	int nIndex = m_pRoleList->getIndex((ui::Widget*)sender);
	g_pPlayerLogic->RequireEnterGameServer(0);
	g_pNFUIManager->CloseDialog();
	NFGameScene::showScene();
}

void NFUISelectRole::OnRoleCreate(Ref* sender)
{
	NFDataList varData;
	int nID = g_pKernelModule->Random(0, 1000);
	g_pPlayerLogic->RequireCreateRole(CCString::createWithFormat("TestRole%lld", varData.Int(0))->getCString(), 1, 1);
}