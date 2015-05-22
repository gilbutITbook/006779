using UnityEngine;
using System.Collections;

public enum FOXFADE_STATE
{
	NON,
	IN,
	OUT,
};

public class zFoxFadeFilter : MonoBehaviour {

	public static zFoxFadeFilter instance = null;

	// === 外部パラメータ（インスペクタ表示） =====================
	public GameObject 		fadeFilterObject 	= null;
	public string			attacheObject		= "FadeFilterPoint";

	// === 外部パラメータ ======================================
	[System.NonSerialized] public FOXFADE_STATE	fadeState;

	// === 内部パラメータ ======================================
	private float 			startTime;
	private float 			fadeTime;
	private Color 			fadeColor;

	private string 			prevSceneName = "(non)";

	// === コード（Monobehaviour基本機能の実装） ================
	void Awake () {
		instance  = this;
		fadeState = FOXFADE_STATE.NON;
	}

	void SetFadeAction(FOXFADE_STATE state,Color color,float time) {
		fadeState = state;
		startTime = Time.time;
		fadeTime  = time;
		fadeColor = color;
	}

	public void FadeIn(Color color,float time) {
		SetFadeAction (FOXFADE_STATE.IN, color, time);
	}

	public void FadeOut(Color color,float time) {
		SetFadeAction (FOXFADE_STATE.OUT, color, time);
	}

	void SetFadeFilterColor(bool enbaled ,Color color) {
		if (fadeFilterObject) {
			fadeFilterObject.renderer.enabled 		 = enbaled;
			fadeFilterObject.renderer.material.color = color;
			SpriteRenderer sprite = fadeFilterObject.GetComponent<SpriteRenderer>();
			if (sprite) {
				sprite.enabled = enbaled;
				sprite.color   = color;
				fadeFilterObject.SetActive(enbaled);
			}
		}
	}
	
	void Update () {
		// フェードフィルターをアタッチする（シーン間移動対応処理）
		if (attacheObject != null && prevSceneName != Application.loadedLevelName) {
			GameObject go = GameObject.Find (attacheObject);
			fadeFilterObject.transform.position = go.transform.position;
			prevSceneName = Application.loadedLevelName;
		}

		// フェード処理
		switch(fadeState) {
		case FOXFADE_STATE.NON :
			break;
			
		case FOXFADE_STATE.IN :
			fadeColor.a = 1.0f - ((Time.time - startTime) / fadeTime);
			if (fadeColor.a > 1.0f || fadeColor.a < 0.0f) {
				fadeColor.a = 0.0f;
				fadeState = FOXFADE_STATE.NON;
				SetFadeFilterColor(false,fadeColor);
				break;
			}
			SetFadeFilterColor(true,fadeColor);
			break;

		case FOXFADE_STATE.OUT :
			fadeColor.a = (Time.time - startTime) / fadeTime;				
			if (fadeColor.a > 1.0f || fadeColor.a < 0.0f) {
				fadeColor.a = 1.0f;
				fadeState = FOXFADE_STATE.NON;
			}
			SetFadeFilterColor(true,fadeColor);
			break;
		}
		// Debug.Log (string.Format ("[FoxFadeFilter] fadeState:{0} fadeColor:{1},fadeTime:{2}", fadeState, fadeColor,fadeTime));
	}
}
