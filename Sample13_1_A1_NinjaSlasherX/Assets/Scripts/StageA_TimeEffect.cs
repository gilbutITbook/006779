using UnityEngine;
using System.Collections;

public class StageA_TimeEffect : MonoBehaviour {
	GameObject 		player;

	SpriteRenderer 	CameraFillter;
	Color 			paperColor_A 	= Color.black;
	Color 			paperColor_B 	= new Color(1.0f,0.0f,0.0f,0.22f);

	LineRenderer 	Stage_BackColor;
	Color 			backColorST_A	= new Color (1.0f , 1.0f  , 1.0f, 1.0f);   // 昼
	Color 			backColorED_A	= new Color (0.0f , 0.916f, 1.0f, 1.0f);
	Color 			backColorST_B	= new Color (1.0f , 0.0f  , 0.0f, 1.0f);   // 夕方
	Color 			backColorED_B	= new Color (0.86f, 1.0f  , 0.0f, 1.0f);

	void Start () {
		player 			= PlayerController.GetGameObject();
		CameraFillter 	= GameObject.Find ("Filter_Paper").GetComponent<SpriteRenderer> ();
		Stage_BackColor = GameObject.Find ("StageA_BackColor").GetComponent<LineRenderer> ();

		paperColor_A  = CameraFillter.color;
	}
	
	void Update () {
		float t = player.transform.position.x / 380.0f;

		CameraFillter.color = Color.Lerp (paperColor_A, paperColor_B, t);

		Color st = Color.Lerp (backColorST_A, backColorST_B, t);
		Color ed = Color.Lerp (backColorED_A, backColorED_B, t);
		Stage_BackColor.SetColors (st, ed);
	}
}
