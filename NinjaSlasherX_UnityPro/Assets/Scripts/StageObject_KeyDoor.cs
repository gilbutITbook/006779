using UnityEngine;
using System.Collections;

public class StageObject_KeyDoor : MonoBehaviour {
	
	public SpriteRenderer keySprite;
	
	GameObject  openDoor;
	GameObject  closeDoor;

	void Awake () {
		openDoor 	= transform.Find ("StageB_ExitA").gameObject;
		closeDoor 	= transform.Find ("StageB_DoorA_Key").gameObject;
		
		openDoor.SetActive (false);
	}
	
	public void OpenDoor() {
		if (keySprite.enabled && closeDoor.activeSelf) {
			openDoor.SetActive (true);
			closeDoor.SetActive (false);
			
			PlayerController.GetController().ActionMove (0.0f);
			PlayerController.GetController().activeSts = false;
			openDoor.GetComponent<StageTrigger_Link> ().Jump ();

			AppSound.instance.SE_OBJ_OPENDOOR.Play();
		}
	}

}
