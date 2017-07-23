using UnityEngine;
using System.Collections;



public class NFMusicMgr : MonoBehaviour
{
    #region Instance
    private static NFMusicMgr _Instance = null;
    public static NFMusicMgr Instance
    {
        get
        {
            return _Instance;
        }

    }
    #endregion

    private int mnLastSceneID = 0;
    private AudioSource mxAudioSource;
    private string mstrLastMusicName = "";
    private float fVolume = 0.6f;

    private float fDelayCloseTime = 5.0f;
    private float fNowCloseTime = 0.0f;

    private float fStartDelayTime = 0.0f;

    void Awake()
    {
        _Instance = this;
        Transform xCam = this.transform.parent;
        AudioListener xAudioListener = xCam.gameObject.GetComponent<AudioListener>();
        if (null != xAudioListener)
        {
            xAudioListener.enabled = false;
        }
    }

    // Use this for initialization
    void Start()
    {
        mxAudioSource = (AudioSource)this.gameObject.GetComponent<AudioSource>();
        if (mxAudioSource == null)
        {
            mxAudioSource = (AudioSource)this.gameObject.AddComponent<AudioSource>();
        }

        PlaySceneMusic(1);
    }

    // Update is called once per frame
    void Update()
    {
        int nCurSceneID = NFRender.Instance.GetCurSceneID();
        if (mnLastSceneID == nCurSceneID)
        {
            return;
        }

        mnLastSceneID = nCurSceneID;
        fNowCloseTime = fDelayCloseTime;
        PlaySceneMusic(mnLastSceneID);
//         if (fNowCloseTime > 0.0f)
//         {
//             fNowCloseTime -= Time.deltaTime;
// 
//             mxAudioSource.volume = fVolume * fNowCloseTime / fDelayCloseTime;
// 
//             if (fNowCloseTime <= 0.0f)
//             {
//                 PlaySceneMusic(mnLastSceneID);
//             }
//         }
//         else
//         {
//             mxAudioSource.volume = fVolume;
//         }
    }

    void Close()
    {

    }

    void PlaySceneMusic(int nSceneID)
    {
        NFrame.NFIElement xElement = NFrame.NFCKernelModule.Instance.GetElementModule().GetElement(nSceneID.ToString());
        if (null != xElement)
        {
            string strName = xElement.QueryString("SoundList");
            string[] sArray = strName.Split(',');
            if (sArray.Length > 0)
            {
                //随机播放一个
                int nIndex = Random.Range(0, (int)sArray.Length);
                if (nIndex >= 0 && nIndex < sArray.Length)
                {
                    string strMusicName = sArray[nIndex];
                    if (mstrLastMusicName != strMusicName)
                    {
                        mstrLastMusicName = strMusicName;

                        AudioClip xAudioClip = GameObject.Instantiate(Resources.Load(strMusicName)) as AudioClip;

                        mxAudioSource.clip = xAudioClip;
                        mxAudioSource.rolloffMode = AudioRolloffMode.Custom;
                        mxAudioSource.volume = fVolume;
                        mxAudioSource.loop = true;
                        mxAudioSource.Play();
                        //mxAudioSource.PlayDelayed(fStartDelayTime);
                    }

                }
            }
        }
    }
}
