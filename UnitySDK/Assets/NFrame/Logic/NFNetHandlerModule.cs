using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;
using NFMsg;
using NFrame;
using Google.Protobuf;
using NFSDK;

namespace NFrame
{
    public class NFNetHandlerModule : NFIModule
    {
        public enum Event : int
        {
            SwapScene = 1,
            PlayerMove,
        };


        class ObjectDataBuff
        {
            public NFMsg.ObjectRecordList xRecordList;
            public NFMsg.ObjectPropertyList xPropertyList;
        };


        private NFIKernelModule mKernelModule;
        private NFIClassModule mClassModule;

        private NFHelpModule mHelpModule;
        private NFNetModule mNetModule;
        private NFNetEventModule mNetEventModule;
        private NFSceneModule mSceneModule;
        private NFLoginModule mLoginModule;
        private NFUIModule mUIModule;

        private NFNetHandlerModule mxNetListener = null;

        private Dictionary<NFGUID, ObjectDataBuff> mxObjectDataBuff = new Dictionary<NFGUID, ObjectDataBuff>();

        public delegate void ResultCodeDelegation(NFMsg.EGameEventCode eventCode, String data);
        private Dictionary<NFMsg.EGameEventCode, ResultCodeDelegation> mhtEventDelegation = new Dictionary<NFMsg.EGameEventCode, ResultCodeDelegation>();

        public NFNetHandlerModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

        public void AddMsgEventCallBack(NFMsg.EGameEventCode code, NFNetHandlerModule.ResultCodeDelegation netHandler)
        {
            if (!mhtEventDelegation.ContainsKey(code))
            {
                ResultCodeDelegation myDelegationHandler = new ResultCodeDelegation(netHandler);
                mhtEventDelegation.Add(code, myDelegationHandler);
            }
            else
            {
                ResultCodeDelegation myDelegationHandler = (ResultCodeDelegation)mhtEventDelegation[code];
                myDelegationHandler += new ResultCodeDelegation(netHandler);
            }
        }

        public void DoResultCodeDelegation(NFMsg.EGameEventCode code, String data)
        {
            if (mhtEventDelegation.ContainsKey(code))
            {
                ResultCodeDelegation myDelegationHandler = (ResultCodeDelegation)mhtEventDelegation[code];
                myDelegationHandler(code, "");
            }
        }

        // Use this for initialization
        public override void Awake()
        {
            mClassModule = mPluginManager.FindModule<NFIClassModule>();
            mKernelModule = mPluginManager.FindModule<NFIKernelModule>();

            mSceneModule = mPluginManager.FindModule<NFSceneModule>();
            mNetModule = mPluginManager.FindModule<NFNetModule>();
            mHelpModule = mPluginManager.FindModule<NFHelpModule>();
            mNetEventModule = mPluginManager.FindModule<NFNetEventModule>();
            mUIModule = mPluginManager.FindModule<NFUIModule>();

        }

