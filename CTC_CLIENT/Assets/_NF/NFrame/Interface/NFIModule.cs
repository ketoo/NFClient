//-----------------------------------------------------------------------
// <copyright file="NFBehaviour.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.ObjectModel;
using UnityEngine;

namespace NFrame
{
	public class NFIModule
    {
		public virtual void Awake ()
		{
		}
		public virtual void Init ()
		{
		}
		public virtual void AfterInit ()
		{
		}

		public virtual void CheckConfig ()
		{
		}
		public virtual void ReadyExecute ()
		{
		}

		public virtual void Execute ()
		{
		}

		public virtual void BeforeShut ()
		{
		}
		public virtual void Shut ()
		{
		}
		public virtual void Finalize ()
		{
		}

		public virtual void OnReloadPlugin ()
		{
		}

		public string strName;
    }
}
