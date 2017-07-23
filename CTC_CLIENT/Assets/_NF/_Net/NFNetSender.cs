using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using ProtoBuf;
using UnityEngine;
using NFMsg;
using NFrame;

namespace NFrame
{

	public class NFNetSender
	{
		NFNetController mxNetController;
		NFMsg.Serializer mxSerializer = new NFMsg.Serializer( );
		NFMsg.MsgBase mxData = new MsgBase();
		MemoryStream mxBody = new MemoryStream();
		MsgHead mxHead = new MsgHead();

		public NFNetSender(NFNetController clientnet)
	    {
	        mxNetController = clientnet;
	    }

	    public void SendMsg(NFrame.NFGUID xID, NFMsg.EGameMsgID unMsgID, MemoryStream stream)
		{
			mxData.player_id = NFNetController.NFToPB(xID);
			mxData.msg_data = stream.ToArray();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, mxData);

			mxHead.unMsgID = (UInt16)unMsgID;
			mxHead.unDataLen = (UInt32)mxBody.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;

			byte[] bodyByte = mxBody.ToArray();
			byte[] headByte = mxHead.EnCode();

			byte[] sendBytes = new byte[mxHead.unDataLen];
			headByte.CopyTo(sendBytes, 0);
			bodyByte.CopyTo(sendBytes, headByte.Length);

			mxNetController.mxNetClient.SendBytes(sendBytes);

			/////////////////////////////////////////////////////////////////


			NFLog.Instance.Log(NFLog.LOG_LEVEL.INFO, "SendMsg:" + unMsgID);
		}