        public override void Init()
        {

            mKernelModule.RegisterClassCallBack(NFrame.Player.ThisName, ClassEventHandler);
            mKernelModule.RegisterClassCallBack(NFrame.NPC.ThisName, ClassEventHandler);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_EVENT_RESULT, EGMI_EVENT_RESULT);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_ENTER_GAME, EGMI_ACK_ENTER_GAME);
            //mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_SWAP_SCENE, EGMI_ACK_SWAP_SCENE);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_ENTER_GAME_FINISH, EGMI_ACK_ENTER_GAME_FINISH);


            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_ENTRY, EGMI_ACK_OBJECT_ENTRY);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_LEAVE, EGMI_ACK_OBJECT_LEAVE);
            /*
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_MOVE, EGMI_ACK_MOVE);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_MOVE_IMMUNE, EGMI_ACK_MOVE_IMMUNE);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_STATE_SYNC, EGMI_ACK_STATE_SYNC);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_POS_SYNC, EGMI_ACK_POS_SYNC);
            */
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_INT, EGMI_ACK_PROPERTY_INT);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_FLOAT, EGMI_ACK_PROPERTY_FLOAT);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_STRING, EGMI_ACK_PROPERTY_STRING);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_OBJECT, EGMI_ACK_PROPERTY_OBJECT);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_VECTOR2, EGMI_ACK_PROPERTY_VECTOR2);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_VECTOR3, EGMI_ACK_PROPERTY_VECTOR3);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_RECORD_INT, EGMI_ACK_RECORD_INT);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_RECORD_FLOAT, EGMI_ACK_RECORD_FLOAT);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_RECORD_STRING, EGMI_ACK_RECORD_STRING);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_RECORD_OBJECT, EGMI_ACK_RECORD_OBJECT);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_SWAP_ROW, EGMI_ACK_SWAP_ROW);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_ADD_ROW, EGMI_ACK_ADD_ROW);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_REMOVE_ROW, EGMI_ACK_REMOVE_ROW);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_RECORD_ENTRY, EGMI_ACK_OBJECT_RECORD_ENTRY);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_PROPERTY_ENTRY, EGMI_ACK_OBJECT_PROPERTY_ENTRY);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_DATA_FINISHED, EGMI_ACK_DATA_FINISHED);
        }

        public override void AfterInit()
        {
        }

        public override void Execute()
        {
        }

        public override void BeforeShut()
        {
        }
        public override void Shut()
        {
        }

        private void EGMI_EVENT_RESULT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            //OnResultMsg
            NFMsg.AckEventResult xResultCode = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.msg_data);
            NFMsg.EGameEventCode eEvent = xResultCode.event_code;

            DoResultCodeDelegation(eEvent, "");
        }



        private void EGMI_ACK_ENTER_GAME(UInt16 id, MemoryStream stream)
        {

            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.msg_data);

            Debug.Log("EGMI_ACK_ENTER_GAME " + xData.event_code.ToString());

            //mSceneModule.LoadScene((int)xData.event_code);
            //可以播放过图动画场景

            mUIModule.CloseAllUI();
            mUIModule.ShowUI<NFUIMain>();
        }

        private void EGMI_ACK_SWAP_SCENE(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ReqAckSwapScene xData = NFMsg.ReqAckSwapScene.Parser.ParseFrom(xMsg.msg_data);

            Debug.Log("SWAP_SCENE: " + xData.scene_id + " " + xData.x + "," + xData.y + "," + xData.z);

            /*
            NFMsg.AckMiningTitle xTileData = null;
            if (null != xData.data && xData.data.Length > 0)
            {
                xTileData = NFMsg.AckMiningTitle.Parser.ParseFrom(xData.data);
            }
            */
            //mSceneModule.LoadScene(xData.scene_id, xData.x, xData.y, xData.z, "");

            //重置主角坐标到出生点
        }

        private void EGMI_ACK_ENTER_GAME_FINISH(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ReqAckEnterGameSuccess xData = NFMsg.ReqAckEnterGameSuccess.Parser.ParseFrom(xMsg.msg_data);

            Debug.Log("Enter game finished: " + xData.ToString());

            // 去掉遮场景的ui
            //主角，等怪物enable，并且充值在相应的position
            //mSceneModule.LoadScene(xData.scene_id, xData.x, xData.y, xData.z);
        }


        private void EGMI_ACK_OBJECT_ENTRY(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.AckPlayerEntryList xData = NFMsg.AckPlayerEntryList.Parser.ParseFrom(xMsg.msg_data);

            for (int i = 0; i < xData.object_list.Count; ++i)
            {
                NFMsg.PlayerEntryInfo xInfo = xData.object_list[i];

                NFVector3 vPos = new NFVector3(xInfo.x, xInfo.y, xInfo.z);

                NFDataList var = new NFDataList();
                var.AddString(NFrame.NPC.Position);
                var.AddVector3(vPos);

                NFGUID xObjectID = mHelpModule.PBToNF(xInfo.object_guid);
                string strClassName = xInfo.class_id.ToStringUtf8();
                string strConfigID = xInfo.config_id.ToStringUtf8();

                Debug.Log("new Object enter: " + strClassName + xObjectID.ToString() + " " + xInfo.x + " " + xInfo.y + " " + xInfo.z);

                ObjectDataBuff xDataBuff = new ObjectDataBuff();
                mxObjectDataBuff.Add(xObjectID, xDataBuff);
                /*
                NFIObject xGO = mKernelModule.CreateObject(xObjectID, xInfo.scene_id, 0, strClassName, strConfigID, var);
                if (null == xGO)
                {
                    Debug.LogError("ID conflict: " + xObjectID.ToString() + "  ConfigID: " + strClassName);
                    continue;
                }
                */
            }
        }

        private void EGMI_ACK_OBJECT_LEAVE(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.AckPlayerLeaveList xData = NFMsg.AckPlayerLeaveList.Parser.ParseFrom(xMsg.msg_data);

            for (int i = 0; i < xData.object_list.Count; ++i)
            {
                mKernelModule.DestroyObject(mHelpModule.PBToNF(xData.object_list[i]));
            }
        }

        private void EGMI_ACK_OBJECT_RECORD_ENTRY(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.MultiObjectRecordList xData = NFMsg.MultiObjectRecordList.Parser.ParseFrom(xMsg.msg_data);

            for (int i = 0; i < xData.multi_player_record.Count; i++)
            {
                NFMsg.ObjectRecordList xObjectRecordList = xData.multi_player_record[i];
                NFGUID xObjectID = mHelpModule.PBToNF(xObjectRecordList.player_id);

                //Debug.Log ("new record enter Object: " + xObjectID.ToString () );

                ObjectDataBuff xDataBuff;
                if (mxObjectDataBuff.TryGetValue(xObjectID, out xDataBuff))
                {
                    xDataBuff.xRecordList = xObjectRecordList;
                }
            }
        }

        private void EGMI_ACK_OBJECT_PROPERTY_ENTRY(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.MultiObjectPropertyList xData = NFMsg.MultiObjectPropertyList.Parser.ParseFrom(xMsg.msg_data);

            for (int i = 0; i < xData.multi_player_property.Count; i++)
            {
                NFMsg.ObjectPropertyList xPropertyData = xData.multi_player_property[i];
                NFGUID xObjectID = mHelpModule.PBToNF(xPropertyData.player_id);

                ObjectDataBuff xDataBuff;
                if (mxObjectDataBuff.TryGetValue(xObjectID, out xDataBuff))
                {

                    xDataBuff.xPropertyList = xPropertyData;
                    if (xObjectID.IsNull())
                    {
                        AttachObjectData(xObjectID);
                    }
                }
                else
                {
                    xDataBuff = new ObjectDataBuff();
                    xDataBuff.xPropertyList = xPropertyData;
                    mxObjectDataBuff[xObjectID] = xDataBuff;
                    AttachObjectData(xObjectID);
                }
            }
        }

        private void AttachObjectData(NFGUID self)
        {
            //Debug.Log ("AttachObjectData : " + self.ToString () );

            ObjectDataBuff xDataBuff;
            if (mxObjectDataBuff.TryGetValue(self, out xDataBuff))
            {
                ////////////////record
                if (xDataBuff.xRecordList != null)
                {
                    for (int j = 0; j < xDataBuff.xRecordList.record_list.Count; j++)
                    {
                        NFMsg.ObjectRecordBase xObjectRecordBase = xDataBuff.xRecordList.record_list[j];
                        string srRecordName = xObjectRecordBase.record_name.ToStringUtf8();

                        for (int k = 0; k < xObjectRecordBase.row_struct.Count; ++k)
                        {
                            NFMsg.RecordAddRowStruct xAddRowStruct = xObjectRecordBase.row_struct[k];

                            ADD_ROW(self, xObjectRecordBase.record_name.ToStringUtf8(), xAddRowStruct);
                        }
                    }
                }
                ////////////////property
                NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xDataBuff.xPropertyList.player_id));
                NFIPropertyManager xPropertyManager = go.GetPropertyManager();

                for (int j = 0; j < xDataBuff.xPropertyList.property_int_list.Count; j++)
                {
                    string strPropertyName = xDataBuff.xPropertyList.property_int_list[j].property_name.ToStringUtf8();
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_INT);
                        xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                    }

                    //string className = mKernelModule.QueryPropertyString(self, NFrame.IObject.ClassName);
                    //Debug.LogError (self.ToString() + " " + className + " " + strPropertyName + " : " + xDataBuff.xPropertyList.property_int_list[j].data);

                    xProperty.SetInt(xDataBuff.xPropertyList.property_int_list[j].data);
                }

                for (int j = 0; j < xDataBuff.xPropertyList.property_float_list.Count; j++)
                {
                    string strPropertyName = xDataBuff.xPropertyList.property_float_list[j].property_name.ToStringUtf8();
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {

                        NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_FLOAT);
                        xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                    }

                    xProperty.SetFloat(xDataBuff.xPropertyList.property_float_list[j].data);
                }

                for (int j = 0; j < xDataBuff.xPropertyList.property_string_list.Count; j++)
                {
                    string strPropertyName = xDataBuff.xPropertyList.property_string_list[j].property_name.ToStringUtf8();
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_STRING);
                        xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                    }

                    //string className = mKernelModule.QueryPropertyString(self, NFrame.IObject.ClassName);
                    //Debug.LogError(self.ToString() + " " + className + " " + strPropertyName + " : " + xDataBuff.xPropertyList.property_string_list[j].data.ToStringUtf8());

                    xProperty.SetString(xDataBuff.xPropertyList.property_string_list[j].data.ToStringUtf8());
                }

                for (int j = 0; j < xDataBuff.xPropertyList.property_object_list.Count; j++)
                {
                    string strPropertyName = xDataBuff.xPropertyList.property_object_list[j].property_name.ToStringUtf8();
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_OBJECT);
                        xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                    }

                    xProperty.SetObject(mHelpModule.PBToNF(xDataBuff.xPropertyList.property_object_list[j].data));
                }

                for (int j = 0; j < xDataBuff.xPropertyList.property_vector2_list.Count; j++)
                {
                    string strPropertyName = xDataBuff.xPropertyList.property_vector2_list[j].property_name.ToStringUtf8();
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_VECTOR2);

                        xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                    }

                    xProperty.SetVector2(mHelpModule.PBToNF(xDataBuff.xPropertyList.property_vector2_list[j].data));
                }

                for (int j = 0; j < xDataBuff.xPropertyList.property_vector3_list.Count; j++)
                {
                    string strPropertyName = xDataBuff.xPropertyList.property_vector3_list[j].property_name.ToStringUtf8();
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (null == xProperty)
                    {
                        NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_VECTOR3);

                        xProperty = xPropertyManager.AddProperty(strPropertyName, var);
                    }

                    xProperty.SetVector3(mHelpModule.PBToNF(xDataBuff.xPropertyList.property_vector3_list[j].data));
                }
            }
        }

        private void ClassEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            switch (eType)
            {
                case NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE:
                    break;
                case NFIObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA:
                    AttachObjectData(self);
                    break;
                case NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH:
                    mxObjectDataBuff.Remove(self);
                    break;
                case NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY:
                    break;
                default:
                    break;
            }
        }

        private void EGMI_ACK_DATA_FINISHED(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.AckPlayerEntryList xData = NFMsg.AckPlayerEntryList.Parser.ParseFrom(xMsg.msg_data);

            for (int i = 0; i < xData.object_list.Count; ++i)
            {
                NFMsg.PlayerEntryInfo xInfo = xData.object_list[i];

                NFVector3 vPos = new NFVector3(xInfo.x, xInfo.y, xInfo.z);

                NFDataList var = new NFDataList();
                var.AddString("Position");
                var.AddVector3(vPos);

                NFGUID xObjectID = mHelpModule.PBToNF(xInfo.object_guid);
                string strClassName = xInfo.class_id.ToStringUtf8();
                string strConfigID = xInfo.config_id.ToStringUtf8();

                Debug.Log("create Object: " + strClassName + " " + xObjectID.ToString() + " " + strConfigID + " (" + xInfo.x + "," + xInfo.y + "," + xInfo.z + ")");

                ObjectDataBuff xDataBuff;
                if (mxObjectDataBuff.TryGetValue(xObjectID, out xDataBuff))
                {
                    NFIObject xGO = mKernelModule.CreateObject(xObjectID, xInfo.scene_id, 0, strClassName, strConfigID, var);
                    if (null == xGO)
                    {
                        Debug.LogError("ID conflict: " + xObjectID.ToString() + "  ConfigID: " + strClassName);
                        continue;
                    }
                }
            }
        }
        /////////////////////////////////////////////////////////////////////
        private void EGMI_ACK_PROPERTY_INT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectPropertyInt xData = NFMsg.ObjectPropertyInt.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                return;
            }

            NFIPropertyManager propertyManager = go.GetPropertyManager();

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                string name = xData.property_list[i].property_name.ToStringUtf8();
                Int64 data = xData.property_list[i].data;

                NFIProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_INT);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetInt(data);
            }
        }

        private void EGMI_ACK_PROPERTY_FLOAT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectPropertyFloat xData = NFMsg.ObjectPropertyFloat.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                return;
            }

            NFIPropertyManager propertyManager = go.GetPropertyManager();
            for (int i = 0; i < xData.property_list.Count; i++)
            {
                string name = xData.property_list[i].property_name.ToStringUtf8();
                float data = xData.property_list[i].data;

                NFIProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_FLOAT);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetFloat(data);
            }
        }

        private void EGMI_ACK_PROPERTY_STRING(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectPropertyString xData = NFMsg.ObjectPropertyString.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIPropertyManager propertyManager = go.GetPropertyManager();

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                string name = xData.property_list[i].property_name.ToStringUtf8();
                string data = xData.property_list[i].data.ToStringUtf8();

                NFIProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_STRING);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetString(data);
            }
        }

        private void EGMI_ACK_PROPERTY_OBJECT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectPropertyObject xData = NFMsg.ObjectPropertyObject.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIPropertyManager propertyManager = go.GetPropertyManager();
            for (int i = 0; i < xData.property_list.Count; i++)
            {
                string name = xData.property_list[i].property_name.ToStringUtf8();
                NFMsg.Ident data = xData.property_list[i].data;

                NFIProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_OBJECT);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetObject(mHelpModule.PBToNF(data));
            }
        }

        private void EGMI_ACK_PROPERTY_VECTOR2(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectPropertyVector2 xData = NFMsg.ObjectPropertyVector2.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIPropertyManager propertyManager = go.GetPropertyManager();

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                string name = xData.property_list[i].property_name.ToStringUtf8();
                NFMsg.Vector2 data = xData.property_list[i].data;

                NFIProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_VECTOR2);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetVector2(mHelpModule.PBToNF(data));
            }
        }

        private void EGMI_ACK_PROPERTY_VECTOR3(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectPropertyVector3 xData = NFMsg.ObjectPropertyVector3.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIPropertyManager propertyManager = go.GetPropertyManager();
            for (int i = 0; i < xData.property_list.Count; i++)
            {
                string name = xData.property_list[i].property_name.ToStringUtf8();
                NFMsg.Vector3 data = xData.property_list[i].data;

                NFIProperty property = propertyManager.GetProperty(name);
                if (null == property)
                {
                    NFDataList.TData var = new NFDataList.TData(NFDataList.VARIANT_TYPE.VTYPE_VECTOR3);
                    property = propertyManager.AddProperty(name, var);
                }

                property.SetVector3(mHelpModule.PBToNF(data));
            }
        }

        private void EGMI_ACK_RECORD_INT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordInt xData = NFMsg.ObjectRecordInt.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(xData.record_name.ToStringUtf8());

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                record.SetInt(xData.property_list[i].row, xData.property_list[i].col, (int)xData.property_list[i].data);
            }
        }

        private void EGMI_ACK_RECORD_FLOAT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordFloat xData = NFMsg.ObjectRecordFloat.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(xData.record_name.ToStringUtf8());

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                record.SetFloat(xData.property_list[i].row, xData.property_list[i].col, (float)xData.property_list[i].data);
            }
        }

        private void EGMI_ACK_RECORD_STRING(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordString xData = NFMsg.ObjectRecordString.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(xData.record_name.ToStringUtf8());

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                record.SetString(xData.property_list[i].row, xData.property_list[i].col, xData.property_list[i].data.ToStringUtf8());
            }
        }

        private void EGMI_ACK_RECORD_OBJECT(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordObject xData = NFMsg.ObjectRecordObject.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(xData.record_name.ToStringUtf8());

            for (int i = 0; i < xData.property_list.Count; i++)
            {
                record.SetObject(xData.property_list[i].row, xData.property_list[i].col, mHelpModule.PBToNF(xData.property_list[i].data));
            }
        }

        private void EGMI_ACK_SWAP_ROW(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordSwap xData = NFMsg.ObjectRecordSwap.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(xData.origin_record_name.ToStringUtf8());

            record.SwapRow(xData.row_origin, xData.row_target);

        }

        private void ADD_ROW(NFGUID self, string strRecordName, NFMsg.RecordAddRowStruct xAddStruct)
        {
            NFIObject go = mKernelModule.GetObject(self);
            if (go == null)
            {
                Debug.LogError("error id" + self);
                return;
            }

            NFIRecordManager xRecordManager = go.GetRecordManager();

            Hashtable recordVecDesc = new Hashtable();
            Hashtable recordVecData = new Hashtable();

            for (int k = 0; k < xAddStruct.record_int_list.Count; ++k)
            {
                NFMsg.RecordInt addIntStruct = (NFMsg.RecordInt)xAddStruct.record_int_list[k];

                if (addIntStruct.col >= 0)
                {
                    recordVecDesc[addIntStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_INT;
                    recordVecData[addIntStruct.col] = addIntStruct.data;
                }
            }

            for (int k = 0; k < xAddStruct.record_float_list.Count; ++k)
            {
                NFMsg.RecordFloat addFloatStruct = (NFMsg.RecordFloat)xAddStruct.record_float_list[k];

                if (addFloatStruct.col >= 0)
                {
                    recordVecDesc[addFloatStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_FLOAT;
                    recordVecData[addFloatStruct.col] = addFloatStruct.data;

                }
            }

            for (int k = 0; k < xAddStruct.record_string_list.Count; ++k)
            {
                NFMsg.RecordString addStringStruct = (NFMsg.RecordString)xAddStruct.record_string_list[k];

                if (addStringStruct.col >= 0)
                {
                    recordVecDesc[addStringStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_STRING;
                    if (addStringStruct.data != null)
                    {
                        recordVecData[addStringStruct.col] = addStringStruct.data.ToStringUtf8();
                    }
                    else
                    {
                        recordVecData[addStringStruct.col] = "";
                    }
                }
            }

            for (int k = 0; k < xAddStruct.record_object_list.Count; ++k)
            {
                NFMsg.RecordObject addObjectStruct = (NFMsg.RecordObject)xAddStruct.record_object_list[k];

                if (addObjectStruct.col >= 0)
                {
                    recordVecDesc[addObjectStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_OBJECT;
                    recordVecData[addObjectStruct.col] = mHelpModule.PBToNF(addObjectStruct.data);

                }
            }

            for (int k = 0; k < xAddStruct.record_vector2_list.Count; ++k)
            {
                NFMsg.RecordVector2 addObjectStruct = (NFMsg.RecordVector2)xAddStruct.record_vector2_list[k];

                if (addObjectStruct.col >= 0)
                {
                    recordVecDesc[addObjectStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_VECTOR2;
                    recordVecData[addObjectStruct.col] = mHelpModule.PBToNF(addObjectStruct.data);

                }
            }

            for (int k = 0; k < xAddStruct.record_vector3_list.Count; ++k)
            {
                NFMsg.RecordVector3 addObjectStruct = (NFMsg.RecordVector3)xAddStruct.record_vector3_list[k];

                if (addObjectStruct.col >= 0)
                {
                    recordVecDesc[addObjectStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_VECTOR3;
                    recordVecData[addObjectStruct.col] = mHelpModule.PBToNF(addObjectStruct.data);

                }
            }

            NFDataList varListDesc = new NFDataList();
            NFDataList varListData = new NFDataList();
            for (int m = 0; m < recordVecDesc.Count; m++)
            {
                if (recordVecDesc.ContainsKey(m) && recordVecData.ContainsKey(m))
                {
                    NFDataList.VARIANT_TYPE nType = (NFDataList.VARIANT_TYPE)recordVecDesc[m];
                    switch (nType)
                    {
                        case NFDataList.VARIANT_TYPE.VTYPE_INT:
                            {
                                varListDesc.AddInt(0);
                                varListData.AddInt((Int64)recordVecData[m]);
                            }

                            break;
                        case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
                            {
                                varListDesc.AddFloat(0.0f);
                                varListData.AddFloat((float)recordVecData[m]);
                            }
                            break;
                        case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                            {
                                varListDesc.AddString("");
                                varListData.AddString((string)recordVecData[m]);
                            }
                            break;
                        case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
                            {
                                varListDesc.AddObject(new NFGUID());
                                varListData.AddObject((NFGUID)recordVecData[m]);
                            }
                            break;
                        case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
                            {
                                varListDesc.AddVector2(new NFVector2());
                                varListData.AddVector2((NFVector2)recordVecData[m]);
                            }
                            break;
                        case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
                            {
                                varListDesc.AddVector3(new NFVector3());
                                varListData.AddVector3((NFVector3)recordVecData[m]);
                            }
                            break;
                        default:
                            break;

                    }
                }
                else
                {
                    //����
                    //Debug.LogException(i);
                }
            }

            NFIRecord xRecord = xRecordManager.GetRecord(strRecordName);
            if (null == xRecord)
            {
                string strClassName = mKernelModule.QueryPropertyString(self, NFrame.IObject.ClassName);
                NFIClass xLogicClass = mClassModule.GetElement(strClassName);
                NFIRecord xStaticRecord = xLogicClass.GetRecordManager().GetRecord(strRecordName);

                xRecord = xRecordManager.AddRecord(strRecordName, 512, varListDesc, xStaticRecord.GetTagData());
            }

            xRecord.AddRow(xAddStruct.row, varListData);
        }

        private void EGMI_ACK_ADD_ROW(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordAddRow xData = NFMsg.ObjectRecordAddRow.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }

            NFIRecordManager recordManager = go.GetRecordManager();

            for (int i = 0; i < xData.row_data.Count; i++)
            {
                ADD_ROW(mHelpModule.PBToNF(xData.player_id), xData.record_name.ToStringUtf8(), xData.row_data[i]);
            }
        }

        private void EGMI_ACK_REMOVE_ROW(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ObjectRecordRemove xData = NFMsg.ObjectRecordRemove.Parser.ParseFrom(xMsg.msg_data);

            NFIObject go = mKernelModule.GetObject(mHelpModule.PBToNF(xData.player_id));
            if (go == null)
            {
                Debug.LogError("error id" + xData.player_id);
                return;
            }
            NFIRecordManager recordManager = go.GetRecordManager();
            NFIRecord record = recordManager.GetRecord(xData.record_name.ToStringUtf8());

            for (int i = 0; i < xData.remove_row.Count; i++)
            {
                record.Remove(xData.remove_row[i]);
            }
        }

        //////////////////////////////////
    }
}