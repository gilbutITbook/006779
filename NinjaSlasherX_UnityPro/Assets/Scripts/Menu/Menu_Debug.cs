using UnityEngine;
using System.Collections;

public class Menu_Debug : MonoBehaviour {
	void Start() {
		zFoxFadeFilter.instance.FadeIn (Color.black, 0.5f);
	}

	void Button_Muteki(MenuObject_Button button) {
		SaveData.debug_Invicible = (SaveData.debug_Invicible) ? false : true;
		button.SetLabelText ((SaveData.debug_Invicible) ? "Muteki(On)" : "Muteki(Off)");
	}

	void Button_DebugJump(MenuObject_Button button) {
		SaveData.continuePlay = false;
		PlayerController.checkPointEnabled = false;
		Application.LoadLevel(button.transform.GetComponentInChildren<TextMesh>().text);
		AppSound.instance.SE_MENU_OK.Play ();
	}

	void Button_Prev(MenuObject_Button button) {
		zFoxFadeFilter.instance.FadeOut (Color.black, 0.5f);
		AppSound.instance.SE_MENU_CANCEL.Play ();
		Invoke ("SceneJump",0.7f);
	}
	
	void SceneJump(){
		Application.LoadLevel("Menu_Option");
	}
}
