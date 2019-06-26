using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.Protobuf;
using UnityEngine;
using NFSDK;

namespace NFrame
{
	public class NFNetModule : NFIModule
    {
		private NFIKernelModule mKernelModule;
		private NFHelpModule mHelpModule;
		private NFLogModule mLogModule;
		private NFLoginModule mLoginModule;

		private NFNetListener mNetListener;
		private NFNetClient mNetClient;

		private string strFirstIP = "";

        //sender
		private NFMsg.MsgBase mxData = new NFMsg.MsgBase();
		private MemoryStream mxBody = new MemoryStream();
		private MsgHead mxHead = new MsgHead();

        public NFNetModule(NFIPluginManager pluginManager)
        {
            mNetListener = new NFNetListener();
            mPluginManager = pluginManager;
        }
        
        public override void Awake()
        {
        }

		public override void Init()
		{
		}

        public override void Execute()
        {
			if (null != mNetClient)
			{
				mNetClient.Execute();
			}
        }

        public override void BeforeShut()
        {
			if (null != mNetClient)
            {
                mNetClient.Disconnect();
            }
        }

        public override void Shut()
        {
			mNetClient = null;
		}

		public override void AfterInit()
		{
			mHelpModule = mPluginManager.FindModule<NFHelpModule>();
			mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
			mLogModule = mPluginManager.FindModule<NFLogModule>();
			mLoginModule = mPluginManager.FindModule<NFLoginModule>();

		}

		public String FirstIP()
		{
			return strFirstIP;
		}

        public void StartConnect(string strIP, int nPort)
        {
            Debug.Log(Time.realtimeSinceStartup.ToString() + " StartConnect " + strIP + " " + nPort.ToString());

			mNetClient = new NFNetClient(mNetListener);

            mNetClient.Connect(strIP, nPort);

            if (strFirstIP.Length <= 0)
            {
                strFirstIP = strIP;
            }
        }

        public NFNetState GetState()
        {
            return mNetClient.GetState();
        }

        public void ConnectServerByDns(string dns, int port)
		{
		}

		public void AddReceiveCallBack(NFMsg.EGameMsgID eMsg, NFSDK.NFNetListener.MsgDelegation netHandler)
        {
			mNetListener.RegisteredDelegation((UInt16)eMsg, netHandler);
        }
  
		public void AddNetEventCallBack(NFSDK.NFNetListener.EventDelegation netHandler)
        {
			mNetListener.RegisteredNetEventHandler(netHandler);
        }

        public void SendMsg(NFMsg.EGameMsgID unMsgID, MemoryStream stream)
        {
            //NFMsg.MsgBase
            mxData.player_id = mHelpModule.NFToPB(mLoginModule.mRoleID);
            mxData.msg_data = ByteString.CopyFrom(stream.ToArray());

            mxBody.SetLength(0);
            mxData.WriteTo(mxBody);

            mxHead.unMsgID = (UInt16)unMsgID;
            mxHead.unDataLen = (UInt32)mxBody.Length + (UInt32)ConstDefine.NF_PACKET_HEAD_SIZE;

            byte[] bodyByte = mxBody.ToArray();
            byte[] headByte = mxHead.EnCode();

            byte[] sendBytes = new byte[mxHead.unDataLen];
            headByte.CopyTo(sendBytes, 0);
            bodyByte.CopyTo(sendBytes, headByte.Length);

            mNetClient.SendBytes(sendBytes);

            /////////////////////////////////////////////////////////////////
        }
      
        public void RequireEnterGameServer()
        {
            NFMsg.ReqEnterGameServer xData = new NFMsg.ReqEnterGameServer();
			xData.name = ByteString.CopyFromUtf8(mLoginModule.mRoleName);
			xData.account = ByteString.CopyFromUtf8(mLoginModule.mAccount);
			xData.game_id = mLoginModule.mServerID;
			xData.id = mHelpModule.NFToPB(mLoginModule.mRoleID);
			
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg(NFMsg.EGameMsgID.EGMI_REQ_ENTER_GAME, mxBody);
        }

        public void RequireEnterGameFinish()
        {
            //only use in the first time when player enter game world
            NFMsg.ReqAckEnterGameSuccess xData = new NFMsg.ReqAckEnterGameSuccess();
            xData.arg = 1;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg(NFMsg.EGameMsgID.EGMI_REQ_ENTER_GAME_FINISH, mxBody);
        }

        //发送心跳
        public void RequireHeartBeat()
        {
            NFMsg.ReqHeartBeat xData = new NFMsg.ReqHeartBeat();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg(NFMsg.EGameMsgID.EGMI_STS_HEART_BEAT, mxBody);
        }

        public void RequireSwapScene(int nTransferType, int nSceneID, int nLineIndex)
        {
            NFMsg.ReqAckSwapScene xData = new NFMsg.ReqAckSwapScene();
            xData.transfer_type = (NFMsg.ReqAckSwapScene.Types.EGameSwapType)nTransferType;
            xData.scene_id = nSceneID;
            xData.line_id = nLineIndex;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);


            SendMsg(NFMsg.EGameMsgID.EGMI_REQ_SWAP_SCENE, mxBody);
        }
        //WSAD移动
        public void RequireMove(NFGUID objectID, int nType, UnityEngine.Vector3 vPos, UnityEngine.Vector3 vTar)
        {
            NFMsg.ReqAckPlayerMove xData = new NFMsg.ReqAckPlayerMove();
            xData.mover = mHelpModule.NFToPB(objectID);
            xData.moveType = nType;
            xData.moveType = 0;
            xData.speed = 4f;
            xData.time = 0;

            NFMsg.Vector3 xNowPos = new NFMsg.Vector3();
            xNowPos.x = vPos.x;
            xNowPos.y = vPos.y;
            xNowPos.z = vPos.z;
            xData.source_pos.Add(xNowPos);

            NFMsg.Vector3 xTargetPos = new NFMsg.Vector3();
            xTargetPos.x = vTar.x;
            xTargetPos.y = vTar.y;
            xTargetPos.z = vTar.z;
            xData.target_pos.Add(xTargetPos);

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            SendMsg(NFMsg.EGameMsgID.EGMI_REQ_MOVE, mxBody);

            //为了表现，客户端先走，后续同步
        }
    }
}