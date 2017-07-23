using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public class NFCWindowMainHallLogicEvent : NFIControl
{	
	
	public enum WMHALL_CONTROL_ENUM
	{
		WMHALLCE__MAIN_HALL,
		
		WMHALLCE_PROPERTY,
		WMHALLCE_BAG,
		WMHALLCE_SKILL,
		WMHALLCE_TALENT,
		WMHALLCE_TASK,
		WMHALLCE_MAGIC,
		WMHALLCE_EQUIPE,
		
		WMHALLCE_SKILL_DETAIL,
		WMHALLCE_TALENT_DETAIL,
		WMHALLCE_TASK_DETAIL,
		WMHALLCE_MAGIC_DETAIL,
		
		WMHALLCE_CLOSE_BTN,
		
		WMHALLCE_PROPERTY_BTN,
		WMHALLCE_BAG_BTN,
		WMHALLCE_SKILL_BTN,
		WMHALLCE_TALENT_BTN,
		WMHALLCE_TASK_BTN,
		WMHALLCE_MAGIC_BTN,
		
		WMHALLCE_HELP_BTN,
	}

	//////////all the windows logic class must need the next three menber elements
	public WMHALL_CONTROL_ENUM eControlType;
	private static Hashtable mhtWindow = new Hashtable();
	private static Transform mRootPanel = null;
	//private static WMHALL_CONTROL_ENUM mFocusControl = WMHALL_CONTROL_ENUM.WMHALLCE_EQUIP_Btn1;
	
	/////////////////////////////////////////////
    void Awake()
    {
        mhtWindow[eControlType] = this.gameObject;
    }

	void Start()
	{
		mRootPanel = NFCUIHelp.Instance.GetRootWindow(this.transform);
		UpdateControl();
    }

    void Update()
	{

	}
	
	public static void SetShowItemIcon(WMHALL_CONTROL_ENUM eControl)
	{
		//GameObject goMain = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE__MAIN_HALL];
	}
	
	public static void ShowItemInfo(WMHALL_CONTROL_ENUM eControl)
	{
			//mFocusControl = eControl;
	}
	
	public static void ShowChild(WMHALL_CONTROL_ENUM eControl)
	{
		HideMainHallStateChild();
		
		mRootPanel.gameObject.SetActive(true);
		
		GameObject goMain = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE__MAIN_HALL];
		goMain.SetActive(true);
		
		switch(eControl)
		{
		
		case WMHALL_CONTROL_ENUM.WMHALLCE_PROPERTY:
		{
			GameObject newGo1 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_EQUIPE];
			newGo1.SetActive(true);
					
			GameObject newGo2 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_PROPERTY];
			newGo2.SetActive(true);
		}
			break;
		
		case WMHALL_CONTROL_ENUM.WMHALLCE_BAG:
		{
			GameObject newGo1 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_EQUIPE];
			newGo1.SetActive(true);
					
			GameObject newGo2 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_BAG];
			newGo2.SetActive(true);
		}
			break;
		
		case WMHALL_CONTROL_ENUM.WMHALLCE_SKILL:
		{
			GameObject newGo1 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_SKILL];
			newGo1.SetActive(true);
					
			GameObject newGo2 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_SKILL_DETAIL];
			newGo2.SetActive(true);
		}
			break;
		
		case WMHALL_CONTROL_ENUM.WMHALLCE_TALENT:
		{
			GameObject newGo1 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_TALENT];
			newGo1.SetActive(true);
					
			GameObject newGo2 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_TALENT_DETAIL];
			newGo2.SetActive(true);
		}
			break;
		
		case WMHALL_CONTROL_ENUM.WMHALLCE_TASK:
		{
			GameObject newGo1 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_TASK];
			newGo1.SetActive(true);
					
			GameObject newGo2 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_TASK_DETAIL];
			newGo2.SetActive(true);
		}
			break;
		
		case WMHALL_CONTROL_ENUM.WMHALLCE_MAGIC:
			{
			GameObject newGo1 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_MAGIC];
			newGo1.SetActive(true);
					
			GameObject newGo2 = (GameObject)mhtWindow[WMHALL_CONTROL_ENUM.WMHALLCE_MAGIC_DETAIL];
			newGo2.SetActive(true);
		}
			break;
			
			
		default:
			break;
		}
	}
	
	private static void HideMainHallStateChild()
	{
		for(int i = (int)WMHALL_CONTROL_ENUM.WMHALLCE_PROPERTY; i < (int)WMHALL_CONTROL_ENUM.WMHALLCE_MAGIC_DETAIL; i++)
		{
			GameObject go = (GameObject)mhtWindow[(WMHALL_CONTROL_ENUM)i];
			go.SetActive(false);
		}
	}
	
	void OnClick ()
	{
		switch(eControlType)
		{
		case WMHALL_CONTROL_ENUM.WMHALLCE_CLOSE_BTN:
			{
				mRootPanel.gameObject.SetActive(false);
			}
			break;
			
		case WMHALL_CONTROL_ENUM.WMHALLCE_HELP_BTN:
			{
				//ShowObjectInfo soi = InitEngine.Instance.GetComponent<ShowObjectInfo>();
				//soi.enabled = true;
			}
			break;
			
		case WMHALL_CONTROL_ENUM.WMHALLCE_PROPERTY_BTN:
			ShowChild(WMHALL_CONTROL_ENUM.WMHALLCE_PROPERTY);
			break;
		case WMHALL_CONTROL_ENUM.WMHALLCE_BAG_BTN:
			ShowChild(WMHALL_CONTROL_ENUM.WMHALLCE_BAG);
			break;
		case WMHALL_CONTROL_ENUM.WMHALLCE_SKILL_BTN:
			ShowChild(WMHALL_CONTROL_ENUM.WMHALLCE_SKILL);
			break;
		case WMHALL_CONTROL_ENUM.WMHALLCE_TALENT_BTN:
			ShowChild(WMHALL_CONTROL_ENUM.WMHALLCE_TALENT);
			break;
		case WMHALL_CONTROL_ENUM.WMHALLCE_TASK_BTN:	
			ShowChild(WMHALL_CONTROL_ENUM.WMHALLCE_TASK);
			break;
		case WMHALL_CONTROL_ENUM.WMHALLCE_MAGIC_BTN:
			ShowChild(WMHALL_CONTROL_ENUM.WMHALLCE_MAGIC);
			break;
			
		default:
			break;
		}
	}
	
}