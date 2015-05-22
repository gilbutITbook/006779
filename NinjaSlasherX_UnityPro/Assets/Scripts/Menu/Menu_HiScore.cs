using UnityEngine;
using System.Collections;

public class Menu_HiScore : MonoBehaviour {
	void Start() {
		SaveData.LoadHiScore ();
		zFoxFadeFilter.instance.FadeIn (Color.black, 0.5f);
		for(int i = 1;i <= 10;i ++) {
			TextMesh tm = GameObject.Find("Rank" + i).GetComponent<TextMesh>();
			if (i == SaveData.newRecord) {
				tm.color = Color.red;
			}
			tm.text = string.Format("{0,2}:{1,10}",i,SaveData.HiScore[(i - 1)]);
		}
	}
	
	void Button_Prev(MenuObject_Button button) {
		zFoxFadeFilter.instance.FadeOut (Color.black, 0.5f);
		Invoke ("SceneJump",0.7f);

		AppSound.instance.SE_MENU_CANCEL.Play ();
	}

	void SceneJump(){
		Application.LoadLevel("Menu_Title");
	}
}
