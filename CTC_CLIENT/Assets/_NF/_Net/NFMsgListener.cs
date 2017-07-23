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
using ProtoBuf;

namespace NFrame
{
	class ObjectDataBuff
	{
		public NFMsg.ObjectRecordList xRecordList;
		public NFMsg.ObjectPropertyList xPropertyList;
	};

	public class NFMsgListener
	{


		NFNetController mxNetController = null;
		NFNetListener mxNetListener = null;
		NFMsg.Serializer mxSerializer = new NFMsg.Serializer( );

		Dictionary<NFGUID, ObjectDataBuff> mxObjectDataBuff = new Dictionary<NFGUID, ObjectDataBuff> ();

		public NFMsgListener(NFNetController net  )
        {
            mxNetController = net;
			mxNetListener = mxNetController.mxNetClient.GetNetListener ();
        }

        ~NFMsgListener()
        {
        }

        
		// Use this for initialization
		public void Init() 
		{

			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_LOGIN, EGMI_ACK_LOGIN);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_EVENT_RESULT, EGMI_EVENT_RESULT);

			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_WORLD_LIST, EGMI_ACK_WORLD_LIST);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_ROLE_LIST, EGMI_ACK_ROLE_LIST);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_CONNECT_WORLD, EGMI_ACK_CONNECT_WORLD);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_CONNECT_KEY, EGMI_ACK_CONNECT_KEY);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_SELECT_SERVER, EGMI_ACK_SELECT_SERVER);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_ENTER_GAME, EGMI_ACK_ENTER_GAME);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_SWAP_SCENE, EGMI_ACK_SWAP_SCENE);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_ENTER_GAME_FINISH, EGMI_ACK_ENTER_GAME_FINISH);


			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_ENTRY, EGMI_ACK_OBJECT_ENTRY);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_LEAVE, EGMI_ACK_OBJECT_LEAVE);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_MOVE, EGMI_ACK_MOVE);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_MOVE_IMMUNE, EGMI_ACK_MOVE_IMMUNE);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_STATE_SYNC, EGMI_ACK_STATE_SYNC);

			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_INT, EGMI_ACK_PROPERTY_INT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_FLOAT, EGMI_ACK_PROPERTY_FLOAT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_STRING, EGMI_ACK_PROPERTY_STRING);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_OBJECT, EGMI_ACK_PROPERTY_OBJECT);

			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_RECORD_INT, EGMI_ACK_RECORD_INT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_RECORD_FLOAT, EGMI_ACK_RECORD_FLOAT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_RECORD_STRING, EGMI_ACK_RECORD_STRING);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_RECORD_OBJECT, EGMI_ACK_RECORD_OBJECT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_SWAP_ROW, EGMI_ACK_SWAP_ROW);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_ADD_ROW, EGMI_ACK_ADD_ROW);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_REMOVE_ROW, EGMI_ACK_REMOVE_ROW);

			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_RECORD_ENTRY, EGMI_ACK_OBJECT_RECORD_ENTRY);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_OBJECT_PROPERTY_ENTRY, EGMI_ACK_OBJECT_PROPERTY_ENTRY);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_DATA_FINISHED, EGMI_ACK_DATA_FINISHED);


			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_SKILL_OBJECTX, EGMI_ACK_SKILL_OBJECTX);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_CHAT, EGMI_ACK_CHAT);


			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGEC_ACK_MINING_TITLE, EGEC_ACK_MINING_TITLE);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_SEARCH_OPPNENT, EGMI_ACK_SEARCH_OPPNENT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_START_OPPNENT, EGMI_ACK_START_OPPNENT);
			mxNetListener.RegisteredDelegation(NFMsg.EGameMsgID.EGMI_ACK_END_OPPNENT, EGMI_ACK_END_OPPNENT);

			NFCKernelModule.Instance.RegisterClassCallBack (NFrame.Player.ThisName, ClassEventHandler);
        }

        private void EGMI_EVENT_RESULT(NFMsg.MsgBase xMsg)
        {
            //OnResultMsg
            NFMsg.AckEventResult xResultCode = new NFMsg.AckEventResult();
			xResultCode = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckEventResult)) as NFMsg.AckEventResult;
            NFMsg.EGameEventCode eEvent = xResultCode.event_code;

			mxNetListener.DoResultCodeDelegation(eEvent);
        }

        private void EGMI_ACK_LOGIN(NFMsg.MsgBase xMsg)
        {
            NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckEventResult)) as NFMsg.AckEventResult;

			if (EGameEventCode.EGEC_ACCOUNT_SUCCESS == xData.event_code)
			{
				Debug.Log("Login  SUCCESS");
				mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_HAS_PLAYER_LOGIN;

				Debug.Log("Start to QueryWorldList");
				mxNetController.mxNetSender.RequireWorldList();
			}
			else
			{
				Debug.Log("Login Faild,Code: " + xData.event_code);
			}

        }
		private void EGMI_ACK_WORLD_LIST(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckServerList xData = new NFMsg.AckServerList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckServerList)) as NFMsg.AckServerList;

			if (ReqServerListType.RSLT_WORLD_SERVER == xData.type)
			{
				mxNetController.aWorldList.Clear ();

				for(int i = 0; i < xData.info.Count; ++i)
				{
					ServerInfo info = xData.info[i];
					Debug.Log("WorldList  ServerId: " + info.server_id + " Name: " + System.Text.Encoding.Default.GetString(info.name) + " Status: " + info.status);
					mxNetController.aWorldList.Add(info);
				}
				if (mxNetController.aWorldList.Count >= 1)
				{
					ServerInfo info = (ServerInfo)mxNetController.aWorldList[0];
					NFNetController.Instance.mxNetSender.RequireConnectWorld(info.server_id);

				}
			}
			else if (ReqServerListType.RSLT_GAMES_ERVER == xData.type)
			{
				mxNetController.aServerList.Clear();

				for (int i = 0; i < xData.info.Count; ++i)
				{
					ServerInfo info = xData.info[i];
					Debug.Log("GameList  ServerId: " + info.server_id + " Name: " + System.Text.Encoding.Default.GetString(info.name) + " Status: " + info.status);
					mxNetController.aServerList.Add(info);
				}

				//NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_SERVERLIST);

				if (mxNetController.aServerList.Count >= 1)
				{
					ServerInfo info = (ServerInfo)mxNetController.aServerList[0];
					NFNetController.Instance.nServerID = info.server_id;
					NFNetController.Instance.mxNetSender.RequireSelectServer(info.server_id);
				}
			}
		}

		private void EGMI_ACK_CONNECT_WORLD(NFMsg.MsgBase xMsg)
		{
			mxNetController.mxNetClient.Disconnect();

			NFMsg.AckConnectWorldResult xData = new NFMsg.AckConnectWorldResult();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckConnectWorldResult)) as NFMsg.AckConnectWorldResult;

			///
			mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_START_CONNECT_TO_GATE;

			mxNetController.strKey = System.Text.Encoding.Default.GetString(xData.world_key);
			mxNetController.strWorldIP = System.Text.Encoding.Default.GetString(xData.world_ip);
			mxNetController.nWorldPort = xData.world_port;
			if (mxNetController.strWorldIP.Equals("127.0.0.1"))
			{
				mxNetController.strWorldIP = mxNetController.strFirstIP;
			}
		}

		private void EGMI_ACK_CONNECT_KEY(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckEventResult)) as NFMsg.AckEventResult;

			if (xData.event_code == EGameEventCode.EGEC_VERIFY_KEY_SUCCESS)
			{
				Debug.Log("VerifyKey SUCCESS");

				mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_HAS_VERIFY;

				Debug.Log("QueryGameList");

				mxNetController.mxNetSender.RequireServerList();
			}
			else
			{
				Debug.Log("VerifyKey Failed");
			}
		}

		private void EGMI_ACK_SELECT_SERVER(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckEventResult)) as NFMsg.AckEventResult;

			if (xData.event_code == EGameEventCode.EGEC_SELECTSERVER_SUCCESS)
			{
				Debug.Log("SelectGame SUCCESS ");

				Debug.Log("QueryRoleList");
		
				mxNetController.mxNetSender.RequireRoleList(mxNetController.strAccount, mxNetController.nServerID);
			}
			else
			{
				Debug.Log("SelectGame Failed ");
			}
		}

		private void EGMI_ACK_ROLE_LIST(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckRoleLiteInfoList xData = new NFMsg.AckRoleLiteInfoList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckRoleLiteInfoList)) as NFMsg.AckRoleLiteInfoList;


			mxNetController.aCharList.Clear();

			for (int i = 0; i < xData.char_data.Count; ++i)
			{
				NFMsg.RoleLiteInfo info = xData.char_data[i];

				Debug.Log("QueryRoleList  SUCCESS : " + System.Text.Encoding.Default.GetString(info.noob_name));

				mxNetController.aCharList.Add(info);
			}

			//////////////////
			if (mxNetController.aCharList.Count > 0)
			{
				//NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_ROLEHALL);

				NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)NFNetController.Instance.aCharList[0];
				NFrame.NFGUID xEnterID = new NFrame.NFGUID ();
				xEnterID.nData64 = xLiteInfo.id.index;
				xEnterID.nHead64 = xLiteInfo.id.svrid;
				mxNetController.mxNetSender.RequireEnterGameServer(xEnterID, mxNetController.strAccount, System.Text.Encoding.Default.GetString(xLiteInfo.noob_name), mxNetController.nServerID);

				mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME;

				Debug.Log("Selected role :" + System.Text.Encoding.Default.GetString(xLiteInfo.noob_name));
			}
			else
			{
				NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_CREATEHALL);

				Debug.Log("No Role!, require to create a new role! ");
			}
		}

		private void EGMI_ACK_ENTER_GAME(NFMsg.MsgBase xMsg)
		{
			mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_PLAYER_GAMEING;
			NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_GAMEING);

			NFMsg.AckEventResult xData = new NFMsg.AckEventResult();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckEventResult)) as NFMsg.AckEventResult;

			NFNetController.Instance.xMainRoleID = NFNetController.PBToNF(xData.event_object);
			NFRender.Instance.LoadScene((int)xData.event_code);

		}

		private void EGMI_ACK_SWAP_SCENE(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckSwapScene)) as NFMsg.ReqAckSwapScene;

			Debug.Log ("SWAP_SCENE: " + xData.scene_id);

			NFMsg.AckMiningTitle xTileData = new NFMsg.AckMiningTitle();
			if (null != xData.data && xData.data.Length > 0)
			{
				xTileData = mxSerializer.Deserialize(new MemoryStream(System.Text.Encoding.Default.GetBytes(xData.data)), null, typeof(NFMsg.AckMiningTitle)) as NFMsg.AckMiningTitle;
			}
			NFRender.Instance.LoadScene(xData.scene_id, xData.x, xData.y, xData.z, xTileData);

		}

		private void EGMI_ACK_ENTER_GAME_FINISH(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckEnterGameSuccess xData = new NFMsg.ReqAckEnterGameSuccess();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckEnterGameSuccess)) as NFMsg.ReqAckEnterGameSuccess;

			// 去掉遮场景的ui
			//NFRender.Instance.LoadScene(xData.scene_id, xData.x, xData.y, xData.z);
		}


		private void EGMI_ACK_OBJECT_ENTRY(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckPlayerEntryList xData = new NFMsg.AckPlayerEntryList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckPlayerEntryList)) as NFMsg.AckPlayerEntryList;

			for (int i = 0; i < xData.object_list.Count; ++i)
			{
				NFMsg.PlayerEntryInfo xInfo = xData.object_list[i];

				NFVector3 vPos = new NFVector3 (xInfo.x, xInfo.y, xInfo.z);

				NFDataList var = new NFDataList ();
				var.AddString ("Position");
				var.AddVector3 (vPos);

				NFGUID xObjectID = NFNetController.PBToNF(xInfo.object_guid);
				string strClassName = System.Text.Encoding.Default.GetString (xInfo.class_id);
				string strConfigID = System.Text.Encoding.Default.GetString (xInfo.config_id);

				Debug.Log ("new " + strClassName + " Object: " + xObjectID.ToString() + " " +xInfo.x + " " + xInfo.y + " " + xInfo.z);

				ObjectDataBuff xDataBuff = new ObjectDataBuff ();
				mxObjectDataBuff.Add (xObjectID, xDataBuff);
				/*
				NFIObject xGO = NFCKernelModule.Instance.CreateObject(xObjectID, xInfo.scene_id, 0, strClassName, strConfigID, var);
				if (null == xGO)
				{
					Debug.LogError("ID conflict: " + xObjectID.ToString() + "  ConfigID: " + strClassName);
					continue;
				}
				*/
			}
		}

		private void EGMI_ACK_OBJECT_LEAVE(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckPlayerLeaveList xData = new NFMsg.AckPlayerLeaveList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckPlayerLeaveList)) as NFMsg.AckPlayerLeaveList;

			for (int i = 0; i < xData.object_list.Count; ++i)
			{
				NFCKernelModule.Instance.DestroyObject(NFNetController.PBToNF(xData.object_list[i]));
			}
		}

		private void EGMI_ACK_OBJECT_RECORD_ENTRY(NFMsg.MsgBase xMsg)
		{
			NFMsg.MultiObjectRecordList xData = new NFMsg.MultiObjectRecordList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.MultiObjectRecordList)) as NFMsg.MultiObjectRecordList;

			for (int i = 0; i < xData.multi_player_record.Count; i++)
			{
				NFMsg.ObjectRecordList xObjectRecordList = xData.multi_player_record[i];
				NFGUID xObjectID = NFNetController.PBToNF (xObjectRecordList.player_id);

				ObjectDataBuff xDataBuff;
				if (mxObjectDataBuff.TryGetValue (xObjectID, out xDataBuff))
				{
					xDataBuff.xRecordList = xObjectRecordList;
				}
				/*
				for (int j = 0; j < xObjectRecordList.record_list.Count; j++)
				{
					NFMsg.ObjectRecordBase xObjectRecordBase = xObjectRecordList.record_list[j];
					string srRecordName = System.Text.Encoding.Default.GetString(xObjectRecordBase.record_name);
					Debug.Log ("EGMI_ACK_OBJECT_RECORD_ENTRY " + xObjectRecordList.player_id.index + "  " + srRecordName + " Data line: " + xObjectRecordBase.row_struct.Count);

					for (int k = 0; k < xObjectRecordBase.row_struct.Count; ++k )
					{
						NFMsg.RecordAddRowStruct xAddRowStruct = xObjectRecordBase.row_struct[k];

						ADD_ROW(xObjectID, System.Text.Encoding.Default.GetString(xObjectRecordBase.record_name), xAddRowStruct);
					}
				}
				*/
			}
		}

		private void EGMI_ACK_OBJECT_PROPERTY_ENTRY(NFMsg.MsgBase xMsg)
		{
			NFMsg.MultiObjectPropertyList xData = new NFMsg.MultiObjectPropertyList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.MultiObjectPropertyList)) as NFMsg.MultiObjectPropertyList;

			for (int i = 0; i < xData.multi_player_property.Count; i++)
			{
				NFMsg.ObjectPropertyList xPropertyData = xData.multi_player_property[i];
				NFGUID xObjectID = NFNetController.PBToNF (xPropertyData.player_id);

				ObjectDataBuff xDataBuff;
				if (mxObjectDataBuff.TryGetValue (xObjectID, out xDataBuff))
				{
					xDataBuff.xPropertyList = xPropertyData;
				}
				/*
				NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xPropertyData.player_id));
				NFIPropertyManager xPropertyManager = go.GetPropertyManager();

				for (int j = 0; j < xPropertyData.property_int_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_int_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddInt(0);

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetInt(xPropertyData.property_int_list[j].data);
				}

				for (int j = 0; j < xPropertyData.property_float_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_float_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddFloat(0);

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetFloat(xPropertyData.property_float_list[j].data);
				}

				for (int j = 0; j < xPropertyData.property_string_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_string_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddString("");

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetString(System.Text.Encoding.Default.GetString(xPropertyData.property_string_list[j].data));
				}

				for (int j = 0; j < xPropertyData.property_object_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xPropertyData.property_object_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddObject(new NFGUID());

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetObject(NFNetController.PBToNF(xPropertyData.property_object_list[j].data));
				}
				*/
			}
		}

		private void AttachObjectData(NFGUID self)
		{
			ObjectDataBuff xDataBuff;
			if (mxObjectDataBuff.TryGetValue (self, out xDataBuff))
			{
				////////////////record
				for (int j = 0; j < xDataBuff.xRecordList.record_list.Count; j++)
				{
					NFMsg.ObjectRecordBase xObjectRecordBase = xDataBuff.xRecordList.record_list[j];
					string srRecordName = System.Text.Encoding.Default.GetString(xObjectRecordBase.record_name);

					for (int k = 0; k < xObjectRecordBase.row_struct.Count; ++k )
					{
						NFMsg.RecordAddRowStruct xAddRowStruct = xObjectRecordBase.row_struct[k];

						ADD_ROW(self, System.Text.Encoding.Default.GetString(xObjectRecordBase.record_name), xAddRowStruct);
					}
				}
				////////////////property
				NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xDataBuff.xPropertyList.player_id));
				NFIPropertyManager xPropertyManager = go.GetPropertyManager();

				for (int j = 0; j < xDataBuff.xPropertyList.property_int_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xDataBuff.xPropertyList.property_int_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddInt(0);

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetInt(xDataBuff.xPropertyList.property_int_list[j].data);
				}

				for (int j = 0; j < xDataBuff.xPropertyList.property_float_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xDataBuff.xPropertyList.property_float_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddFloat(0);

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetFloat(xDataBuff.xPropertyList.property_float_list[j].data);
				}

				for (int j = 0; j < xDataBuff.xPropertyList.property_string_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xDataBuff.xPropertyList.property_string_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddString("");

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetString(System.Text.Encoding.Default.GetString(xDataBuff.xPropertyList.property_string_list[j].data));
				}

				for (int j = 0; j < xDataBuff.xPropertyList.property_object_list.Count; j++)
				{
					string strPropertyName = System.Text.Encoding.Default.GetString(xDataBuff.xPropertyList.property_object_list[j].property_name);
					NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
					if (null == xProperty)
					{
						NFDataList varList = new NFDataList();
						varList.AddObject(new NFGUID());

						xProperty = xPropertyManager.AddProperty(strPropertyName, varList);
					}

					xProperty.SetObject(NFNetController.PBToNF(xDataBuff.xPropertyList.property_object_list[j].data));
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
					AttachObjectData (self);
					break;
				case NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH:
					mxObjectDataBuff.Remove (self);
					break;
				case NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY:
					break;
				default:
					break;
			}
		}

		private void EGMI_ACK_DATA_FINISHED(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckPlayerEntryList xData = new NFMsg.AckPlayerEntryList();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckPlayerEntryList)) as NFMsg.AckPlayerEntryList;

			for (int i = 0; i < xData.object_list.Count; ++i)
			{
				NFMsg.PlayerEntryInfo xInfo = xData.object_list [i];

				NFVector3 vPos = new NFVector3 (xInfo.x, xInfo.y, xInfo.z);

				NFDataList var = new NFDataList ();
				var.AddString ("Position");
				var.AddVector3 (vPos);

				NFGUID xObjectID = NFNetController.PBToNF (xInfo.object_guid);
				string strClassName = System.Text.Encoding.Default.GetString (xInfo.class_id);
				string strConfigID = System.Text.Encoding.Default.GetString (xInfo.config_id);

				Debug.Log ("new " + strClassName + " Object: " + xObjectID.ToString () + " " + xInfo.x + " " + xInfo.y + " " + xInfo.z);

				ObjectDataBuff xDataBuff;
				if (mxObjectDataBuff.TryGetValue (xObjectID, out xDataBuff))
				{
					NFIObject xGO = NFCKernelModule.Instance.CreateObject (xObjectID, xInfo.scene_id, 0, strClassName, strConfigID, var);
					if (null == xGO)
					{
						Debug.LogError ("ID conflict: " + xObjectID.ToString () + "  ConfigID: " + strClassName);
						continue;
					}
				}
			}
		}
		/////////////////////////////////////////////////////////////////////
		private void EGMI_ACK_PROPERTY_INT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectPropertyInt xData = new NFMsg.ObjectPropertyInt();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectPropertyInt)) as NFMsg.ObjectPropertyInt;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIPropertyManager propertyManager = go.GetPropertyManager();

			for(int i = 0; i < xData.property_list.Count; i++)
			{
				NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name));
				if(null == property)
				{
					NFDataList varList = new NFDataList();
					varList.AddInt(0);

					property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name), varList);
				}

				property.SetInt(xData.property_list[i].data);
			}
		}

		private void EGMI_ACK_PROPERTY_FLOAT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectPropertyFloat xData = new NFMsg.ObjectPropertyFloat();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectPropertyFloat)) as NFMsg.ObjectPropertyFloat;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));

			for(int i = 0; i < xData.property_list.Count; i++)
			{
				NFIPropertyManager propertyManager = go.GetPropertyManager();
				NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name));
				if (null == property)
				{
					NFDataList varList = new NFDataList();
					varList.AddFloat(0.0f);

					property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name), varList);
				}

				property.SetFloat(xData.property_list[i].data);
			}
		}

		private void EGMI_ACK_PROPERTY_STRING(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectPropertyString xData = new NFMsg.ObjectPropertyString();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectPropertyString)) as NFMsg.ObjectPropertyString;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));

			for(int i = 0; i < xData.property_list.Count; i++)
			{
				NFIPropertyManager propertyManager = go.GetPropertyManager();
				NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name));
				if (null == property)
				{
					NFDataList varList = new NFDataList();
					varList.AddString("");

					property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name), varList);
				}

				property.SetString(System.Text.Encoding.Default.GetString(xData.property_list[i].data));
			}
		}

		private void EGMI_ACK_PROPERTY_OBJECT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectPropertyObject xData = new NFMsg.ObjectPropertyObject();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectPropertyObject)) as NFMsg.ObjectPropertyObject;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));

			for(int i = 0; i < xData.property_list.Count; i++)
			{
				NFIPropertyManager propertyManager = go.GetPropertyManager();
				NFIProperty property = propertyManager.GetProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name));
				if (null == property)
				{
					NFDataList varList = new NFDataList();
					varList.AddObject(new NFGUID());

					property = propertyManager.AddProperty(System.Text.Encoding.Default.GetString(xData.property_list[i].property_name), varList);
				}

				property.SetObject(NFNetController.PBToNF(xData.property_list[i].data));
			}
		}

		private void EGMI_ACK_RECORD_INT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordInt xData = new NFMsg.ObjectRecordInt();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordInt)) as NFMsg.ObjectRecordInt;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();
			NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(xData.record_name));

			for (int i = 0; i < xData.property_list.Count; i++)
			{
				record.SetInt(xData.property_list[i].row, xData.property_list[i].col, (int)xData.property_list[i].data);
			}
		}

		private void EGMI_ACK_RECORD_FLOAT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordFloat xData = new NFMsg.ObjectRecordFloat();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordFloat)) as NFMsg.ObjectRecordFloat;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();
			NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(xData.record_name));

			for (int i = 0; i < xData.property_list.Count; i++)
			{
				record.SetFloat(xData.property_list[i].row, xData.property_list[i].col, (float)xData.property_list[i].data);
			}
		}

		private void EGMI_ACK_RECORD_STRING(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordString xData = new NFMsg.ObjectRecordString();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordString)) as NFMsg.ObjectRecordString;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();
			NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(xData.record_name));

			for (int i = 0; i < xData.property_list.Count; i++)
			{
				record.SetString(xData.property_list[i].row, xData.property_list[i].col, System.Text.Encoding.Default.GetString(xData.property_list[i].data));
			}
		}

		private void EGMI_ACK_RECORD_OBJECT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordObject xData = new NFMsg.ObjectRecordObject();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordObject)) as NFMsg.ObjectRecordObject;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();
			NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(xData.record_name));

			for (int i = 0; i < xData.property_list.Count; i++)
			{
				record.SetObject(xData.property_list[i].row, xData.property_list[i].col, NFNetController.PBToNF(xData.property_list[i].data));
			}
		}

		private void EGMI_ACK_SWAP_ROW(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordSwap xData = new NFMsg.ObjectRecordSwap();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordSwap)) as NFMsg.ObjectRecordSwap;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();
			NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(xData.origin_record_name));

			record.SwapRow(xData.row_origin, xData.row_target);

		}

		private void ADD_ROW(NFGUID self, string strRecordName, NFMsg.RecordAddRowStruct xAddStruct)
		{
			NFIObject go = NFCKernelModule.Instance.GetObject(self);
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
					recordVecData[addStringStruct.col] = System.Text.Encoding.Default.GetString(addStringStruct.data);

				}
			}

			for (int k = 0; k < xAddStruct.record_object_list.Count; ++k)
			{
				NFMsg.RecordObject addObjectStruct = (NFMsg.RecordObject)xAddStruct.record_object_list[k];

				if (addObjectStruct.col >= 0)
				{
					recordVecDesc[addObjectStruct.col] = NFDataList.VARIANT_TYPE.VTYPE_OBJECT;
					recordVecData[addObjectStruct.col] = NFNetController.PBToNF(addObjectStruct.data);

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
				xRecord = xRecordManager.AddRecord(strRecordName, 512, varListDesc);
			}

			xRecord.AddRow(xAddStruct.row, varListData);
		}

		private void EGMI_ACK_ADD_ROW(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordAddRow xData = new NFMsg.ObjectRecordAddRow();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordAddRow)) as NFMsg.ObjectRecordAddRow;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();

			for (int i = 0; i < xData.row_data.Count; i++)
			{
				ADD_ROW(NFNetController.PBToNF(xData.player_id), System.Text.Encoding.Default.GetString(xData.record_name), xData.row_data[i]);
			}
		}

		private void EGMI_ACK_REMOVE_ROW(NFMsg.MsgBase xMsg)
		{
			NFMsg.ObjectRecordRemove xData = new NFMsg.ObjectRecordRemove();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ObjectRecordRemove)) as NFMsg.ObjectRecordRemove;

			NFIObject go = NFCKernelModule.Instance.GetObject(NFNetController.PBToNF(xData.player_id));
			NFIRecordManager recordManager = go.GetRecordManager();
			NFIRecord record = recordManager.GetRecord(System.Text.Encoding.Default.GetString(xData.record_name));

			for (int i = 0; i < xData.remove_row.Count; i++)
			{
				record.Remove(xData.remove_row[i]);
			}
		}


		//////////////////////////////////
		/// 
		private void EGMI_ACK_MOVE(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckPlayerMove)) as NFMsg.ReqAckPlayerMove;

			if (xData.target_pos.Count <= 0)
			{
				return;
			}

			float fSpeed = NFCKernelModule.Instance.QueryPropertyInt(NFNetController.PBToNF(xData.mover), "MOVE_SPEED") / 10000.0f;
			//NFRender.Instance.MoveTo(PBToNF(xData.mover), new Vector3(xData.target_pos[0].x, xData.target_pos[0].y, xData.target_pos[0].z), fSpeed, true);
		}

		private void EGMI_ACK_MOVE_IMMUNE(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckPlayerMove)) as NFMsg.ReqAckPlayerMove;

			if (xData.target_pos.Count <= 0)
			{
				return;
			}

			float fSpeed = NFCKernelModule.Instance.QueryPropertyInt(NFNetController.PBToNF(xData.mover), "MOVE_SPEED") / 10000.0f;
			fSpeed *= 1.5f;

			//NFRender.Instance.MoveImmuneBySpeed(PBToNF(xData.mover), new Vector3(xData.target_pos[0].x, xData.target_pos[0].y, xData.target_pos[0].z), fSpeed, true);

		}

		private void EGMI_ACK_STATE_SYNC(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckPlayerMove)) as NFMsg.ReqAckPlayerMove;

			if (xData.target_pos.Count <= 0)
			{
				return;
			}

			NFGUID xGUID = NFNetController.PBToNF (xData.mover);
			NFIObject xObject = NFCKernelModule.Instance.GetObject (xGUID);
			if (xObject != null)
			{
				
			}
		}

		private void EGMI_ACK_SKILL_OBJECTX(NFMsg.MsgBase xMsg)
		{

		}

		private void EGMI_ACK_CHAT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckPlayerChat xData = new NFMsg.ReqAckPlayerChat();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckPlayerChat)) as NFMsg.ReqAckPlayerChat;

			mxNetController.aChatMsgList.Add(NFNetController.PBToNF(xData.chat_id).ToString() + ":" + System.Text.Encoding.Default.GetString(xData.chat_info));
		}

		private void EGEC_ACK_MINING_TITLE(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckMiningTitle xData = new NFMsg.AckMiningTitle();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckMiningTitle)) as NFMsg.AckMiningTitle;
		}

		private void EGMI_ACK_SEARCH_OPPNENT(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckSearchOppnent xData = new NFMsg.AckSearchOppnent();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckSearchOppnent)) as NFMsg.AckSearchOppnent;

			NFStart.Instance.SetJoyEnable (false);
			NFCWindowManager.Instance.HideAllWindows();
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_READY_FIGHTING);
		}

		private void EGMI_ACK_START_OPPNENT(NFMsg.MsgBase xMsg)
		{
			NFMsg.ReqAckStartBattle xData = new NFMsg.ReqAckStartBattle();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.ReqAckStartBattle)) as NFMsg.ReqAckStartBattle;

			NFCWindowManager.Instance.HideAllWindows();
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_FIGHT_TOP);
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_MONEY_TOP);
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_MAIN_SKILL);
			NFStart.Instance.SetJoyEnable (true);
		}

		private void EGMI_ACK_END_OPPNENT(NFMsg.MsgBase xMsg)
		{
			NFMsg.AckEndBattle xData = new NFMsg.AckEndBattle();
			xData = mxSerializer.Deserialize(new MemoryStream(xMsg.msg_data), null, typeof(NFMsg.AckEndBattle)) as NFMsg.AckEndBattle;

			NFStart.Instance.SetJoyEnable (false);
			NFCWindowManager.Instance.HideAllWindows();
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_RETURN);
			NFCWindowManager.Instance.ShowWindows(NFCWindows.UI_WIN_STATE.UI_WIN_FIGHTING_RESULT);
		}
    }
}