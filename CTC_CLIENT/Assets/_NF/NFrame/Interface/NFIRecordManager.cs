//-----------------------------------------------------------------------
// <copyright file="NFIRecordManager.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace NFrame
{
    public abstract class NFIRecordManager
    {
        public abstract NFIRecord AddRecord(string strRecordName,  int nRow, NFDataList varData);
		public abstract NFIRecord GetRecord(string strRecordName);
		public abstract NFDataList GetRecordList();
		
		public abstract void RegisterCallback(string strRecordName, NFIRecord.RecordEventHandler handler);
    }
}