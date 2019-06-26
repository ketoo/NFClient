using UnityEngine;
using System.Collections;
using NFSDK;
using NFrame;

public class NFRoot : MonoBehaviour 
{

    public bool cmdTool = false;


	NFIClassModule mClassModule;
	NFNetModule mNetModule;
	NFUIModule mUIModule;
    UIGMTool mTool = new UIGMTool();

    private NFPluginManager mPluginManager;
    private static NFRoot _instance = null;
    public static NFRoot Instance()
    {
        return _instance;
    }

    void Start()
    {
        _instance = this;

        Debug.Log("Root Start");
        mPluginManager = new NFPluginManager();
        mPluginManager.Registered(new NFSDKPlugin(mPluginManager));
        mPluginManager.Registered(new NFLogicPlugin(mPluginManager));
		mPluginManager.Registered(new NFUIPlugin(mPluginManager));
		mPluginManager.Registered(new NFScenePlugin(mPluginManager));


		mClassModule = mPluginManager.FindModule<NFIClassModule>();
		mNetModule = mPluginManager.FindModule<NFNetModule>();
		mUIModule = mPluginManager.FindModule<NFUIModule>();

        //mClassModule.SetDataPath("../../_Out/");

        if (RuntimePlatform.Android == Application.platform
		    ||RuntimePlatform.IPhonePlayer == Application.platform)
		{
			//mPluginManager.FindModule<NFIClassModule>().SetDataPath("./");
		}

        mPluginManager.Awake();
        mPluginManager.Init();
        mPluginManager.AfterInit();

		mUIModule.ShowUI<NFUILogin>();
        mTool.Init();

        mNetModule.StartConnect("192.168.13.133", 14001);

        DontDestroyOnLoad(gameObject);
	}
	
    void OnDestroy()
    {
        Debug.Log("Root OnDestroy");
        mPluginManager.BeforeShut();
        mPluginManager.Shut();
        mPluginManager = null;
    }
	
	void Update () 
    {
		mPluginManager.Execute();
	}

    private void OnGUI()
    {
        if (cmdTool)
        {
            mTool.OnGUI();
        }
    }
}
