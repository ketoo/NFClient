using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFrame;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NFCUtility
{
	#region Instance
    private static NFCUtility _Instance = null;

    public static NFCUtility Instance
	{
		get
		{
			if(null == _Instance)
			{
                _Instance = new NFCUtility();
			}
			return _Instance;
		}
		
	}
	#endregion


    Hashtable mhtData = new Hashtable();

	public Transform GetRootWindow(Transform trans)
	{
		Transform rootPanel = trans;
		
		while(rootPanel.parent)
		{
			if(rootPanel.parent.name == "UI_GO")
			{
				break;
			}
			else
			{
				rootPanel = rootPanel.parent;
			}
		}
		
		return rootPanel;
		
	}
	
	public void SetUIScale(Hashtable htControl, int nControlIndex, float fScale)
	{
		GameObject go = (GameObject)htControl[nControlIndex];
		if(null != go)
		{
			Hashtable parameters = new Hashtable();
			parameters.Add("x",fScale);
			parameters.Add("y",fScale);
			iTween.ScaleTo(go, parameters);
		}
		
	}
	
	public void CheckUIChileObjectNull(Hashtable htControl, int nControlIndex)
	{
		for(int i = 0; i < nControlIndex; i++)
		{
			GameObject go = (GameObject)htControl[i];
			if(null == go)
			{
				Debug.LogError(" This control is null!!!!!!!----");
				Debug.LogError(i );
			}
		}
	}


    public void SetUILable(GameObject xGO, object xText)
    {
        Text xLbl = xGO.GetComponent<Text>();
        xLbl.text = xText.ToString();
    }

    public void SetUISprite(GameObject xGO, string strSprite)
    {
        Sprite xData = Resources.Load<Sprite>(strSprite);
        if (null != xData)
        {
            Image xImage = xGO.GetComponent<Image>();
            xImage.sprite = xData;
        }
    }

    public void SetUISlider(GameObject xGO, int nValue, int nMaxValue)
    {
        if (nMaxValue <=0 )
        {
            return;
        }

        float fFill = nValue / (float)nMaxValue;
        if (null != xGO)
        {
            Slider xSlider = xGO.GetComponent<Slider>();
            xSlider.value = fFill;
            xSlider.maxValue = 1;
            xSlider.minValue = 0;
        }
    }

    //在某个位置附近随机一个坐标
    public Vector3 RandomPos(Vector3 vPos, float fRang)
    {
        float fx = UnityEngine.Random.Range(vPos.x - fRang, vPos.x + fRang);
        float fz = UnityEngine.Random.Range(vPos.z - fRang, vPos.z + fRang);
        return new Vector3(fx, vPos.y, fz);
    }

    public void NFSleep(float fTime)
    {
        //StartCoroutine(WaiterForFight(1.0f));
    }

    private IEnumerator WaiterForFight(float fTime)
    {
        yield return new WaitForSeconds(fTime);
    }
}

