using UnityEngine;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System;
using NFrame;
using System.Collections.Generic;

public class NFRender
{
    #region Instance
    private static NFRender _Instance = null;
    private static readonly object _syncLock = new object();
    public static NFRender Instance
    {
        get
        {
            lock (_syncLock)
            {
                if (_Instance == null)
                {
                    _Instance = new NFRender();
                    _Instance.Init();
                }
                return _Instance;
            }
        }
    }
    #endregion

    public void Init()
    {
        mnScene = 0;
		NFCKernelModule.Instance.RegisterClassCallBack(NFrame.Player.ThisName, OnClassPlayerEventHandler);
		NFCKernelModule.Instance.RegisterClassCallBack(NFrame.NPC.ThisName, OnClassNPCEventHandler);
    }

	public void InitPlayerComponent(NFrame.NFGUID xID, GameObject self, bool bMainRole)
    {
        if (null == self)
        {
            return;
        }

        

        BodyIdent xBodyIdent = self.GetComponent<BodyIdent>();
        if (null != xBodyIdent)
        {//不能没有
            xBodyIdent.enabled = true;
            xBodyIdent.eType = BodyIdent.BodyType.NFRoot;
			xBodyIdent.SetObjectID(xID);
        }
        else
        {
            Debug.LogError("No 'BodyIdent' component in " + self.tag);
        }

    }

    private void OnNPCHPHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
    {
        if (newVar.IntVal() <= 0)
        {
            GameObject xNPC = GetObject(self);
            if (null != xNPC)
            {
                //HeroAnima xHeroAnima = xNPC.GetComponent<HeroAnima>();
                //xHeroAnima.DeathTrigger();
                //xHeroAnima.SetState(HeroAnima.AnimaState.DEAD, true);
            }
        }
    }



