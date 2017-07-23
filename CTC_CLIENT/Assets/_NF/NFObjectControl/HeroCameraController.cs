using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NFrame;

public class OffestPos
{
    public float offsetX = 0;
    public float offsetY = 12;
    public float offsetZ = 9;

    public float XRot = 45;
    public float YRot = 180;
}

public class HeroCameraController : MonoBehaviour 
{
	static public GameObject player;
    static private Transform mxThis;		// Reference to the player's transform.


    public float maxinumDistance = 2;
    public float playerVelocity = 10;
	public float RotSmooth = 5;
    public float ScrollCoff = 10;

    public OffestPos mxOffestPos = new OffestPos();
    Dictionary<int, OffestPos> mxOffestPosList = new Dictionary<int, OffestPos>();

	float movementX = 0;
	float movementY = 0;
    float movementZ = 0;
    int mnLastSceneID = 0;

	Quaternion targetRotation;

	float scrollDegree = 0;

    Camera xCamera;
	// Use this for initialization
	void Start ()
    {
        mxThis = this.transform;
        xCamera = this.GetComponent<Camera>();

        NFrame.NFIClass xLogicClass = NFrame.NFCKernelModule.Instance.GetLogicClassModule().GetElement("Scene");
        List<string> xList = xLogicClass.GetConfigNameList();
        for (int i = 0; i < xList.Count; ++i )
        {
            string strName = (string)xList[i];

            string strCamOffestPos = NFrame.NFCKernelModule.Instance.GetElementModule().QueryPropertyString(strName, "CamOffestPos");
            string strCamOffestRot = NFrame.NFCKernelModule.Instance.GetElementModule().QueryPropertyString(strName, "CamOffestRot");

            NFrame.NFDataList xCamOffestPosList = new NFrame.NFDataList(strCamOffestPos, ',');
            NFrame.NFDataList xCamOffestRotList = new NFrame.NFDataList(strCamOffestRot, ',');
            if (xCamOffestPosList.Count() == 3 && xCamOffestRotList.Count() == 2)
            {
                OffestPos xOffestPos = new OffestPos();
                xOffestPos.offsetX = float.Parse(xCamOffestPosList.StringVal(0));
                xOffestPos.offsetY = float.Parse(xCamOffestPosList.StringVal(1));
                xOffestPos.offsetZ = float.Parse(xCamOffestPosList.StringVal(2));

                xOffestPos.XRot = float.Parse(xCamOffestRotList.StringVal(0));
                xOffestPos.YRot = float.Parse(xCamOffestRotList.StringVal(1));

                mxOffestPosList[int.Parse(strName)] = xOffestPos;
            }
        }
	}

    //完整安新的来，否则按照老的来
        
	// Update is called once per frame
	void Update ()
    {
        int nCurSceneID = NFRender.Instance.GetCurSceneID();
        if (mnLastSceneID != nCurSceneID)
        {
            mnLastSceneID = nCurSceneID;
            if (mxOffestPosList.ContainsKey(mnLastSceneID))
            {
                mxOffestPos = mxOffestPosList[nCurSceneID];
            }
            else
            {
                mxOffestPos = new OffestPos();
            }
        }

        if (null == player)
        {
            if (NFStart.Instance)
            {

					NFrame.NFGUID xID = NFNetController.Instance.xMainRoleID;
                    GameObject xGO = (GameObject)NFRender.Instance.GetObject(xID);
                    if (null != xGO)
                    {
                        player = xGO;
                    }
            }
        }

        if (null == player)
        {
//             if (null != xCamera)
//             {
//                 xCamera.enabled = false;
//                 this.tag = "";
//             }

            return;
        }

        xCamera.enabled = true;
        this.tag = "MainCamera";

		//near Scroll +
		//far Scroll -
		//Debug.Log ("mouse "+Input.GetAxis("Mouse ScrollWheel"));
		//if (Input.GetAxis ("Mouse ScrollWheel") != 0) {
			scrollDegree += Input.GetAxis("Mouse ScrollWheel")*ScrollCoff;
		//}
            //scrollDegree = Mathf.Max(0, Mathf.Min(45, scrollDegree));
            scrollDegree = Mathf.Max(0, Mathf.Min(60, scrollDegree));
		Vector3 npos = new Vector3 (0, 0, 1);
		npos = Quaternion.Euler(new Vector3(scrollDegree, 0, 0)) * npos;
        Vector3 newCameraPos = mxOffestPos.offsetZ * npos + (new Vector3(0, mxOffestPos.offsetY, 0));

		//Vector3 viewDir = (new Vector3 (0, -offsetY, 0))-npos;

        //targetRotation = Quaternion.Euler(new Vector3(mxOffestPos.XRot, mxOffestPos.YRot, 0));
        //float xDir = 90 - (180 - (90 - scrollDegree)) / 2;
        float xDir = mxOffestPos.XRot;
		//var qua = Quaternion.LookRotation (viewDir);
        targetRotation = Quaternion.Euler(new Vector3(xDir, mxOffestPos.YRot, 0));


        movementX = (player.transform.position.x + mxOffestPos .offsetX- this.transform.position.x) / maxinumDistance;
		movementZ = (player.transform.position.z+newCameraPos.z-this.transform.position.z)/maxinumDistance;
		movementY = (player.transform.position.y + newCameraPos.y - this.transform.position.y) / maxinumDistance;

		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * RotSmooth);
		this.transform.position += new Vector3(movementX*playerVelocity*Time.deltaTime, movementY*playerVelocity*Time.deltaTime, movementZ*playerVelocity*Time.deltaTime);

	}

    static public void Shake(GameObject xSender, Vector3 vAmount, float fTime)
    {
        if (null == mxThis)
        {
            return;
        }

        if (player)
        {
            if (player == xSender)
            {
                iTween.ShakePosition(player.gameObject, vAmount, fTime);
                iTween.ShakePosition(mxThis.gameObject, vAmount, fTime);
            }
        }
        else
        {
            iTween.ShakePosition(mxThis.gameObject, vAmount, fTime);
        }
    }
}
