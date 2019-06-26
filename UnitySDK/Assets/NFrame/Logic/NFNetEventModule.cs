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
using Google.Protobuf;
using NFSDK;

namespace NFrame
{
	public class NFNetEventModule : NFIModule
	{
		private NFIKernelModule mKernelModule;
		private NFIEventModule mEventModule;
        private NFHelpModule mHelpModule;
		private NFNetModule mNetModule;
		private NFLogModule mLogModule;

		public NFNetEventModule(NFIPluginManager pluginManager)
        {
            mPluginManager = pluginManager;
        }

		public override void Awake()
        {
            mNetModule = mPluginManager.FindModule<NFNetModule>();
            mHelpModule = mPluginManager.FindModule<NFHelpModule>();
			mKernelModule = mPluginManager.FindModule<NFIKernelModule>();
			mLogModule = mPluginManager.FindModule<NFLogModule>();
			mEventModule = mPluginManager.FindModule<NFIEventModule>();
        }

        public override void Init()
        {
			mNetModule.AddNetEventCallBack(NetEventDelegation);

			//mNetModule.RegisteredResultCodeDelegation(NFMsg.EGameEventCode.EGEC_UNKOWN_ERROR, EGEC_UNKOWN_ERROR);
			//mNetModule.RegisteredResultCodeDelegation(NFMsg.EGameEventCode.EGEC_ACCOUNT_SUCCESS, EGEC_ACCOUNT_SUCCESS);
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

		private void NetEventDelegation(NFNetEventType eventType)
		{
            Debug.Log(Time.realtimeSinceStartup.ToString() + " " + eventType.ToString());

			switch (eventType)
			{
				case NFNetEventType.Connected:
					mEventModule.DoEvent((int)NFLoginModule.Event.Connected);
					break;
				case NFNetEventType.Disconnected:
					mEventModule.DoEvent((int)NFLoginModule.Event.Disconnected);
                    break;
				case NFNetEventType.ConnectionRefused:
					mEventModule.DoEvent((int)NFLoginModule.Event.ConnectionRefused);
                    break;
				default:
					break;
			}
		}

	    private void EGEC_UNKOWN_ERROR(NFMsg.EGameEventCode eCode)
	    {

	    }

		private void EGEC_ACCOUNT_SUCCESS(NFMsg.EGameEventCode eCode)
		{
		}
	}
}