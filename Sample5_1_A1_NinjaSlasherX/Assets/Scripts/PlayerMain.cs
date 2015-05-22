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

		// パッド処理
		float joyMv = Input.GetAxis ("Horizontal");
		playerCtrl.ActionMove (joyMv);

		// ジャンプ
		if (Input.GetButtonDown ("Jump")) {
			playerCtrl.ActionJump ();
		}
	}

}
