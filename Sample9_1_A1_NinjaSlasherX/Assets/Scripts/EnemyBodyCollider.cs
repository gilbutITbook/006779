using UnityEngine;
using System.Collections;

public class EnemyBodyCollider : MonoBehaviour {

	EnemyController 	enemyCtrl;
	Animator 			playerAnim;
	int 				attackHash = 0;

	void Awake () {
		enemyCtrl 	= GetComponentInParent<EnemyController>();
		playerAnim 	= PlayerController.GetAnimator();
	}

	void OnTriggerEnter2D(Collider2D other) {
		//Debug.Log ("Enemy OnTriggerEnter2D : " + other.name);
		if (enemyCtrl.cameraRendered) {
			if (other.tag == "PlayerArm") {
				AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
				if (attackHash != stateInfo.nameHash) {
					attackHash = stateInfo.nameHash;
					enemyCtrl.ActionDamage ();

					Camera.main.GetComponent<CameraFollow>().AddCameraSize(-0.01f,-0.3f);
				}
			} else
			if (other.tag == "PlayerArmBullet") {
				Destroy (other.gameObject);
				enemyCtrl.ActionDamage ();
			}
		}
	}

	void Update () {
		AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
		if (attackHash != 0 && stateInfo.nameHash == PlayerController.ANISTS_Idle) {
			attackHash = 0;
		}
	}

	void HitStop() {
		enemyCtrl.animator.speed = 1.0f;
		playerAnim.speed = 1.0f;
	}

}

