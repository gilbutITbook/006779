using UnityEngine;
using System.Collections;

public class BaseCharacterController : MonoBehaviour {

	// === 外部パラメータ（インスペクタ表示） =====================
	public Vector2			velocityMin				 = new Vector2(-100.0f,-100.0f);
	public Vector2			velocityMax				 = new Vector2(+100.0f,+50.0f);
	public bool				superArmor				 = false;
	public bool				superArmor_jumpAttackDmg = true;
	public GameObject[] 	fireObjectList;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public float		hpMax			= 10.0f;
	[System.NonSerialized] public float		hp	 			= 10.0f;
	[System.NonSerialized] public float		dir	 			= 1.0f;
	[System.NonSerialized] public float		speed			= 6.0f;
	[System.NonSerialized] public float 	basScaleX		= 1.0f;
	[System.NonSerialized] public bool		activeSts 		= false;
	[System.NonSerialized] public bool 		jumped 			= false;
	[System.NonSerialized] public bool 		grounded 		= false;
	[System.NonSerialized] public bool 		groundedPrev 	= false;
//	[System.NonSerialized] public bool 		freez			= false;
//	[System.NonSerialized] public float		freezStartTime 	= 0.0f;

	// === キャッシュ ==========================================
	[System.NonSerialized] public Animator	animator;
	protected Transform		groundCheck_L;
	protected Transform 	groundCheck_C;
	protected Transform 	groundCheck_R;

	// === 内部パラメータ ======================================
	protected float 	 	speedVx 			= 0.0f;
	protected float 		speedVxAddPower		= 0.0f;

	protected GameObject	groundCheck_OnRoadObject;
	protected GameObject	groundCheck_OnMoveObject;
	protected GameObject	groundCheck_OnEnemyObject;

	protected float			jumpStartTime		= 0.0f;

	protected float 		gravityScale 		= 10.0f;

	protected bool			addForceVxEnabled	= false;
	protected float			addForceVxStartTime = 0.0f;

	protected bool			addVelocityEnabled	= false;
	protected float			addVelocityVx 		= 0.0f;
	protected float			addVelocityVy 		= 0.0f;

	protected bool			setVelocityVxEnabled= false;
	protected bool			setVelocityVyEnabled= false;
	protected float			setVelocityVx 		= 0.0f;
	protected float			setVelocityVy 		= 0.0f;

	protected AudioSource[]	seAnimationList;


	protected Collider2D[][] 	groundCheckCollider = new Collider2D[3][];
	protected int[] 			gccMax = new int[3];
	public 	  bool 				groundCheckWork = true;

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


		groundCheckCollider [0] = new Collider2D[16];
		groundCheckCollider [1] = new Collider2D[16];
		groundCheckCollider [2] = new Collider2D[16];
	}

	protected virtual void Start () {
	}
	
	protected virtual void Update () {	
		// 落下チェック
		if (transform.position.y < -30.0f) {
			if (activeSts) {
				Dead(false); // 死亡
			}
		}
	}



	protected virtual void FixedUpdate () {	
		// 地面チェック
		groundedPrev = grounded;
		grounded 	 = false;

		groundCheck_OnRoadObject  = null;
		groundCheck_OnMoveObject  = null;
		groundCheck_OnEnemyObject = null;

#if xxx
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
						if (groundCheck.tag == "EnemyObject") {
							groundCheck_OnEnemyObject = groundCheck.gameObject;
						}
					}
				}
			}
		}
#endif
//#if xxx
		if (groundCheckWork) {
			gccMax[0] = Physics2D.OverlapPointNonAlloc (groundCheck_L.position,groundCheckCollider[0]);
			gccMax[1] = Physics2D.OverlapPointNonAlloc (groundCheck_C.position,groundCheckCollider[1]);
			gccMax[2] = Physics2D.OverlapPointNonAlloc (groundCheck_R.position,groundCheckCollider[2]);
			
			for(int i = 0;i < 3;i ++) {
				for(int j = 0;j < gccMax[i];j ++) {
					if (groundCheckCollider[i][j] != null) {
						if (!groundCheckCollider[i][j].isTrigger) {
							grounded = true;
							if (groundCheckCollider[i][j].tag == "Road") {
								groundCheck_OnRoadObject = groundCheckCollider[i][j].gameObject;
							} else 
							if (groundCheckCollider[i][j].tag == "MoveObject") {
								groundCheck_OnMoveObject = groundCheckCollider[i][j].gameObject;
							} else 
							if (groundCheckCollider[i][j].tag == "EnemyObject") {
								groundCheck_OnEnemyObject = groundCheckCollider[i][j].gameObject;
							}
						}
					}
				}
			}
		} else {
			groundCheckWork = true;
			grounded = true;
			return;
		}
