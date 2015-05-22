using UnityEngine;
using System.Collections;

public class StageTrigger_EnabledSwitch : MonoBehaviour {

	public GameObject switchObject;

	void OnTriggerEnter2D_PlayerEvent (GameObject go) {
		switchObject.SetActive (true);
	}
}
