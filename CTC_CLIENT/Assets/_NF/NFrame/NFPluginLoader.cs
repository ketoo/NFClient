using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NFrame
{
	public class NFPluginLoader : NFIModule 
	{

		public override void Awake ()
		{
			//mxModule.Add (NFIKernelModule, new NFCKernelModule (this));
		}
		public override void Init ()
		{
		}
		public override void AfterInit ()
		{
		}

		public override void CheckConfig ()
		{
		}
		public override void ReadyExecute ()
		{
		}

		public override void Execute ()
		{
		}

		public override void BeforeShut ()
		{
		}
		public override void Shut ()
		{
		}
		public override void Finalize ()
		{
		}

		public override void OnReloadPlugin ()
		{
		}


		private Dictionary<string, NFIModule> mxModule = new Dictionary<string, NFIModule>();
	}

}