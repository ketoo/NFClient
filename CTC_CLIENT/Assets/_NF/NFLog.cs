using UnityEngine;
using System.Collections;
using System.Text;
using System.Threading;
using System.IO;
using System;

public class NFLog
{
    #region Instance
    private static NFLog _Instance = null;
    public static NFLog Instance
    {
        get
        {
            if (null == _Instance)
            {
                _Instance = new NFLog();

                _Instance.FinalLog();

                _Instance.InitLog(LOG_LEVEL.DEBUG);
                _Instance.InitLog(LOG_LEVEL.INFO);
                _Instance.InitLog(LOG_LEVEL.WARING);
                _Instance.InitLog(LOG_LEVEL.ERROR);
            }
            return _Instance;
        }

    }
    #endregion

    struct FileData
    {
        public FileStream fs;
        public StreamWriter sw;
    }

    private FileData[] _FileData = new FileData[(int)LOG_LEVEL.MAX];

    public enum LOG_LEVEL
    {
        DEBUG = 0,
        INFO = 1,
        WARING = 2,
        ERROR = 3,
        MAX = 4,
    }

    void InitLog(LOG_LEVEL eLevel)
    {
        DirectoryInfo oDir = new DirectoryInfo(Path.GetFullPath("./log/"));
        if (!oDir.Exists)
        {
            oDir.Create();
        }


        int nLevel = (int)eLevel;
        _FileData[nLevel].fs = new FileStream("./log/" + DateTime.Now.Year + "_" + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + "_" + DateTime.Now.Second + "_" + eLevel.ToString() + ".log", FileMode.OpenOrCreate);
        _FileData[nLevel].sw = new StreamWriter(_FileData[nLevel].fs, Encoding.Default);
    }

    void FinalLog()
    {
        for (int i = 0; i < (int)LOG_LEVEL.MAX; ++i)
        {
            if (null != _FileData[i].sw)
            {
                _FileData[i].sw.Close();
                _FileData[i].fs.Close();
                _FileData[i].sw = null;
                _FileData[i].fs = null;
            }
        }
    }

    public void Log(LOG_LEVEL eLevel, string text)
    {
        if (false == NFStart.Instance.bLog)
        {
            return;
        }

        int nLevel = (int)eLevel;
        if (nLevel >= (int)LOG_LEVEL.MAX || nLevel < 0)
        {
            return;
        }

        FileStream fs = _FileData[nLevel].fs;
        StreamWriter sw = _FileData[nLevel].sw;

        if (null != sw)
        {
            string strData = "[" + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second + "] " + text;

            switch(eLevel)
            {
                case LOG_LEVEL.DEBUG:
                    {
                        Debug.Log(strData);
                        strData = "[DEBUG]" + strData;
                    }
                    break;
                case LOG_LEVEL.WARING:
                    {
                        Debug.LogWarning(strData);
                        strData = "[WARING]" + strData;
                    }
                    break;
                case LOG_LEVEL.ERROR:
                    {
                        Debug.LogError(strData);
                        strData = "[ERROR]" + strData;
                    }
                    break;
                default:
                    strData = "[INFO]" + strData;
                    break;
            }

            sw.WriteLine(strData);
            sw.Flush();
        }
    }
}