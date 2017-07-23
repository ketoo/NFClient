using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using NFrame;

namespace NFrame
{
	public class NFNetController
	{
		#region Instance
		private static NFNetController _Instance = null;
		private static readonly object _syncLock = new object();
		public static NFNetController Instance
		{
			get
			{
				lock (_syncLock)
				{
					if (_Instance == null)
					{
						_Instance = new NFNetController();
					}
					return _Instance;
				}
			}
		}
		#endregion

		static public NFMsg.Ident NFToPB(NFrame.NFGUID xID)
		{
			NFMsg.Ident xIdent = new NFMsg.Ident();
			xIdent.svrid = xID.nHead64;
			xIdent.index = xID.nData64;

			return xIdent;
		}
		static public NFrame.NFGUID PBToNF(NFMsg.Ident xID)
		{
			NFrame.NFGUID xIdent = new NFrame.NFGUID();
			xIdent.nHead64 = xID.svrid;
			xIdent.nData64 = xID.index;

			return xIdent;
		}
		static public NFMsg.Vector2 NFToPB(NFVector2 value)
		{
			NFMsg.Vector2 vector = new NFMsg.Vector2();
			vector.x = value.X();
			vector.y = value.Y();

			return vector;
		}
		static public NFVector2 PBToNF(NFMsg.Vector2 value)
		{
			NFVector2 vector = new NFVector2();
			vector.SetX(value.x);
			vector.SetY(value.y);

			return vector;
		}
		static public NFMsg.Vector3 NFToPB(NFVector3 value)
		{
			NFMsg.Vector3 vector = new NFMsg.Vector3();
			vector.x = value.X();
			vector.y = value.Y();
			vector.z = value.Z();

			return vector;
		}
		static public NFVector3 PBToNF(NFMsg.Vector3  value)
		{
			NFVector3 vector = new NFVector3();
			vector.SetX(value.x);
			vector.SetY(value.y);
			vector.SetZ(value.z);

			return vector;
		}
		////////////////////////////////////////////////////////////////

		public NFNetClient mxNetClient = null;
		public NFNetSender mxNetSender = null;

	    public NFMsgEventResult mxMsgEventResult = null;
		public NFMsgListener mxMsgListener = null;

	    public enum PLAYER_STATE
	    {
	        E_NONE,//等待选择服务器登录
			E_CONECTING,//等待连接...
	        E_CONNECTED,//等待登录(已经连接成功)
			E_HAS_PLAYER_LOGIN,

			E_START_CONNECT_TO_GATE,
			E_WATING_VERIFY,
			E_HAS_VERIFY,

			E_HAS_PLAYER_ROLELIST,
			E_PLAYER_WAITING_TO_GAME,
	        E_PLAYER_GAMEING,//游戏中
	        E_DISCOUNT,//掉线

	    };

		///////////////////////////////////////////////////////////////////////////////////////

	    public NFNetController()
	    {
	    }

	    public void StartConnect(string strIP, int nPort)
	    {
	        mxNetClient = new NFNetClient(this);        
	        mxNetSender = new NFNetSender(this);

	        mxMsgEventResult = new NFMsgEventResult(this);
			mxMsgListener = new NFMsgListener (this);

			mxMsgListener.Init();

	        mxNetClient.Connect(strIP, nPort);

			if (strFirstIP.Length <= 0)
			{
				strFirstIP = strIP;
			}
	    }  

	    // Update is called once per frame
	    float fTime = 0.0f;
	    public void Execute()
	    {
	        if (null != mxNetClient)
	        {
	            mxNetClient.Update();
	            fTime += Time.deltaTime;

				if (mxNetClient.IsConnected ())
				{
					if (fTime > 10.0f)
					{
						fTime = 0.0f;
						mxNetSender.RequireHeartBeat (xMainRoleID);
					}
				}
	        }

	    }

	    public void Destroy()
	    {
	        if (null != mxNetClient)
	        {
	            mxNetClient.Disconnect();
	        }
	    }

		public string strFirstIP = "";
		public string strWorldIP = "";
		public int nWorldPort = 0;
	    public string strKey = "";
		public int nServerID = 0;

	    public string strAccount = "server1";
	    public string strPassword = "123456";
	    public string strRoleName = "";
		public NFGUID xMainRoleID = new NFGUID();//主角ID
	    public NFGUID xFocusBookID = new NFGUID();//BOOKID

		public PLAYER_STATE mPlayerState = PLAYER_STATE.E_NONE;
		public NFGUID mxFightHero = new NFGUID();

		public ArrayList aWorldList = new ArrayList();
		public ArrayList aServerList = new ArrayList();
		public ArrayList aCharList = new ArrayList();

		public ArrayList aChatMsgList = new ArrayList();
		public ArrayList aMsgList = new ArrayList();
	}
}
