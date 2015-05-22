using UnityEngine;
using System.Collections;

public class StageObject_Switch : MonoBehaviour {

	// === 外部パラメータ（インスペクタ表示） =====================
	public bool 		switchOn		 		= false;
	public bool			onlyOnce 				= false;
	public bool			switchTurnMode 			= true;
	public bool			switchObjectsTurnMode 	= true;
	public float 		resetTime 				= 0.0f;
	public bool 		switchObjectsInit 		= true;
	public GameObject[] switchOnObjects;
	public GameObject[] switchOffObjects;
	public GameObject[] switchInstantiateObjects;
	public bool			switchinstantiateObjectsDestroy	= true;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public int 	switchTurnCount = 0;

	// === 内部パラメータ ======================================
	GameObject 			goLever;
	float 				switchTurnTime;
	GameObject[] 		switchinstantiateObjectsList;

	// === コード（Monobehaviour基本機能の実装） ================
	void Awake () {
		goLever = transform.Find ("Stage_Switch_Lever_1").gameObject;

		switchTurnTime = 0.0f;
		switchinstantiateObjectsList = new GameObject[switchInstantiateObjects.Length];

		SwitchOnGraphics (switchOn);
		if (switchObjectsInit) {
			SwitchOnObjects (switchOn, false);
		}
	}

	void Update() {
		if (resetTime > 0.0f && switchTurnTime > 0.0f) {
			if (switchTurnTime + resetTime <= Time.fixedTime) {
				SwitchOnWork(!switchOn);
				switchTurnTime = 0.0f;
			}
		}
	}
	
	void SwitchOnGraphics (bool sw) {
		if (goLever) {
			float r = (sw) ? 90.0f : 0.0f;
			goLever.transform.localRotation = Quaternion.Euler (0.0f, 0.0f, r);
		}
	}

	void SwitchOnObjects (bool sw,bool turn) {
		if (!turn) {
			if (switchTurnMode) {
				foreach (GameObject switchOnObject in switchOnObjects) {
					if (switchOnObject) switchOnObject.SetActive (sw);
				}
				foreach (GameObject switchOffObject in switchOffObjects) {
					if (switchOffObject) switchOffObject.SetActive (!sw);
				}
			} else {
				foreach (GameObject switchOnObject in switchOnObjects) {
					if (switchOnObject) switchOnObject.SetActive (true);
				}
				foreach (GameObject switchOffObject in switchOffObjects) {
					if (switchOffObject) switchOffObject.SetActive (false);
				}
			}
		} else {
			foreach (GameObject switchOnObject in switchOnObjects) {
				if (switchOnObject) switchOnObject.SetActive (!switchOnObject.activeSelf);
			}
			foreach (GameObject switchOffObject in switchOffObjects) {
				if (switchOffObject) switchOffObject.SetActive (!switchOffObject.activeSelf);
			}
		}

		for(int i = 0;i < switchInstantiateObjects.Length;i ++) {
			if (switchInstantiateObjects[i]) {
				if (sw) {
					switchinstantiateObjectsList[i] = Instantiate(switchInstantiateObjects[i]) as GameObject;
				} else {
					if (switchinstantiateObjectsDestroy) Destroy(switchinstantiateObjectsList[i]);
				}
			}
		}
	}

	void SwitchOnWork (bool sw) {
		switchOn = sw;
		SwitchOnGraphics (sw);
		SwitchOnObjects  (sw,switchObjectsTurnMode);
		switchTurnTime = Time.fixedTime;
		switchTurnCount ++;

		AppSound.instance.SE_OBJ_SWITCH.Play ();
	}

	public void SwitchOn (bool sw) {
		if (onlyOnce && switchTurnCount > 0) return;
		if (resetTime > 0.0f && switchTurnTime > 0.0f) return;

		SwitchOnWork (sw);
	}

	public void SwitchTurn() {
		SwitchOn (!switchOn);
	}
}


