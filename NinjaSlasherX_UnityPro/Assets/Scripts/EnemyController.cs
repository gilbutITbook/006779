using UnityEngine;
using System.Collections;


public class EnemyController : BaseCharacterController {

	// === 外部パラメータ（インスペクタ表示） =====================
	public float 		initHpMax 			= 5.0f;
	public float 		initSpeed 			= 6.0f;
	public bool			jumpActionEnabled 	= true;
	public Vector2 		jumpPower		 	= new Vector2(0.0f,1500.0f); 
	public int			addScore 		 	= 500;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public bool 		cameraRendered			= false;
	[System.NonSerialized] public bool 		attackEnabled			= false;
	[System.NonSerialized] public int		attackDamage 			= 1;
	[System.NonSerialized] public Vector2	attackNockBackVector	= Vector3.zero;
	
	// アニメーションのハッシュ名
	public readonly static int ANISTS_Idle 	 	= Animator.StringToHash("Base Layer.Enemy_Idle");
	public readonly static int ANISTS_Run 	 	= Animator.StringToHash("Base Layer.Enemy_Run");
	public readonly static int ANISTS_Jump 	 	= Animator.StringToHash("Base Layer.Enemy_Jump");
	public readonly static int ANITAG_ATTACK 	= Animator.StringToHash("Attack");
	public readonly static int ANISTS_DMG_A		= Animator.StringToHash("Base Layer.Enemy_DMG_A");
	public readonly static int ANISTS_DMG_B 	= Animator.StringToHash("Base Layer.Enemy_DMG_B");
	public readonly static int ANISTS_Dead 	 	= Animator.StringToHash("Base Layer.Enemy_Dead");

	// === キャッシュ ==========================================
	PlayerController 	playerCtrl;
	Animator 			playerAnim;
	
	// === コード（Monobehaviour基本機能の実装） ================
	protected override void Awake () {
		base.Awake ();
#if xxx
		Debug.Log (">>> ANISTS_Idle : {0}" + ANISTS_Idle);
		Debug.Log (">>> ANISTS_Run : " + ANISTS_Run);
		Debug.Log (">>> ANISTS_Jump : " + ANISTS_Jump);
		Debug.Log (">>> ANITAG_ATTACK : " + ANITAG_ATTACK);
		Debug.Log (">>> ANISTS_DMG_A : " + ANISTS_DMG_A);
		Debug.Log (">>> ANISTS_DMG_B : " + ANISTS_DMG_B);
		Debug.Log (">>> ANISTS_Dead : " + ANISTS_Dead);
		Debug.Log(string.Format("0 -> {0}",animator.GetLayerName (0)));
		Debug.Log(string.Format("1 -> {0}",animator.GetLayerName (1)));
#endif
		playerCtrl 	= PlayerController.GetController();
		playerAnim 	= playerCtrl.GetComponent<Animator>();

		hpMax 	= initHpMax;
		hp 		= hpMax;
		speed 	= initSpeed;
	}

	protected override void Start() {
		base.Start ();

		seAnimationList = new AudioSource[5];
		seAnimationList [0] = AppSound.instance.SE_ATK_B1;
		seAnimationList [1] = AppSound.instance.SE_ATK_B2;
		seAnimationList [2] = AppSound.instance.SE_ATK_B3;
		seAnimationList [3] = AppSound.instance.SE_ATK_ARIAL;
		seAnimationList [4] = AppSound.instance.SE_ATK_SYURIKEN;
	}
	
	protected override void FixedUpdateCharacter () {
		if (!cameraRendered) {
			return;
		}

		// ジャンプチェック
		if (jumped) {
			// 着地チェック(A:接地瞬間判定 B:接地と時間による判定)
			if ((grounded && !groundedPrev) || 
			    (grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
				jumped = false;
			}
			if (Time.fixedTime > jumpStartTime + 1.0f) {
				if (rigidbody2D.gravityScale < gravityScale) {
					rigidbody2D.gravityScale = gravityScale;
				}
			}
		} else {
			rigidbody2D.gravityScale = gravityScale;
		}

		// キャラの方向
		transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);

		// Memo:空中ダメージではX移動を禁止
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == EnemyController.ANISTS_DMG_A ||
		    stateInfo.nameHash == EnemyController.ANISTS_DMG_B ||
		    stateInfo.nameHash == EnemyController.ANISTS_Dead) {
			speedVx = 0.0f;
			rigidbody2D.velocity = new Vector2 (0.0f, rigidbody2D.velocity.y);
		}
	}

	// === コード（基本アクション） =============================
	public bool ActionJump() {
		if (jumpActionEnabled && grounded && !jumped) {
			animator.SetTrigger ("Jump");
			rigidbody2D.AddForce (jumpPower);
			jumped 		  = true;
			jumpStartTime = Time.fixedTime;
		}
		return jumped;
	}
	
	public void ActionAttack(string atkname,int damage) {
		attackEnabled = true;
		attackDamage  = damage;
		animator.SetTrigger (atkname);
	}

	public void ActionDamage() {
		int damage = 0;

		if (hp <= 0) {
			return;
		}

		if (superArmor) {
			animator.SetTrigger ("SuperArmor");
		}

		AnimatorStateInfo stateInfo = playerAnim.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == PlayerController.ANISTS_ATTACK_C) {
			damage = 3;
			if (!superArmor || superArmor_jumpAttackDmg) {
				animator.SetTrigger ("DMG_B");
				jumped 			= true;
				jumpStartTime 	= Time.fixedTime;
				AddForceAnimatorVy (1500.0f);
				Debug.Log(string.Format(">>> DMG_B Jump {0}",stateInfo.nameHash));
			}
		} else
		if (!grounded) {
			damage = 2;
			if (!superArmor || superArmor_jumpAttackDmg) {
				animator.SetTrigger ("DMG_B");
				jumped 			= true;
				jumpStartTime 	= Time.fixedTime;
				//AddForceAnimatorVy (10.0f);
				playerCtrl.rigidbody2D.AddForce(new Vector2(0.0f,20.0f));
				Debug.Log(string.Format(">>> DMG_B {0}",stateInfo.nameHash));
			}
		} else {
			damage = 1;
			if (!superArmor) {
				animator.SetTrigger ("DMG_A");
				Debug.Log(string.Format(">>> DMG_A {0}",stateInfo.nameHash));
			}
		}

		damage *= (playerCtrl.superMode ? 5 : 1);
		if (SetHP(hp - damage,hpMax)) {
			Dead(false);

			int addScoreV = ((int)((float)addScore * (playerCtrl.hp / playerCtrl.hpMax))) * playerCtrl.comboCount;
			addScoreV = (int)((float)addScore * (grounded ? 1.0 : 1.5f));
			PlayerController.score += addScoreV;
		}

		playerCtrl.AddCombo();

		AppSound.instance.SE_HIT_A1.Play ();
	}

	// === コード（その他） ====================================
	public override void Dead(bool gameOver) {
		base.Dead (gameOver);
		Destroy(gameObject,(name != "EnemyD_Boss") ? 1.0f : 3.0f);
	}
}