    private void OnClassPlayerEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
    {
		if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
		{
		}
		else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA)
		{
		}
        else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
        {
            DestroyObject(self);
        }
		else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
		{

			NFGUID xHeroID = NFCKernelModule.Instance.QueryPropertyObject (self, NFrame.Player.FightHero);
			NFDataList.TData data = new NFDataList.TData ();
			data.Set (xHeroID);
			OnHeroChangeHandler (self, NFrame.Player.FightHero, data, data );


			NFCKernelModule.Instance.RegisterPropertyCallback(self, NFrame.Player.FightHero, OnHeroChangeHandler);
		}
    }

    private void OnClassNPCEventHandler(NFGUID self, int nContainerID, int nGroupID, NFIObject.CLASS_EVENT_TYPE eType, string strClassName, string strConfigIndex)
    {
        if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE)
        {
			string strConfigID = NFCKernelModule.Instance.QueryPropertyString(self, NFrame.NPC.ConfigID);
			NFVector3 vec3 = NFCKernelModule.Instance.QueryPropertyVector3 (self, NFrame.NPC.Position);

			Vector3 vec = new Vector3();
			vec.x = vec3.X ();
			vec.y = vec3.Y ();
			vec.z = vec3.Z ();

            string strPrefabPath = "";
            if (strConfigID.Length <= 0)
            {
				strPrefabPath = NFrame.NFCKernelModule.Instance.GetElementModule().QueryPropertyString("Enemy", NPC.Prefab);
            }
            else
            {
				strPrefabPath = NFrame.NFCKernelModule.Instance.GetElementModule().QueryPropertyString(strConfigID, NPC.Prefab);
            }

            GameObject xNPC = CreateObject(self, strPrefabPath, vec, strClassName);
			xNPC.name = strConfigIndex;
			xNPC.transform.Rotate(new Vector3(0,90,0));

            InitPlayerComponent(self, xNPC, false);
            
			NFCKernelModule.Instance.RegisterPropertyCallback(self, NPC.HP, OnNPCHPHandler);
		}
		else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_LOADDATA)
		{

		}
        else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_DESTROY)
        {
            DestroyObject(self);
        }
		else if (eType == NFIObject.CLASS_EVENT_TYPE.OBJECT_CREATE_FINISH)
		{
			//NFCKernelModule.Instance.RegisterPropertyCallback(self, NFrame.Player.PrefabPath, OnClassPrefabEventHandler);
		}
    }

	private Vector3 GetRenderObjectPosition(NFGUID self)
	{
		if (mhtObject.ContainsKey(self))
		{
			GameObject xGameObject = (GameObject)mhtObject[self];
			return xGameObject.transform.position;
		}

		return Vector3.zero;
	}

	private void OnHeroChangeHandler(NFGUID self, string strProperty, NFDataList.TData oldVar, NFDataList.TData newVar)
	{
		Vector3 vec = new Vector3();
		vec = GetRenderObjectPosition (self);

		DestroyObject(self);

		if (vec.Equals(Vector3.zero))
		{
			NFVector3 vec3 = NFCKernelModule.Instance.QueryPropertyVector3 (self, NPC.Position);
			vec.x = vec3.X ();
			vec.y = vec3.Y ();
			vec.z = vec3.Z ();	
		}

		string strPrefabPath = "";

		NFGUID xHeroID = NFCKernelModule.Instance.QueryPropertyObject (self, NFrame.Player.FightHero);
		NFIRecord xRecord = NFCKernelModule.Instance.FindRecord (self, NFrame.Player.PlayerHero.ThisName);
		int nRow = xRecord.FindObject ((int)NFrame.Player.PlayerHero.GUID, xHeroID);
		if (nRow >= 0)
		{
			String strCnfID = xRecord.QueryString (nRow, (int)NFrame.Player.PlayerHero.ConfigID);
			strPrefabPath = NFrame.NFCKernelModule.Instance.GetElementModule ().QueryPropertyString (strCnfID, NPC.Prefab);
		}

		GameObject xPlayer = CreateObject (self, strPrefabPath, vec, NFrame.Player.ThisName);
		xPlayer.name = NFrame.Player.ThisName;
		xPlayer.transform.Rotate (new Vector3 (0, 90, 0));

		if (self == NFNetController.Instance.xMainRoleID)
		{
			InitPlayerComponent (self, xPlayer, true);
		}
		else
		{
			InitPlayerComponent (self, xPlayer, false);
		}

		Debug.Log ("Create Object " + NFrame.Player.ThisName + " " + vec.ToString () + " " + self.ToString ());

	}
    public bool IsMainRole(GameObject xGameObject)
    {
        if (null == xGameObject)
        {
            return false;
        }

        if (null == NFStart.Instance || null == NFNetController.Instance)
        {
            return true;
        }

        if (GetObject(NFNetController.Instance.xMainRoleID) == xGameObject)
        {
            return true;
        }

        return false;
    }


    public GameObject CreateObject(NFrame.NFGUID ident, string strPrefabName, Vector3 vec, string strTag)
    {
        if (!mhtObject.ContainsKey(ident))
        {
            try
            {
                GameObject xGameObject = GameObject.Instantiate(Resources.Load(strPrefabName)) as GameObject;

                mhtObject.Add(ident, xGameObject);
                GameObject.DontDestroyOnLoad(xGameObject);

                xGameObject.transform.position = vec;

                return xGameObject;
            }
            catch
            {
                Debug.LogError("Load Prefab Failed " + ident.ToString() + " " + strPrefabName);
            }
            
        }

        return null;
    }

    public bool DestroyObject(NFrame.NFGUID ident)
    {
        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
            mhtObject.Remove(ident);

            GameObject.DestroyObject(xGameObject);
            return true;
        }

        return false;
    }

    public GameObject GetObject(NFrame.NFGUID ident)
    {
        if (mhtObject.ContainsKey(ident))
        {
            return (GameObject)mhtObject[ident];
        }

        return null;
    }

    public bool AttackObject(NFrame.NFGUID ident, Hashtable beAttackInfo, string strStateName, Hashtable resultInfo)
    {
        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
           
        }

        return false;
    }

    public bool MoveTo(NFrame.NFGUID ident, Vector3 vPos, Vector3 vTar, float fSpeed, bool bRun)
    {
        if (fSpeed <= 0.01f)
        {
            return false;
        }

        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
           
        }

        return false;
    }

    public bool MoveTo(NFrame.NFGUID ident, Vector3 vTar, float fSpeed, bool bRun)
    {
        if (fSpeed <= 0.01f)
        {
            return false;
        }

        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
           
        }

        return false;
    }

    public bool MoveImmune(NFrame.NFGUID ident, Vector3 vPos, float fTime, bool bFaceToPos)
    {
        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
            
        }

        return false;
    }

    public bool MoveImmuneBySpeed(NFrame.NFGUID ident, Vector3 vPos, float fSpeed, bool bFaceToPos)
    {
        if (fSpeed <= 0.01f)
        {
            return false;
        }

        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
            float fDis = Vector3.Distance(xGameObject.transform.position, vPos);
            float fTime = fDis / fSpeed;
            MoveImmune(ident, vPos, fTime, bFaceToPos);
        }

        return false;
    }

    public void PausePlayAnimation(bool bPause)
    {
        //         foreach (AnimationState animState in character.animation)
        //         {
        //             if (animState.speed != 0.005f) myMoveSetScript.animSpeedStorage[i] = animState.speed;
        //             animState.speed = 0.005f;
        //             i++;
        //         }
    }

    public void SnakeCam()
    {

    }

    public int GetCurSceneID()
    {
        return mnScene;
    }

    public void SetMainRoleAgentState(bool bActive)
    {
		NFNetController xNet = NFNetController.Instance;
        if(null == xNet)
        {
            return;
        }

        if (!mhtObject.ContainsKey(xNet.xMainRoleID))
        {
            return;
        }

        GameObject xGameObject = (GameObject)mhtObject[xNet.xMainRoleID];
        if (null == xGameObject)
        {
            return;
        }

    }

	public void LoadScene(int nSceneID)
	{
		mbLoadedScene = true;
		mnScene = nSceneID;

		//NFLoadingMgr.Instance.SendMessage("LoadLevel", mnScene, null);
		NFLoadingMgr.Instance.LoadLevel (mnScene, null);
	}

	public void LoadScene(int nSceneID, float fX, float fY, float fZ, NFMsg.AckMiningTitle strData)
    {
        mbLoadedScene = true;
        mnScene = nSceneID;
        mvSceneBornPos.x = fX;
        mvSceneBornPos.y = fY;
        mvSceneBornPos.z = fZ;

		//NFLoadingMgr.Instance.SendMessage("LoadLevel", mnScene, (object)strData);
		NFLoadingMgr.Instance.LoadLevel (mnScene, strData);

        if (null == NFNetController.Instance || !mhtObject.ContainsKey(NFNetController.Instance.xMainRoleID))
        {
            return;
        }

        //主角贴地，出生点
        GameObject xGameObject = (GameObject)mhtObject[NFNetController.Instance.xMainRoleID];
        if (null != xGameObject)
        {
            xGameObject.transform.position = mvSceneBornPos;
        }
    }

    public void LoadSceneEnd(int nSceneID)
    {
        if(false == mbLoadedScene)
        {
            return;
        }

        mbLoadedScene = false;

        if (mhtObject.ContainsKey(NFNetController.Instance.xMainRoleID))
        {
            GameObject xGameObject = (GameObject)mhtObject[NFNetController.Instance.xMainRoleID];
            if (null != xGameObject)
            {
                //xGameObject.transform.position = mvSceneBornPos;
            }
        }

        //在城镇 
        //         if (1 == nSceneID)
        //         {
        //             NFCWindowManager.Instance.SetGameState(NFCWindows.UI_WIN_STATE.UI_WIN_CITY_TOP);
        //         }
        //         else if (5 == nSceneID)
        //         {
        //             NFCWindowManager.Instance.SetGameState(NFCWindows.UI_WIN_STATE.UI_WIN_FIGHT_TOP);
        //         }
    }

    public void SwapEquip(NFrame.NFGUID ident, string strOldEquipIndex, string strNewEquipIndex)
    {
        //         if (mhtObject.ContainsKey(ident))
        //         {
        //             GameObject xGameObject = (GameObject)mhtObject[ident];
        //             HeroDress dressModule = xGameObject.GetComponent<HeroDress>();
        //             NFCEquipResource.EquipInfo newInfo = NFCEquipResource.Instance.GetConfig(strNewEquipIndex);
        //             NFCEquipResource.EquipInfo oldInfo = NFCEquipResource.Instance.GetConfig(strOldEquipIndex);
        // 			
        // 			if(null == newInfo && null == oldInfo)
        // 			{
        // 				return;
        // 			}
        // 			
        // 			NFrame.NFGUID mainIdent = new NFrame.NFGUID(NFCWindowMainTopLogicEvent.GetMainRoleIdent());
        // 			int nSex = NFrame.NFCKernel.Instance.QueryPropertyInt(mainIdent, "Sex");
        // 			int nJob = NFrame.NFCKernel.Instance.QueryPropertyInt(mainIdent, "Job");
        // 				
        //             if(null == newInfo)
        // 			{
        // 				newInfo = NFCEquipResource.Instance.GetConfig((NFObject_def.NFSexType)nSex, (NFObject_def.NFJobType)nJob, 0, (NFItem_def.EGameItemType)oldInfo.nItemType);
        // 			}
        // 				//get job deflat equip
        // 				//job sex equipSubType camp race
        // 			if(null == oldInfo)
        // 			{
        // 				oldInfo = NFCEquipResource.Instance.GetConfig((NFObject_def.NFSexType)nSex, (NFObject_def.NFJobType)nJob, 0, (NFItem_def.EGameItemType)newInfo.nItemType);
        // 			}
        // 			
        // 			if(newInfo.nItemType < (int)NFItem_def.EGameItemType.EGIT_WEAPON_END)
        // 			{
        // 				dressModule.RemoveBindPart("Bip01 R Hand");
        // 				dressModule.BindPart("Bip01 R Hand", newInfo.strPartPrefab, Vector3.zero, Quaternion.identity);
        // 			}
        // 			else
        // 			{
        // 				dressModule.ChangePart(oldInfo.strPartPrefab, newInfo.strPartPrefab, newInfo.strPrefabPath);
        // 			}
        //         }
    }


    public void SetVisibleAll(bool bVisible)
    {
        foreach (KeyValuePair<NFGUID, GameObject> kv in mhtObject)
        {
            GameObject go = (GameObject)kv.Value;
            go.SetActive(bVisible);
        }
    }

    public void SetVisible(NFrame.NFGUID ident, bool bVisible)
    {
        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];
            xGameObject.SetActive(bVisible);
        }
    }

    public void DetroyAll()
    {
        foreach (KeyValuePair<NFGUID, GameObject> kv in mhtObject)
        {
            GameObject go = (GameObject)kv.Value;
            GameObject.Destroy(go);
        }

        mhtObject.Clear();
    }

    public void UseItem(NFrame.NFGUID ident, string strItemConfigID)
    {
        if (mhtObject.ContainsKey(ident))
        {
            GameObject xGameObject = (GameObject)mhtObject[ident];

        }
    }

    public void UseSkill(NFrame.NFGUID ident, string strSkillConfigID, int nIndex, float fNowX, float xNowZ, float fTarX, float fTarZ, List<NFMsg.EffectData> xEffDataList, bool bClient)
    {
        if (mhtObject.ContainsKey(ident))
        {
            if (bClient)
            {
                //执行状态
                //发送数据

                GameObject xGameObject = (GameObject)mhtObject[ident];
                if (null != xGameObject)
                {
                        //如果位置不一致，那么就闪烁(奔跑)前进，然后释放技能
                        //而且先保存伤害对象和伤害值，动作帧触发的时候调阅闪出<角色ID,动作,<被打角色ID，伤害值>>
                        ///////////////////////////////////////////////
                        //为某个技能确认需要打击的人以及信息，每次hit一个后删除此hit的一个，技能状态完成时全删除
                        //xSkillHitData.UseSkill(strSkillConfigID, objectList, damageList, resultList);

                        //////////////////////////////////////////////////
                        string strConfigID = NFrame.NFCKernelModule.Instance.QueryPropertyString(ident, "ConfigID");
                        string strSkillRef = NFrame.NFCKernelModule.Instance.GetElementModule().QueryPropertyString(strConfigID, "SkillIDRef");
                        string strSkill = NFrame.NFCKernelModule.Instance.GetElementModule().QueryPropertyString(strConfigID, strSkillConfigID);

                        NFIElement xSkillElement = NFrame.NFCKernelModule.Instance.GetElementModule().GetElement(strSkillConfigID);
                        if (null != xSkillElement)
                        {
                            
                        }
                }
            }
            else
            {
                //找伤害索引
                //插入到帧


            }
        }
    }


    public void PlaySound(string strSoundName, Vector3 vecPos)
    {
        //time to live
        if (strSoundName.Length > 0)
        {
            GameObject xGO = new GameObject();
            AudioClip source = GameObject.Instantiate(Resources.Load(strSoundName)) as AudioClip;
            xGO.AddComponent<AudioSource>();
            xGO.GetComponent<AudioSource>().clip = source;
            xGO.GetComponent<AudioSource>().volume = 1.0f;
            xGO.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Custom;
            xGO.GetComponent<AudioSource>().maxDistance = 100.0f;
            xGO.GetComponent<AudioSource>().Play();
            GameObject.Destroy(xGO, 10.0f);
        }
    }

    public void PlayParticle(string strEffect, Vector3 vecPos, Vector3 vOrien)
    {
        //Particle system
        if (strEffect.Length > 0)
        {
            GameObject xGO = GameObject.Instantiate(Resources.Load(strEffect)) as GameObject;
            xGO.transform.position = vecPos;
            xGO.transform.LookAt(vOrien);
            xGO.GetComponent<ParticleSystem>().Play();
            GameObject.Destroy(xGO, 10.0f);
        }
    }
    public void AckStartActivity(NFGUID xIdent, NFMsg.ReqAckJoinActivity.EGameActivityType eType, NFMsg.ReqAckJoinActivity.EGameActivitySubType eSubType)
    {
        if (mhtObject.ContainsKey(xIdent))
        {
            //GameObject xGO = (GameObject)mhtObject[targetIdent];
            if (NFMsg.ReqAckJoinActivity.EGameActivityType.EGAT_PVP == eType)
            {
                NFCWindowManager.Instance.SetGameWindows(NFCWindows.UI_WIN_STATE.UI_WIN_NONE);
            }
 
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////
    Dictionary<NFGUID, GameObject> mhtObject = new Dictionary<NFGUID, GameObject>();
    int mnScene = 0;
    bool mbLoadedScene = false;
    Vector3 mvSceneBornPos = new Vector3();
}