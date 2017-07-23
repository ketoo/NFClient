using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NFrame;

public class NFPropertyView : MonoBehaviour {

	public enum ViewType
	{
		ORIGINAL = 0,
		HERO_GUID_ICON,
		NPC_CNFID_ICON,
		ITEM_CNFID_ICON
	};

	public string propertyName;
	public ViewType type = ViewType.ORIGINAL;

	// Use this for initialization
	void Start () {
		//register callback
		NFrame.NFCKernelModule.Instance.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
	{
		if (NFNetController.Instance.xMainRoleID == self)
		{
			if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
			{
				NFrame.NFCKernelModule.Instance.RegisterPropertyCallback (self, propertyName, PropertyEventHandler);
			}
		}

	}

	void PropertyEventHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
	{
		NFDataList.TData data = newVar;

		switch (type)
		{
			case ViewType.ITEM_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strIconName = NFrame.NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (data.ToString(), NFrame.Item.Icon);
						string strIconFileName = NFrame.NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (data.ToString(), NFrame.Item.SpriteFile);
						Sprite xSprite = null;//load your sprite
						if (xSprite != null)
						{
							xImage.overrideSprite = xSprite;
							if (xImage.sprite == null) 
							{
								xImage.enabled = false;
							}
							else 
							{
								xImage.enabled = true;
							}
						}
					}
				}
				break;
			case ViewType.HERO_GUID_ICON:
			case ViewType.NPC_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						NFIRecord xRecord = NFCKernelModule.Instance.FindRecord (self, NFrame.Player.PlayerHero.ThisName);
						int nRow = xRecord.FindObject ((int)NFrame.Player.PlayerHero.GUID, data.ObjectVal ());
						if (nRow >= 0)
						{
							string strCnfID = xRecord.QueryString (nRow, (int)NFrame.Player.PlayerHero.ConfigID);
							string strIconName = NFrame.NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (strCnfID, NFrame.Item.Icon);
							string strIconFileName = NFrame.NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (strCnfID, NFrame.Item.SpriteFile);
							Sprite xSprite = null;//load your sprite
							if (xSprite != null)
							{
								xImage.overrideSprite = xSprite;
								if (xImage.sprite == null)
								{
									xImage.enabled = false;
								}
								else
								{
									xImage.enabled = true;
								}
							}
						}
						else
						{
							xImage.enabled = false;
						}
					}
				}
				break;
			default:
				{
					Text xText = gameObject.GetComponent<Text> ();
					if (xText != null)
					{
						xText.text = data.ToString ();
					}
				}
				break;
		}
	}
}
