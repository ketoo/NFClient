using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NFSDK;
using NFrame;

public class NFRecordColView : MonoBehaviour 
{
	public enum ViewType
	{
		ORIGINAL = 0,
		HERO_GUID_ICON,
		NPC_CNFID_ICON,
		ITEM_CNFID_ICON
	};

	public int col;
	public ViewType type = ViewType.ORIGINAL;

	private NFRecordRowView rowView;

	private NFIKernelModule mkernelModule;
    private NFIClassModule mClassModule;
    private NFIElementModule mElementModule;
    private NFLoginModule mLoginModule;

    // Use this for initialization
    private void Awake()
    {
        mkernelModule = NFPluginManager.Instance().FindModule<NFIKernelModule>();
        mClassModule = NFPluginManager.Instance().FindModule<NFIClassModule>();
        mLoginModule = NFPluginManager.Instance().FindModule<NFLoginModule>();
        mElementModule = NFPluginManager.Instance().FindModule<NFIElementModule>();

		int iNum = 0;
		Transform tParent = this.transform.parent;
		while(tParent)
		{
			rowView = tParent.GetComponent<NFRecordRowView> ();
			if (rowView != null)
			{
				rowView.AddColView (col, this);
				break;
			}
		
			iNum++;
			if (iNum > 3)
			{
				break;
			}

			tParent = tParent.parent;
		}
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
	public void Refresh(NFGUID self, NFDataList.TData data)
	{
		switch (type)
		{
			case ViewType.ITEM_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strIconName = mElementModule.QueryPropertyString (data.ToString(), NFrame.Item.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (data.ToString(), NFrame.Item.SpriteFile);

                        Sprite xSprite = null;// = NFTexturePacker.Instance.GetSprit(strIconFileName, strIconName);
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
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						NFIRecord xRecord = mkernelModule.FindRecord (self, NFrame.Player.PlayerHero.ThisName);
						int nRow = xRecord.FindObject ((int)NFrame.Player.PlayerHero.GUID, data.ObjectVal ());
						if (nRow >= 0)
						{
							string strCnfID = xRecord.QueryString (nRow, (int)NFrame.Player.PlayerHero.ConfigID);
							string strIconName = mElementModule.QueryPropertyString (strCnfID, NFrame.Item.Icon);
							string strIconFileName = mElementModule.QueryPropertyString (strCnfID, NFrame.Item.SpriteFile);
							
                            Sprite xSprite = null;//= NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
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
			case ViewType.NPC_CNFID_ICON:
				{

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