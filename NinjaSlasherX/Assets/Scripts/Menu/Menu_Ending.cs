using UnityEngine;
using System.Collections;

public class Menu_Ending : MonoBehaviour {
	void Start () {
		Invoke ("WhiteOut",  2.0f);
		Invoke ("End"	  , 15.0f);
	}

	void WhiteOut() {
		zFoxFadeFilter.instance.FadeOut (Color.white, 12.0f);
	}

	void End() {
		PlayerController playerCtrl = PlayerController.GetController();
		playerCtrl.GameOver ();
		Application.LoadLevel ("Menu_HiScore");
	}
}
