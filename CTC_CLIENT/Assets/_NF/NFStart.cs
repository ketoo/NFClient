#if !NF_CLIENT_FRAME
#define NF_CLIENT_FRAME
#endif

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NFrame;

public class NFStart : MonoBehaviour
{
    public NFObjectElement mxObjectElement = null;
    public NFObjectGM mxObjectGM = null;

    public NFConfig mConfig = null;
    public string strTargetIP = "";
    public int nPort = 0;
    public bool bCommand = false;
    public bool bLog = false;

    public Transform[] mMainNodeTrans;
    public Transform mJoyNode;


	public void SetJoyEnable(bool b)
	{
		if (mJoyNode)
		{
		}
	}
    #region Instance
    private static NFStart _Instance = null;
    public static NFStart Instance
    {
        get
        {
            return _Instance;
        }

    }
    #endregion

	void Test()
	{

	}

    void Awake()
    {
		Test ();


        _Instance = this;

        foreach (Transform trans in mMainNodeTrans)
        {
            if (null != trans)
            {
                trans.gameObject.SetActive(true);
            }
        }

        mxObjectElement = new NFObjectElement();
        mxObjectGM = new NFObjectGM();

        mConfig = new NFConfig();
        mConfig.Load();
        mConfig.GetSelectServer(ref strTargetIP, ref nPort);

        NFCKernelModule.Instance.GetLogicClassModule().SetDataPath(mConfig.GetDataPath());

        NFCKernelModule.Instance.Init();
    }

    // Use this for initialization
    void Start()
    {

		SetJoyEnable (false);

        NFCKernelModule.Instance.AfterInit();
        NFRender.Instance.Init();

        NFCSectionManager.Instance.Start();
		NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_LOGIN);

        foreach (NFCWindows.UI_WIN_STATE eState in Enum.GetValues(typeof(NFCWindows.UI_WIN_STATE)))
        {
            NFCWindowManager.Instance.RegisterWindowStateCallback(eState, OnWindowsStateChangede);
        }

    }

    void OnDestroy()
    {

    }

    void OnGUI()
    {
        if (bCommand)
        {
            mxObjectElement.OnGUI(NFCKernelModule.Instance, 640, 960);
            mxObjectGM.GUICall(800, 600);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            bCommand = !bCommand;
        }

        NFCKernelModule.Instance.Execute(Time.deltaTime);
		NFNetController.Instance.Execute ();

			switch (NFNetController.Instance.mPlayerState)
            {
				case NFNetController.PLAYER_STATE.E_DISCOUNT:
					{
						NFNetController.Instance.mPlayerState = NFNetController.PLAYER_STATE.E_NONE;
						NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_LOGIN);

					}
					break;
				case NFNetController.PLAYER_STATE.E_NONE:
                    {
                        if (strTargetIP.Length > 0)
                        {
							NFNetController.Instance.StartConnect(strTargetIP, nPort);
							NFNetController.Instance.mPlayerState = NFNetController.PLAYER_STATE.E_CONECTING;
                        }
                    }
                    break;
				case NFNetController.PLAYER_STATE.E_CONNECTED:
                    {
						if (NFNetController.Instance.strKey.Length > 0)
                        {
							//NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_SERVERLIST);
							
							Debug.Log ("Start to verify key!");

							NFNetController.Instance.mxNetSender.RequireVerifyWorldKey(NFNetController.Instance.strAccount, NFNetController.Instance.strKey);
							NFNetController.Instance.mPlayerState = NFNetController.PLAYER_STATE.E_WATING_VERIFY;
                        }
                        else
                        {
							NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_LOGIN);
                        }
                    }
                    break;
				case NFNetController.PLAYER_STATE.E_START_CONNECT_TO_GATE:
				{
						string strWorpdIP = NFNetController.Instance.strWorldIP;
						string strWorpdKey = NFNetController.Instance.strKey;
						string strAccount = NFNetController.Instance.strKey;
						int nPort = NFNetController.Instance.nWorldPort;


						NFNetController.Instance.strWorldIP = strWorpdIP;
						NFNetController.Instance.strKey = strWorpdKey;
						NFNetController.Instance.strAccount = strAccount;
						NFNetController.Instance.nWorldPort = nPort;


						NFNetController.Instance.StartConnect (strWorpdIP, nPort);

						NFNetController.Instance.mPlayerState = NFNetController.PLAYER_STATE.E_CONECTING;
				}
				break;


				case NFNetController.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME:
                    {

                    }
                    break;

				case NFNetController.PLAYER_STATE.E_PLAYER_GAMEING:
                    {
						//SetJoyEnable (true);
                       
                    }
                    break;

                default:
                    break;

            }
    }

    void OnWindowsStateChangede(NFCWindows.UI_WIN_STATE eState, bool bNowActive, int nActiveCount)
    {
        if (NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP == eState)
        {
            return;
        }

    }
}
