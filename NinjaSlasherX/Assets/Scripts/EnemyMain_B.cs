using UnityEngine;
using System.Collections;

public class EnemyMain_B : EnemyMain {

	// === 外部パラメータ（インスペクタ表示） =====================
	public int aiIfRUNTOPLAYER 			= 30;
	public int aiIfESCAPE 				= 20;
	public int aiIfRETURNTODOGPILE 		= 10;

	public int damageAttack_A 			= 1;
	public int damageAttack_B 			= 2;

	// === コード（AI思考処理） =================================
	public override void FixedUpdateAI () {
		// AIステート
//		Debug.Log (string.Format(">>> aists {0}",aiState)); // 常に表示すると遅くなるので注意
		switch (aiState) {
		case ENEMYAISTS.ACTIONSELECT	: // 思考の起点
			// アクションの選択
			int n = SelectRandomAIState();
			if (n < aiIfRUNTOPLAYER) {
				SetAIState(ENEMYAISTS.RUNTOPLAYER,3.0f);
			} else
			if (n < aiIfRUNTOPLAYER + aiIfESCAPE) {
				SetAIState(ENEMYAISTS.ESCAPE,Random.Range(2.0f,5.0f));
			} else
			if (n < aiIfRUNTOPLAYER + aiIfESCAPE + aiIfRETURNTODOGPILE) {
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

		case ENEMYAISTS.RUNTOPLAYER		: // 近寄る
			if (GetDistanePlayerY() < 3.0f) {
				if (!enemyCtrl.ActionMoveToNear(player,2.0f)) {
					Attack_A();
				}
			} else {
				if (GetDistanePlayerX() > 3.0f && !enemyCtrl.ActionMoveToNear(player,5.0f)) {
					Attack_A();
				}
			}
			break;

		case ENEMYAISTS.ESCAPE			: // 遠ざかる
			if (!enemyCtrl.ActionMoveToFar(player,4.0f)) {
				Attack_B();
			}
			break;
		
		case ENEMYAISTS.RETURNTODOGPILE	: // ドッグパイルに戻る
			if (!enemyCtrl.ActionMoveToNear(dogPile,2.0f)) {
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
		enemyCtrl.attackNockBackVector = new Vector2(500.0f,2000.0f);
		SetAIState(ENEMYAISTS.WAIT,3.0f);
		
		AppSound.instance.SE_ATK_B2.Play ();
	}

	void Attack_B() {
		enemyCtrl.ActionLookup(player,0.1f);
		enemyCtrl.ActionMove (0.0f);
		enemyCtrl.ActionAttack("Attack_B",damageAttack_B);
		enemyCtrl.attackNockBackVector = new Vector2(500.0f,1000.0f);
		SetAIState(ENEMYAISTS.FREEZ,5.0f);
		
		AppSound.instance.SE_ATK_ARIAL.Play ();
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
