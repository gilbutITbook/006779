#define FPS_ONGUI

using UnityEngine;
using System.Collections;

public class zFoxDebugFrameRate : MonoBehaviour {

	public bool 	DontDestroyEnabled 	= true;

	public bool 	OnGUIDraw 			= true;
	public int  	OnGUIFontSize 		= 14;
	public Color  	OnGUIFontColor 		= Color.white;
	public Vector2	OnGUIDrawPosition	= Vector2.zero;

	[System.NonSerialized] public float UpdateNowFPS;
	[System.NonSerialized] public float UpdateAvgFPS;
	[System.NonSerialized] public float UpdateDeltaTime;
	[System.NonSerialized] public float FixedUpdateNowFPS;
	[System.NonSerialized] public float FixedUpdateAvgFPS;
	[System.NonSerialized] public float FixedUpdateDeltaTime;
	
#if FPS_ONGUI	
	private GUIStyle 		style;
	private GUIStyleState 	styleState;
#endif

	TextMesh tm;

	void Start () {
		if (DontDestroyEnabled) {
			DontDestroyOnLoad (this);
		}

#if FPS_ONGUI	
		styleState 				= new GUIStyleState();
		styleState.textColor 	= OnGUIFontColor;
		style 					= new GUIStyle();
		style.fontSize 			= OnGUIFontSize;
		style.normal 			= styleState;
#endif

		UpdateNowFPS 			= 0.0f;
		UpdateAvgFPS 			= 0.0f;
		UpdateDeltaTime 		= 0.0f;
		FixedUpdateNowFPS 		= 0.0f;
		FixedUpdateAvgFPS 		= 0.0f;
		FixedUpdateDeltaTime 	= 0.0f;

		tm = GetComponent<TextMesh> ();
	}
	
	void Update () {
		// Update FPS
		UpdateNowFPS = (1.0f / Time.deltaTime);
		UpdateAvgFPS = (UpdateAvgFPS + UpdateNowFPS) / 2.0f;
		UpdateDeltaTime = Time.deltaTime;

		// 3D Text
		if (tm) {
			float fps, fpsAvg;

			fps    = Mathf.FloorToInt (UpdateNowFPS);
			fpsAvg = Mathf.FloorToInt (UpdateAvgFPS);	
			tm.text = string.Format("U FPS : now {0} / avg {1} : T {2}",fps,fpsAvg,UpdateDeltaTime);
			tm.text += "\r\n";
			
			fps    = Mathf.FloorToInt (UpdateNowFPS);
			fpsAvg = Mathf.FloorToInt (UpdateAvgFPS);
			tm.text += string.Format("F FPS : now {0} / avg {1} : T {2}",fps,fpsAvg,FixedUpdateDeltaTime);
		}
	}

	void FixedUpdate () {
		// FixedUpdate FPS
		FixedUpdateNowFPS = (1.0f / Time.fixedDeltaTime);
		FixedUpdateAvgFPS = (FixedUpdateAvgFPS + FixedUpdateNowFPS) / 2.0f;
		FixedUpdateDeltaTime = Time.fixedDeltaTime;
	}

#if FPS_ONGUI	
	void OnGUI() { 

		if (!OnGUIDraw) {
			return;
		}

		float gx = OnGUIDrawPosition.x;
		float gy = OnGUIDrawPosition.y;

		float fps, fpsAvg;
		fps    = Mathf.FloorToInt (UpdateNowFPS);
		fpsAvg = Mathf.FloorToInt (UpdateAvgFPS);
		GUI.Label (new Rect(gx, gy, 200, style.fontSize),
		           string.Format("U FPS : now {0} / avg {1} : T {2}",fps,fpsAvg,UpdateDeltaTime),
		           style);
		fps    = Mathf.FloorToInt (UpdateNowFPS);
		fpsAvg = Mathf.FloorToInt (UpdateAvgFPS);
		GUI.Label (new Rect(gx, gy + style.fontSize * 1.0f, 200, style.fontSize),
		           string.Format("F FPS : now {0} / avg {1} : T {2}",fps,fpsAvg,FixedUpdateDeltaTime),
		           style);
	}
#endif
}
