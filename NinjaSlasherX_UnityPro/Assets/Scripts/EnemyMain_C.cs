using UnityEngine;
using System.Collections;


public class EnemyMain_C : EnemyMain {

	// === 外部パラメータ（インスペクタ表示） =====================
	public int 		aiIfATTACKONSIGHT 		= 50;
	public int 		aiIfRUNTOPLAYER 		= 10;
	public int	 	aiIfESCAPE 				= 10;
	public int 		aiIfRETURNTODOGPILE 	= 10;
	public float 	aiPlayerEscapeDistance = 0.0f;

	public int 		damageAttack_A 			= 1;

	public int 		fireAttack_A			= 3;
	public float 	waitAttack_A			= 10.0f;


	// === 内部パラメータ ======================================
	int fireCountAttack_A = 0;

	// === コード（AI思考処理） =================================
	public override void FixedUpdateAI () {
		// プレイヤーが来たら逃げる
		enemyCtrl.ActionMoveToFar (player, aiPlayerEscapeDistance);

		// AIステート
//		Debug.Log (string.Format(">>> aists {0}",aiState)); // 常に表示すると遅くなるので注意
		switch (aiState) {
		case ENEMYAISTS.ACTIONSELECT	: // 思考の起点
			// アクションの選択
			int n = SelectRandomAIState();
			if (n < aiIfATTACKONSIGHT) {
				SetAIState(ENEMYAISTS.ATTACKONSIGHT,100.0f);
			} else
			if (n < aiIfATTACKONSIGHT + aiIfRUNTOPLAYER) {
				SetAIState(ENEMYAISTS.RUNTOPLAYER,3.0f);
			} else
			if (n < aiIfATTACKONSIGHT + aiIfRUNTOPLAYER + aiIfESCAPE) {
				SetAIState(ENEMYAISTS.ESCAPE,Random.Range(2.0f,5.0f));
			} else
			if (n < aiIfATTACKONSIGHT + aiIfRUNTOPLAYER + aiIfESCAPE + aiIfRETURNTODOGPILE) {
				if (dogPile != null) {
					SetAIState(ENEMYAISTS.RETURNTODOGPILE,3.0f);
				}
			} else {
				SetAIState(ENEMYAISTS.WAIT,1.0f + Random.Range(0.0f,1.0f));
			}
			enemyCtrl.ActionMove (0.0f);
			break;

		case ENEMYAISTS.WAIT			: // 休憩
			enemyCtrl.ActionLookup(player,0.1f);
			enemyCtrl.ActionMove (0.0f);
			break;

		case ENEMYAISTS.ATTACKONSIGHT 	: // その場で攻撃
			Attack_A();
			break;

		case ENEMYAISTS.RUNTOPLAYER		: // 近寄る
			if (!enemyCtrl.ActionMoveToNear(player,10.0f)) {
				Attack_A();
			}
			break;

		case ENEMYAISTS.ESCAPE			: // 遠ざかる
			if (!enemyCtrl.ActionMoveToFar(player,4.0f)) {
				Attack_A();
			}
			break;
		
		case ENEMYAISTS.RETURNTODOGPILE: // ドッグパイルに戻る
			if (enemyCtrl.ActionMoveToNear(dogPile,2.0f)) {
				if (GetDistanePlayer() < 2.0f) {
					Attack_A();
				}
			} else {
				SetAIState(ENEMYAISTS.ACTIONSELECT,1.0f);
			}
			break;
		}
	}

	// === コード（アクション処理） ==============================
	void Attack_A() {
		enemyCtrl.ActionLookup(player,0.1f);
		enemyCtrl.ActionMove (0.0f);
		enemyCtrl.ActionAttack("Attack_A",damageAttack_A);
		AppSound.instance.SE_ATK_A1.Play ();

		fireCountAttack_A ++;
		if (fireCountAttack_A >= fireAttack_A) {
			fireCountAttack_A = 0;
			SetAIState (ENEMYAISTS.FREEZ, waitAttack_A);
		}
	}
	
	// === コード（COMBAT AI対応処理） ==========================
	public override void SetCombatAIState(ENEMYAISTS sts) {
		base.SetCombatAIState (sts);
		switch (aiState) {
		case ENEMYAISTS.ACTIONSELECT	: break;
		case ENEMYAISTS.WAIT			: aiActionTimeLength = 1.0f + Random.Range(0.0f,1.0f); break;
		case ENEMYAISTS.RUNTOPLAYER		: aiActionTimeLength = 3.0f; break;
		case ENEMYAISTS.JUMPTOPLAYER	: aiActionTimeLength = 1.0f; break;
		case ENEMYAISTS.ESCAPE			: aiActionTimeLength = Random.Range(2.0f,5.0f); break;
		case ENEMYAISTS.RETURNTODOGPILE	: aiActionTimeLength = 3.0f; break;
		}
	}
}
