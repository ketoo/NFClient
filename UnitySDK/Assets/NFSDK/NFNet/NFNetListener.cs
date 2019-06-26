using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NFSDK
{
	public class ConstDefine
	{
		public static UInt32 NF_PACKET_HEAD_SIZE = 6;
		public static int MAX_PACKET_LEN = 1024 * 1024 * 1;
	};

	public class MsgHead
	{
		public MsgHead()
		{
			unMsgID = 0;
			unDataLen = 0;
		}
		public UInt16 unMsgID;
		public UInt32 unDataLen;

		public byte[] EnCode()
		{
			byte[] byMsgID = BitConverter.GetBytes(unMsgID);
			byte[] byDataLen = BitConverter.GetBytes(unDataLen);

			bool isLittle = BitConverter.IsLittleEndian;
			if (isLittle)
			{
				Array.Reverse(byMsgID);
				Array.Reverse(byDataLen);
			}

			byte[] byHead = new byte[ConstDefine.NF_PACKET_HEAD_SIZE];
			Array.Copy(byMsgID, 0, byHead, 0, sizeof(UInt16));
			Array.Copy(byDataLen, 0, byHead, sizeof(UInt16), sizeof(UInt32));

			return byHead;
		}

		public bool DeCode(byte[] strData)
		{
			if (strData.Length == ConstDefine.NF_PACKET_HEAD_SIZE)
			{
				byte[] byMsgID = new byte[sizeof(UInt16)];
				byte[] byDataLen = new byte[sizeof(UInt32)];

				Array.Copy(strData, 0, byMsgID, 0, sizeof(UInt16));
				Array.Copy(strData, sizeof(UInt16), byDataLen, 0, sizeof(UInt32));

				bool isLittle = BitConverter.IsLittleEndian;
				if (isLittle)
				{
					Array.Reverse(byMsgID);
					Array.Reverse(byDataLen);
				}

				unMsgID = BitConverter.ToUInt16(byMsgID,0);
				unDataLen = BitConverter.ToUInt32(byDataLen,0);

				return true;
			}

			return false;
		}
	};

    public class NFNetListener
    {      
        private UInt32 mnPacketSize = 0;
        private byte[] mPacket = new byte[ConstDefine.MAX_PACKET_LEN];

		public delegate void EventDelegation(NFNetEventType eventType);
		private EventDelegation mHandlerDelegation;
        
		public delegate void MsgDelegation(UInt16 id, MemoryStream stream);
		private Dictionary<UInt16, MsgDelegation> mhtMsgDelegation = new Dictionary<UInt16, MsgDelegation>();
        
		//////////////////////////////////////////////////////////////////
		/// 
        public void OnClientConnect(NFNetEventParams eventParams)
		{
            Array.Clear(mPacket, 0, ConstDefine.MAX_PACKET_LEN);

            //NFLogModule.Instance.Log(NFLogModule.LOG_LEVEL.DEBUG, "Client connected");

			mHandlerDelegation(NFNetEventType.Connected);
        }

		public void OnClientDisconnect(NFNetEventParams eventParams)
        {
			//NFLogModule.Instance.Log(NFLogModule.LOG_LEVEL.DEBUG, "Client disconnected");

			mHandlerDelegation(NFNetEventType.Disconnected);
        }

        public void OnClientConnectionRefused(NFNetEventParams eventParams)
        {
			//NFLogModule.Instance.Log(NFLogModule.LOG_LEVEL.DEBUG, "Client refused");

			mHandlerDelegation(NFNetEventType.ConnectionRefused);
        }

		public void OnDataReceived(NFNetEventParams eventParams)
		{

			byte[] bytes = eventParams.packet.bytes;
			int bytesCount = eventParams.packet.bytesCount;

			//NFLogModule.Instance.Log(NFLogModule.LOG_LEVEL.INFO, "ReciveDataSize:" + bytesCount);
            
			if (mnPacketSize + bytesCount < ConstDefine.MAX_PACKET_LEN)
			{
				Array.Copy(bytes, 0, mPacket, mnPacketSize, bytesCount);
				mnPacketSize += (UInt32)bytesCount;

				OnDataReceived();
			}
		}

		void OnDataReceived()
		{
			//ensure the length > size of MsgHead at all time
			if (mnPacketSize >= ConstDefine.NF_PACKET_HEAD_SIZE)
			{
				byte[] headBytes = new byte[ConstDefine.NF_PACKET_HEAD_SIZE];
				Array.Copy(mPacket, 0, headBytes, 0, ConstDefine.NF_PACKET_HEAD_SIZE);

				MsgHead head = new MsgHead ();

				if(head.DeCode(headBytes))
				{
					if (head.unDataLen == mnPacketSize)
					{
						byte[] body_head = new byte[head.unDataLen];
						Array.Copy(mPacket, 0, body_head, 0, head.unDataLen);
						mnPacketSize = 0;

						if (false == OnDataReceived(body_head, head.unDataLen))
						{
							OnClientDisconnect(new NFNetEventParams());
						}
					}
					else if (mnPacketSize > head.unDataLen)
					{
						UInt32 nNewLen = mnPacketSize - head.unDataLen;
						byte[] newpacket = new byte[nNewLen];
						Array.Copy(mPacket, head.unDataLen, newpacket, 0, nNewLen);

						byte[] body_head = new byte[head.unDataLen];
						Array.Copy(mPacket, 0, body_head, 0, head.unDataLen);

						//memset 0
						Array.Clear(mPacket, 0, ConstDefine.MAX_PACKET_LEN);
						mnPacketSize = nNewLen;
						Array.Copy(newpacket, 0, mPacket, 0, nNewLen);


						if (false == OnDataReceived(body_head, head.unDataLen))
						{
							OnClientDisconnect(new NFNetEventParams());
						}

						OnDataReceived();
					}
				}
			}
		}

		bool OnDataReceived(byte[] bytes, UInt32 bytesCount)
		{
			if (bytes.Length == bytesCount)
			{

				byte[] headBytes = new byte[ConstDefine.NF_PACKET_HEAD_SIZE];
				Array.Copy(bytes, 0, headBytes, 0, ConstDefine.NF_PACKET_HEAD_SIZE);

				MsgHead head = new MsgHead ();
				if(head.DeCode(headBytes) && head.unDataLen == bytesCount)
				{
					Int32 nBodyLen = (Int32)bytesCount - (Int32)ConstDefine.NF_PACKET_HEAD_SIZE;
					if (nBodyLen > 0)
					{
						byte[] body = new byte[nBodyLen];
						Array.Copy(bytes, ConstDefine.NF_PACKET_HEAD_SIZE, body, 0, nBodyLen);

						OnMessageEvent(head, body);

						return true;
					}
					else
					{
						//space packet
					}
				}


			}

			return false;
		}

		private void OnMessageEvent(MsgHead head, byte[] bytes)
        {
            if (head.unDataLen != bytes.Length + ConstDefine.NF_PACKET_HEAD_SIZE)
            {
				Debug.LogError("ReciveMsg:" + head.unMsgID + "  Size:" + head.unDataLen);

                return;
            }

            if (mhtMsgDelegation.ContainsKey(head.unMsgID))
            {
                MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[head.unMsgID];
                myDelegationHandler(head.unMsgID, new MemoryStream(bytes));
            }
            else
            {
				Debug.LogError("ReciveMsg:" + head.unMsgID + "  and no handler!!!!");
            }
        }

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		public bool RegisteredNetEventHandler(EventDelegation eventHandler)
        {
            mHandlerDelegation = eventHandler;
            return true;
        }

		public bool RegisteredDelegation(UInt16 eMsg, MsgDelegation msgDelegate)
		{
			if(!mhtMsgDelegation.ContainsKey(eMsg))
			{
				MsgDelegation myDelegationHandler = new MsgDelegation(msgDelegate);
				mhtMsgDelegation.Add(eMsg, myDelegationHandler);
			}
			else
			{
				MsgDelegation myDelegationHandler = (MsgDelegation)mhtMsgDelegation[eMsg];
				myDelegationHandler += new MsgDelegation(msgDelegate);
			}

			return true;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	
    }
}