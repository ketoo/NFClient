using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;
using NFrame;

public class NFUIMain : NFUIDialog
{
	   
	private NFLoginModule mLoginModule;
	private NFUIModule mUIModule;
	private NFIEventModule mEventModule;
    private NFNetModule mNetModule;

    public override void Init()
    {

        NFIPluginManager xPluginManager = NFPluginManager.Instance();
        mLoginModule = xPluginManager.FindModule<NFLoginModule>();
        mUIModule = xPluginManager.FindModule<NFUIModule>();
        mEventModule = xPluginManager.FindModule<NFIEventModule>();
        mNetModule = xPluginManager.FindModule<NFNetModule>();
    }

    // Use this for initialization
    void Start ()
	{
        mNetModule.RequireSwapScene(0, 1, 1);
    }

}
