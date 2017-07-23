using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Collections;
using System.IO;
using ProtoBuf;

namespace NFrame
{
    public class NFCUplaoaModule : NFIUploadModule
    {
        public override void Init()
        {
			NFCKernelModule.Instance.RegisterClassCallBack(NFrame.Player.ThisName, OnClassHandler);
        }

        public override void AfterInit()
        {
        }

        public override void BeforeShut()
        {
        }

        public override void Shut()
        {
        }


        public override void Execute()
        {

        }

        public void OnClassHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
        {
            if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
            {
                NFIObject xObject = NFCKernelModule.Instance.GetObject(self);

                NFIPropertyManager xPropertyManager = xObject.GetPropertyManager();
                NFDataList xPropertyNameList = xPropertyManager.GetPropertyList();
                for (int i = 0; i < xPropertyNameList.Count(); i++)
                {
                    string strPropertyName = xPropertyNameList.StringVal(i);
                    NFIProperty xProperty = xPropertyManager.GetProperty(strPropertyName);
                    if (xProperty.GetUpload())
                    {
                        xProperty.RegisterCallback(OnPropertyHandler);
                    }
                }

                NFIRecordManager xRecordManager = xObject.GetRecordManager();
                NFDataList xRecordNameList = xRecordManager.GetRecordList();
                for (int i = 0; i < xRecordNameList.Count(); i++)
                {
                    string strRecodeName = xRecordNameList.StringVal(i);
                    NFIRecord xRecord = xRecordManager.GetRecord(strRecodeName);
                    if(xRecord.GetUpload())
                    {
                        xRecord.RegisterCallback(OnRecordHandler);
                    }
                }
            }
        }

        public void OnPropertyHandler(NFGUID self, string strPropertyName, NFDataList.TData oldVar, NFDataList.TData newVar)
        {
            NFIObject xObject = NFCKernelModule.Instance.GetObject(self);
            NFIProperty xProperty = xObject.GetPropertyManager().GetProperty(strPropertyName);
            if (!xProperty.GetUpload())
            {
                return;
            }

            switch (oldVar.GetType())
            {
                case NFDataList.VARIANT_TYPE.VTYPE_INT:
                    {
                        NFNetController.Instance.mxNetSender.RequirePropertyInt(self, strPropertyName, newVar);
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
                    {
                        NFNetController.Instance.mxNetSender.RequirePropertyFloat(self, strPropertyName, newVar);
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                    {
                        NFNetController.Instance.mxNetSender.RequirePropertyString(self, strPropertyName, newVar);
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
                    {
                        NFNetController.Instance.mxNetSender.RequirePropertyObject(self, strPropertyName, newVar);
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
                    {
                        NFNetController.Instance.mxNetSender.RequirePropertyVector2(self, strPropertyName, newVar);
                    }
                    break;
                case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
                    {
                        NFNetController.Instance.mxNetSender.RequirePropertyVector3(self, strPropertyName, newVar);
                    }
                    break;
            }

        }

        public void OnRecordHandler(NFGUID self, string strRecordName, NFIRecord.eRecordOptype eType, int nRow, int nCol, NFDataList.TData oldVar, NFDataList.TData newVar)
        {
            NFIObject xObject = NFCKernelModule.Instance.GetObject(self);
            NFIRecord xRecord = xObject.GetRecordManager().GetRecord(strRecordName);
            if (!xRecord.GetUpload())
            {
                return;
            }

            switch(eType)
            {
                case NFIRecord.eRecordOptype.Add:
                    {
                        NFNetController.Instance.mxNetSender.RequireAddRow(self, strRecordName, nRow);
                    }
                    break;
                case NFIRecord.eRecordOptype.Del:
                    {
                        NFNetController.Instance.mxNetSender.RequireRemoveRow(self, strRecordName, nRow);
                    }
                    break;
                case NFIRecord.eRecordOptype.Swap:
                    {
                        NFNetController.Instance.mxNetSender.RequireSwapRow(self, strRecordName, nRow, nCol);
                    }
                    break;
                case NFIRecord.eRecordOptype.Update:
                    {
                        switch(oldVar.GetType())
                        {
                            case NFDataList.VARIANT_TYPE.VTYPE_INT:
                                {
                                    NFNetController.Instance.mxNetSender.RequireRecordInt(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_FLOAT:
                                {
                                    NFNetController.Instance.mxNetSender.RequireRecordFloat(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_STRING:
                                {
                                    NFNetController.Instance.mxNetSender.RequireRecordString(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_OBJECT:
                                {
                                    NFNetController.Instance.mxNetSender.RequireRecordObject(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_VECTOR2:
                                {
                                    NFNetController.Instance.mxNetSender.RequireRecordVector2(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                            case NFDataList.VARIANT_TYPE.VTYPE_VECTOR3:
                                {
                                    NFNetController.Instance.mxNetSender.RequireRecordVector3(self, strRecordName, nRow, nCol, newVar);
                                }
                                break;
                        }
                    }
                    break;
            }
        }
    }
}