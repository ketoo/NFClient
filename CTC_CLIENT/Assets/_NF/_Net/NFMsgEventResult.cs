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
using ProtoBuf;

namespace NFrame
{
	public class NFMsgEventResult
	{
		NFNetController mxNetController = null;
		public NFMsgEventResult(NFNetController net)
	    {
	        mxNetController = net;
			mxNetController.mxNetClient.GetNetListener().RegisteredResultCodeDelegation(NFMsg.EGameEventCode.EGEC_UNKOWN_ERROR, EGEC_UNKOWN_ERROR);
			mxNetController.mxNetClient.GetNetListener().RegisteredResultCodeDelegation(NFMsg.EGameEventCode.EGEC_ACCOUNT_SUCCESS, EGEC_ACCOUNT_SUCCESS);
	    }

	    private void EGEC_UNKOWN_ERROR(NFMsg.EGameEventCode eCode)
	    {

	    }

		private void EGEC_ACCOUNT_SUCCESS(NFMsg.EGameEventCode eCode)
		{
		}
	}
}