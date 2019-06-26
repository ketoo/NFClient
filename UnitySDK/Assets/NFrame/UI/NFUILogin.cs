using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;
using NFrame;
using System.Text;

public class NFUILogin : NFUIDialog
{
    private NFIEventModule mEventModule;


    private NFNetModule mNetModule;
    private NFLoginModule mLoginModule;
    private NFUIModule mUIModule;
    private NFHelpModule mHelpModule;

    public InputField mAccount;
    public InputField mPassword;
    public Button mLogin;
    // Use this for initialization


    public override void Init()
    {
        mEventModule = NFPluginManager.Instance().FindModule<NFIEventModule>();

        mNetModule = NFPluginManager.Instance().FindModule<NFNetModule>();
        mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
        mUIModule = NFPluginManager.Instance().FindModule<NFUIModule>();
        mHelpModule = NFPluginManager.Instance().FindModule<NFHelpModule>();

        mLogin.onClick.AddListener(OnLoginClick);

        mEventModule.RegisterCallback((int)NFLoginModule.Event.LoginSuccess, OnLoginSuccess);
        mEventModule.RegisterCallback((int)NFLoginModule.Event.WorldList, OnWorldList);
        mEventModule.RegisterCallback((int)NFLoginModule.Event.ServerList, OnServerList);
        mEventModule.RegisterCallback((int)NFLoginModule.Event.SelectServerSuccess, OnSelectServer);
        mEventModule.RegisterCallback((int)NFLoginModule.Event.RoleList, OnRoleList);
    }

    void Start()
    {
        mAccount.text = PlayerPrefs.GetString("account");
        mPassword.text = PlayerPrefs.GetString("password");
    }

    // UI Event
    private void OnLoginClick()
    {
        PlayerPrefs.SetString("account", mAccount.text);
        PlayerPrefs.SetString("password", mPassword.text);
        mLoginModule.LoginPB(mAccount.text, mPassword.text, "");
    }

    // Logic Event
    public void OnLoginSuccess(NFDataList valueList)
    {
        //mUIModule.ShowUI<NFUISelectServer>();

        mLoginModule.RequireWorldList();
    }

    private void OnRoleClick(int nIndex)
    {
        ArrayList roleList = mLoginModule.mRoleList;
        NFMsg.RoleLiteInfo info = (NFMsg.RoleLiteInfo)roleList[nIndex];

        mLoginModule.mRoleID = mHelpModule.PBToNF(info.id);
        mLoginModule.mRoleName = info.noob_name.ToStringUtf8();

        mNetModule.RequireEnterGameServer();
        //mUIModule.CloseAllUI();
    }

    private void OnCreateRoleClick()
    {

        string strRoleName = mLoginModule.mAccount + "_Role";
        //string strRoleName = mLoginModule.mAccount + "_Role" + UnityEngine.Random.Range(1000, 10000);
        mLoginModule.RequireCreateRole(strRoleName, 1, 1);
    }

    // Logic Event
    public void OnRoleList(NFDataList valueList)
    {
        ArrayList roleList = mLoginModule.mRoleList;

        foreach (NFMsg.RoleLiteInfo info in roleList)
        {
            OnRoleClick(0);
            break;
        }

        OnCreateRoleClick();
    }

    public void OnSelectServer(NFDataList valueList)
    {
        //mUIModule.ShowUI<NFUISelectRole>();

        mLoginModule.RequireRoleList();
    }

    private void OnWorldServerClick(NFMsg.ServerInfo info)
    {
        mLoginModule.RequireConnectWorld(info.server_id);
    }

    private void OnGameServerClick(NFMsg.ServerInfo info)
    {
        mLoginModule.RequireSelectServer(info.server_id);
    }

    // Logic Event
    public void OnWorldList(NFDataList valueList)
    {
        ArrayList serverList = mLoginModule.mWorldServerList;

        Debug.Log("OnWorldList" + serverList.Count);

        foreach (NFMsg.ServerInfo info in serverList)
        {
            OnWorldServerClick(info);
            break;
        }
    }

    public void OnServerList(NFDataList valueList)
    {
        ArrayList serverList = mLoginModule.mGameServerList;

        Debug.Log("OnServerList" + serverList.Count);


        foreach (NFMsg.ServerInfo info in serverList)
        {
            OnGameServerClick(info);
            break;
        }
    }
}
