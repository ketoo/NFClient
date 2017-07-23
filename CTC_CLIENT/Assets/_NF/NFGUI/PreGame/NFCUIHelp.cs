using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NFCUIHelp
{
	#region Instance
	private static NFCUIHelp _Instance = null;
	
	public static NFCUIHelp Instance
	{
		get
		{
			if(null == _Instance)
			{
				_Instance = new NFCUIHelp();
			}
			return _Instance;
		}
		
	}
	#endregion
	
	
	
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

    public void SetLabelText(GameObject xLabel, string strText)
    {
        if (null != xLabel)
        {
            Text xLabelText = xLabel.GetComponent<Text>();
            if (null != xLabelText)
            {
                xLabelText.text = strText;
            }
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
	
	
	
}

