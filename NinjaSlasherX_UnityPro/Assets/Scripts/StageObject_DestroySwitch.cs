using UnityEngine;
using System.Collections;

public class StageObject_DestroySwitch : MonoBehaviour {

	public GameObject[] destroyObjectList;

	public void DestroyStageObject() {
		AppSound.instance.SE_OBJ_SWITCH.Play ();
		foreach (GameObject go in destroyObjectList) {
			Destroy (go);
		}
		Destroy (this.gameObject);
	}
}
