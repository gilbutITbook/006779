using UnityEngine;
using System.Collections;

public class PlayerBodyCollider : MonoBehaviour {

	PlayerController playerCtrl;

	void Awake () {
		playerCtrl = transform.parent.GetComponent<PlayerController> ();
	}

	void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log ("Player OnTriggerEnter2D : " + other.name);

		// トリガーチェック
		if (other.tag == "EnemyArm") {
			EnemyController enemyCtrl  = other.GetComponentInParent<EnemyController>();
			//Debug.Log(string.Format("EnemyArm Hit {0}",ec.attackEnable));
			if (enemyCtrl.attackEnabled) {
				enemyCtrl.attackEnabled = false;
				playerCtrl.dir = (playerCtrl.transform.position.x < enemyCtrl.transform.position.x) ? +1 : -1; 
				playerCtrl.AddForceAnimatorVx(-enemyCtrl.attackNockBackVector.x);
				playerCtrl.AddForceAnimatorVy( enemyCtrl.attackNockBackVector.y);
				playerCtrl.ActionDamage (enemyCtrl.attackDamage);
			}
		}
	}

}
