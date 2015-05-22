using UnityEngine;
using System.Collections;

public class StageObject_Elevator : MonoBehaviour {

	public float 	switchingTime = 5.0f; 

	SliderJoint2D 	slide;
	float 			changeTime;

	void Start () {
		slide 		= GetComponent<SliderJoint2D> ();
		changeTime  = Time.fixedTime;
	}
	
	void Update () {
		if (Time.fixedTime > changeTime + switchingTime) {
			slide.useMotor  = (slide.useMotor) ? false : true;
			changeTime 		= Time.fixedTime;
		}
	}
}
