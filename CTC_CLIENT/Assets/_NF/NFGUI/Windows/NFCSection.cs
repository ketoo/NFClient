using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

// -------------------------------------------------------------------------
//    @FileName             :    NFCSectionInit.cpp
//    @Author               :    LvSheng.Huang
//    @Date                 :    2012-07-05
//    @Module               :    NFCSectionInit
//    @Desc                 :    ÓÎÏ·ÕÂ½Ú³õÊ¼Æ÷
// -------------------------------------------------------------------------

public class NFCSection : MonoBehaviour
{
	public enum UI_SECTION_STATE
	{
		UISS_NONE,
		UISS_LOGIN,
		UISS_ROLEHALL,
		UISS_CREATEHALL,
		UISS_SERVERLIST,
		UISS_GAMEING,
		UISS_LOADING,
	};

    public bool Test = false;
    public Transform[] mUINodeTrans;
    public UI_SECTION_STATE meUIState = UI_SECTION_STATE.UISS_NONE;

    void Awake()
    {
        if (UI_SECTION_STATE.UISS_NONE == meUIState)
        {
            Debug.LogError("NFCSectionInit ERROR!!");
            return;
        }

		NFCSectionManager.Instance.AddGameSectionWindow(meUIState, this.gameObject);

		foreach (Transform child in transform)
		{
			child.gameObject.SetActive(true);
		}

        if (!Test)
        {
            foreach (Transform trans in mUINodeTrans)
            {
                if (null != trans)
                {
					/*
					GameObject prefab = Resources.Load<GameObject>(name);
					GameObject go = Instantiate(prefab) as GameObject;
					go.transform.parent = this.transform;
					go.transform.localPosition = prefab.transform.position;
					go.transform.localRotation = prefab.transform.rotation;
					go.transform.localScale = prefab.transform.localScale;
*/

                    Transform UI = GameObject.Instantiate<Transform>(trans);
					UI.SetParent(this.transform);

					//RectTransform rectTrans = UI.GetComponent<RectTransform>();

					UI.localPosition = trans.position;
					//rectTrans.localScale = new Vector3(1.0f, 1.0f, 1.0f);

                    UI.gameObject.SetActive(true);
                }
            }
        }
    }


    void Start()
    {

    }

    void Update()
    {

    }

}