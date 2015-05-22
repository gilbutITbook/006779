using UnityEngine;
using System.Collections;

public class Menu_Option : MonoBehaviour {
	void Start() {
		zFoxFadeFilter.instance.FadeIn (Color.black, 0.5f);
		SaveData.LoadOption ();
		MenuObject_Button.FindMessage (GameObject.Find("MenuFormA"), "Button_VRPad").SetLabelText ((SaveData.VRPadEnabled ? "On" : "Off"));
	}

	void Update() {
		GameObject.Find ("SaveData_Date").GetComponent<TextMesh> ().text = SaveData.SaveDate;
	}

	void Slidebar_Init(MenuObject_Slidebar slidebar) {
		if (slidebar.label == "BGM") {
			slidebar.SetPosition (new Vector2 (SaveData.SoundBGMVolume, 0.0f));
		}
		if (slidebar.label == "SE") {
			slidebar.SetPosition (new Vector2 (SaveData.SoundSEVolume, 0.0f));
		}
	}

	void Slidebar_Drag(MenuObject_Slidebar slidebar) {
		if (slidebar.label == "BGM") {
			SaveData.SoundBGMVolume = slidebar.curosorPosition.x;
			AppSound.instance.fm.SetVolume("BGM",SaveData.SoundBGMVolume);
		}
		if (slidebar.label == "SE") {
			SaveData.SoundSEVolume = slidebar.curosorPosition.x;
			AppSound.instance.fm.SetVolume("SE",SaveData.SoundSEVolume);
		}
		Debug.Log (string.Format("{0}:{1}",slidebar.label,slidebar.curosorPosition));
	}
	
	void Button_VRPad(MenuObject_Button button) {
		SaveData.VRPadEnabled = SaveData.VRPadEnabled ? false : true;
		button.SetLabelText ((SaveData.VRPadEnabled ? "On" : "Off"));
		AppSound.instance.SE_MENU_OK.Play ();
	}

	void Button_SaveDataReset(MenuObject_Button button) {
		GameObject.Find ("MenuFormA").transform.position = new Vector3 (-100.0f, 0.0f, 0.0f);
		GameObject.Find ("MenuFormB").transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
		AppSound.instance.SE_MENU_OK.Play ();
	}

	void Button_Prev(MenuObject_Button button) {
		zFoxFadeFilter.instance.FadeOut (Color.black, 0.5f);
		Invoke ("SceneJump",0.7f);
		AppSound.instance.SE_MENU_CANCEL.Play ();
	}

	void SceneJump(){
		SaveData.SaveOption ();
		Application.LoadLevel("Menu_Title");
	}

	void Button_SaveDataReset_Yes(MenuObject_Button button) {
		GameObject.Find ("MenuFormA").transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
		GameObject.Find ("MenuFormB").transform.position = new Vector3 (100.0f, 0.0f, 0.0f);

		SaveData.DeleteAndInit (true);

		AppSound.instance.fm.SetVolume("BGM",SaveData.SoundBGMVolume);
		AppSound.instance.fm.SetVolume("SE",SaveData.SoundSEVolume);

		MenuObject_Slidebar[] slidebarList = GameObject.Find ("MenuFormA").GetComponentsInChildren<MenuObject_Slidebar> ();
		foreach(MenuObject_Slidebar slidebar in slidebarList) {
			slidebar.Init();
		}
		AppSound.instance.SE_MENU_OK.Play ();
	}

	void Button_SaveDataReset_No(MenuObject_Button button) {
		GameObject.Find ("MenuFormA").transform.position = new Vector3 (0.0f, 0.0f, 0.0f);
		GameObject.Find ("MenuFormB").transform.position = new Vector3 (100.0f, 0.0f, 0.0f);
		AppSound.instance.SE_MENU_CANCEL.Play ();
	}
	
	void Button_Debug(MenuObject_Button button) {
		Application.LoadLevel("Menu_Debug");
	}
}
