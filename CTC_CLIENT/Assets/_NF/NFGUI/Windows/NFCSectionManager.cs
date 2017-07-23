using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// -------------------------------------------------------------------------
//    @FileName             :    NFCSectionManager.cpp
//    @Author               :    LvSheng.Huang
//    @Date                 :    2012-07-05
//    @Module               :    NFCSectionManager
//    @Desc                 :    游戏章节管理器，主要管理游戏大章节资源
// -------------------------------------------------------------------------

public class NFCSectionManager
{
	public delegate void SectionStateEventHandler(NFCSection.UI_SECTION_STATE eState, bool bNowActive);

    #region Instance
	private NFCSection.UI_SECTION_STATE meUIState;
    private static NFCSectionManager _Instance = null;
    public static NFCSectionManager Instance
    {
        get
        {
            if(null == _Instance)
            {
                _Instance = new NFCSectionManager();
            }
            return _Instance;
        }

    }
    #endregion


	class SectionData
	{
		public int nWindowID = 0;
		public GameObject xWindowGameObject;
		public SectionStateEventHandler doWindowHandleDel;

	}

	private Dictionary<NFCSection.UI_SECTION_STATE, SectionData> mhtWindow = new Dictionary<NFCSection.UI_SECTION_STATE, SectionData> ();
	//private Hashtable mhtWindow = new Hashtable();

    public void Start()
    {
        HideAll();
    }

    void Update()
    {

    }

	public void SetGameState(NFCSection.UI_SECTION_STATE eState)
    {
        if (eState == meUIState)
        {
			SectionData xData = null;
			if (!mhtWindow.TryGetValue (eState, out xData)
				|| xData.xWindowGameObject.activeSelf == true)
			{
				return;
			}
        }

		HideState(meUIState);
		meUIState = eState;
		ShowState();
    }

	public void HideState(NFCSection.UI_SECTION_STATE eState)
    {
		SectionData xData = null;
		if (mhtWindow.TryGetValue (eState, out xData))
		{
			xData.xWindowGameObject.SetActive (false);

			if (null != xData.doWindowHandleDel)
			{
				xData.doWindowHandleDel(eState, false);
			}
		}
	}

    private void ShowState()
    {
		SectionData xData = null;
		if (mhtWindow.TryGetValue (meUIState, out xData))
		{
			xData.xWindowGameObject.SetActive (true);
			if (null != xData.doWindowHandleDel)
			{
				xData.doWindowHandleDel(meUIState, true);
			}
		}
    }

	public NFCSection.UI_SECTION_STATE GetGameState()
    {
        return meUIState;
    }

	public void AddGameSectionWindow(NFCSection.UI_SECTION_STATE eState, GameObject go)
    {
		SectionData xData = null;
		if (mhtWindow.TryGetValue (eState, out xData)) 
		{
			xData.xWindowGameObject = go;
		}
		else
		{
			xData = new SectionData ();
			xData.xWindowGameObject = go;
			mhtWindow [eState] = xData;
		}
    }

	public GameObject GetGameSectionWindow(NFCSection.UI_SECTION_STATE eState)
    {
		SectionData xData = null;
		if (mhtWindow.TryGetValue (eState, out xData)) 
		{
			return xData.xWindowGameObject;
		}

        return null;
    }

    public void HideAll()
    {
		foreach (KeyValuePair<NFCSection.UI_SECTION_STATE, SectionData> kv in mhtWindow)
        {
			SectionData xData = kv.Value;
			if (null != xData.xWindowGameObject)
            {
				xData.xWindowGameObject.SetActive(false);
				if (null != xData.doWindowHandleDel)
				{
					xData.doWindowHandleDel(meUIState, false);
				}
            }
        }
    }

	public void RegisterSectionStateCallback(NFCSection.UI_SECTION_STATE eState, SectionStateEventHandler handler)
	{
		SectionData xData = null;
		if (mhtWindow.TryGetValue (eState, out xData)) 
		{
			xData.doWindowHandleDel += handler;
		}
	}
}