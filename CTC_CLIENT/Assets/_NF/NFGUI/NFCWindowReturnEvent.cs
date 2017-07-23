using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFrame;

public class NFCWindowReturnEvent : NFIControl
{

	public enum WMAINTOP_RETURN_CONTROL_ENUM
	{

		WMTRCE_ROOT = 0,

		WMTRCE_RETURN_BTN = 1
	}

	//////////all the windows logic class must need the next three menber elements
	public WMAINTOP_RETURN_CONTROL_ENUM eControlType;

	private static Hashtable mhtWindow = new Hashtable ();
	private Transform mRootPanel = null;
	

	///////////////////////////////////////////////	
	void Awake ()
	{
		mhtWindow [eControlType] = this.gameObject;

		foreach (NFCWindows.UI_WIN_STATE eState in Enum.GetValues(typeof(NFCWindows.UI_WIN_STATE)))
		{
			NFCWindowManager.Instance.RegisterWindowStateCallback (eState, OnWindowsStateChangede);
		}
	}

	void Start ()
	{
		if (WMAINTOP_RETURN_CONTROL_ENUM.WMTRCE_RETURN_BTN == eControlType)
		{
			Button btn = this.gameObject.GetComponent<Button> ();
			btn.onClick.AddListener (delegate ()
			{
				this.OnFunctionClick (this.gameObject);
			});
		}

		UpdateControl ();
	}

	void OnFunctionClick (GameObject go)
	{
		if (NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_SEARCH_OPPNENT)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_GUILD)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_HERO_LIST)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_HERO_CENTER)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_RANK)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_ROLE)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_HERO_CENTER))
		{
			NFCWindowManager.Instance.HideAllWindows ();
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP);
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MAIN_SKILL);
			NFStart.Instance.SetJoyEnable (true);

			return;
		}
		if (NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_MAP))
		{
				NFCWindowManager.Instance.HideAllWindows ();
				NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
				NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP);
				NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MAIN_SKILL);
				NFStart.Instance.SetJoyEnable (true);

			return;
		}
		if (NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_SHOPDIAMOND)
			||NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_SHOPGOLD)
			||NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_SHOPSP))
		{
			NFCWindowManager.Instance.HideAllWindows ();
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP);

			return;
		}

		if (NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_READY_FIGHTING)
			|| NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_FIGHTING_RESULT))
		{
			//require to swap scene to home scene
			NFGUID xPlayerID = NFNetController.Instance.xMainRoleID;
			NFNetController.Instance.mxNetSender.RequireSwapToHomeScene(xPlayerID);

			NFCWindowManager.Instance.HideAllWindows ();
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP);
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MAIN_SKILL);
			NFStart.Instance.SetJoyEnable (true);
			return;
		}

		if (NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_FIGHT_TOP))
		{
			//require to swap scene to home scene
			NFGUID xPlayerID = NFNetController.Instance.xMainRoleID;
			NFNetController.Instance.mxNetSender.RequireEndPvp(xPlayerID);

			NFCWindowManager.Instance.HideAllWindows ();
			NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
			//NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_FIGHTING_RESULT);

			NFStart.Instance.SetJoyEnable (true);
			return;
		}
	}

	void OnWindowsStateChangede (NFCWindows.UI_WIN_STATE eState, bool bNowActive, int nActiveCount)
	{
		if (bNowActive)
		{
			switch(eState)
			{
				case NFCWindows.UI_WIN_STATE.UI_WIN_SEARCH_OPPNENT:
					{
						//NFCWindowManager.Instance.HideAllWindows (eState);
						//NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
						//NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					}
				break;
				default:
					break;
			}
		}
		else
		{
			switch(eState)
			{
				case NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP:
					break;
				default:
					break;
			}
		}


	}
}