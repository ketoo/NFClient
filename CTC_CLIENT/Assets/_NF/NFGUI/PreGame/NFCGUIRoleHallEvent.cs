using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFrame;

public class NFCGUIRoleHallEvent : NFIControl
{

    public enum WROLE_LIST_CONTROL_ENUM
    {
        WLCE_ENTRY_GAME,
        WLCE_ROLE_NAME_LBL,
        WLCE_ROLE_DETAIL_LBL,

    }
    //////////////all the windows logic class must need the next three menber elements
    public WROLE_LIST_CONTROL_ENUM eControlType;
    private static Hashtable mhtWindow = new Hashtable();
    void Awake()
    {
        mhtWindow[eControlType] = this.gameObject;
    }

	// Use this for initialization
	void Start () 
    {
        if (WROLE_LIST_CONTROL_ENUM.WLCE_ENTRY_GAME == eControlType)
        {
            Button btn = this.gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate () { this.OnEntryClick(this.gameObject); });
        }
	}
	
	// Update is called once per frame
	void Update () 
    {
        //set name
		if (NFNetController.Instance.aCharList.Count > 0)
        {
			NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)NFNetController.Instance.aCharList[0];
            SetName(System.Text.Encoding.Default.GetString(xLiteInfo.noob_name));
            SetDetailData("Level " + xLiteInfo.role_level);
        }
	}

    void OnEntryClick(GameObject go)
    {
		if (NFNetController.Instance.aCharList.Count > 0)
        {
			NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)NFNetController.Instance.aCharList[0];
			NFrame.NFGUID xEnterID = new NFrame.NFGUID ();
			xEnterID.nData64 = xLiteInfo.id.index;
			xEnterID.nHead64 = xLiteInfo.id.svrid;
			NFNetController.Instance.mxNetSender.RequireEnterGameServer(xEnterID, NFNetController.Instance.strAccount, System.Text.Encoding.Default.GetString(xLiteInfo.noob_name), NFNetController.Instance.nServerID);
        }
    }

    void SetName(string strName)
    {
        GameObject go = (GameObject)mhtWindow[WROLE_LIST_CONTROL_ENUM.WLCE_ROLE_NAME_LBL];
        if (null != go)
        {
            Text xUIName = go.GetComponent<Text>();
            xUIName.text = strName;
        }
    }

    void SetDetailData(string strData)
    {
        GameObject go = (GameObject)mhtWindow[WROLE_LIST_CONTROL_ENUM.WLCE_ROLE_DETAIL_LBL];
        if (null != go)
        {
            Text xUIName = go.GetComponent<Text>();
            xUIName.text = strData;
        }
    }
}
