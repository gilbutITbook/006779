using UnityEngine;
using System.Collections;

public class BaseCharacterController : MonoBehaviour {

	// === 外部パラメータ（インスペクタ表示） =====================
	public Vector2			velocityMin				 = new Vector2(-100.0f,-100.0f);
	public Vector2			velocityMax				 = new Vector2(+100.0f,+50.0f);

	// === 外部パラメータ ======================================
	[System.NonSerialized] public float		hpMax			= 10.0f;
	[System.NonSerialized] public float		hp	 			= 10.0f;
	[System.NonSerialized] public float		dir	 			= 1.0f;
	[System.NonSerialized] public float		speed			= 6.0f;
	[System.NonSerialized] public float 	basScaleX		= 1.0f;
	[System.NonSerialized] public bool		activeSts	 	= false;
	[System.NonSerialized] public bool 		jumped 			= false;
	[System.NonSerialized] public bool 		grounded 		= false;
	[System.NonSerialized] public bool 		groundedPrev 	= false;

	// === キャッシュ ==========================================
	[System.NonSerialized] public Animator	animator;
	protected Transform		groundCheck_L;
	protected Transform 	groundCheck_C;
	protected Transform 	groundCheck_R;

	// === 内部パラメータ ======================================
	protected float 	 	speedVx 			= 0.0f;
	protected float 		speedVxAddPower		= 0.0f;
	protected float 		gravityScale 		= 10.0f;
	protected float 		jumpStartTime		= 0.0f;

	protected GameObject	groundCheck_OnRoadObject;
	protected GameObject	groundCheck_OnMoveObject;
	protected GameObject	groundCheck_OnEnemyObject;

	// === コード（Monobehaviour基本機能の実装） ================
	protected virtual void Awake () {
		animator 			= GetComponent <Animator>();
		groundCheck_L 		= transform.Find("GroundCheck_L");
		groundCheck_C 		= transform.Find("GroundCheck_C");
		groundCheck_R 		= transform.Find("GroundCheck_R");

		dir 				= (transform.localScale.x > 0.0f) ? 1 : -1;
		basScaleX 			= transform.localScale.x * dir;
		transform.localScale = new Vector3 (basScaleX, transform.localScale.y, transform.localScale.z);

		activeSts 			= true;
		gravityScale 		= rigidbody2D.gravityScale;
	}

	protected virtual void Start () {
	}
	
	protected virtual void Update () {	
	}

	protected virtual void FixedUpdate () {	
		// 落下チェック
		if (transform.position.y < -30.0f) {
			Dead(false); // 死亡
		}

		// 地面チェック
		groundedPrev = grounded;
		grounded 	 = false;

		groundCheck_OnRoadObject = null;
		groundCheck_OnMoveObject = null;
		groundCheck_OnEnemyObject = null;

		Collider2D[][] groundCheckCollider = new Collider2D[3][];
		groundCheckCollider [0] = Physics2D.OverlapPointAll (groundCheck_L.position);
		groundCheckCollider [1] = Physics2D.OverlapPointAll (groundCheck_C.position);
		groundCheckCollider [2] = Physics2D.OverlapPointAll (groundCheck_R.position);

		foreach(Collider2D[] groundCheckList in groundCheckCollider) {
			foreach(Collider2D groundCheck in groundCheckList) {
				if (groundCheck != null) {
					if (!groundCheck.isTrigger) {
						grounded = true;
						if (groundCheck.tag == "Road") {
							groundCheck_OnRoadObject = groundCheck.gameObject;
						} else 
						if (groundCheck.tag == "MoveObject") {
							groundCheck_OnMoveObject = groundCheck.gameObject;
						} else 
						if (groundCheck.tag == "Enemy") {
							groundCheck_OnEnemyObject = groundCheck.gameObject;
						}
					}
				}
			}
		}

		// キャラクタ個別の処理
		FixedUpdateCharacter (); 

		// 移動計算
		rigidbody2D.velocity = new Vector2 (speedVx, rigidbody2D.velocity.y);

		// Veclocityの値をチェック
		float vx = Mathf.Clamp (rigidbody2D.velocity.x, velocityMin.x, velocityMax.x);
		float vy = Mathf.Clamp (rigidbody2D.velocity.y, velocityMin.y, velocityMax.y);
		rigidbody2D.velocity = new Vector2(vx,vy);
	}

	protected virtual void FixedUpdateCharacter () {	
	}

	// === コード（基本アクション） =============================
	public virtual void ActionMove(float n) {
		if (n != 0.0f) {
			dir 	= Mathf.Sign(n);
			speedVx = speed * n;
			animator.SetTrigger("Run");
		} else {
			speedVx = 0;
			animator.SetTrigger("Idle");
		}
	}
	
	// === コード（その他） ====================================
	public virtual void Dead (bool gameOver) {
		if (!activeSts) {
			return;
		}
		activeSts = false;

		animator.SetTrigger("Dead");
	}

	public virtual bool SetHP(float _hp,float _hpMax) {
		hp 	  		= _hp;
		hpMax 		= _hpMax;
		return (hp <= 0);
	}

}
