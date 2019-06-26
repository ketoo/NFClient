using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using System.Text;

namespace NFSDK
{
    public enum NFNetState
    {
        Connecting,
        Connected,
        Disconnected
    }
	
	public enum NFNetEventType
    {
        None,
        Connected,
        Disconnected,
        ConnectionRefused,
        DataReceived
    }

    public class NFSocketPacket
    {
        public byte[] bytes = null;
        public int bytesCount = 0;

        public NFSocketPacket(byte[] bytes, int bytesCount)
        {
            this.bytes = bytes;
            this.bytesCount = bytesCount;
        }

    }

    public class NFNetEventParams
    {
        public NFNetClient client = null;
        public int clientID = 0;
        public TcpClient socket = null;
        public NFNetEventType eventType = NFNetEventType.None;
        public string message = "";
        public NFSocketPacket packet = null;

    }
	
    public class NFNetClient
    {
        
		public NFNetClient(NFNetListener xNetListener)
        {
			mxNetListener = xNetListener;
            Init();
        }

        void Init()
        {

            mxState = NFNetState.Disconnected;
            mxEvents = new Queue<NFNetEventType>();
            mxMessages = new Queue<string>();
            mxPackets = new Queue<NFSocketPacket>();
        }

		private int bufferSize = 65536;

        private NFNetState mxState;
        private NetworkStream mxStream;
        private StreamWriter mxWriter;
        private StreamReader mxReader;
        private Thread mxReadThread;
        private TcpClient mxClient;
        private Queue<NFNetEventType> mxEvents;
        private Queue<string> mxMessages;
        private Queue<NFSocketPacket> mxPackets;

		private NFNetListener mxNetListener;


        public bool IsConnected()
        {
            return mxState == NFNetState.Connected;
        }

        public NFNetState GetState()
        {
            return mxState;
        }

        public NFNetListener GetNetListener()
        {
            return mxNetListener;
        }

		public void Execute()
        {
			
            while (mxEvents.Count > 0)
            {
                lock (mxEvents)
                {
                    NFNetEventType eventType = mxEvents.Dequeue();

                    NFNetEventParams eventParams = new NFNetEventParams();
                    eventParams.eventType = eventType;
                    eventParams.client = this;
                    eventParams.socket = mxClient;

                    if (eventType == NFNetEventType.Connected)
                    {
                        mxNetListener.OnClientConnect(eventParams);
                    }
                    else if (eventType == NFNetEventType.Disconnected)
                    {
						mxNetListener.OnClientDisconnect(eventParams);

                        mxReader.Close();
                        mxWriter.Close();
                        mxClient.Close();

                    }
                    else if (eventType == NFNetEventType.DataReceived)
                    {
                        lock (mxPackets)
                        {
                            eventParams.packet = mxPackets.Dequeue();
                        
                            mxNetListener.OnDataReceived(eventParams);
                        }
                    }
                    else if (eventType == NFNetEventType.ConnectionRefused)
                    {

                    }
                }
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {

                TcpClient tcpClient = (TcpClient)ar.AsyncState;
                tcpClient.EndConnect(ar);

                SetTcpClient(tcpClient);

            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NFNetEventType.ConnectionRefused);
                }
            }
        }

        private void ReadData()
        {
            bool endOfStream = false;

            while (!endOfStream)
            {
               int bytesRead = 0;
               byte[] bytes = new byte[bufferSize];
               try
               {
                   bytesRead = mxStream.Read(bytes, 0, bufferSize);
               }
               catch (Exception e)
               {
                   e.ToString();
               }

               if (bytesRead == 0)
               {

                   endOfStream = true;

               }
               else
               {
                   lock (mxEvents)
                   {

                       mxEvents.Enqueue(NFNetEventType.DataReceived);
                   }
                   lock (mxPackets)
                   {
                       mxPackets.Enqueue(new NFSocketPacket(bytes, bytesRead));
                   }

               }
            }

            mxState = NFNetState.Disconnected;

            mxClient.Close();
            lock (mxEvents)
            {
                mxEvents.Enqueue(NFNetEventType.Disconnected);
            }

        }

        // Public
        public void Connect(string hostname, int port)
        {
            if (mxState == NFNetState.Connected)
            {
                return;
            }

            mxState = NFNetState.Connecting;

            mxMessages.Clear();
            mxEvents.Clear();

            mxClient = new TcpClient();

            mxClient.BeginConnect(hostname,
                                 port,
                                 new AsyncCallback(ConnectCallback),
                                 mxClient);

        }

        public void Disconnect()
        {
            mxState = NFNetState.Disconnected;

            try { if (mxReader != null) mxReader.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxWriter != null) mxWriter.Close(); }
            catch (Exception e) { e.ToString(); }
            try { if (mxClient != null) mxClient.Close(); }
            catch (Exception e) { e.ToString(); }

        }

        public void SendBytes(byte[] bytes)
        {
            SendBytes(bytes, 0, bytes.Length);
        }

        private void SendBytes(byte[] bytes, int offset, int size)
        {

            if (!IsConnected())
                return;
            try
            {
                mxStream.Write(bytes, offset, size);
                mxStream.Flush();
            }
            catch (Exception e)
            {
                lock (mxEvents)
                {
                    mxEvents.Enqueue(NFNetEventType.Disconnected);
                    Disconnect();
                }
            }

        }

        private void SetTcpClient(TcpClient tcpClient)
        {

            mxClient = tcpClient;

            if (mxClient.Connected)
            {

                mxStream = mxClient.GetStream();
                mxReader = new StreamReader(mxStream);
                mxWriter = new StreamWriter(mxStream);

                mxState = NFNetState.Connected;

                mxEvents.Enqueue(NFNetEventType.Connected);

                mxReadThread = new Thread(ReadData);
                mxReadThread.IsBackground = true;
                mxReadThread.Start();
            }
            else
            {
                mxState = NFNetState.Disconnected;
            }
        }
    }
}