using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// -------------------------------------------------------------------------
//    @FileName             :    NFCWindowManager.cpp
//    @Author               :    LvSheng.Huang
//    @Date                 :    2012-07-05
//    @Module               :    NFCWindowManager
//    @Desc                 :    游戏窗口管理器，主要管理游戏中运行时窗口资源
// -------------------------------------------------------------------------

public class NFCWindowManager
{
    public delegate void WindowStateEventHandler(NFCWindows.UI_WIN_STATE eState, bool bNowActive, int nActiveCount);
    public delegate void ControlStateEventHandler(NFCWindows.UI_WIN_STATE eState, int nControlID, bool bNowActive, int nActiveCount);

    #region Instance
	private NFCWindows.UI_WIN_STATE meUIState;
	private NFCWindows.UI_WIN_STATE meLastShowUI;
    private static NFCWindowManager _Instance = null;
    public static NFCWindowManager Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = new NFCWindowManager();
            }
            return _Instance;
        }

    }
    #endregion

    class ControlData
    {
        public int nControlID = 0;
        public int nActiveCount = 0;
        public ControlStateEventHandler doControlHandleDel;
        public GameObject xControlGameObject;
    }

    class WindowData
    {
        public int nWindowID = 0;
        public int nActiveCount = 0;
        public GameObject xWindowGameObject;
        public WindowStateEventHandler doWindowHandleDel;

        public Dictionary<int, ControlData> mhtControl = new Dictionary<int, ControlData>();//子控件
    }

    private Dictionary<NFCWindows.UI_WIN_STATE, WindowData> mhtWindows = new Dictionary<NFCWindows.UI_WIN_STATE,WindowData>();
    

    void Start()
    {
        HideAllWindows();
    }

    public void RegisterWindowStateCallback(NFCWindows.UI_WIN_STATE eState, WindowStateEventHandler handler)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            return;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        xWindowData.doWindowHandleDel += handler;
    }

    public void RegisterControlStateCallback(NFCWindows.UI_WIN_STATE eState, int nControlID, ControlStateEventHandler handler)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            return;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        if (!xWindowData.mhtControl.ContainsKey(nControlID))
        {
            return;
        }

        xWindowData.mhtControl[nControlID].doControlHandleDel += handler;
    }

    public GameObject SetGameWindows(NFCWindows.UI_WIN_STATE eState)
    {

        if(eState != meUIState)
        {
            HideWindows(meUIState);
        }
        meUIState = eState;
        ShowWindows(meUIState);

        return GetGameWindow(eState);
    }

    public void HideWindows(NFCWindows.UI_WIN_STATE eState)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            return;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        if (null == xWindowData || null == xWindowData.xWindowGameObject)
        {
            return;
        }
        if (null != xWindowData.doWindowHandleDel)
        {
            xWindowData.doWindowHandleDel(eState, false, xWindowData.nActiveCount);
        }

        xWindowData.xWindowGameObject.SetActive(false);


        meUIState = NFCWindows.UI_WIN_STATE.UI_WIN_NONE;
    }

    public void ShowWindows(NFCWindows.UI_WIN_STATE eState)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            return;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        if (null == xWindowData || null == xWindowData.xWindowGameObject)
        {
            return;
        }

        xWindowData.xWindowGameObject.SetActive(true);
        if (null != xWindowData.doWindowHandleDel)
        {
            xWindowData.nActiveCount += 1;
            xWindowData.doWindowHandleDel(eState, true, xWindowData.nActiveCount);

			meLastShowUI = eState;
        }
    }

	public NFCWindows.UI_WIN_STATE GetLastStateUI()
	{
		return meLastShowUI;
	}

    public void HideControl(NFCWindows.UI_WIN_STATE eState, int nControlID)
    {
        if (mhtWindows.ContainsKey(eState))
        {
            WindowData xWindowData = (WindowData)mhtWindows[eState];
            if (xWindowData.mhtControl.ContainsKey(nControlID))
            {
                ControlData xControl = xWindowData.mhtControl[nControlID];
                if (null != xControl && null != xControl.xControlGameObject)
                {
                    if (null != xControl.doControlHandleDel)
                    {
                        xControl.doControlHandleDel(eState, nControlID, false, xControl.nActiveCount);
                    }
                    xControl.xControlGameObject.SetActive(false);
                }
            }
        }
    }

    public void ShowControl(NFCWindows.UI_WIN_STATE eState, int nControlID)
    {
        if (mhtWindows.ContainsKey(eState))
        {
            WindowData xWindowData = (WindowData)mhtWindows[eState];

            ControlData xControl = xWindowData.mhtControl[nControlID];
            if (null != xControl && null != xControl.xControlGameObject)
            {
                xControl.xControlGameObject.SetActive(true);

                if (null != xControl.doControlHandleDel)
                {
                    xControl.nActiveCount += 1;
                    xControl.doControlHandleDel(eState, nControlID, true, xControl.nActiveCount);
                }
            }
        }
    }


    public NFCWindows.UI_WIN_STATE GetGameState()
    {
        return meUIState;
    }

    public void AddGameWindow(NFCWindows.UI_WIN_STATE eState, GameObject go)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            WindowData xData = new WindowData();
            mhtWindows[eState] = xData;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        xWindowData.xWindowGameObject = go;

    }

    public void AddWindowControl(NFCWindows.UI_WIN_STATE eState, int nControlID, GameObject go)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            WindowData xData = new WindowData();

            mhtWindows[eState] = xData;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        if (!xWindowData.mhtControl.ContainsKey(nControlID))
        {
            ControlData xControlData = new ControlData();

            xWindowData.mhtControl[nControlID] = xControlData;
        }

        xWindowData.mhtControl[nControlID].xControlGameObject = go;
    }

	public bool GetWindowControlEnable(NFCWindows.UI_WIN_STATE eState, int nControlID)
	{
		GameObject go = GetWindowControl (eState, nControlID);
		if (go)
		{
			return go.activeSelf;
		}

		return false;
	}

    public GameObject GetWindowControl(NFCWindows.UI_WIN_STATE eState, int nControlID)
    {
        if (!mhtWindows.ContainsKey(eState))
        {
            return null;
        }

        WindowData xWindowData = (WindowData)mhtWindows[eState];
        if (null == xWindowData)
        {
            return null;
        }

        if (!xWindowData.mhtControl.ContainsKey(nControlID))
        {
            return null;
        }

        return xWindowData.mhtControl[nControlID].xControlGameObject;
    }

	public bool GetGameWindowEnable(NFCWindows.UI_WIN_STATE eState)
	{
		GameObject go = GetGameWindow (eState);
		if (go)
		{
			return go.activeSelf;
		}

		return false;
	}
    public GameObject GetGameWindow(NFCWindows.UI_WIN_STATE eState)
    {
        if (mhtWindows.ContainsKey(eState))
        {
            WindowData xWindowData = (WindowData)mhtWindows[eState];
            return xWindowData.xWindowGameObject;
        }

        return null;
	}

	public void HideAllWindows(NFCWindows.UI_WIN_STATE eExcept)
	{
		meUIState = eExcept;

		foreach (WindowData xWindowData in mhtWindows.Values)
		{
			if (null != xWindowData.xWindowGameObject)
			{
				if (null != xWindowData.doWindowHandleDel
					&& (NFCWindows.UI_WIN_STATE)xWindowData.nWindowID != eExcept)
				{
					xWindowData.doWindowHandleDel((NFCWindows.UI_WIN_STATE)xWindowData.nWindowID, false, xWindowData.nActiveCount);

					xWindowData.xWindowGameObject.SetActive(false);
				}
			}
		}	
	}

    public void HideAllWindows()
    {
        meUIState = NFCWindows.UI_WIN_STATE.UI_WIN_NONE;

        foreach (WindowData xWindowData in mhtWindows.Values)
        {
            if (null != xWindowData.xWindowGameObject)
            {
                if (null != xWindowData.doWindowHandleDel)
                {
                    xWindowData.doWindowHandleDel((NFCWindows.UI_WIN_STATE)xWindowData.nWindowID, false, xWindowData.nActiveCount);
                }

                xWindowData.xWindowGameObject.SetActive(false);
            }
        }
    }

    public void HideWindowsAllControl(NFCWindows.UI_WIN_STATE eState)
    {
        if (mhtWindows.ContainsKey(eState))
        {
            WindowData xWindowData = (WindowData)mhtWindows[eState];
            foreach (ControlData xControlData in xWindowData.mhtControl.Values)
            {
                if (null != xControlData.xControlGameObject)
                {
                    if (null != xControlData.doControlHandleDel)
                    {
                        xControlData.doControlHandleDel(eState, xControlData.nControlID, false, xControlData.nActiveCount);
                    }

                    xControlData.xControlGameObject.SetActive(false);
                }
            }
        }
    }
}