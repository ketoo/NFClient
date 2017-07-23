using UnityEngine;
using System.Collections;

public class UIObjRotation : MonoBehaviour 
{

	// Use this for initialization
	void Start () 
	{
		if(rotType ==0)
		{
			CanRotationSoldier = false;
		}
		if(rotType ==1)
		{
			CanRotationSoldier = true;
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		RotateCharacter ();
	}

	//控制鼠标拖动角色旋转
	public GameObject RotationY;
	public GameObject RotationX;
	public float rotSpeed = 1;
	public int rotType =0;
	private Vector3 mousePos;
	private Vector3 regMousePos;
	private Vector3 direction;
	public bool CanRotationSoldier = false;

	void RotateCharacter ()
	{
		if(Input.GetMouseButton(0))
		{
			if(Input.GetMouseButtonDown(0))
				regMousePos = Input.mousePosition;
			
			mousePos = Input.mousePosition;
			if(Input.GetMouseButtonUp(0))
				direction =new Vector3 (0,0,0);
			direction = mousePos - regMousePos;
			regMousePos = Input.mousePosition;
			if((RotationX!=null)&&(CanRotationSoldier == true))
				RotationX.transform.Rotate(0, -direction.x * rotSpeed,0) ;
			if((RotationY!=null)&&(CanRotationSoldier == true))
				RotationY.transform.Rotate(-direction.y * rotSpeed, 0, 0);
		}
	}
}
