using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using NFrame;

[ExecuteInEditMode]
public class NFLoadingMgr : MonoBehaviour
{
	private static bool bInitSend = false;

    private AsyncOperation asy;
    private int mnProgress = 0;

    private GameObject mxBG;
    private GameObject mxProgress;
    private GameObject mxLbl;

	public Sprite[] xTextureList;
	public GameObject GUIHolder;

    private float fDelayTime = 0.0f;
    private float fMaxDelayTime = 3.0f;
    private int mnSceneID = 0;

    #region Instance
    private static NFLoadingMgr _Instance = null;
    public static NFLoadingMgr Instance
    {
        get
        {
            return _Instance;
        }

    }
    #endregion


    private void Awake()
    {
        _Instance = this;

    }
    private void Start()
    {

    }

    void OnGUI()
    {
        if (asy != null)
        {
            if (!asy.isDone)
            {
                CpPercentage();
                DrawLoadingUI();

                fDelayTime = fMaxDelayTime;
            }
            else
            {
                if (fDelayTime > 0.0f)
                {
                    fDelayTime -= Time.deltaTime;

                    CpPercentage();
                    DrawLoadingUI();
                }
                else
                {
                    NFRender.Instance.LoadSceneEnd(mnSceneID);
                }
            }
        }
        
    }

    private void Update()
    {
    }

	private void RemoveTile(NFMsg.AckMiningTitle strData)
	{
		if (null == strData)
		{
			return;
		}

		for (int i = 0; i < strData.tile.Count; ++i)
		{
			NFMsg.TileState xTileState = strData.tile [i];
			if (xTileState.opr == 0) 
			{
				//挖
			}
			else
				if (xTileState.opr == 1) 
				{
					//补
					//NFCTileMng.Instance.AddTile(xTileState.x, xTileState.y);
				}

		}
	}

	public void LoadLevel(int nSceneID, NFMsg.AckMiningTitle strData)
    {
        mnSceneID = nSceneID;

        NFRender.Instance.SetMainRoleAgentState(false);

       mnProgress = 0;
       NFrame.NFIElement xElement = NFrame.NFCKernelModule.Instance.GetElementModule().GetElement(nSceneID.ToString());
		if (null != xElement) 
		{
			string strName = xElement.QueryString ("SceneName");
			string strUIName = xElement.QueryString ("LoadingUI");

			UnityEngine.SceneManagement.Scene xSceneInfo = SceneManager.GetActiveScene ();
			if (xSceneInfo.name == strName)
			{
				//Debug.LogWarning ("begin the same scene" + strSceneID);
				//SceneManager.LoadScene (xSceneInfo.buildIndex);
				//Debug.LogWarning ("end the same scene" + strSceneID);
				//load a empty scene then load this scene asy

				SceneManager.LoadScene ("EmptyScene");
			}

			StartCoroutine (LoadLevel (strName, strUIName, strData));
		}
		else 
		{
			Debug.LogError ("LoadLevel error: " + nSceneID);
		}

        //NFRender.Instance.SetMainRoleAgentState(true);
    }

	private IEnumerator LoadLevel(string strSceneID, string strUI, NFMsg.AckMiningTitle strData)
    {
		asy = SceneManager.LoadSceneAsync (strSceneID);

        DirectoryInfo mydir = new DirectoryInfo(strUI);
        if (mydir.Exists)
        {
            //xTexture = GameObject.Instantiate(Resources.Load(strUI)) as Texture;
        }

		int nIndex = Random.Range (0, xTextureList.Length);
		Sprite xSprite = xTextureList [nIndex];
		Image xImage = GUIHolder.GetComponent<Image> ();
		xImage.overrideSprite = xSprite;
		GUIHolder.SetActive (true);

		NFStart.Instance.SetJoyEnable (false);
		NFCWindowManager.Instance.HideWindows (NFCWindows.UI_WIN_STATE.UI_WIN_SEARCH_OPPNENT);

        while (!asy.isDone)
        {
			int nProgress = CpPercentage();
			if (nProgress >= 100)
			{
				break;
			}

            yield return new WaitForEndOfFrame();
        }

		if (!bInitSend)
		{
			bInitSend = true;

			NFNetController.Instance.mxNetSender.RequireEnterGameFinish (NFNetController.Instance.xMainRoleID);
		}

		GUIHolder.SetActive (false);


		//joy will show if this scene is a flighting scene
		RemoveTile(strData);

		NFCWindowManager.Instance.HideWindows (NFCWindows.UI_WIN_STATE.UI_WIN_SEARCH_OPPNENT);

		bool bEnable = NFCWindowManager.Instance.GetGameWindowEnable (NFCWindows.UI_WIN_STATE.UI_WIN_MAIN_SKILL);
		NFStart.Instance.SetJoyEnable (bEnable);
   }

    int CpPercentage()
    {
        int nNorPprogress = (int)(asy.progress * 100.0f);
        if (mnProgress < nNorPprogress)
        {
            int nStep = 2;
            if (nNorPprogress - mnProgress> 10)
            {
                nStep = 5;
            }
            
            mnProgress += nStep;
        }
        //未完成
        if (mnProgress >= 90)
        {
            mnProgress += (int)((1.0f - fDelayTime / fMaxDelayTime) * 10);
            if (mnProgress > 100)
            {
                mnProgress = 100;
            }
        }

		return mnProgress;
    }

    private void DrawLoadingUI()
    {

        string strDesc = mnProgress.ToString() + " / 100";

        int nLeft = (int)(Screen.width * 0.25f);
        int nWidth = (int)(Screen.width * 0.5f);
        int nTop = Screen.height - 100;
        int nHeight = 50;

        //GUI.HorizontalScrollbar(new Rect(nLeft, nTop, nWidth, nTop + nHeight), mnProgress * 1.0f, mnProgress * 1.0f, 0f, 100f);
        GUI.Label(new Rect(Screen.width / 2 - 50, Screen.height - 50, Screen.width, Screen.height), strDesc);
    }
}