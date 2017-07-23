using UnityEngine;
using System.Collections;

public class NFDestroySelf : MonoBehaviour {
    public float DestroyTime = 0.0f;

    void Awake()
    {
        if (DestroyTime <= 0.01f)
        {
            GameObject.Destroy(this.gameObject);
        }
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