//#endif

		// キャラ個別の処理を実行
		//Debug.Log(string.Format("ChrCom FixedUpdate {0} {1}",name,grounded));
		FixedUpdateCharacter (); 

		// 乗り物チェック
		if (grounded) {
			speedVxAddPower = 0.0f;
			if (groundCheck_OnMoveObject != null) {
				Rigidbody2D rb2D = groundCheck_OnMoveObject.GetComponentInParent<Rigidbody2D>();
				speedVxAddPower = rb2D.velocity.x;
			}
		}

		// 移動計算
		if (addForceVxEnabled) {
			// 移動計算は物理演算にまかせる
			if (Time.fixedTime - addForceVxStartTime > 0.5f) {
				addForceVxEnabled = false;
			}
		} else {
			// 移動計算
			//Debug.Log (">>>> " + string.Format("speedVx {0} y {1} g{2}",speedVx,rigidbody2D.velocity.y,grounded));
			rigidbody2D.velocity = new Vector2 (speedVx + speedVxAddPower, rigidbody2D.velocity.y);
		}

		// 最終的なVelocity計算
		if (addVelocityEnabled) {
			addVelocityEnabled = false;
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x + addVelocityVx, rigidbody2D.velocity.y + addVelocityVy);
		}

		// 強制的にVelocityの値をセット
		if (setVelocityVxEnabled) {
			setVelocityVxEnabled = false;
			rigidbody2D.velocity = new Vector2 (setVelocityVx, rigidbody2D.velocity.y);
		}
		if (setVelocityVyEnabled) {
			setVelocityVyEnabled = false;
			rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x,setVelocityVy);
		}

		// Veclocityの値をチェック
		float vx = Mathf.Clamp (rigidbody2D.velocity.x, velocityMin.x, velocityMax.x);
		float vy = Mathf.Clamp (rigidbody2D.velocity.y, velocityMin.y, velocityMax.y);
		rigidbody2D.velocity = new Vector2(vx,vy);
	}

	protected virtual void FixedUpdateCharacter () {	
	}

	// === コード（アニメーションイベント用コード） ===============
	public virtual void AddForceAnimatorVx(float vx) {
		//Debug.Log (string.Format("--- AddForceAnimatorVx {0} ----------------",vx));
		if (vx != 0.0f) {
			rigidbody2D.AddForce (new Vector2(vx * dir,0.0f));
			addForceVxEnabled	= true;
			addForceVxStartTime = Time.fixedTime;
		}
	}
	
	public virtual void AddForceAnimatorVy(float vy) {
		//Debug.Log (string.Format("--- AddForceAnimatorVy {0} ----------------",vy));
		if (vy != 0.0f) {
			rigidbody2D.AddForce (new Vector2(0.0f,vy));
			jumped = true;
			jumpStartTime = Time.fixedTime;
		}
	}
	
	public virtual void AddVelocityVx(float vx) {
		//Debug.Log (string.Format("--- AddVelocityVx {0} ----------------",vx));
		addVelocityEnabled = true;
		addVelocityVx = vx * dir;
	}
	public virtual void AddVelocityVy(float vy) {
		//Debug.Log (string.Format("--- AddVelocityVy {0} ----------------",vy));
		addVelocityEnabled = true;
		addVelocityVy = vy;
	}
	
	public virtual void SetVelocityVx(float vx) {
		//Debug.Log (string.Format("--- setelocityVx {0} ----------------",vx));
		setVelocityVxEnabled = true;
		setVelocityVx = vx * dir;
	}
	public virtual void SetVelocityVy(float vy) {
		//Debug.Log (string.Format("--- setVelocityVy {0} ----------------",vy));
		setVelocityVyEnabled = true;
		setVelocityVy = vy;
	}
	
	public virtual void SetLightGravity() {
		//Debug.Log ("--- SetLightGravity ----------------");
		rigidbody2D.velocity 	 = Vector2.zero;
		rigidbody2D.gravityScale = 0.1f;
	}
	
	public void EnableFreez() {
		//Debug.Log ("--- EnableFreez ----------------");
		//freez = true;
		//freezStartTime = Time.fixedTime;
	}
	public void DisableFreez() {
		//Debug.Log ("--- DisableFreez ----------------");
		//freez = false;
	}
	
	public void EnableSuperArmor() {
		//Debug.Log ("--- EnableSuperArmor ----------------");
		superArmor = true;
	}
	public void DisableSuperArmor() {
		//Debug.Log ("--- DisableSuperArmor ----------------");
		superArmor = false;
	}
	
	public virtual void PlayAnimationSE(int i) {
		//Debug.Log (string.Format("--- PlayAnimationSE {0} ----------------",i));
		seAnimationList [i].Play ();
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

	public void ActionFire() {
		Transform goFire = transform.Find ("Muzzle");
		foreach(GameObject fireObject in fireObjectList) {
			GameObject go = Instantiate (fireObject,goFire.position,Quaternion.identity) as GameObject;
			go.GetComponent<FireBullet>().ownwer = transform;
		}
	}
	
	public bool ActionLookup(GameObject go,float near) {
		if (Vector3.Distance(transform.position,go.transform.position) > near) {
			dir = (transform.position.x < go.transform.position.x) ? +1 : -1;
			return true;
		}
		return false;
	}
	
	public bool ActionMoveToNear(GameObject go,float near) {
		if (Vector3.Distance(transform.position,go.transform.position) > near) {
			ActionMove( (transform.position.x < go.transform.position.x) ? +1.0f : -1.0f );
			return true;
		}
		return false;
	}
	
	public bool ActionMoveToFar(GameObject go,float far) {
		if (Vector3.Distance(transform.position,go.transform.position) < far) {
			ActionMove( (transform.position.x > go.transform.position.x) ? +1.0f : -1.0f );
			return true;
		}
		return false;
	}

	// === コード（その他） ====================================
	public virtual void Dead (bool gameOver) {
		animator.SetTrigger ("Dead");
		activeSts = false;
	}

	public virtual bool SetHP(float _hp,float _hpMax) {
		hp 	  		= _hp;
		hpMax 		= _hpMax;
		return (hp <= 0);
	}
}
