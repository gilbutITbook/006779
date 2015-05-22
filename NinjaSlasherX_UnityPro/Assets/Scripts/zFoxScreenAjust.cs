using UnityEngine;
using System.Collections;

public class zFoxScreenAjust : MonoBehaviour {

	public float asepectWH  = 1.6f;
	public float asepectAdd = 0.05f;

	public bool StartScreenAjust  = true;
	public bool UpdateScreenAjust = false;

	Vector3 localScale;

	void Start () {
		localScale = transform.localScale;
		if (StartScreenAjust) {
			ScreenAjust();
		}
	}
	
	void Update () {
		if (UpdateScreenAjust) {
			ScreenAjust();
		}
	}

	void ScreenAjust() {
		float wh = (float)Screen.width / (float)Screen.height;
		//Debug.Log (string.Format("asepectWH:{0} wh:{1}",asepectWH,wh));
		if (wh < asepectWH) {
			transform.localScale = new Vector3(localScale.x - (asepectWH - wh) + asepectAdd,
			                                   localScale.y,
			                                   localScale.z);
		} else {
			transform.localScale = localScale;
		}
	}
}
