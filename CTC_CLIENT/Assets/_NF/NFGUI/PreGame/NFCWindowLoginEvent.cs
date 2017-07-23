using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFrame;

public class NFCWindowLoginEvent : NFIControl
{

    public enum WLOGIN_CONTROL_ENUM
    {
        WLCE_LOGIN,
        WLCE_REGISTER,
        WLCE_INPUTACCOUNT,
        WLCE_INPUTPASSWORD,
        WLCE_RETURN,
        WLCE_SAVEINFO,
    }

    //////////all the windows logic class must need the next three menber elements
    public WLOGIN_CONTROL_ENUM eControlType;
    private static Hashtable mhtWindow = new Hashtable();
    /////////////////////////////////////////////
    void Awake()
    {
        mhtWindow[eControlType] = this.gameObject;
    }

    void Start()
	{
        if (WLOGIN_CONTROL_ENUM.WLCE_LOGIN == eControlType
			||WLOGIN_CONTROL_ENUM.WLCE_REGISTER == eControlType)
        {
            Button btn = this.gameObject.GetComponent<Button>();
            btn.onClick.AddListener(delegate () { this.OnClick(this.gameObject); });
        }

        UpdateControl();
    }

    void Update()
    {

    }

    string GetAccount()
    {
        GameObject go = (GameObject)mhtWindow[WLOGIN_CONTROL_ENUM.WLCE_INPUTACCOUNT];
        if(null != go)
        {
            InputField xUIAccount = go.GetComponent<InputField>();
            return xUIAccount.text;
        }

        return "";
    }

    string GetPassWord()
    {
        GameObject go = (GameObject)mhtWindow[WLOGIN_CONTROL_ENUM.WLCE_INPUTPASSWORD];
        if (null != go)
        {
            InputField xUIAccount = go.GetComponent<InputField>();
            return xUIAccount.text;
        }

        return "";
    }

    void OnClick(GameObject go)
    {
        switch (eControlType)
        {
            case WLOGIN_CONTROL_ENUM.WLCE_LOGIN:
                OnLogin();
                break;
            case WLOGIN_CONTROL_ENUM.WLCE_REGISTER:
                OnRegister();
                break;
            case WLOGIN_CONTROL_ENUM.WLCE_SAVEINFO:
                OnSaveInfo();
                break;
            default:
                break;
        }

    }

    void OnLogin()
    {
        string strAccount = GetAccount();
        string strPassword = GetPassWord();

        if(strAccount.Length > 0 && strPassword.Length > 0)
        {
            NFNetController.Instance.mxNetSender.LoginPB(strAccount, strPassword, "");
        }
    }

    void OnRegister()
    {
		Application.OpenURL("https://github.com/ketoo/NoahGameFrame");
    }

    void OnSaveInfo()
    {
    }
}