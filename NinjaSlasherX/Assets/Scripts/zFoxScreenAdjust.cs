using UnityEngine;
using System.Collections;

public class zFoxScreenAdjust : MonoBehaviour {

	public float aspectWH  = 1.6f;
	public float aspectAdd = 0.05f;

	public bool StartScreenAdjust  = true;
	public bool UpdateScreenAdjust = false;

	Vector3 localScale;

	void Start () {
		localScale = transform.localScale;
		if (StartScreenAdjust) {
			ScreenAdjust();
		}
	}
	
	void Update () {
		if (UpdateScreenAdjust) {
			ScreenAdjust();
		}
	}

	void ScreenAdjust() {
		float wh = (float)Screen.width / (float)Screen.height;
		//Debug.Log (string.Format("asepectWH:{0} wh:{1}",asepectWH,wh));
		if (wh < aspectWH) {
			transform.localScale = new Vector3(localScale.x - (aspectWH - wh) + aspectAdd,
			                                   localScale.y,
			                                   localScale.z);
		} else {
			transform.localScale = localScale;
		}
	}
}
