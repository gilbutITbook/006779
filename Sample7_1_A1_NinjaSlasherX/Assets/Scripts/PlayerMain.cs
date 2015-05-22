using UnityEngine;
using System.Collections;

public class PlayerMain : MonoBehaviour {

	// === キャッシュ ==========================================
	PlayerController 	playerCtrl;

	// === コード（Monobehaviour基本機能の実装） ================
	void Awake () {
		playerCtrl = GetComponent<PlayerController>();
	}

	void Update () {
		// 操作可能か？
		if (!playerCtrl.activeSts) {
			return;
		}

		// 移動
		float joyMv = Input.GetAxis ("Horizontal");
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
				playerCtrl.ActionAttackJump();
			}
		}
	}

}
