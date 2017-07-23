using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFrame;

public class NFCWindowMainTopLogicEvent : NFIControl
{

	public enum WMAINTOP_CONTROL_ENUM
	{

		WMTCE_ROOT = 0,

		WMTCE_MAINBTN_BEGIN = 299,
		WMTCE_SHOP = 300,
		WMTCE_MAINHEAD = 301,
		WMTCE_MAINMAP = 302,
		WMTCE_MAINCHAT = 303,
		WMTCE_GIFT = 304,
		WMTCE_SEARCH_OPPNENT = 305,
		WMTCE_ROLE = 306,
		WMTCE_AWARD = 307,
		WMTCE_TASK = 308,
		WMTCE_OPTION = 309,
		WMTCE_GUILD = 310,
		WMTCE_HERO = 311,
		WMTCE_MAP = 312,


		WMTCE_MAINBTN_END = 399,

		WMTCE_CHAT_DOT = 1000,
		WMTCE_CHAT_DOT_NUMBER = 1001,
		WMTCE_GIFT_DOT = 1002,
		WMTCE_GIFT_DOT_NUMBER = 1003,
		WMTCE_BATTLE_DOT = 1004,
		WMTCE_ROLE_DOT = 1005,
		WMTCE_ROLE_DOT_NUMBER = 1006,
		WMTCE_SHOP_DOT = 1007,
		WMTCE_AWARD_DOT = 1008,
		WMTCE_TASK_DOT = 1009,
		WMTCE_GUILD_DOT = 1010,


		WMTCE_END,
	}

	//////////all the windows logic class must need the next three menber elements
	public WMAINTOP_CONTROL_ENUM eControlType;

	private static Hashtable mhtWindow = new Hashtable ();
	private Transform mRootPanel = null;

	void OnClassHandler (NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
	{
		NFrame.NFGUID xIdent = NFNetController.Instance.xMainRoleID;
		if (self == xIdent)
		{
			if (NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH == eType)
			{
			}
		}
	}

	///////////////////////////////////////////////

	void SetChatDot (bool bActive, int nNumber)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_CHAT_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);

			GameObject xDotNumber = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_CHAT_DOT_NUMBER];
			if (null != xDotNumber)
			{
				Text xLabel = xDotNumber.GetComponent<Text> ();
				if (null != xLabel)
				{
					xLabel.text = nNumber.ToString ();
				}
			}
		}
	}

	void SetGiftDot (bool bActive, int nNumber)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_GIFT_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);

			NFCUIHelp.Instance.SetLabelText ((GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_GIFT_DOT_NUMBER], nNumber.ToString ());
		}
	}

	void SetBattleDot (bool bActive)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_BATTLE_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);
		}
	}

	void SetRoleDot (bool bActive, int nNumber)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_ROLE_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);

			NFCUIHelp.Instance.SetLabelText ((GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_ROLE_DOT_NUMBER], nNumber.ToString ());
		}
	}

	void SetShopDot (bool bActive)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_SHOP_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);
		}
	}

	void SetAwardDot (bool bActive)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_AWARD_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);
		}
	}

	void SetTaskDot (bool bActive)
	{
		GameObject xDot = (GameObject)mhtWindow [WMAINTOP_CONTROL_ENUM.WMTCE_TASK_DOT];
		if (null != xDot)
		{
			xDot.SetActive (bActive);
		}
	}
		
	///////////////////////////////////////////////	
	void Awake ()
	{
		mhtWindow [eControlType] = this.gameObject;

		if (WMAINTOP_CONTROL_ENUM.WMTCE_ROOT == eControlType)
		{
			NFCKernelModule.Instance.RegisterClassCallBack (NFrame.Player.ThisName, OnClassHandler);
		}
	}

	void Start ()
	{
		if (WMAINTOP_CONTROL_ENUM.WMTCE_MAINBTN_BEGIN < eControlType
		    && WMAINTOP_CONTROL_ENUM.WMTCE_MAINBTN_END > eControlType)
		{
			Debug.Log (eControlType);
			Button btn = this.gameObject.GetComponent<Button> ();
			btn.onClick.AddListener (delegate ()
			{
				this.OnFunctionClick (this.gameObject);
			});
		}

		if (WMAINTOP_CONTROL_ENUM.WMTCE_ROOT == eControlType)
		{
			foreach (NFCWindows.UI_WIN_STATE eState in Enum.GetValues(typeof(NFCWindows.UI_WIN_STATE)))
			{
				NFCWindowManager.Instance.RegisterWindowStateCallback (eState, OnWindowsStateChangede);
			}
		}

		UpdateControl ();
	}

	void Update ()
	{

	}

	void OnGUI ()
	{

	}

	void OnWindowsStateChangede (NFCWindows.UI_WIN_STATE eState, bool bNowActive, int nActiveCount)
	{
		if (NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP == eState)
		{
			return;
		}
	}

	void OnFunctionClick (GameObject go)
	{

		switch (eControlType)
		{
			case WMAINTOP_CONTROL_ENUM.WMTCE_SEARCH_OPPNENT:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.HideAllWindows ();
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_SEARCH_OPPNENT);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);

				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_MAINHEAD:
			case WMAINTOP_CONTROL_ENUM.WMTCE_ROLE:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
					NFCWindowManager.Instance.SetGameWindows (NFCWindows.UI_WIN_STATE.UI_WIN_ROLE);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_MAINCHAT:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
					NFCWindowManager.Instance.SetGameWindows (NFCWindows.UI_WIN_STATE.UI_WIN_CHAT);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_GIFT:
				{
					//NFCWindowManager.Instance.SetGameWindows(NFCWindows.UI_WIN_STATE.UI_WIN_ROLE);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_AWARD:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.SetGameWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RANK);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_TASK:
				{
					//NFCWindowManager.Instance.SetGameWindows(NFCWindows.UI_WIN_STATE.UI_WIN_ROLE);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_OPTION:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.SetGameWindows (NFCWindows.UI_WIN_STATE.UI_WIN_OPTION);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_GUILD:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
					NFCWindowManager.Instance.SetGameWindows (NFCWindows.UI_WIN_STATE.UI_WIN_GUILD);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_MAP:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
					NFCWindowManager.Instance.SetGameWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MAP);
				}
				break;
			case WMAINTOP_CONTROL_ENUM.WMTCE_HERO:
				{
					NFStart.Instance.SetJoyEnable (false);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_HERO_LIST);
					NFCWindowManager.Instance.ShowWindows (NFCWindows.UI_WIN_STATE.UI_WIN_HERO_CENTER);
				}
				break;
			default:
				break;
		}
	}
}