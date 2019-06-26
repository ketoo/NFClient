using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading;
using System.IO;
using System;
using NFSDK;
using System.Collections.Generic;

namespace NFrame
{
	public class NFLogModule : NFIModule
	{
		public NFLogModule(NFIPluginManager pluginManager)
		{
			mPluginManager = pluginManager;
		}
        
		private class LogData
		{
			public LogData(LogType logType, string logData)
			{
				type = logType;
				data = logData;

			}
			public bool stack;
			public LogType type;
			public string data;
		}

		private List<LogData> mLines = new List<LogData>();
		private List<string> mWriteTxt = new List<string>();

		struct FileData
		{
			public FileStream fs;
			public StreamWriter sw;
		}

		private FileData _FileData = new FileData();

		void HandleLog(string logString, string stackTrace, LogType type)
        {
			if (type == LogType.Warning)
			{
				return;
			}

            mWriteTxt.Add(logString);
            //if (type == LogType.Error || type == LogType.Exception)
            {
				string data = logString + "\n" + stackTrace;
				Log(type, data);
				//Log(type, stackTrace, true);
            }
        }

        public override void Awake()
        {
        }

        public override void Init()
        {
            String outpath = null;
            if (RuntimePlatform.Android == Application.platform
                || RuntimePlatform.IPhonePlayer == Application.platform)
            {
                outpath = Application.persistentDataPath + "/" + DateTime.Now.Year + "_" + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".log";
            }
            else
            {
                outpath = "./log/" + DateTime.Now.Year + "_" + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + "_" + DateTime.Now.Second + ".log";
            }

            //Debug.unityLogger.logHandler
            //Application.logMessageReceived += HandleLog;

            _FileData.fs = new FileStream(outpath, FileMode.OpenOrCreate);
			_FileData.sw = new StreamWriter(_FileData.fs, Encoding.UTF8);

			Debug.Log(outpath);
            
		}

		public override void AfterInit()
        {
        }

        public override void Execute()
        {
			if (mWriteTxt.Count > 0)
            {
				foreach (string t in mWriteTxt)
                {
					_FileData.sw.WriteLine(t);
                }
                
				_FileData.sw.Flush();
                
				mWriteTxt.Clear();
            }
        }

        public override void BeforeShut()
        {
			_FileData.sw.Close();
			_FileData.fs.Close();
        }

        public override void Shut()
        {
        }

		bool bShowLog = false;
        private Vector2 scrollPositionSecond = Vector2.zero;
		public void PrintGUILog()
        {
			if (GUI.Button(new Rect(Screen.width - 40, 0, 40, 20), "Log"))
			{
				bShowLog = !bShowLog;
			}

			if (bShowLog)
			{
				scrollPositionSecond = GUI.BeginScrollView(new Rect(0, 0, Screen.width, Screen.height), scrollPositionSecond, new Rect(0, 0, Screen.width - 50, Screen.height*5));

				int nPosition = 0;
				for (int i = mLines.Count - 1; i > 0; --i)
                {
					if (mLines[i].type == LogType.Log)
					{
						GUI.color = Color.white;
					}
					else
					{
						GUI.color = Color.red;
					}
                    
					int nHeight = 100;
					nPosition += nHeight;
					GUI.Label(new Rect(0, nPosition, Screen.width, nHeight), mLines[i].data);
                }

                GUI.EndScrollView();
			}
        }

		private void Log(LogType logType, string log)
        {
			string text = logType.ToString() + " " + log;
           
            if (Application.isPlaying)
            {
                if (mLines.Count > 50)
                {
                    mLines.RemoveAt(0);
                }

				mLines.Add(new LogData(logType, text));
            }
        }
	}
}