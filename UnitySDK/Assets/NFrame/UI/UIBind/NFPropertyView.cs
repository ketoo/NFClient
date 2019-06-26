using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NFrame;
using NFSDK;

public class NFPropertyView : MonoBehaviour {

	public enum ViewType
	{
		ORIGINAL = 0,
		HERO_GUID_ICON,
		NPC_CNFID_ICON,
		ITEM_CNFID_ICON,
		SKILL_CNFID_ICON
	};

    public string propertyName;
    public bool group = false;
	public ViewType type = ViewType.ORIGINAL;

    private NFGUID bindID = new NFGUID();
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
	}

    private void OnDestroy()
    {
        //TODO
        //unregister callback
    }

    void Start () 
	{
        bindID = group ? NFGUID.Zero : mLoginModule.mRoleID;

        //register callback
        mkernelModule.RegisterClassCallBack(Player.ThisName, OnClassPlayerEventHandler);

		//generally speaking, this object will be created after the player login (be created)
        //as a result, we must add the data when the UI object creating to show the data at the UI.

		NFIProperty xProperty = mkernelModule.FindProperty(bindID, propertyName);
		if (xProperty != null)
        {
            mkernelModule.RegisterPropertyCallback(bindID, propertyName, PropertyEventHandler);
            mkernelModule.RegisterGroupPropertyCallback(propertyName, PropertyEventHandler);

			PropertyEventHandler(bindID, propertyName, null, null);

        }
        else
        {
			Debug.LogError("there have not a property named: " + propertyName + "  " + this.transform.parent.parent.name + "/" + this.transform.parent.name + "/" + this.gameObject.name);
        }
        /*
		switch (xProperty.GetType())
		{
			case NFDataList.VARIANT_TYPE.VTYPE_INT:
				
			
		}
		*/



		if (type == ViewType.ORIGINAL)
		{
			Text xText = gameObject.GetComponent<Text> ();
			if (xText != null)
			{
				if (xText.text == "0" || xText.text.Length <= 0)
				{
					//xText.enabled = false;
				}
			}
		}


	}

	// Update is called once per frame
	void Update () 
	{

	}

	void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
	{
		{
			if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
			{
				mkernelModule.RegisterPropertyCallback (self, propertyName, PropertyEventHandler);
			}
		}
	}

	void PropertyEventHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
	{
		NFIProperty xProperty = mkernelModule.FindProperty(self, propertyName);
		NFDataList.TData data = xProperty.GetData();
		switch (type)
		{
			case ViewType.ITEM_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strIconName = mElementModule.QueryPropertyString (data.ToString(), NFrame.Item.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (data.ToString(), NFrame.Item.SpriteFile);
						
                        Sprite xSprite = null;//NFTexturePacker.Instance.GetSprit(strIconFileName, strIconName);
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
							
                            Sprite xSprite = null;//NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
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
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strCnfID = data.StringVal ();
						string strIconName = mElementModule.QueryPropertyString (strCnfID, NFrame.NPC.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (strCnfID, NFrame.NPC.SpriteFile);
						
                        Sprite xSprite = null;//NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
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
			case ViewType.SKILL_CNFID_ICON:
				{
					Image xImage = gameObject.GetComponent<Image> ();
					if (xImage != null)
					{
						string strIconName = mElementModule.QueryPropertyString (propertyName, NFrame.Skill.Icon);
						string strIconFileName = mElementModule.QueryPropertyString (propertyName, NFrame.Skill.SpriteFile);
						
                        Sprite xSprite = null;//NFTexturePacker.Instance.GetSprit (strIconFileName, strIconName);
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
			default:
				{
					Text xText = gameObject.GetComponent<Text> ();
					if (xText != null)
					{
						xText.text = data.ToString ();
						if (xText.text == "0" || xText.text.Length <= 0)
						{
							//xText.enabled = false;
						}
					}
				}
				break;
		}
	}
}