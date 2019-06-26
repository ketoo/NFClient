using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;
using NFSDK;
namespace NFrame
{

	public class NFLanguageModule : NFIModule
	{
		private Dictionary<GameObject, string> mxUIGO;
		private string mstrLocalLanguage = NFrame.Language.Chinese;


		private NFIElementModule mElementModule;

		public NFLanguageModule(NFIPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}

		public override void Awake(){}
		public override void Init()
		{
			mElementModule = mPluginManager.FindModule<NFIElementModule>();
		}

		public override void AfterInit(){ }
		public override void Execute(){ }
		public override void BeforeShut(){ }
		public override void Shut(){ }

		public string GetLocalLanguage(string strLanguageID)
		{
			//NFrame.Language.Chinese
			return mElementModule.QueryPropertyString(strLanguageID, mstrLocalLanguage);
		}

		public void SetLocalLanguage(string strLanguageName)
		{
			mstrLocalLanguage = strLanguageName;

			RefreshUILanguage();
		}

		public void AddLanguageUI(GameObject go)
		{
			mxUIGO.Add(go, "");
		}

		public void RemLanguageUI(GameObject go)
		{
			mxUIGO.Remove(go);
		}

		void RefreshUILanguage()
		{
			foreach (var x in mxUIGO)
			{
				GameObject go = x.Key;
				/*
                NFLanguage xLanguage = go.GetComponent<NFLanguage>();
				if (xLanguage)
				{
					xLanguage.RefreshUIData();
				}
				*/
			}
		}
	}
}