		////////////////////////////////////修改自身属性
		public void RequirePropertyInt(NFrame.NFGUID objectID, string strPropertyName, NFDataList.TData newVar)
		{
			NFMsg.ObjectPropertyInt xData = new NFMsg.ObjectPropertyInt();
			xData.player_id = NFNetController.NFToPB(objectID);

			NFMsg.PropertyInt xPropertyInt = new NFMsg.PropertyInt();
			xPropertyInt.property_name = System.Text.Encoding.Default.GetBytes(strPropertyName);
			xPropertyInt.data = newVar.IntVal();
			xData.property_list.Add(xPropertyInt);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload int");
			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_INT, mxBody);
		}

		public void RequirePropertyFloat(NFrame.NFGUID objectID, string strPropertyName, NFDataList.TData newVar)
		{
			NFMsg.ObjectPropertyFloat xData = new NFMsg.ObjectPropertyFloat();
			xData.player_id = NFNetController.NFToPB(objectID);

			NFMsg.PropertyFloat xPropertyFloat = new NFMsg.PropertyFloat();
			xPropertyFloat.property_name = System.Text.Encoding.Default.GetBytes(strPropertyName);
			xPropertyFloat.data = (float)newVar.FloatVal();
			xData.property_list.Add(xPropertyFloat);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload Float");
			NFNetController.Instance.mxNetSender.SendMsg(objectID, NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_FLOAT, mxBody);
		}

		public void RequirePropertyString(NFrame.NFGUID objectID, string strPropertyName, NFDataList.TData newVar)
		{
			NFMsg.ObjectPropertyString xData = new NFMsg.ObjectPropertyString();
			xData.player_id = NFNetController.NFToPB(objectID);

			NFMsg.PropertyString xPropertyString = new NFMsg.PropertyString();
			xPropertyString.property_name = System.Text.Encoding.Default.GetBytes(strPropertyName);
			xPropertyString.data = System.Text.Encoding.Default.GetBytes(newVar.StringVal());
			xData.property_list.Add(xPropertyString);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload String");
			NFNetController.Instance.mxNetSender.SendMsg(objectID, NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_STRING, mxBody);
		}

		public void RequirePropertyObject(NFrame.NFGUID objectID, string strPropertyName, NFDataList.TData newVar)
		{
			NFMsg.ObjectPropertyObject xData = new NFMsg.ObjectPropertyObject();
			xData.player_id = NFNetController.NFToPB(objectID);

			NFMsg.PropertyObject xPropertyObject = new NFMsg.PropertyObject();
			xPropertyObject.property_name = System.Text.Encoding.Default.GetBytes(strPropertyName);
			xPropertyObject.data = NFNetController.NFToPB(newVar.ObjectVal());
			xData.property_list.Add(xPropertyObject);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload Object");
			NFNetController.Instance.mxNetSender.SendMsg(objectID, NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_OBJECT, mxBody);
		}

		public void RequirePropertyVector2(NFrame.NFGUID objectID, string strPropertyName, NFDataList.TData newVar)
		{
			NFMsg.ObjectPropertyVector2 xData = new NFMsg.ObjectPropertyVector2();
			xData.player_id = NFNetController.NFToPB(objectID);

			NFMsg.PropertyVector2 xProperty = new NFMsg.PropertyVector2();
			xProperty.property_name = System.Text.Encoding.Default.GetBytes(strPropertyName);
			xProperty.data = NFNetController.NFToPB(newVar.Vector2Val());
			xData.property_list.Add(xProperty);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			NFNetController.Instance.mxNetSender.SendMsg(objectID, NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_VECTOR2, mxBody);
		}

		public void RequirePropertyVector3(NFrame.NFGUID objectID, string strPropertyName, NFDataList.TData newVar)
		{
			NFMsg.ObjectPropertyVector3 xData = new NFMsg.ObjectPropertyVector3();
			xData.player_id = NFNetController.NFToPB(objectID);

			NFMsg.PropertyVector3 xProperty = new NFMsg.PropertyVector3();
			xProperty.property_name = System.Text.Encoding.Default.GetBytes(strPropertyName);
			xProperty.data = NFNetController.NFToPB(newVar.Vector3Val());
			xData.property_list.Add(xProperty);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			NFNetController.Instance.mxNetSender.SendMsg(objectID, NFMsg.EGameMsgID.EGMI_ACK_PROPERTY_VECTOR3, mxBody);
		}

		public void RequireAddRow(NFrame.NFGUID self, string strRecordName, int nRow)
		{
			NFMsg.ObjectRecordAddRow xData = new NFMsg.ObjectRecordAddRow();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordAddRowStruct xRecordAddRowStruct = new NFMsg.RecordAddRowStruct();
			xData.row_data.Add(xRecordAddRowStruct);
			xRecordAddRowStruct.row = nRow;

			NFIObject xObject = NFCKernelModule.Instance.GetObject(self);
			NFIRecord xRecord = xObject.GetRecordManager().GetRecord(strRecordName);
			NFDataList xRowData = xRecord.QueryRow(nRow);
			for(int i = 0;i<xRowData.Count();i++)
			{
				switch(xRowData.GetType(i))
				{
					case NFDataList.VARIANT_TYPE.VTYPE_INT:
					{
						NFMsg.RecordInt xRecordInt = new NFMsg.RecordInt();
						xRecordInt.row = nRow;
						xRecordInt.col = i;
						xRecordInt.data = xRowData.IntVal(i);
						xRecordAddRowStruct.record_int_list.Add(xRecordInt);
					}
					break;
					case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
					{
						NFMsg.RecordFloat xRecordFloat = new NFMsg.RecordFloat();
						xRecordFloat.row = nRow;
						xRecordFloat.col = i;
						xRecordFloat.data = (float)xRowData.FloatVal(i);
						xRecordAddRowStruct.record_float_list.Add(xRecordFloat);
					}
					break;
					case NFDataList.VARIANT_TYPE.VTYPE_STRING:
					{
						NFMsg.RecordString xRecordString = new NFMsg.RecordString();
						xRecordString.row = nRow;
						xRecordString.col = i;
						xRecordString.data = System.Text.Encoding.Default.GetBytes(xRowData.StringVal(i));
						xRecordAddRowStruct.record_string_list.Add(xRecordString);
					}
					break;
					case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
					{
						NFMsg.RecordObject xRecordObject = new NFMsg.RecordObject();
						xRecordObject.row = nRow;
						xRecordObject.col = i;
						xRecordObject.data = NFNetController.NFToPB(xRowData.ObjectVal(i));
						xRecordAddRowStruct.record_object_list.Add(xRecordObject);
					}
					break;
					case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
					{
						NFMsg.RecordVector2 xRecordVector = new NFMsg.RecordVector2();
						xRecordVector.row = nRow;
						xRecordVector.col = i;
						xRecordVector.data = NFNetController.NFToPB(xRowData.Vector2Val(i));
						xRecordAddRowStruct.record_vector2_list.Add(xRecordVector);
					}
					break;
					case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
					{
						NFMsg.RecordVector3 xRecordVector = new NFMsg.RecordVector3();
						xRecordVector.row = nRow;
						xRecordVector.col = i;
						xRecordVector.data = NFNetController.NFToPB(xRowData.Vector3Val(i));
						xRecordAddRowStruct.record_vector3_list.Add(xRecordVector);
					}
					break;

				}
			}

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record addRow");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_ADD_ROW, mxBody);
		}

		public void RequireRemoveRow(NFrame.NFGUID self, string strRecordName, int nRow)
		{
			NFMsg.ObjectRecordRemove xData = new NFMsg.ObjectRecordRemove();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);
			xData.remove_row.Add(nRow);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record removeRow");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_REMOVE_ROW, mxBody);
		}

		public void RequireSwapRow(NFrame.NFGUID self, string strRecordName, int nOriginRow, int nTargetRow)
		{
			NFMsg.ObjectRecordSwap xData = new NFMsg.ObjectRecordSwap();
			xData.player_id = NFNetController.NFToPB(self);
			xData.origin_record_name = System.Text.Encoding.Default.GetBytes(strRecordName);
			xData.target_record_name = System.Text.Encoding.Default.GetBytes(strRecordName);
			xData.row_origin = nOriginRow;
			xData.row_target = nTargetRow;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record swapRow");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_SWAP_ROW, mxBody);
		}

		public void RequireRecordInt(NFrame.NFGUID self, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
		{
			NFMsg.ObjectRecordInt xData = new NFMsg.ObjectRecordInt();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordInt xRecordInt = new NFMsg.RecordInt();
			xData.property_list.Add(xRecordInt);
			xRecordInt.row = nRow;
			xRecordInt.col = nCol;
			xRecordInt.data = newVar.IntVal();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record int");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_RECORD_INT, mxBody);
		}

		public void RequireRecordFloat(NFrame.NFGUID self, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
		{
			NFMsg.ObjectRecordFloat xData = new NFMsg.ObjectRecordFloat();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordFloat xRecordFloat = new NFMsg.RecordFloat();
			xData.property_list.Add(xRecordFloat);
			xRecordFloat.row = nRow;
			xRecordFloat.col = nCol;
			xRecordFloat.data = (float)newVar.FloatVal();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record float");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_RECORD_FLOAT, mxBody);
		}

		public void RequireRecordString(NFrame.NFGUID self, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
		{
			NFMsg.ObjectRecordString xData = new NFMsg.ObjectRecordString();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordString xRecordString = new NFMsg.RecordString();
			xData.property_list.Add(xRecordString);
			xRecordString.row = nRow;
			xRecordString.col = nCol;
			xRecordString.data = System.Text.Encoding.Default.GetBytes(newVar.StringVal());

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record string");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_RECORD_STRING, mxBody);
		}

		public void RequireRecordObject(NFrame.NFGUID self, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
		{
			NFMsg.ObjectRecordObject xData = new NFMsg.ObjectRecordObject();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordObject xRecordObject = new NFMsg.RecordObject();
			xData.property_list.Add(xRecordObject);
			xRecordObject.row = nRow;
			xRecordObject.col = nCol;
			xRecordObject.data = NFNetController.NFToPB(newVar.ObjectVal());

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			Debug.Log("send upload record object");
			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_RECORD_OBJECT, mxBody);
		}

		public void RequireRecordVector2(NFrame.NFGUID self, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
		{
			NFMsg.ObjectRecordVector2 xData = new NFMsg.ObjectRecordVector2();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordVector2 xRecordVector = new NFMsg.RecordVector2();
			xRecordVector.row = nRow;
			xRecordVector.col = nCol;
			xRecordVector.data = NFNetController.NFToPB(newVar.Vector2Val());

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_RECORD_VECTOR2, mxBody);
		}

		public void RequireRecordVector3(NFrame.NFGUID self, string strRecordName, int nRow, int nCol, NFDataList.TData newVar)
		{
			NFMsg.ObjectRecordVector3 xData = new NFMsg.ObjectRecordVector3();
			xData.player_id = NFNetController.NFToPB(self);
			xData.record_name = System.Text.Encoding.Default.GetBytes(strRecordName);

			NFMsg.RecordVector3 xRecordVector = new NFMsg.RecordVector3();
			xRecordVector.row = nRow;
			xRecordVector.col = nCol;
			xRecordVector.data = NFNetController.NFToPB(newVar.Vector3Val());

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(self, NFMsg.EGameMsgID.EGMI_ACK_RECORD_VECTOR3, mxBody);
		}

		////////////////////////////////////修改自身属性end
		/////////////////////////////////////////////////////////////////
	    //申请登陆
	    public void LoginPB(string strAccount, string strPassword, string strSessionKey)
	    {
	        NFMsg.ReqAccountLogin xData = new NFMsg.ReqAccountLogin();
	        xData.account = System.Text.Encoding.Default.GetBytes(strAccount);
	        xData.password = System.Text.Encoding.Default.GetBytes(strPassword);
	        xData.security_code = System.Text.Encoding.Default.GetBytes(strSessionKey);
	        xData.signBuff = System.Text.Encoding.Default.GetBytes("");
	        xData.clientVersion = 1;
	        xData.loginMode = 0;
	        xData.clientIP = 0;
	        xData.clientMAC = 0;
	        xData.device_info = System.Text.Encoding.Default.GetBytes("");
	        xData.extra_info = System.Text.Encoding.Default.GetBytes("");

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_LOGIN, mxBody);
	    }

		public void RequireWorldList()
		{
			NFMsg.ReqServerList xData = new NFMsg.ReqServerList();
			xData.type = NFMsg.ReqServerListType.RSLT_WORLD_SERVER;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_WORLD_LIST, mxBody);
		}

		public void RequireConnectWorld(int nWorldID)
		{
			NFMsg.ReqConnectWorld xData = new NFMsg.ReqConnectWorld();
			xData.world_id = nWorldID;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_CONNECT_WORLD, mxBody);
		}

		public void RequireVerifyWorldKey(string strAccount, string strKey)
		{
			NFMsg.ReqAccountLogin xData = new NFMsg.ReqAccountLogin();
			xData.account = System.Text.Encoding.Default.GetBytes(strAccount);
			xData.password = System.Text.Encoding.Default.GetBytes("");
			xData.security_code = System.Text.Encoding.Default.GetBytes(strKey);
			xData.signBuff = System.Text.Encoding.Default.GetBytes("");
			xData.clientVersion = 1;
			xData.loginMode = 0;
			xData.clientIP = 0;
			xData.clientMAC = 0;
			xData.device_info = System.Text.Encoding.Default.GetBytes("");
			xData.extra_info = System.Text.Encoding.Default.GetBytes("");

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_CONNECT_KEY, mxBody);
		}

		public void RequireServerList()
		{
			NFMsg.ReqServerList xData = new NFMsg.ReqServerList();
			xData.type = NFMsg.ReqServerListType.RSLT_GAMES_ERVER;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_WORLD_LIST, mxBody);
		}

		public void RequireSelectServer(int nServerID)
		{
			NFMsg.ReqSelectServer xData = new NFMsg.ReqSelectServer();
			xData.world_id = nServerID;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_SELECT_SERVER, mxBody);
		}

	    //申请角色列表
		public void RequireRoleList(string strAccount, int nGameID)
		{
			NFMsg.ReqRoleList xData = new NFMsg.ReqRoleList();
			xData.game_id = nGameID;
			xData.account = UnicodeEncoding.Default.GetBytes(strAccount);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_ROLE_LIST, mxBody);
		}

		public void RequireCreateRole(string strAccount, string strRoleName, int byCareer, int bySex, int nGameID)
		{
			if (strRoleName.Length >= 20 || strRoleName.Length < 1)
			{
				return;
			}

			NFMsg.ReqCreateRole xData = new NFMsg.ReqCreateRole();
			xData.career = byCareer;
			xData.sex = bySex;
			xData.noob_name = UnicodeEncoding.Default.GetBytes(strRoleName);
			xData.account = UnicodeEncoding.Default.GetBytes(strAccount);
			xData.race = 0;
			xData.game_id = nGameID;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(new NFrame.NFGUID(), NFMsg.EGameMsgID.EGMI_REQ_CREATE_ROLE, mxBody);

		}

		public void RequireDelRole(NFrame.NFGUID objectID, string strAccount, string strRoleName, int nGameID)
		{
			NFMsg.ReqDeleteRole xData = new NFMsg.ReqDeleteRole();
			xData.name = UnicodeEncoding.Default.GetBytes(strRoleName);
			xData.account = UnicodeEncoding.Default.GetBytes(strAccount);
			xData.game_id = nGameID;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_DELETE_ROLE, mxBody);
		}

		public void RequireEnterGameServer(NFrame.NFGUID objectID, string strAccount, string strRoleName, int nServerID)
		{
			NFMsg.ReqEnterGameServer xData = new NFMsg.ReqEnterGameServer();
			xData.name = UnicodeEncoding.Default.GetBytes(strRoleName);
			xData.account = UnicodeEncoding.Default.GetBytes(strAccount);
			xData.game_id = nServerID;
			xData.id = NFNetController.NFToPB(objectID);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_ENTER_GAME, mxBody);
		}

		public void RequireEnterGameFinish(NFrame.NFGUID objectID)
		{
			//only use in the first time when player enter game world
			NFMsg.ReqAckEnterGameSuccess xData = new NFMsg.ReqAckEnterGameSuccess();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_ENTER_GAME_FINISH, mxBody);
		}

	    //发送心跳
	    public void RequireHeartBeat(NFrame.NFGUID objectID)
	    {
	        NFMsg.ReqHeartBeat xData = new NFMsg.ReqHeartBeat();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_STS_HEART_BEAT, mxBody);
	    }

	    //WSAD移动
	    public void RequireMove(NFrame.NFGUID objectID, int nType, float fPosX, float fPosY, float fTarX, float fTarY)
	    {
	        NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
			xData.mover = NFNetController.NFToPB(objectID);
			xData.moveType = nType;

			NFMsg.Vector3 xNowPos = new NFMsg.Vector3();
	        xNowPos.x = fPosX;
	        xNowPos.y = 0.0f;
	        xNowPos.z = fPosY;
	        xData.source_pos.Add(xNowPos);

			NFMsg.Vector3 xTargetPos = new NFMsg.Vector3();
	        xTargetPos.x = fTarX;
	        xTargetPos.y = 0.0f;
	        xTargetPos.z = fTarY;
	        xData.target_pos.Add(xTargetPos);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_MOVE, mxBody);

	        //为了表现，客户端先走，后续同步
	    }
		public void RequireMoveImmune(NFrame.NFGUID objectID, float fX, float fZ)
		{
			NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
			xData.mover = NFNetController.NFToPB(objectID);
			xData.moveType = 0;
			NFMsg.Vector3 xTargetPos = new NFMsg.Vector3();
			xTargetPos.x = fX;
			xTargetPos.z = fZ;
			xData.target_pos.Add(xTargetPos);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_MOVE_IMMUNE, mxBody);
		}
		//申请状态机同步
		public void RequireStateSync(NFrame.NFGUID objectID, NFMsg.ReqAckPlayerMove xData)
		{
			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_STATE_SYNC, mxBody);

		}
		//有可能是他副本的NPC移动,因此增加64对象ID
		public void RequireUseSkill(NFrame.NFGUID objectID, string strKillID, NFrame.NFGUID nTargetID, float fNowX, float fNowZ, float fTarX, float fTarZ)
		{
			NFMsg.Vector3 xNowPos = new NFMsg.Vector3();
			NFMsg.Vector3 xTarPos = new NFMsg.Vector3();

			xNowPos.x = fNowX;
			xNowPos.y = 0.0f;
			xNowPos.z = fNowZ;
			xTarPos.x = fTarX;
			xTarPos.y = 0.0f;
			xTarPos.z = fTarZ;

			NFMsg.ReqAckUseSkill xData = new NFMsg.ReqAckUseSkill();
			xData.user = NFNetController.NFToPB(objectID);
			xData.skill_id = System.Text.Encoding.Default.GetBytes(strKillID);
			xData.tar_pos = xTarPos;
			xData.now_pos = xNowPos;

			NFMsg.EffectData xEffData = new NFMsg.EffectData();
			xEffData.effect_ident = (NFNetController.NFToPB(nTargetID));
			xEffData.effect_value = 0;
			xEffData.effect_rlt = 0;

			xData.effect_data.Add(xEffData);


			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_SKILL_OBJECTX, mxBody);
		}

		public void RequireUseItem(NFrame.NFGUID objectID, string strItemID, NFrame.NFGUID nTargetID)
		{
			NFMsg.ReqAckUseItem xData = new NFMsg.ReqAckUseItem();
			xData.user = NFNetController.NFToPB(objectID);
			xData.item_guid = new NFMsg.Ident ();
			xData.item = new ItemStruct ();
			xData.item.item_id = strItemID;
			xData.item.item_count = 1;
			xData.targetid = (NFNetController.NFToPB(nTargetID));

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_ITEM_OBJECT, mxBody);
		}

		public void RequireChat(NFrame.NFGUID objectID, NFrame.NFGUID targetID, int nType, string strData)
		{
			NFMsg.ReqAckPlayerChat xData = new NFMsg.ReqAckPlayerChat();
			xData.chat_id = NFNetController.NFToPB(targetID);
			xData.chat_name = UnicodeEncoding.Default.GetBytes(mxNetController.strRoleName);
			xData.chat_type = (NFMsg.ReqAckPlayerChat.EGameChatType)nType;
			xData.chat_info = UnicodeEncoding.Default.GetBytes(strData);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_CHAT, mxBody);
		}

		public void RequireSwapScene(NFrame.NFGUID objectID, int nTransferType, int nSceneID, int nLineIndex)
		{
			NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
			xData.transfer_type = (NFMsg.ReqAckSwapScene.EGameSwapType)nTransferType;
			xData.scene_id = nSceneID;
			xData.line_id = nLineIndex;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_SWAP_SCENE, mxBody);
		}


		public void RequireAcceptTask(NFrame.NFGUID objectID, string strTaskID)
		{
			NFMsg.ReqAcceptTask xData = new NFMsg.ReqAcceptTask();
			xData.task_id = UnicodeEncoding.Default.GetBytes(strTaskID);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_ACCEPT_TASK, mxBody);
		}

		public void RequireCompeleteTask(NFrame.NFGUID objectID, string strTaskID)
		{
			NFMsg.ReqCompeleteTask xData = new NFMsg.ReqCompeleteTask();
			xData.task_id = UnicodeEncoding.Default.GetBytes(strTaskID);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_COMPELETE_TASK, mxBody);
		}

		public void RequirePickUpItem(NFrame.NFGUID objectID, NFrame.NFGUID nItemID)
		{
			NFMsg.ReqPickDropItem xData = new NFMsg.ReqPickDropItem();
			xData.item_guid = NFNetController.NFToPB(nItemID);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);


			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_PICK_ITEM, mxBody);
		}


	    //申请查找对手
	    public void RequireSearchOppnent(NFrame.NFGUID objectID)
	    {
			NFMsg.ReqSearchOppnent xData = new NFMsg.ReqSearchOppnent();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_SEARCH_OPPNENT, mxBody);
	    }

		//申请挖掉某个矿石
		public void RequireMineTile(NFrame.NFGUID objectID, int nX, int nY)
		{
			NFMsg.ReqMiningTitle xData = new NFMsg.ReqMiningTitle();
			xData.x = nX;
			xData.y = nY;
			xData.opr = 0;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGEC_REQ_MINING_TITLE, mxBody);
		}
 		//申请切换到homescene
		public void RequireSwapToHomeScene(NFrame.NFGUID objectID)
		{
			NFMsg.ReqAckHomeScene xData = new NFMsg.ReqAckHomeScene();
	

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_SWAP_HOME_SCENE, mxBody);
		}

		//申请开始pvp
		public void RequireStartPVP(NFrame.NFGUID objectID, int nGold, int nDiamond)
		{
			NFMsg.ReqAckStartBattle xData = new NFMsg.ReqAckStartBattle();
			xData.diamond = nDiamond;
			xData.gold = nGold;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_START_OPPNENT, mxBody);
		}
		//申请结束pvp
		public void RequireEndPvp(NFrame.NFGUID objectID)
		{
			NFMsg.ReqEndBattle xData = new NFMsg.ReqEndBattle();

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGMI_REQ_END_OPPNENT, mxBody);
		}

		//set fight hero
		public void RequireSetFightHero(NFrame.NFGUID objectID, NFrame.NFGUID hero, int nSet)
		{
			NFMsg.ReqSetFightHero xData = new NFMsg.ReqSetFightHero();

			xData.Heroid = NFNetController.NFToPB(hero);
			xData.Set = nSet;

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGEC_REQ_SET_FIGHT_HERO, mxBody);
		}

		//switch fight hero
		public void RequireSwitchFightHero(NFrame.NFGUID objectID,  NFrame.NFGUID hero)
		{
			NFMsg.ReqSwitchFightHero xData = new NFMsg.ReqSwitchFightHero();

			xData.Heroid = NFNetController.NFToPB(hero);

			mxBody.SetLength(0);
			mxSerializer.Serialize(mxBody, xData);

			SendMsg(objectID, NFMsg.EGameMsgID.EGEC_REQ_SWITCH_FIGHT_HERO, mxBody);
		}
	}
}