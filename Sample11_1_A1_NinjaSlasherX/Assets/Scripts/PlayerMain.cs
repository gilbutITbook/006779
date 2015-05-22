using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour {

	// === 内部パラメータ ======================================
	PlayerController 	playerCtrl;

	bool 				actionEtcRun = true;

	// === コード（Monobehaviour基本機能の実装） ================
	void Awake () {
		playerCtrl 		= GetComponent<PlayerController>();
	}

	void Update () {
		// 操作可能か？
		if (!playerCtrl.activeSts) {
			return;
		}

		// 移動
		float joyMv = Input.GetAxis ("Horizontal");
//		joyMv = Mathf.Pow(Mathf.Abs(joyMv),3.0f) * Mathf.Sign(joyMv);
		playerCtrl.ActionMove (joyMv);


		// ジャンプ
		if (Input.GetButtonDown ("Jump")) {
			playerCtrl.ActionJump ();
			return;
		}

		// 攻撃
		if (Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2") || Input.GetButtonDown("Fire3")) {
			if (Input.GetAxisRaw ("Vertical") < 0.5f) {
				playerCtrl.ActionAttack();
			} else {
				//Debug.Log (string.Format ("Vertical {0} {1}",Input.GetAxisRaw ("Vertical"),vp.vertical));
				playerCtrl.ActionAttackJump();
			}
			return;
		}

		// ドアを開けたり通路に入る
		if (Input.GetAxisRaw ("Vertical") > 0.7f) {
			if (actionEtcRun) {
				playerCtrl.ActionEtc ();
				actionEtcRun = false;
			}
		} else {
			actionEtcRun = true;
		}
	}
}
