using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NFrame;

public class NFCWindowSkillEvent : NFIControl
{
	public enum WSE_CONTROL_ENUM
	{
		
		WSE_MAIN_SKILL = 1,
		WSE_SKILL1 = 2,
		WSE_SKILL2 = 3,

	}

	//////////all the windows logic class must need the next three menber elements
	public WSE_CONTROL_ENUM eControlType;

	private static Hashtable mhtWindow = new Hashtable ();

	/////////////////////////////////////////////
	void Awake ()
	{
		mhtWindow [eControlType] = this.gameObject;
		NFCWindowManager.Instance.AddWindowControl (NFCWindows.UI_WIN_STATE.UI_WIN_ROLE, (int)eControlType, this.gameObject);
	}

	void Start ()
	{
		if (WSE_CONTROL_ENUM.WSE_MAIN_SKILL == eControlType
		   || WSE_CONTROL_ENUM.WSE_SKILL1 == eControlType
		   || WSE_CONTROL_ENUM.WSE_SKILL2 == eControlType)
		{
			Button btn = this.gameObject.GetComponent<Button> ();
			btn.onClick.AddListener (delegate ()
			{
				this.OnClick (this.gameObject);
			});
		}

		UpdateControl ();
	}

	private void OnWindowStateChanged (NFCWindows.UI_WIN_STATE eState, bool bNowActive, int nActiveCount)
	{
		Debug.Log ("Win: " + eState + " Active: " + bNowActive + " Count: " + nActiveCount);
	}



	void OnClick (GameObject go)
	{
		switch (eControlType)
		{
			case WSE_CONTROL_ENUM.WSE_MAIN_SKILL:
				{
					NFGUID xMainID = NFNetController.Instance.xMainRoleID;
					GameObject xgo = NFRender.Instance.GetObject (xMainID);
					if (null != xgo)
					{
						string strSkillID = NFCKernelModule.Instance.QueryPropertyString (xMainID, NFrame.Player.Skill1);
					
						string strSkillStateList = NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (strSkillID, NFrame.Skill.AnimaState);
						string[] strSkillList = strSkillStateList.Split (',');
						int nRandom = NFCKernelModule.Instance.Random (0, strSkillList.Length);
						string strAnimation = strSkillList [nRandom];
						/*
						HeroAnima xHeroAnima = xgo.GetComponent<HeroAnima> ();

						HeroAnima.AnimaState xAnimaState = xHeroAnima.GetState (strAnimation);

						xHeroAnima.SetState (xAnimaState);
						*/
					}
				}
				break;
			case WSE_CONTROL_ENUM.WSE_SKILL1:
				{
					NFGUID xMainID = NFNetController.Instance.xMainRoleID;
					GameObject xgo = NFRender.Instance.GetObject (xMainID);
					if (null != xgo)
					{
						string strSkillID = NFCKernelModule.Instance.QueryPropertyString (xMainID, NFrame.Player.Skill2);

						string strSkillStateList = NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (strSkillID, NFrame.Skill.AnimaState);
						string[] strSkillList = strSkillStateList.Split (',');
						int nRandom = NFCKernelModule.Instance.Random (0, strSkillList.Length);
						string strAnimation = strSkillList [nRandom];
						/*
						HeroAnima xHeroAnima = xgo.GetComponent<HeroAnima> ();

						HeroAnima.AnimaState xAnimaState = xHeroAnima.GetState (strAnimation);

						xHeroAnima.SetState (xAnimaState);
						*/
					}
				}
				break;
			case WSE_CONTROL_ENUM.WSE_SKILL2:
				{
					NFGUID xMainID = NFNetController.Instance.xMainRoleID;
					GameObject xgo = NFRender.Instance.GetObject (xMainID);
					if (null != xgo)
					{
						string strSkillID = NFCKernelModule.Instance.QueryPropertyString (xMainID, NFrame.Player.Skill3);

						string strSkillStateList = NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (strSkillID, NFrame.Skill.AnimaState);
						string[] strSkillList = strSkillStateList.Split (',');
						int nRandom = NFCKernelModule.Instance.Random (0, strSkillList.Length);
						string strAnimation = strSkillList [nRandom];
						/*
						HeroAnima xHeroAnima = xgo.GetComponent<HeroAnima> ();

						HeroAnima.AnimaState xAnimaState = xHeroAnima.GetState (strAnimation);

						xHeroAnima.SetState (xAnimaState);
						*/
					}
				}
				break;
			default:
				break;
		}
	}
}