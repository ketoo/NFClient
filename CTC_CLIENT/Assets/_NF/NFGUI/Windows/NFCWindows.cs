using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// -------------------------------------------------------------------------
//    @FileName             :    NFCWindowsInit.cpp
//    @Author               :    LvSheng.Huang
//    @Date                 :    2012-07-05
//    @Module               :    NFCWindowsInit
//    @Desc                 :    游戏窗口初始器
// -------------------------------------------------------------------------

public class NFIControl : MonoBehaviour
{
    public bool bActive = true;
    private bool bInitState = false;

    ////设置某个控件是否显示
    public void UpdateControl()
    {
        if (!bInitState)
        {
            bInitState = true;
            this.gameObject.SetActive(bActive);
        }
    }
}

//窗口管理一些控件

public class NFCWindows : NFIControl
{
    public enum UI_WIN_STATE
    {
        UI_WIN_NONE = 0,
        UI_WIN_RETURN = 1,
        UI_WIN_MONEY_TOP = 2,
        UI_WIN_RANK = 3,
        UI_WIN_ROLE = 4,
        UI_WIN_CITY_TOP = 5,
        UI_WIN_FIGHT_TOP = 6,
        UI_WIN_CHAT = 7,
        UI_WIN_OPTION = 8,
        UI_WIN_GUILD = 9,
        UI_WIN_MAP = 10,
        UI_WIN_MSG_BOX = 11,
        UI_WIN_HERO_CENTER = 12,
        UI_WIN_HERO_LIST = 13,
        UI_WIN_SETTING = 14,
        UI_WIN_SHOPGOLD = 15,
        UI_WIN_SHOPSP = 16,
        UI_WIN_SHOPDIAMOND = 17,
		UI_WIN_GMCENTER = 18,
		UI_WIN_SEARCH_OPPNENT = 19,
		UI_WIN_READY_FIGHTING = 20,
		UI_WIN_FIGHTING_RESULT = 21,
		UI_WIN_MAIN_SKILL = 22,
    };


    public UI_WIN_STATE meUIState = UI_WIN_STATE.UI_WIN_NONE;

    void Awake()
    {
        if (UI_WIN_STATE.UI_WIN_NONE == meUIState)
        {
            Debug.LogError("NFCWindowsInit ERROR!!");
            return;
        }

        NFCWindowManager.Instance.AddGameWindow(meUIState, this.gameObject);

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);

        }
    }

    void Start()
    {
    }

    void OnShow(bool bShow)
    {
    }

    void Update()
	{
		UpdateControl();
    }
}