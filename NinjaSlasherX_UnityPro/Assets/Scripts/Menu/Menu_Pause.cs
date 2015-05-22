using UnityEngine;
using System.Collections;

public class Menu_Pause : MonoBehaviour {

	PlayerController 	playerCtrl;
	GameObject 			pauseButton;
	GameObject 			exitButton;
	float				activeTime;

	void Start() {
		playerCtrl = PlayerController.GetController ();
		pauseButton = transform.Find ("MenuButton_Pause").gameObject;
		exitButton  = transform.Find ("MenuButton_Exit").gameObject;
		exitButton.SetActive (false);
		activeTime = Time.time;
	}

	void Update () {
		transform.position = new Vector3(Camera.main.transform.position.x,Camera.main.transform.position.y,0.0f);
		if (!Input.anyKey && 
		    (Mathf.Abs(Input.GetAxisRaw ("Vertical"))   < 0.05f) &&
			(Mathf.Abs(Input.GetAxisRaw ("Horizontal")) < 0.05f) ) {
			if (Time.time > activeTime + 2.0f) {
				pauseButton.SetActive (true);
			}
		} else {
			Invoke ("Check",1.0f);
		}
	}

	void Check() {
		if (Time.timeScale > 0.0f) {
			pauseButton.SetActive (false);
			activeTime = Time.time;
		}
	}

	void Button_Pause() {
		AppSound.instance.SE_MENU_OK.Play ();
		Time.timeScale 		 = (Time.timeScale > 0.0f) ? 0.0f : 1.0f;
		playerCtrl.activeSts = (Time.timeScale > 0.0f) ? true : false;
		exitButton.SetActive((Time.timeScale <= 0.0f) ? true : false);
	}
	void Button_Exit() {
		AppSound.instance.SE_MENU_CANCEL.Play ();
		Time.timeScale = 1.0f;
		Application.LoadLevel ("Menu_Title");
	}
}
