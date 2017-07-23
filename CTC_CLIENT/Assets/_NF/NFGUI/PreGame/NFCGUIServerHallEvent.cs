using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFrame;

public class NFCGUIServerHallEvent : NFIControl
{

    public enum WSERVERLIST_CONTROL_ENUM
    {
        WSLC_AREA1,
        WSLC_AREA2,
        WSLC_AREA3,
        WSLC_AREA4,
        WSLC_AREA5,

        WSLC_ENTER,
        WSLC_RETURN,

        WSLC_SERVER0,
        WSLC_SERVER1,
        WSLC_SERVER2,
        WSLC_SERVER3,
        WSLC_SERVER4,
        WSLC_SERVER5,
        WSLC_SERVER6,
        WSLC_SERVER7,
        WSLC_SERVER8,
        WSLC_SERVER9,

        WSLC_AREA_LBL1,
        WSLC_AREA_LBL2,
        WSLC_AREA_LBL3,
        WSLC_AREA_LBL4,
        WSLC_AREA_LBL5,

        WSLC_SERVER_LBL0,
        WSLC_SERVER_LBL1,
        WSLC_SERVER_LBL2,
        WSLC_SERVER_LBL3,
        WSLC_SERVER_LBL4,
        WSLC_SERVER_LBL5,
        WSLC_SERVER_LBL6,
        WSLC_SERVER_LBL7,
        WSLC_SERVER_LBL8,
        WSLC_SERVER_LBL9,

        WSLC_END,
    }

    //////////////all the windows logic class must need the next three menber elements
    public WSERVERLIST_CONTROL_ENUM eControlType;
    private static Hashtable mhtWindow = new Hashtable();
    /////////////////////////////////////////////

    public static void RefreshWorld(ArrayList aWorldList)
    {
        for (int i = 0; i <= (int)WSERVERLIST_CONTROL_ENUM.WSLC_AREA5 - (int)WSERVERLIST_CONTROL_ENUM.WSLC_AREA1; i++)
        {
            if (i - (int)WSERVERLIST_CONTROL_ENUM.WSLC_AREA1 < aWorldList.Count)
            {
                NFMsg.ServerInfo info = (NFMsg.ServerInfo)aWorldList[i];
                if (null != info)
                {
                    GameObject go = (GameObject)mhtWindow[(WSERVERLIST_CONTROL_ENUM)(i + (int)WSERVERLIST_CONTROL_ENUM.WSLC_AREA1)];
                    if (null != go)
                    {
                        go.SetActive(true);

                        int j = i + (int)WSERVERLIST_CONTROL_ENUM.WSLC_AREA_LBL1;
                        GameObject goLbl = (GameObject)mhtWindow[(WSERVERLIST_CONTROL_ENUM)j];
                        if (null != goLbl)
                        {
                            goLbl.SetActive(true);
                            goLbl.GetComponent<Text>().text = System.Text.Encoding.Default.GetString(info.name);
                        }
                    }
                }
            }
            else
            {
                GameObject go = (GameObject)mhtWindow[(WSERVERLIST_CONTROL_ENUM)i + (int)WSERVERLIST_CONTROL_ENUM.WSLC_AREA1];
                if (null != go)
                {
                    go.SetActive(false);
                }
            }
        }
    }

    public static void RefresServer(ArrayList aServerList)
    {
        for (int i = 0; i <= (int)WSERVERLIST_CONTROL_ENUM.WSLC_SERVER9 - (int)WSERVERLIST_CONTROL_ENUM.WSLC_SERVER0; i++)
        {
            if (i < aServerList.Count)
            {
                NFMsg.ServerInfo info = (NFMsg.ServerInfo)aServerList[i];
                if (null != info)
                {
                    GameObject go = (GameObject)mhtWindow[(WSERVERLIST_CONTROL_ENUM)(i + (int)WSERVERLIST_CONTROL_ENUM.WSLC_SERVER0)];
                    if (null != go)
                    {
                        go.SetActive(true);

                        int j = i + (int)WSERVERLIST_CONTROL_ENUM.WSLC_SERVER_LBL0;
                        GameObject goLbl = (GameObject)mhtWindow[(WSERVERLIST_CONTROL_ENUM)j];
                        if (null != goLbl)
                        {
                            goLbl.SetActive(true);
                            goLbl.GetComponent<Text>().text = System.Text.Encoding.Default.GetString(info.name);
                        }
                    }
                }
            }
            else
            {
                GameObject go = (GameObject)mhtWindow[(WSERVERLIST_CONTROL_ENUM)(i + (int)WSERVERLIST_CONTROL_ENUM.WSLC_SERVER0)];
                if (null != go)
                {
                    go.SetActive(false);
                }
            }
        }
    }

    void Awake()
    {
        mhtWindow[eControlType] = this.gameObject;
    }

    void Start()
    {
        if (eControlType >= WSERVERLIST_CONTROL_ENUM.WSLC_AREA1 && eControlType <= WSERVERLIST_CONTROL_ENUM.WSLC_AREA5)
        {
            Button btn = this.gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate () { this.OnGroupClick(this.gameObject); });
        }

        if (eControlType >= WSERVERLIST_CONTROL_ENUM.WSLC_SERVER0 && eControlType <= WSERVERLIST_CONTROL_ENUM.WSLC_SERVER9)
        {
            Button btn = this.gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate () { this.OnServerClick(this.gameObject); });
        }
    }

    void Update()
    {
		NFNetController xNet = NFNetController.Instance;
		if (null != xNet && WSERVERLIST_CONTROL_ENUM.WSLC_ENTER == eControlType)
        {
			RefreshWorld(xNet.aWorldList);
			RefresServer(xNet.aServerList);
        }
    }

    void OnGroupClick(GameObject go)
    {
        switch (eControlType)
        {
            case WSERVERLIST_CONTROL_ENUM.WSLC_AREA1:
            case WSERVERLIST_CONTROL_ENUM.WSLC_AREA2:
            case WSERVERLIST_CONTROL_ENUM.WSLC_AREA3:
            case WSERVERLIST_CONTROL_ENUM.WSLC_AREA4:
                {
                    int i = eControlType - WSERVERLIST_CONTROL_ENUM.WSLC_AREA1;
					NFNetController xNet = NFNetController.Instance;
					if (xNet.aWorldList.Count >= i)
                    {
						NFMsg.ServerInfo info = (NFMsg.ServerInfo)xNet.aWorldList[i];
                        if (null != info)
                        {
                            NFNetController.Instance.mxNetSender.RequireConnectWorld(info.server_id);
                        }
                    }
                }
                break;

            default:
                break;
        }

    }

    void OnServerClick(GameObject go)
    {

        switch (eControlType)
        {
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER0:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER1:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER2:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER3:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER4:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER5:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER6:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER7:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER8:
            case WSERVERLIST_CONTROL_ENUM.WSLC_SERVER9:
                {
                    int i = eControlType - WSERVERLIST_CONTROL_ENUM.WSLC_SERVER0;
					NFNetController xNet = NFNetController.Instance;
					if (xNet.aServerList.Count >= i)
                    {
						NFMsg.ServerInfo info = (NFMsg.ServerInfo)xNet.aServerList[i];
                        if (null != info)
                        {
                            NFNetController.Instance.nServerID = info.server_id;
                            NFNetController.Instance.mxNetSender.RequireSelectServer(info.server_id);
                        }
                    }
                }
                break;

            default:
                break;
        }
    }
}