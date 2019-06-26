using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using NFSDK;
using NFrame;

public class NFRecordRowView : MonoBehaviour
{
	public delegate void RowClickEventHandler(NFRecordRowData data);
	public delegate void RowViewUpdateEventHandler(NFGUID self, string recordName, int nRow, NFRecordRowView view);

	//must working as the col-view manager
	private Dictionary<int, NFRecordColView> colViewList = new Dictionary<int, NFRecordColView>();
	private NFRecordRowData data;
	private NFRecordController controller;
	private List<RowClickEventHandler> eventHandler = new List<RowClickEventHandler>();

	public GameObject selectPanel;
	private static GameObject lastSelect;
    //public Text text;

	private NFIKernelModule mkernelModule;
    private NFIClassModule mClassModule;
    private NFIElementModule mElementModule;
    private NFLoginModule mLoginModule;

	// Use this for initialization
	private void Awake()
	{
		mkernelModule = NFPluginManager.Instance().FindModule<NFIKernelModule>();
		mClassModule = NFPluginManager.Instance().FindModule<NFIClassModule>();
		mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
		mElementModule = NFPluginManager.Instance().FindModule<NFIElementModule>();

	}

    void Start()
    {
    	Button btn = this.gameObject.GetComponent<Button>();
    	if (btn == null)
    	{
    		btn = this.gameObject.AddComponent<Button> ();
    	}
    
    	btn.enabled = true;
    	btn.onClick.AddListener(delegate () { this.OnClick(this.gameObject); });
    }
    
    public void AddColView(int col, NFRecordColView colView)
    {
    	colViewList.Add (col, colView);
    }
    
    public void RemoveAllClickEvent()
    {
    	eventHandler.Clear ();
    }
    
    public void RegisterClickEvent(RowClickEventHandler handler)
    {
    	eventHandler.Add (handler);
    }

    public NFRecordRowData GetData()
    {
        return data;
    }

    public void SetData(NFGUID xGUID, string strRecordName, NFRecordController xController, NFRecordRowData xData)
    {
		data = xData;
		controller = xController;

		if (data != null)
		{
			foreach (KeyValuePair<int, NFRecordColView> entry in colViewList)
			{
			    NFIRecord xRecord = mkernelModule.FindRecord (xGUID, strRecordName);

				entry.Value.Refresh (xGUID, xRecord.QueryRowCol (data.row, entry.Key));
			}

			xController.UpdateEvent (xData.id, xData.recordName, xData.row, this);
		}
    }

    void OnClick(GameObject go)
    {
    	for (int i = 0; i < eventHandler.Count; ++i)
    	{
    		RowClickEventHandler handler = eventHandler [i];
    		handler(data);
    	}
    
    	controller.ClickEvent (data);
    	if (lastSelect != null)
    	{
    		lastSelect.SetActive (false);
    	}
    
    	if (selectPanel != null)
    	{
    		selectPanel.SetActive (true);
    	}
    
    	lastSelect = selectPanel;
    }
}