using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NFMsg;
using UnityEngine;
using NFSDK;
using Google.Protobuf;

namespace NFrame
{
	public class NFLoginModule : NFIModule
    {    
		public enum Event : int
        {
			Connected = 0,
            Disconnected,
            ConnectionRefused,

            RoleList = 10,
			LoginSuccess,
            LoginFailure,
            WorldList,
            ServerList,
            SelectServerSuccess,
        };


        public string mAccount;
        public string mKey;
        public int mServerID;
        public ArrayList mWorldServerList = new ArrayList();
        public ArrayList mGameServerList = new ArrayList();


		public NFGUID mRoleID = new NFGUID();
        public string mRoleName;
        public ArrayList mRoleList = new ArrayList();

		private MemoryStream mxBody = new MemoryStream();

        private NFNetModule mNetModule;
        private NFUIModule mUIModule;
        private NFIEventModule mEventModule;
        private NFIKernelModule mKernelModule;

        public NFLoginModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
		}

        public override void Awake()
		{
            mNetModule = mPluginManager.FindModule<NFNetModule>();
            mUIModule = mPluginManager.FindModule<NFUIModule>();
            mEventModule = mPluginManager.FindModule<NFIEventModule>();
            mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
        }

        public override void Init()
		{
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_LOGIN, OnLoginProcess);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_WORLD_LIST, OnWorldList);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_CONNECT_WORLD, OnConnectWorld);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_CONNECT_KEY, OnConnectKey);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_SELECT_SERVER, OnSelectServer);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_ROLE_LIST, EGMI_ACK_ROLE_LIST);



			mEventModule.RegisterCallback((int)NFLoginModule.Event.Connected, OnConnected);
			mEventModule.RegisterCallback((int)NFLoginModule.Event.Disconnected, OnDisconnected);
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

		public void OnConnected(NFDataList valueList)
        {
			if (mKey != null && mKey.Length > 0)
			{
				//verify token
                RequireVerifyWorldKey(mAccount, mKey);
			}
        }

		public void OnDisconnected(NFDataList valueList)
        {
			if (mKey != null)
            {
                //reconnect
                mAccount = "";
                mKey = "";
                mServerID = 0;
                mWorldServerList.Clear();
                mGameServerList.Clear();
                mRoleID = new NFGUID();
                mRoleName = "";
                mRoleList.Clear();

                //Clear all players and UI objects
                NFDataList xDataList = mKernelModule.GetObjectList();
                for (int i = 0; i < xDataList.Count(); ++i)
                {
                    mKernelModule.DestroyObject(xDataList.ObjectVal(i));
                }

                //mUIModule.DestroyAllUI();
                //mUIModule.ShowUI<NFUILogin>();
            }
        }
        
        // 请求消息
	    public void LoginPB(string strAccount, string strPwd, string strKey)
        {
            NFMsg.ReqAccountLogin xData = new NFMsg.ReqAccountLogin();
            xData.account = ByteString.CopyFromUtf8(strAccount);
            xData.password = ByteString.CopyFromUtf8(strPwd);
            xData.security_code = ByteString.CopyFromUtf8(strKey);
            xData.signBuff = ByteString.CopyFromUtf8("");
            xData.clientVersion = 1;
            xData.loginMode = NFMsg.ELoginMode.ELM_AUTO_REGISTER_LOGIN;
            xData.clientIP = 0;
            xData.clientMAC = 0;
            xData.device_info = ByteString.CopyFromUtf8("");
            xData.extra_info = ByteString.CopyFromUtf8("");

            mAccount = strAccount;
            /*
            MemoryStream stream = new MemoryStream();
            xData.WriteTo(stream);
            mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_LOGIN, stream);
*/
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

            mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_LOGIN, mxBody);
        }

	    public void RequireWorldList()
        {
            NFMsg.ReqServerList xData = new NFMsg.ReqServerList();
            xData.type = NFMsg.ReqServerListType.RSLT_WORLD_SERVER;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_WORLD_LIST, mxBody);
        }

	    public void RequireConnectWorld(int nWorldID)
        {
            NFMsg.ReqConnectWorld xData = new NFMsg.ReqConnectWorld();
            xData.world_id = nWorldID;
            xData.login_id = 0;
            xData.account = ByteString.CopyFromUtf8("");
            xData.sender = new Ident();

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_CONNECT_WORLD, mxBody);
        }

	    public void RequireVerifyWorldKey(string strAccount, string strKey)
        {
            NFMsg.ReqAccountLogin xData = new NFMsg.ReqAccountLogin();
            xData.account = ByteString.CopyFromUtf8(strAccount);
            xData.password = ByteString.CopyFromUtf8("");
            xData.security_code = ByteString.CopyFromUtf8(strKey);
            xData.signBuff = ByteString.CopyFromUtf8("");
            xData.clientVersion = 1;
            xData.loginMode = 0;
            xData.clientIP = 0;
            xData.clientMAC = 0;
            xData.device_info = ByteString.CopyFromUtf8("");
            xData.extra_info = ByteString.CopyFromUtf8("");

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_CONNECT_KEY, mxBody);
        }

	    public void RequireServerList()
        {
            NFMsg.ReqServerList xData = new NFMsg.ReqServerList();
            xData.type = NFMsg.ReqServerListType.RSLT_GAMES_ERVER;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_WORLD_LIST, mxBody);
        }

	    public void RequireSelectServer(int nServerID)
        {
            NFMsg.ReqSelectServer xData = new NFMsg.ReqSelectServer();
            xData.world_id = nServerID;
            mServerID = nServerID;
                        
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_SELECT_SERVER, mxBody);
        }

        // 接收消息
		private void OnLoginProcess(UInt16 id, MemoryStream stream)
        {
			NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.msg_data);

            if (EGameEventCode.EGEC_ACCOUNT_SUCCESS == xData.event_code)
            {
				Debug.Log("Login  SUCCESS");
				mEventModule.DoEvent((int)NFLoginModule.Event.LoginSuccess);
            }
            else
            {
                Debug.Log("Login Faild,Code: " + xData.event_code);
                NFDataList varList = new NFDataList();
                varList.AddInt((Int64)xData.event_code);
				mEventModule.DoEvent((int)NFLoginModule.Event.LoginFailure);
            }
        }

        private void OnWorldList(UInt16 id, MemoryStream stream)
        {
            
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckServerList xData = NFMsg.AckServerList.Parser.ParseFrom(xMsg.msg_data);

            if (ReqServerListType.RSLT_WORLD_SERVER == xData.type)
            {
                for (int i = 0; i < xData.info.Count; ++i)
                {
                    ServerInfo info = xData.info[i];
                    Debug.Log("WorldList  ServerId: " + info.server_id + " Name: " + info.name.ToStringUtf8() + " Status: " + info.status);
                    mWorldServerList.Add(info);
                }

				mEventModule.DoEvent((int)NFLoginModule.Event.WorldList);
            }
            else if (ReqServerListType.RSLT_GAMES_ERVER == xData.type)
            {
                for (int i = 0; i < xData.info.Count; ++i)
                {
                    ServerInfo info = xData.info[i];
                    Debug.Log("GameList  ServerId: " + info.server_id + " Name: " + info.name.ToStringUtf8() + " Status: " + info.status);
                    mGameServerList.Add(info);
                }
				mEventModule.DoEvent((int)NFLoginModule.Event.ServerList);
            }
        }

        private void OnConnectWorld(UInt16 id, MemoryStream stream)
        {
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckConnectWorldResult xData = NFMsg.AckConnectWorldResult.Parser.ParseFrom(xMsg.msg_data);

            mKey = xData.world_key.ToStringUtf8();
            
			mNetModule.BeforeShut();
			mNetModule.Shut();

			String strIP = xData.world_ip.ToStringUtf8();
			if (strIP == "127.0.0.1")
			{
				strIP = mNetModule.FirstIP();
			}
			mNetModule.StartConnect(strIP, xData.world_port);

        }
        private void OnConnectKey(UInt16 id, MemoryStream stream)
        {
	        NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);
            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.msg_data);

            if (xData.event_code == EGameEventCode.EGEC_VERIFY_KEY_SUCCESS)
            {
                Debug.Log("VerifyKey SUCCESS");
                RequireServerList();
            }
            else
            {
                Debug.Log("VerifyKey Failed");
            }
        }

        private void OnSelectServer(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream); 

            NFMsg.AckEventResult xData = NFMsg.AckEventResult.Parser.ParseFrom(xMsg.msg_data);

            if (xData.event_code == EGameEventCode.EGEC_SELECTSERVER_SUCCESS)
            {
                Debug.Log("SelectGame SUCCESS ");
				mEventModule.DoEvent((int)NFLoginModule.Event.SelectServerSuccess);
            }
            else
            {
                Debug.Log("SelectGame Failed ");
            }
        }


        private void EGMI_ACK_ROLE_LIST(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.AckRoleLiteInfoList xData = NFMsg.AckRoleLiteInfoList.Parser.ParseFrom(xMsg.msg_data);

			Debug.Log("QueryRoleList  SUCCESS : " + xData.char_data.Count);

			mRoleList.Clear();

            for (int i = 0; i < xData.char_data.Count; ++i)
            {
                NFMsg.RoleLiteInfo info = xData.char_data[i];

                Debug.Log("QueryRoleList  SUCCESS : " + info.noob_name.ToStringUtf8());

				mRoleList.Add(info);
            }


			mEventModule.DoEvent((int)NFLoginModule.Event.RoleList);

            //////////////////
            /*
			if (mRoleList.Count > 0)
            {
                //NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_ROLEHALL);

				NFMsg.RoleLiteInfo xLiteInfo = (NFMsg.RoleLiteInfo)mRoleList[0];
                NFGUID xEnterID = new NFGUID();
                xEnterID.nData64 = xLiteInfo.id.index;
                xEnterID.nHead64 = xLiteInfo.id.svrid;

				mNetModule.RequireEnterGameServer(xEnterID, mAccount, xLiteInfo.noob_name.ToStringUtf8(), mServerID);

                //mxNetController.mPlayerState = NFNetController.PLAYER_STATE.E_PLAYER_WAITING_TO_GAME;

                Debug.Log("Selected role :" + xLiteInfo.noob_name.ToStringUtf8());
            }
            else
            {
                //NFCSectionManager.Instance.SetGameState(NFCSection.UI_SECTION_STATE.UISS_CREATEHALL);
				RequireCreateRole( mAccount, 0, 0, mServerID);
                Debug.Log("No Role!, require to create a new role! ");
            }
            */
        }

        //申请角色列表
        public void RequireRoleList()
        {
            NFMsg.ReqRoleList xData = new NFMsg.ReqRoleList();
			xData.game_id = mServerID;
			xData.account = ByteString.CopyFromUtf8(mAccount);
            
            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_ROLE_LIST, mxBody);
        }

        public void RequireCreateRole(string strRoleName, int byCareer, int bySex)
        {
            if (strRoleName.Length >= 20 || strRoleName.Length < 1)
            {
                return;
            }

            NFMsg.ReqCreateRole xData = new NFMsg.ReqCreateRole();
            xData.career = byCareer;
            xData.sex = bySex;
            xData.noob_name = ByteString.CopyFromUtf8(strRoleName);
			xData.account = ByteString.CopyFromUtf8(mAccount);
            xData.race = 0;
			xData.game_id = mServerID;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_CREATE_ROLE, mxBody);
        }


        public void RequireDelRole(string strRoleName)
        {
            NFMsg.ReqDeleteRole xData = new NFMsg.ReqDeleteRole();
            xData.name = ByteString.CopyFromUtf8(strRoleName);
            xData.account = ByteString.CopyFromUtf8(mAccount);
			xData.game_id = mServerID;

            mxBody.SetLength(0);
            xData.WriteTo(mxBody);

			mNetModule.SendMsg(NFMsg.EGameMsgID.EGMI_REQ_DELETE_ROLE, mxBody);
        }

    };
}