using UnityEngine;
using System.Collections;

public class PlayerController : BaseCharacterController {

	// === 外部パラメータ（インスペクタ表示） =====================
						 public float 	initHpMax = 20.0f;
	[Range(0.1f,100.0f)] public float 	initSpeed = 12.0f;

	// === 内部パラメータ ======================================
	int 			jumpCount			= 0;
	bool			breakEnabled		= true;
	float 			groundFriction		= 0.0f;

	// === コード（Monobehaviour基本機能の実装） ================
	protected override void Awake () {
		base.Awake ();

		// パラメータ初期化
		speed = initSpeed;
		SetHP(initHpMax,initHpMax);
	}
	
	protected override void FixedUpdateCharacter () {
		// 着地チェック
		if (jumped) {
			if ((grounded && !groundedPrev) || 
				(grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
				animator.SetTrigger ("Idle");
				jumped = false;
				jumpCount = 0;
			}
		} 
		if (!jumped) {
			jumpCount = 0;
		}

		// キャラの方向
		transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);

		// ジャンプ中の横移動減速
		if (jumped && !grounded) {
			if (breakEnabled) {
				breakEnabled = false;
				speedVx *= 0.9f;
			}
		}

		// 移動停止（減速）処理
		if (breakEnabled) {
			speedVx *= groundFriction;
		}

		// カメラ
		Camera.main.transform.position = transform.position - Vector3.forward;
	}

	// === コード（基本アクション） =============================
	public override void ActionMove(float n) {
		if (!activeSts) {
			return;
		}
		
		// 初期化
		float dirOld = dir;
		breakEnabled = false;
		
		// アニメーション指定
		float moveSpeed = Mathf.Clamp(Mathf.Abs (n),-1.0f,+1.0f);
		animator.SetFloat("MovSpeed",moveSpeed);
		//animator.speed = 1.0f + moveSpeed;
		
		// 移動チェック
		if (n != 0.0f) {
			// 移動
			dir 	  = Mathf.Sign(n);
			moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
			speedVx   = initSpeed * moveSpeed * dir;
		} else {
			// 移動停止
			breakEnabled = true;
		}
		
		// その場振り向きチェック
		if (dirOld != dir) {
			breakEnabled = true;
		}
	}

	public void ActionJump() {
		switch(jumpCount) {
		case 0 :
			if (grounded) {
				animator.SetTrigger ("Jump");
				rigidbody2D.velocity = Vector2.up * 30.0f;
				jumpStartTime = Time.fixedTime;
				jumped = true;
				jumpCount ++;
			}
			break;
		case 1 :
			if (!grounded) {
				animator.Play("Player_Jump",0,0.0f);
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,20.0f);
				jumped = true;
				jumpCount ++;
			}
			break;
		}
	}

}


