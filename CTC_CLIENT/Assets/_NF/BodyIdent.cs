using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BodyIdent : MonoBehaviour 
{
	public string ObjectID;
    public enum BodyType { NFWeapon, NFBody, NFRoot, UNKNOW }
    public BodyType eType = BodyType.UNKNOW;
    public Transform xBody = null;
    public Transform xWeapon = null;
    public Transform xFirePoint = null;
    public Transform xBeHitPoint = null;
    public Transform xHeadPoint = null;


    private Dictionary<string, Transform> xChildList = new Dictionary<string,Transform>();
	private NFrame.NFGUID mxID;

	public NFrame.NFGUID GetObjectID()
	{
		return mxID;
	}

	public void SetObjectID(NFrame.NFGUID xID)
	{
		mxID = xID;
		ObjectID = mxID.ToString ();
	}

    public Transform GetChildObject(string strChild)
    {
        if (strChild.Length > 0 && xChildList.ContainsKey(strChild))
        {
            return xChildList[strChild];
        }

        return null;
    }

    void AddChild(Transform x)
    {
        if (x.tag == "NFBody")
        {
            xBody = x;
        }
        else if (x.tag == "NFWeapon")
        {
            xWeapon = x;
        }
        else if (x.tag == "NFFirePoint")
        {
            xFirePoint = x;
        }

        foreach ( Transform xChildVar in x )
        {
            xChildList[xChildVar.name] = xChildVar;
            AddChild(xChildVar);
        }
    }

    void Awake()
    {
        if (BodyType.UNKNOW == eType)
        {
            eType = BodyType.NFRoot;
            AddChild(this.transform);
        }
    }


	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
