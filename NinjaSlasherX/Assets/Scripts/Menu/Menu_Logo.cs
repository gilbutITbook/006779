using UnityEngine;
using System.Collections;

public class Menu_Logo : MonoBehaviour {
	void Start () {
		StartCoroutine ("LogoWork");
	}

	IEnumerator LogoWork() {
		zFoxFadeFilter.instance.FadeIn (Color.black, 1.0f);
		yield return new WaitForSeconds (3.0f);
		zFoxFadeFilter.instance.FadeOut (Color.black, 1.0f);
		yield return new WaitForSeconds (1.2f);
		Application.LoadLevel ("Menu_Title");
	}
}
