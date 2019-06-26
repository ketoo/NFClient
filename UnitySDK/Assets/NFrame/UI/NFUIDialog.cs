using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using NFSDK;

public abstract class NFUIDialog : MonoBehaviour
{
    public NFDataList mUserData = null;

    // Use this for initialization
    public abstract void Init();
    //public void OnEnable();
    //public void OnDisable();
}