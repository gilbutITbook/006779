using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatAI : MonoBehaviour {

	// === 外部パラメータ（インスペクタ表示） =====================
	public int freeAIMax 		= 3;
	public int blockAttackAIMax = 10;

	// === コード（Monobehaviour基本機能の実装） ================
	void FixedUpdate () {
		var activeEnemyMainList = new List<EnemyMain>();

		// カメラに写っている敵を検索
		GameObject[] enemyList = GameObject.FindGameObjectsWithTag ("Enemy");
		if (enemyList == null) {
			return;
		}
		foreach (GameObject enemy in enemyList) {
			//Debug.Log (string.Format(">>> obj Name {0} position {1}",enemy.name,enemy.transform.position));
			EnemyMain enemyMain = enemy.GetComponent<EnemyMain> ();
			if (enemyMain != null) {
				if (enemyMain.combatAIOerder && enemyMain.cameraEnabled) {
					activeEnemyMainList.Add (enemyMain);
				}
			} else {
				Debug.LogWarning(string.Format("CombatAI : EnemyMain null : {0} {1}",enemy.name,enemy.transform.position));
			}
		}

		// 攻撃する敵を抑制
		int i = 0;
		foreach (EnemyMain enemyMain in activeEnemyMainList) {
			if (i < freeAIMax) {
				// そのまま自由に行動させる
			} else
			if (i < freeAIMax + blockAttackAIMax) {
				// 攻撃を抑制する
				if (enemyMain.aiState == ENEMYAISTS.RUNTOPLAYER) {
					enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
				}
			} else {
				// 行動を停止する
				if (enemyMain.aiState != ENEMYAISTS.WAIT) {
					enemyMain.SetCombatAIState(ENEMYAISTS.WAIT);
				}
			}
			i ++;
		}

		//Debug.Log(string.Format(">>> Combat AI {0}",i));
	}
}
