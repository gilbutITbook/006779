using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour {

	// === 内部パラメータ ======================================
	PlayerController 	playerCtrl;
	zFoxVirtualPad 		vpad;

	bool 				actionEtcRun = true;

	// === コード（Monobehaviour基本機能の実装） ================
	void Awake () {
		playerCtrl 		= GetComponent<PlayerController>();
		vpad 			= GameObject.FindObjectOfType<zFoxVirtualPad> ();
	}

	void Update () {
		// 操作可能か？
		if (!playerCtrl.activeSts) {
			return;
		}

		// バーチャルパッド
		float vpad_vertical 	= 0.0f;
		float vpad_horizontal 	= 0.0f;
		zFOXVPAD_BUTTON  vpad_btnA = zFOXVPAD_BUTTON.NON;
		zFOXVPAD_BUTTON  vpad_btnB = zFOXVPAD_BUTTON.NON;
		if (vpad != null) {
			vpad_vertical 	= vpad.vertical;
			vpad_horizontal = vpad.horizontal;
			vpad_btnA 		= vpad.buttonA;
			vpad_btnB 		= vpad.buttonB;
		}


		// 移動
		float joyMv = Input.GetAxis ("Horizontal");
//		joyMv = Mathf.Pow(Mathf.Abs(joyMv),3.0f) * Mathf.Sign(joyMv);

		float vpadMv = vpad_horizontal;
		vpadMv = Mathf.Pow(Mathf.Abs(vpadMv),1.5f) * Mathf.Sign(vpadMv);
		playerCtrl.ActionMove (joyMv + vpadMv);


		// ジャンプ
		if (Input.GetButtonDown ("Jump") || vpad_btnA == zFOXVPAD_BUTTON.DOWN) {
			playerCtrl.ActionJump ();
			return;
		}

		// 攻撃
		if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3") || 
		    vpad_btnB == zFOXVPAD_BUTTON.DOWN) {
			if (Input.GetAxisRaw ("Vertical") + vpad_vertical < 0.5f) {
				playerCtrl.ActionAttack();
			} else {
				//Debug.Log (string.Format ("Vertical {0} {1}",Input.GetAxisRaw ("Vertical"),vp.vertical));
				playerCtrl.ActionAttackJump();
			}
			return;
		}

		// ドアを開けたり通路に入る
		if (Input.GetAxisRaw ("Vertical") + vpad_vertical > 0.7f) {
			if (actionEtcRun) {
				playerCtrl.ActionEtc ();
				actionEtcRun = false;
			}
		} else {
			actionEtcRun = true;
		}
	}
}
