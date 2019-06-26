using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFSDK;
using NFrame;

public class NFRecordController : MonoBehaviour
{
    private List<NFRecordRowData> _data;

	private NFGUID xGUID;
	private List<NFRecordRowView.RowClickEventHandler> clickEventHandler = new List<NFRecordRowView.RowClickEventHandler>();
	private List<NFRecordRowView.RowViewUpdateEventHandler> updateEventHandler = new List<NFRecordRowView.RowViewUpdateEventHandler>();

	public NFRecordRowView rowViewItem;
    public bool group = false;

    public string recordName = "";

	[HideInInspector]
	public bool ColValueCondition = false;
	[HideInInspector]
	public bool ColPropertyCondition = false;
	[HideInInspector]
	public int ColConditionNum= 0;
	[HideInInspector]
	public string ColConditionContent = "";
	[HideInInspector]
	public string ColConditionPropertyName = "";
	[HideInInspector]
	public string ColConditionPropertyValue = "";


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
        // tell the scroller that this script will be its delegate
        _data = new List<NFRecordRowData>();

		mkernelModule.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);

        //generally speaking, this object will be created after the player login (be created)
        //as a result, we must add the data when the UI object creating to show the data at the UI.
        if (!group)
        {
            xGUID = mLoginModule.mRoleID;
        }
        else
        {
            xGUID = new NFGUID();
        }
        {
			NFIRecord xRecord = mkernelModule.FindRecord(mLoginModule.mRoleID, recordName);
            if (xRecord != null)
            {
				mkernelModule.RegisterRecordCallback(mLoginModule.mRoleID, recordName, RecordEventHandler);
				for (int i = 0; i < xRecord.GetRows(); ++i)
				{
					if (xRecord.IsUsed(i))
					{
						RecordEventHandler(mLoginModule.mRoleID, recordName, NFIRecord.ERecordOptype.Add, i, 0, null, null);
					}
				}
            }
            else
            {
                Debug.LogError("no this record " + recordName);
            }
        }
        
        // load in a large set of data
        //LoadData();
    }


    /////////////////////////////////////////


	public void RemoveAllClickEvent()
	{
		clickEventHandler.Clear ();
	}
	public void RemoveAllUpdateClickEvent()
	{
		updateEventHandler.Clear ();
	}
	public void RegisterUpdateEvent(NFRecordRowView.RowViewUpdateEventHandler handler)
	{
		updateEventHandler.Add (handler);
	}

	public void RegisterClickEvent(NFRecordRowView.RowClickEventHandler handler)
	{
		clickEventHandler.Add (handler);
	}
	public void UpdateEvent(NFGUID self, string recordName, int nRow, NFRecordRowView view)
	{
		for (int i = 0; i < updateEventHandler.Count; ++i)
		{
			NFRecordRowView.RowViewUpdateEventHandler handler = updateEventHandler [i];
			handler(self, recordName, nRow, view);
		}
	}
	public void ClickEvent(NFRecordRowData data)
	{
		for (int i = 0; i < clickEventHandler.Count; ++i)
		{
			NFRecordRowView.RowClickEventHandler handler = clickEventHandler [i];
			handler(data);
		}
	}

	void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
	{
		if (mLoginModule.mRoleID == self)
		{
			xGUID = self;

			if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
			{
				NFIRecord xRecord = mkernelModule.FindRecord(self, recordName);
				if (xRecord != null)
				{
					mkernelModule.RegisterRecordCallback(self, recordName, RecordEventHandler);
				}
				else
				{
					Debug.LogError("no this record " + recordName);
				}
			}
		}
	}

	void RecordEventHandler(NFGUID self, string strRecordName, NFIRecord.ERecordOptype eType, int nRow, int nCol, NFDataList.TData oldVar, NFDataList.TData newVar)
	{
		if (ColValueCondition)
		{
            if (ColConditionNum >= 0 && ColConditionContent.Length > 0)
            {
                NFIRecord xRecord = mkernelModule.FindRecord(self, recordName);
                if (xRecord != null)
                {
                    if (xRecord.GetCols() > ColConditionNum)
                    {
                        switch (xRecord.GetColType(ColConditionNum))
                        {
                            case NFDataList.VARIANT_TYPE.VTYPE_INT:
                                {
                                    long value = xRecord.QueryInt(nRow, ColConditionNum);
                                    if (value.ToString() != ColConditionContent)
                                    {
                                        return;
                                    }
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                                {
                                    string value = xRecord.QueryString(nRow, ColConditionNum);
                                    if (value != ColConditionContent)
                                    {
                                        return;
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                }

            }
		}

		if (ColPropertyCondition)
        {
			if (ColConditionNum >= 0 && ColConditionPropertyName.Length > 0 && ColConditionPropertyValue.Length > 0)
            {
                NFIRecord xRecord = mkernelModule.FindRecord(self, recordName);
                if (xRecord != null)
                {
                    if (xRecord.GetCols() > ColConditionNum)
                    {
						switch (xRecord.GetColType(ColConditionNum))
                        {
                            case NFDataList.VARIANT_TYPE.VTYPE_INT:
                                {
									long value = xRecord.QueryInt(nRow, ColConditionNum);
                                    NFIElement xElement = mElementModule.GetElement(value.ToString());
                                    if (xElement == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value.ToString());
                                        return;
                                    }

                                    NFIProperty xProperty = xElement.GetPropertyManager().GetProperty(ColConditionPropertyName);
                                    if (xProperty == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value.ToString() + " Property:" + ColConditionPropertyName);
                                        return;
                                    }

                                    if (xProperty.GetData() != null)
                                    {
                                        switch (xProperty.GetData().GetType())
                                        {
                                            case NFDataList.VARIANT_TYPE.VTYPE_INT:
                                                {
                                                    if (xProperty.QueryInt().ToString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                            case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                                                {
                                                    if (xProperty.QueryString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                                {
                                    string value = xRecord.QueryString(nRow, ColConditionNum);
									NFIElement xElement = mElementModule.GetElement(value);
                                    if (xElement == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value);
                                        return;
                                    }

                                    NFIProperty xProperty = xElement.GetPropertyManager().GetProperty(ColConditionPropertyName);
                                    if (xProperty == null)
                                    {
                                        Debug.LogError("Col:" + ColConditionNum.ToString() + " Value:" + value + " Property:" + ColConditionPropertyName);
                                        return;
                                    }

                                    if (xProperty.GetData() != null)
                                    {
                                        switch (xProperty.GetData().GetType())
                                        {
                                            case NFDataList.VARIANT_TYPE.VTYPE_INT:
                                                {
                                                    if (xProperty.QueryInt().ToString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                            case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                                                {
                                                    if (xProperty.QueryString() != ColConditionPropertyValue)
                                                    {
                                                        return;
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                }
                                break;
                            default:
                                break;
                        }


						            
                    }
                }

            }
        }

		switch(eType)
		{
			case NFIRecord.ERecordOptype.Add:
				{
					NFRecordRowData rowData = new NFRecordRowData ();
					rowData.row = nRow;
					rowData.recordName = strRecordName;
					rowData.id = self;

					_data.Add (rowData);

                    StartCoroutine(CreateObject(self, rowViewItem, rowData));

				}
				break;
			case NFIRecord.ERecordOptype.Del:
				{
					int nIndex = -1;
					for (int i = 0; i < _data.Count; ++i)
					{
						NFRecordRowData rowData = _data[i];
						if (rowData.row == nRow)
						{
							nIndex = nRow;
                            break;
						}
					}

                    if (nIndex >= 0)
                    {
                        _data.RemoveAt(nIndex);
                    }

                    NFRecordRowView[] rowViews = this.GetComponentsInChildren<NFRecordRowView>();
                    for (int i = 0; i < rowViews.Length; ++i)
                    {
                        NFRecordRowData rowData = rowViews[i].GetData();
                        if (rowData != null
                           && rowData.row == nRow)
                        {
                            GameObject.Destroy(rowViews[i].gameObject);
                            break;
                        }
                    }
                }
				break;
			case NFIRecord.ERecordOptype.Update:
				{
                    NFRecordRowView[] rowViews = this.GetComponentsInChildren<NFRecordRowView>();
                    for (int i = 0; i < rowViews.Length; ++i)
                    {
                        NFRecordRowData rowData = rowViews[i].GetData();
                        if (rowData != null
                           && rowData.row == nRow)
                        {
                            rowViews[i].SetData(self, recordName, this, rowData);
                            break;
                        }
                    }
				}
				break;
			case NFIRecord.ERecordOptype.Create:
				break;
			case NFIRecord.ERecordOptype.Cleared:
                _data.Clear();
                break;
			default:
				break;
		}

	}

    private IEnumerator CreateObject(NFGUID self, NFRecordRowView go, NFRecordRowData rowData)
    {
        NFRecordRowView rowObject = GameObject.Instantiate(rowViewItem);
        if (rowObject)
        {
            rowObject.transform.parent = this.transform;
            rowObject.SetData(self, recordName, this, rowData);
        }

        yield return 0;
    }
}