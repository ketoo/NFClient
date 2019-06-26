using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NFrame;
using System.IO;
using System;

namespace NFSDK
{
	public class NFSceneModule : NFIModule
	{
		private NFIKernelModule mKernelModule;
		private NFNetModule mNetModule;
		private NFIEventModule mEventModule;
        private NFHelpModule mHelpModule;
        private NFLoginModule mLoginModule;


		private Dictionary<NFGUID, GameObject> mGameObjectMap = new Dictionary<NFGUID, GameObject>();

        public NFSceneModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }
  

		public override void Awake() 
		{ 
		}
		public override void Init()
		{ 
		}

		public override void AfterInit() 
		{
			mNetModule = FindModule<NFNetModule>();
			mKernelModule = FindModule<NFIKernelModule>();
			mEventModule = FindModule<NFIEventModule>();
            mHelpModule = FindModule<NFHelpModule>();
            mLoginModule = FindModule<NFLoginModule>();

            mKernelModule.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);
            mKernelModule.RegisterClassCallBack(NFrame.NPC.ThisName, OnClassNPCEventHandler);

            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_SWAP_SCENE, SwapSceneEventHandler);
            mNetModule.AddReceiveCallBack(NFMsg.EGameMsgID.EGMI_ACK_MOVE, OnPlayerMove);

            //mEventModule.RegisterCallback((int)NFPlayerModule.Event.SwapScene, SwapSceneEventHandler);
			//mEventModule.RegisterCallback((int)NFPlayerModule.Event.PlayerMove, OnPlayerMove);
		}

		public override void Execute() {}
		public override void BeforeShut() {  }
		public override void Shut() {}

		protected void SwapSceneEventHandler(UInt16 id, MemoryStream stream)
		{

            //TODO
			Application.LoadLevel(1);
		}

		GameObject GetObject(NFGUID id)
		{
			GameObject gameObject = null;
			mGameObjectMap.TryGetValue(id, out gameObject);

			return gameObject;
		}
  

		public void OnPlayerMove(UInt16 id, MemoryStream stream)
        {
            NFMsg.MsgBase xMsg = NFMsg.MsgBase.Parser.ParseFrom(stream);

            NFMsg.ReqAckPlayerMove xData = NFMsg.ReqAckPlayerMove.Parser.ParseFrom(xMsg.msg_data);

            if (xData.target_pos.Count <= 0)
            {
                return;
            }

            NFGUID tar = mHelpModule.PBToNF(xData.mover);

            if (tar == mLoginModule.mRoleID)
                return;

			GameObject player = GetObject(tar);
			OtherPlayer otherPlayer = player.GetComponent<OtherPlayer>();

            float fSpeed = xData.speed;

            long nType = xData.moveType;
            NFVector3 pos = mHelpModule.PBToNF(xData.target_pos[0]);
            Vector3 vPos = new Vector3(pos.X(), pos.Y(), pos.Z());

			if (nType > 0)
			{
				otherPlayer.JumpTo(vPos);
			}
			else
			{
				otherPlayer.MoveTo((float)fSpeed, vPos);
			}

			Debug.Log("Player Move:" + vPos.ToString());
        }

        private void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
                Debug.Log("OBJECT_CREATE:" + self.ToString());

                string strConfigID = mKernelModule.QueryPropertyString(self, NFrame.Player.ConfigID);
                Vector3 vec = new Vector3();
                NFVector3 vector3 = mKernelModule.QueryPropertyVector3(self, NFrame.Player.Position);
                //vec.x = vector3.X();
                //vec.y = vector3.Y();
                //vec.z = vector3.Z();

                //MainPlayer
                string strPrefabPath = "Player/AIThirdPersonController";
                if (self == mLoginModule.mRoleID)
                {
                    strPrefabPath = "Player/ThirdPersonController";
                }
                //if (strConfigID.Length <= 0)
                //{
                //    strPrefabPath = NFCElementModule.Instance().QueryPropertyString("Player", "Prefab");
                //}
                //else
                //{
                //    strPrefabPath = NFCKernelModule.Instance.GetElementModule().QueryPropertyString(strConfigID, "Prefab");
                //}

                //CreateObject(self, strPrefabPath, vec, strClassName);

                GameObject perfb = Resources.Load<GameObject>(strPrefabPath);
                GameObject player = GameObject.Instantiate(perfb);

				mGameObjectMap.Add(self, player);

				GameObject.DontDestroyOnLoad(player);

                player.name = self.ToString();
                player.transform.position = vec;

                //MainPlayer
				if (self == mLoginModule.mRoleID)
                {
                    player.AddComponent<MainPlayer>();
                }
                else
                {
                    player.AddComponent<OtherPlayer>();
                }
            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
            {
				//DestroyObject(transform.Find(self.ToString()));
				GameObject go = GetObject(self);
				if (go != null)
				{
					mGameObjectMap.Remove(self);

					GameObject.DestroyObject(go);
				}
            }
		}

        private void OnClassNPCEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
                string strConfigID = mKernelModule.QueryPropertyString(self, NFrame.Player.ConfigID);
                Vector3 vec = new Vector3();
                NFVector3 vector3 = mKernelModule.QueryPropertyVector3(self, NFrame.Player.Position);
                //vec.x = vector3.X();
                //vec.y = vector3.Y();
                //vec.z = vector3.Z();

                string strPrefabPath = "";
            //    if (strConfigID.Length <= 0)
            //    {
            //        strPrefabPath = NFCElementModule.Instance().QueryPropertyString("Player", "Prefab");
            //    }
            //    else
            //    {
            //        strPrefabPath = NFCElementModule.Instance()..QueryPropertyString(strConfigID, "Prefab");
            //    }

                strPrefabPath = "Player/AIThirdPersonController";
                GameObject playerPerf = Resources.Load<GameObject>(strPrefabPath);
				GameObject player = GameObject.Instantiate(playerPerf);


                mGameObjectMap.Add(self, player);

            }
            else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
            {
				GameObject go = GetObject(self);
                if (go != null)
                {
                    mGameObjectMap.Remove(self);

                    GameObject.DestroyObject(go);
                }
            }

        }
	}
}
