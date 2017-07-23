using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using System;

public class NFObjectGM
{
    private bool bShowHelp = false;
    private bool bShowAddItem = false;
    private Vector2 scrollPositionFirst = Vector2.zero;
    private Hashtable mxGM = new Hashtable();
    private Hashtable mxGMDesc = new Hashtable();

    private string mstrItemID = "";
    private string mstrItemCount = "1";

    private Vector2 AddItemscrollPositionFirst = Vector2.zero;

    public NFObjectGM()
    {
        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_INT] = "VTYPE_STRING,VTYPE_INT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_INT] = "Property: Name,int";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_STR] = "VTYPE_STRING,VTYPE_STRING";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_STR] = "Property: Name,string";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_FLOAT] = "VTYPE_STRING,VTYPE_FLOAT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_FLOAT] = "Property: Name,float";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_OBJECT] = "VTYPE_STRING,VTYPE_OBJECT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_OBJECT] = "Property: Name,GUID";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_INT] = "VTYPE_STRING,VTYPE_INT,VTYPE_INT,VTYPE_INT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_INT] = "Record: Name,Row,Col,int";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_STR] = "VTYPE_STRING,VTYPE_INT,VTYPE_INT,VTYPE_STRING";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_STR] = "Record: Name,Row,Col,string";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_FLOAT] = "VTYPE_STRING,VTYPE_INT,VTYPE_INT,VTYPE_FLOAT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_FLOAT] = "Record: Name,Row,Col,float";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_OBJECT] = "VTYPE_STRING,VTYPE_INT,VTYPE_INT,VTYPE_OBJECT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_OBJECT] = "Record: Name,Row,Col,GUID";

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_CMD_NORMAL] = "VTYPE_INT,VTYPE_STRING,VTYPE_STRING,VTYPE_INT,VTYPE_FLOAT,VTYPE_OBJECT";
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_CMD_NORMAL] = "cmdID,name,strValue,intValue,floatValue,ObjectValue";


        ///////////////////////////////////////
        //协议ID，参数列表
        mxGM[NFMsg.EGameMsgID.EGMI_REQ_SWAP_SCENE] = "VTYPE_INT,VTYPE_INT,VTYPE_INT,VTYPE_FLOAT,VTYPE_FLOAT,VTYPE_FLOAT";//切换场景
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_SWAP_SCENE] = "SwapScene: Type,SceneID,LineID,X,Y,Z";//切换场景

        

        mxGM[NFMsg.EGameMsgID.EGMI_REQ_PICK_ITEM] = "VTYPE_OBJECT";//拾取
        mxGMDesc[NFMsg.EGameMsgID.EGMI_REQ_PICK_ITEM] = "Pick: GUID";//拾取
        
    }

    public bool SetShowHelp(bool bShow)
    {
        bShowHelp = bShow;
        return bShowHelp;
    }

    public bool GetShowHelp()
    {
        return bShowHelp;
    }

    public bool SetShowAddItem(bool bShow)
    {
        bShowAddItem = bShow;
        return bShowAddItem;
    }

    public bool GetShowAddItem()
    {
        return bShowAddItem;
    }

    public bool DoCmd(string strArg, string strGMID)
    {
        NFMsg.EGameMsgID xMsgID = (NFMsg.EGameMsgID)Enum.Parse(typeof(NFMsg.EGameMsgID), strGMID);
        if (!mxGM.ContainsKey(xMsgID))
        {
            Debug.LogWarning("GMID Error");
            return false;
        }

        string strOriginArg = (string)mxGM[xMsgID];

        NFDataList xOriginArgList = new NFDataList(strOriginArg, ',');
        NFDataList xNowArgList = new NFDataList(strArg, ',');
        if (xOriginArgList.Count() != xNowArgList.Count())
        {
            Debug.LogWarning("Arg Count Error");
            return false;
        }

        NFDataList xNowDataList = new NFDataList();
        for (int i = 0; i < xOriginArgList.Count(); ++i)
        {
            string strOriginType = xOriginArgList.StringVal(i);
            switch(strOriginType)
            {
                case "VTYPE_INT":
                    {
                        string strNowDataInt = xNowArgList.StringVal(i);
                        int nData = 0;
                        if (!int.TryParse(strNowDataInt, out nData))
                        {
                            Debug.LogWarning("Arg:" + i + " Can not parse to int:" + strNowDataInt);
                            return false;
                        }

                        xNowDataList.AddInt(nData);
                    }
                    break;
                case "VTYPE_FLOAT":
                    {
                        string strNowDataFloat = xNowArgList.StringVal(i);
                        float fData = 0;
                        if (!float.TryParse(strNowDataFloat, out fData))
                        {
                            Debug.LogWarning("Arg:" + i + " Can not parse to float:" + strNowDataFloat);
                            return false;
                        }

                        xNowDataList.AddFloat(fData);
                    }
                    break;
                case "VTYPE_STRING":
                    {
                        string strNowData = xNowArgList.StringVal(i);
                        xNowDataList.AddString(strNowData);
                    }
                    break;
                case "VTYPE_OBJECT":
                    {
                        string strNowDataObject = xNowArgList.StringVal(i);
                        NFGUID xIdent = new NFGUID();
                        if (!xIdent.Parse(strNowDataObject, out xIdent))
                        {
                            Debug.LogWarning("Arg:" + i + " Can not parse to Object:" + strNowDataObject);
                            return false;
                        }

                        xNowDataList.AddObject(xIdent);
                    }
                    break;

                default:
                    break;
            }
            //NFDataList.VARIANT_TYPE
        }

        switch (xMsgID)
        {
            case NFMsg.EGameMsgID.EGMI_REQ_SWAP_SCENE:
                {
                    //NFNetController.Instance.mxNetSender.RequireSwapScene(NFNetController.Instance.xMainRoleID, (int)xNowDataList.IntVal(0), (int)xNowDataList.IntVal(1), (int)xNowDataList.IntVal(3), 0f, 0f, 0f);
                }
                break;

            case NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_INT:
            {
                //NFNetController.Instance.mxNetSender.RequireGMPropertyInt(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.IntVal(1));
            }
            break;
            case NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_FLOAT:
            {
                //NFNetController.Instance.mxNetSender.RequireGMPropertyFloat(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.FloatVal(1));
            }
            break;
            case NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_STR:
            {
                //NFNetController.Instance.mxNetSender.RequireGMPropertyStr(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.StringVal(1));
            }
            break;
            case NFMsg.EGameMsgID.EGMI_REQ_CMD_PROPERTY_OBJECT:
            {
                //NFNetController.Instance.mxNetSender.RequireGMPropertyObject(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.ObjectVal(1));
            }
            break;

            case NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_INT:
            {
                //NFNetController.Instance.mxNetSender.RequireGMRecordInt(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.IntVal(1), xNowDataList.IntVal(2), xNowDataList.IntVal(3));
            }
            break;
            case NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_FLOAT:
            {
                //NFNetController.Instance.mxNetSender.RequireGMRecordFloat(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.IntVal(1), xNowDataList.IntVal(2), xNowDataList.FloatVal(3));
            }
            break;
            case NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_STR:
            {
                //NFNetController.Instance.mxNetSender.RequireGMRecordStr(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.IntVal(1), xNowDataList.IntVal(2), xNowDataList.StringVal(3));
            }
            break;
            case NFMsg.EGameMsgID.EGMI_REQ_CMD_RECORD_OBJECT:
            {
                //NFNetController.Instance.mxNetSender.RequireGMRecordObject(NFNetController.Instance.xMainRoleID, xNowDataList.StringVal(0), xNowDataList.IntVal(1), xNowDataList.IntVal(2), xNowDataList.ObjectVal(3));
            }
            break;



            case NFMsg.EGameMsgID.EGMI_REQ_PICK_ITEM:
            {
                //NFNetController.Instance.mxNetSender.RequirePickUpItem(NFNetController.Instance.xMainRoleID, xNowDataList.ObjectVal(0));
            }
            break;

            case NFMsg.EGameMsgID.EGMI_REQ_CMD_NORMAL:
            {
                //NFNetController.Instance.mxNetSender.RequireNormalGM(NFNetController.Instance.xMainRoleID, (NFMsg.ReqCommand.EGameCommandType)xNowDataList.IntVal(0)
                //    , xNowDataList.StringVal(1), xNowDataList.StringVal(2), xNowDataList.IntVal(3), xNowDataList.FloatVal(4), xNowDataList.ObjectVal(5));
            }
            break;

            default:
                break;

        }

        return true;
    }

    public void GUICall(int nWidth, int nHeight)
    {
        if (bShowHelp)
        {
            ShowHelp(nWidth, nHeight);
        }

        if (bShowAddItem)
        {
            ShowAddItem(nWidth, nHeight);
        }
    }

    public bool ShowHelp(int nWidth, int nHeight)
    {
        int nElementWidth = 150;
        int nElementHeight = 20;

        scrollPositionFirst = GUI.BeginScrollView(new Rect(0, nElementHeight, nWidth, nHeight), scrollPositionFirst, new Rect(0, 0, nElementWidth, mxGM.Count * (nElementHeight)));

        int nCount = 0;
        foreach (DictionaryEntry de in mxGM)
        {
            GUI.TextField(new Rect(0, nElementHeight * nCount, nWidth, nElementHeight), "    " + de.Key.ToString() + " = " + de.Value.ToString());
            GUI.TextField(new Rect(0, nElementHeight * (nCount + 1), nWidth, nElementHeight), "    " + mxGMDesc[de.Key]);


            nCount += 2;
        }

        GUI.EndScrollView();

        return true;
    }

    public bool ShowAddItem(int nWidth, int nHeight)
    {
        NFIElementModule xElementModule = NFCKernelModule.Instance.GetElementModule();
        NFIClassModule xLogicClassModule = NFCKernelModule.Instance.GetLogicClassModule();
        if (null == xLogicClassModule || null == xElementModule)
        {
            return false;
        }

        int nElementWidth = 150;
        int nElementHeight = 20;

        GUI.color = Color.green;
        mstrItemID  = GUI.TextField(new Rect(nElementWidth * 1, nElementHeight, nElementWidth * 1, nElementHeight), mstrItemID);
        mstrItemCount  = GUI.TextField(new Rect(nElementWidth * 2, nElementHeight, nElementWidth * 1, nElementHeight), mstrItemCount);
        GUI.color = Color.white;
        if (GUI.Button(new Rect(nElementWidth * 3, nElementHeight, nElementWidth / 2, nElementHeight), "Add"))
        {
            long nItemCount = 1;
            long.TryParse(mstrItemCount, out nItemCount);

			//NFNetController.Instance.mxNetSender.RequireNormalGM(NFNetController.Instance.xMainRoleID, NFMsg.ReqCommand.EGameCommandType.EGCT_MODIY_ITEM, mstrItemID, mstrItemID, nItemCount, 0.0f, new NFGUID());
        }

        NFIClass xItemClass = xLogicClassModule.GetElement("Item");
        NFIClass xEquipClass = xLogicClassModule.GetElement("Equip");

        List<string> xItemIDList = null;
        List<string> xEquipIDList = null;
        if (null != xItemClass)
        {
            xItemIDList = xItemClass.GetConfigNameList();
        }

        if (null != xEquipClass)
        {
            xEquipIDList = xEquipClass.GetConfigNameList();
        }

        
        int nmaxCount = xItemIDList.Count;
        if (nmaxCount < xEquipIDList.Count)
        {
            nmaxCount = xEquipIDList.Count;
        }

        AddItemscrollPositionFirst = GUI.BeginScrollView(new Rect(0, nElementHeight *2, nWidth, nHeight), AddItemscrollPositionFirst, new Rect(0, 0, nElementWidth * 2.5f, nmaxCount * (nElementHeight)));
        for (int i = 0; i < nmaxCount; i++)
        {
            string strItemID = "";
            string strEquipID = "";

            if (i < xItemIDList.Count)
            {
                strItemID = xItemIDList[i];
            }
            if (i < xEquipIDList.Count)
            {
                strEquipID = xEquipIDList[i];
            }


            int BeginHeight = 0;

            BeginHeight = nElementHeight  + nElementHeight * i;
            if (strItemID != "")
            {
                if (GUI.Button(new Rect(nElementWidth * 1, BeginHeight, nElementWidth, nElementHeight), strItemID))
                {
                    mstrItemID = strItemID;
                }
            }
        
            if (strEquipID != "")
            {
                if (GUI.Button(new Rect(nElementWidth * 2, BeginHeight, nElementWidth, nElementHeight), strEquipID))
                {
                    mstrItemID = strEquipID;
                }
            }
            
        }

        GUI.EndScrollView();

        return true;

    }
}
