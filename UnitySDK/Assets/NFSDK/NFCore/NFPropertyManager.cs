//-----------------------------------------------------------------------
// <copyright file="NFCPropertyManager.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFrame>
// </copyright>
//-----------------------------------------------------------------------
using System.Collections;

namespace NFSDK
{
	public class NFPropertyManager : NFIPropertyManager
	{
		public NFPropertyManager(NFGUID self)
		{
			mSelf = self;
			mhtProperty = new Hashtable();
		}

        public override NFIProperty AddProperty(string strPropertyName, NFDataList.TData varData)
        {
            NFIProperty xProperty = null;
            if (!mhtProperty.ContainsKey(strPropertyName))
            {
                xProperty = new NFProperty(mSelf, strPropertyName, varData);
                mhtProperty[strPropertyName] = xProperty;
            }

            return xProperty;
        }

		public override bool SetProperty(string strPropertyName, NFDataList.TData varData)
		{
			if (mhtProperty.ContainsKey(strPropertyName))
			{
				NFIProperty xProperty = (NFProperty)mhtProperty[strPropertyName];
				if (null != xProperty)
				{
					xProperty.SetData(varData);
				} 
			}
			return true;
		}

		public override NFIProperty GetProperty(string strPropertyName)
		{
			NFIProperty xProperty = null;
			if (mhtProperty.ContainsKey(strPropertyName))
			{
				xProperty = (NFProperty)mhtProperty[strPropertyName];
				return xProperty;
			}

			return xProperty;
		}

		public override void RegisterCallback(string strPropertyName, NFIProperty.PropertyEventHandler handler)
		{
			if (mhtProperty.ContainsKey(strPropertyName))
			{
				NFIProperty xProperty = (NFProperty)mhtProperty[strPropertyName];
				xProperty.RegisterCallback(handler);
			}
		}
		
		public override NFDataList GetPropertyList()
		{
			NFDataList varData = new NFDataList();
			foreach( DictionaryEntry de in mhtProperty) 
			{
				varData.AddString(de.Key.ToString());				
			}
			
			return varData;
		}
		
		NFGUID mSelf;
		Hashtable mhtProperty;
	}
}