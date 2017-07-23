using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFrame;

public class NFCGUICreaterHallEvent : NFIControl
{

    public enum WCREATER_CONTROL_ENUM
    {
        WCHC_JOB1,
        WCHC_JOB2,
        WCHC_JOB3,

        WCHC_SEX1,
        WCHC_SEX2,

        WCHC_ENTER,
        WCHC_RETURN,

        WCHC_CAMP1,
        WCHC_CAMP2,
        WCHC_CAMP3,
        WCHC_CAMP4,

        WCHC_INPUTNAME,
        WCHC_RESIT,

		WHCH_CAMERANODE = 100,
		WHCH_TARGETNODE_1,
		WHCH_TARGETNODE_2,
		WHCH_TARGETNODE_3,
		WHCH_HERO_NODE_1 = 110,
		WHCH_HERO_NODE_2,
		WHCH_HERO_NODE_3,
		WHCH_DESC_NODE_1 = 120,
		WHCH_DESC_NODE_2,
		WHCH_DESC_NODE_3,

		WHCH_JOB_DESC = 200,

    }
    //////////////all the windows logic class must need the next three menber elements
    public WCREATER_CONTROL_ENUM eControlType;
    private static Hashtable mhtWindow = new Hashtable();
    private Transform mRootPanel = null;
    /////////////////////////////////////////////

	static int nCurrJob = 0;

    void Awake()
    {
        mhtWindow[eControlType] = this.gameObject;
    }

    void Start()
    {
        if (WCREATER_CONTROL_ENUM.WCHC_ENTER == eControlType
			||WCREATER_CONTROL_ENUM.WCHC_JOB1 == eControlType
			||WCREATER_CONTROL_ENUM.WCHC_JOB2 == eControlType
			||WCREATER_CONTROL_ENUM.WCHC_JOB3 == eControlType)
        {
            Button btn = this.gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate{ this.OnCreateClick(this.gameObject); });
        }

		if (WCREATER_CONTROL_ENUM.WCHC_ENTER == eControlType)
		{
			NFCSectionManager.Instance.RegisterSectionStateCallback (NFCSection.UI_SECTION_STATE.UISS_CREATEHALL, SectionStateEventHandler);
		
			//添加一个手指滑动的事件。
		}
    }

	public void SectionStateEventHandler(NFCSection.UI_SECTION_STATE eState, bool bNowActive)
	{
		if (eState == NFCSection.UI_SECTION_STATE.UISS_CREATEHALL && bNowActive)
		{
			SelectJob ();
		}
	}

	void OnDestroy()
	{
		if (WCREATER_CONTROL_ENUM.WCHC_ENTER == eControlType)
		{
		}
	}

    void Update()
    {
		WCREATER_CONTROL_ENUM eCurrHeroNodeValue = (WCREATER_CONTROL_ENUM)((int)WCREATER_CONTROL_ENUM.WHCH_HERO_NODE_1 + nCurrJob);
		GameObject xCurrHeroNode = (GameObject)mhtWindow[eCurrHeroNodeValue];
    }

    string GetInputName()
    {
        GameObject go = (GameObject)mhtWindow[WCREATER_CONTROL_ENUM.WCHC_INPUTNAME];
        if (null != go)
        {
            InputField xUIAccount = go.GetComponent<InputField>();
            return xUIAccount.text;
        }

        return "";
    }

	void HideLastJob(int nLastJob)
	{
		WCREATER_CONTROL_ENUM eLastDescValue = (WCREATER_CONTROL_ENUM)((int)WCREATER_CONTROL_ENUM.WHCH_DESC_NODE_1 + nLastJob);
		GameObject xLastDescNode = (GameObject)mhtWindow[eLastDescValue];
		if (xLastDescNode)
		{
			xLastDescNode.SetActive (false);
		}
	}

	void SelectJob()
	{
		GameObject xCameraTarn = (GameObject)mhtWindow[WCREATER_CONTROL_ENUM.WHCH_CAMERANODE];

		WCREATER_CONTROL_ENUM eCurrHeroNodeValue = (WCREATER_CONTROL_ENUM)((int)WCREATER_CONTROL_ENUM.WHCH_TARGETNODE_1 + nCurrJob);
		WCREATER_CONTROL_ENUM eCurrDescValue = (WCREATER_CONTROL_ENUM)((int)WCREATER_CONTROL_ENUM.WHCH_DESC_NODE_1 + nCurrJob);

		GameObject xCurrJobCamNode = (GameObject)mhtWindow[eCurrHeroNodeValue];
		if(xCurrJobCamNode)
		{
		}
		GameObject xCurrDescNode = (GameObject)mhtWindow[eCurrDescValue];
		if (xCurrDescNode && xCameraTarn)
		{
			xCurrDescNode.SetActive (true);
			xCameraTarn.transform.position = xCurrJobCamNode.transform.position;
		}

	}


    void OnCreateClick(GameObject go)
    {
        switch (eControlType)
        {
			case WCREATER_CONTROL_ENUM.WCHC_JOB1:
			case WCREATER_CONTROL_ENUM.WCHC_JOB2:
			case WCREATER_CONTROL_ENUM.WCHC_JOB3:
			{
				HideLastJob (nCurrJob);
				nCurrJob = eControlType - WCREATER_CONTROL_ENUM.WCHC_JOB1;
				SelectJob ();
			}
            break;

            case WCREATER_CONTROL_ENUM.WCHC_CAMP1:
            case WCREATER_CONTROL_ENUM.WCHC_CAMP2:
            case WCREATER_CONTROL_ENUM.WCHC_CAMP3:
            case WCREATER_CONTROL_ENUM.WCHC_CAMP4:
                //nLastCamp = (int)(eControlType - WCREATER_CONTROL_ENUM.WCHC_CAMP1);
                break;

            case WCREATER_CONTROL_ENUM.WCHC_SEX1:
            case WCREATER_CONTROL_ENUM.WCHC_SEX2:
                //nLastSex = (int)(eControlType - WCREATER_CONTROL_ENUM.WCHC_SEX1);
                break;

            case WCREATER_CONTROL_ENUM.WCHC_ENTER:
                CreateRole();
                break;

            case WCREATER_CONTROL_ENUM.WCHC_RETURN:
                break;

            case WCREATER_CONTROL_ENUM.WCHC_RESIT:
                break;
            default:
                break;
        }

    }

    void CreateRole()
    {
        string strInputName = GetInputName();
        GameObject go = (GameObject)mhtWindow[WCREATER_CONTROL_ENUM.WCHC_INPUTNAME];
        if (null != go)
        {
			NFNetController xNet = NFNetController.Instance;
            if (null != xNet)
            {
				xNet.mxNetSender.RequireCreateRole(xNet.strAccount, strInputName, 0, 0, xNet.nServerID);
            }
        }
    }
}