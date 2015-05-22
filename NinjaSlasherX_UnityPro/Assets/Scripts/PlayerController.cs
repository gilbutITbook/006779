using UnityEngine;
using System.Collections;

public class PlayerController : BaseCharacterController {

	// === 外部パラメータ（インスペクタ表示） =====================
						 public float 	initHpMax = 20.0f;
	[Range(0.1f,100.0f)] public float 	initSpeed = 12.0f;

	// === 外部パラメータ ======================================
	// セーブデータパラメータ
	public static	float 		nowHpMax 				= 0;
	public static	float 		nowHp 	  				= 0;
	public static 	int			score 					= 0;
	
	public static 	bool 		checkPointEnabled 		= false;
	public static 	string		checkPointSceneName 	= "";
	public static 	string		checkPointLabelName 	= "";
	public static	float 		checkPointHp 			= 0;
	
	public static 	bool 		itemKeyA				= false;
	public static 	bool 		itemKeyB 				= false;
	public static 	bool 		itemKeyC 				= false;

	// 外部からの処理操作用パラメータ
	public static 	bool		initParam 	  			= true;
	public static 	float		startFadeTime 			= 2.0f;
	
	// 基本パラメータ
	[System.NonSerialized] public float		groundY 	= 0.0f;
	[System.NonSerialized] public bool		superMode	= false;

	[System.NonSerialized] public int 		comboCount	= 0;

	[System.NonSerialized] public Vector3 	enemyActiveZonePointA;
	[System.NonSerialized] public Vector3 	enemyActiveZonePointB;

	// アニメーションのハッシュ名
	public readonly static int ANISTS_Idle 	 		= Animator.StringToHash("Base Layer.Player_Idle");
	public readonly static int ANISTS_Walk 	 		= Animator.StringToHash("Base Layer.Player_Walk");
	public readonly static int ANISTS_Run 	 	 	= Animator.StringToHash("Base Layer.Player_Run");
	public readonly static int ANISTS_Jump 	 		= Animator.StringToHash("Base Layer.Player_Jump");
	public readonly static int ANISTS_ATTACK_A 		= Animator.StringToHash("Base Layer.Player_ATK_A");
	public readonly static int ANISTS_ATTACK_B 		= Animator.StringToHash("Base Layer.Player_ATK_B");
	public readonly static int ANISTS_ATTACK_C	 	= Animator.StringToHash("Base Layer.Player_ATK_C");
	public readonly static int ANISTS_ATTACKJUMP_A  = Animator.StringToHash("Base Layer.Player_ATKJUMP_A");
	public readonly static int ANISTS_ATTACKJUMP_B  = Animator.StringToHash("Base Layer.Player_ATKJUMP_B");
	public readonly static int ANISTS_DEAD  		= Animator.StringToHash("Base Layer.Player_Dead");

	// === キャッシュ ==========================================
	LineRenderer	hudHpBar;
	TextMesh		hudScore;
	TextMesh 		hudCombo;

	// === 内部パラメータ ======================================
	int 			jumpCount			= 0;

	volatile bool 	atkInputEnabled		= false;
	volatile bool	atkInputNow			= false;

	bool			breakEnabled		= true;
	float 			groundFriction		= 0.0f;

	float 			comboTimer 			= 0.0f;
	

	// === コード（サポート関数） ===============================
	public static GameObject GetGameObject() {
		return GameObject.FindGameObjectWithTag ("Player");
	}
	public static Transform GetTranform() {
		return GameObject.FindGameObjectWithTag ("Player").transform;
	}
	public static PlayerController GetController() {
		return GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController>();
	}
	public static Animator GetAnimator() {
		return GameObject.FindGameObjectWithTag ("Player").GetComponent<Animator>();
	}

	// === コード（Monobehaviour基本機能の実装） ================
	protected override void Awake () {
		base.Awake ();

		#if xxx
		Debug.Log (">>> ANISTS_Idle : " + ANISTS_Idle);
		Debug.Log (">>> ANISTS_Walk : " + ANISTS_Walk);
		Debug.Log (">>> ANISTS_Run : " + ANISTS_Run);
		Debug.Log (">>> ANISTS_Jump : " + ANISTS_Jump);
		Debug.Log (">>> ANITAG_ATTACK_A : " + ANISTS_ATTACK_A);
		Debug.Log (">>> ANITAG_ATTACK_B : " + ANISTS_ATTACK_B);
		Debug.Log (">>> ANITAG_ATTACK_C : " + ANISTS_ATTACK_C);
		Debug.Log(string.Format("0 -> {0}",playerAnim.GetLayerName (0)));
		Debug.Log(string.Format("1 -> {0}",playerAnim.GetLayerName (1)));
		#endif

		// !!! ガベコレ強制実行 !!!
		System.GC.Collect ();
		// !!!!!!!!!!!!!!!!!!!!!

		// キャッシュ
		hudHpBar 		= GameObject.Find ("HUD_HPBar").GetComponent<LineRenderer> ();
		hudScore 		= GameObject.Find ("HUD_Score").GetComponent<TextMesh> ();
		hudCombo 		= GameObject.Find ("HUD_Combo").GetComponent<TextMesh> ();

		// パラメータ初期化
		speed 			= initSpeed;
		groundY 		= groundCheck_C.transform.position.y + 2.0f;

		// アクティブゾーンをBoxCollider2Dから取得
		BoxCollider2D boxCol2D = transform.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
		enemyActiveZonePointA = new Vector3 (boxCol2D.center.x - boxCol2D.size.x / 2.0f, boxCol2D.center.y - boxCol2D.size.y / 2.0f);
		enemyActiveZonePointB = new Vector3 (boxCol2D.center.x + boxCol2D.size.x / 2.0f, boxCol2D.center.y + boxCol2D.size.y / 2.0f);
		boxCol2D.transform.gameObject.SetActive(false);

		// コンティニューチェック
		if (SaveData.continuePlay) {
			// コンティニュー
			if (!SaveData.LoadGamePlay (true)) {
				initParam = false;
			}
			SaveData.continuePlay  = false;
		}
		if (initParam) {
			// ニュー（初めからプレイ）
			SetHP(initHpMax,initHpMax);
			PlayerController.score = 0;
			PlayerController.checkPointEnabled   = false;
			PlayerController.checkPointLabelName = "";
			PlayerController.checkPointSceneName = Application.loadedLevelName;
			PlayerController.checkPointHp 		 = initHpMax;
			PlayerController.itemKeyA 			 = false;
			PlayerController.itemKeyB 			 = false;
			PlayerController.itemKeyC 			 = false;
			SaveData.DeleteAndInit(false);
			SaveData.SaveGamePlay ();
			initParam = false;
		} else {
			// コンティニューでもニューでもないリンクからのジャンプの場合は
			// ステージの状態だけをセーブデータからロードする
			SaveData.LoadGamePlay (false);
		}
		if (SetHP(PlayerController.nowHp,PlayerController.nowHpMax)) {
			// HPがない場合は1からスタート
			SetHP(1,initHpMax);
		}

		// チェックポイントからの再開
		if (checkPointEnabled) {
			StageTrigger_CheckPoint[] triggerList = GameObject.Find("Stage").GetComponentsInChildren<StageTrigger_CheckPoint>();
			foreach(StageTrigger_CheckPoint trigger in triggerList) {
				if (trigger.labelName == checkPointLabelName) {
					transform.position = trigger.transform.position;
					groundY = transform.position.y;
					Camera.main.GetComponent<CameraFollow>().SetCamera(trigger.cameraParam);
					break;
				}
			}
		}
		Camera.main.transform.position = new Vector3(transform.position.x,
		                                             groundY,
		                                             Camera.main.transform.position.z);

		// Virtual Pad,HUD表示状態を設定
		GameObject.Find ("VRPad").SetActive (SaveData.VRPadEnabled);

		Transform hud = GameObject.FindGameObjectWithTag ("SubCamera").transform;
		hud.Find("Stage_Item_Key_A").GetComponent<SpriteRenderer>().enabled = itemKeyA;
		hud.Find("Stage_Item_Key_B").GetComponent<SpriteRenderer>().enabled = itemKeyB;
		hud.Find("Stage_Item_Key_C").GetComponent<SpriteRenderer>().enabled = itemKeyC;
	}

	protected override void Start() {
		base.Start ();

		zFoxFadeFilter.instance.FadeIn (Color.black, startFadeTime);
		startFadeTime = 2.0f;

		seAnimationList = new AudioSource[5];
		seAnimationList [0] = AppSound.instance.SE_ATK_A1;
		seAnimationList [1] = AppSound.instance.SE_ATK_A2;
		seAnimationList [2] = AppSound.instance.SE_ATK_A3;
		seAnimationList [3] = AppSound.instance.SE_ATK_ARIAL;
		seAnimationList [4] = AppSound.instance.SE_MOV_JUMP;
	}


	int hudScoreNum = -1;

	protected override void Update () {
		base.Update ();

		// ステータス表示
		hudHpBar.SetPosition (1, new Vector3 (5.0f * (hp / hpMax), 0.0f, 0.0f));
		if (hudScoreNum != score) {
			hudScore.text = string.Format("Score {0}",score);
			hudScoreNum = score; 
		}

		if (comboTimer <= 0.0f) {
			hudCombo.gameObject.SetActive(false);
			comboCount = 0;
			comboTimer = 0.0f;
		} else {
			comboTimer -= Time.deltaTime;
			if (comboTimer > 5.0f) {
				comboTimer = 5.0f;
			}
			float s = 0.3f + 0.5f * comboTimer;
			hudCombo.gameObject.SetActive(true);
			hudCombo.transform.localScale = new Vector3(s,s,1.0f);
		}

#if xxx
		// Debug
		BoxCollider2D boxCol2D = GameObject.Find("Collider_EnemyActiveZone").GetComponent<BoxCollider2D>();
		Vector3 vecA = transform.position + new Vector3 (boxCol2D.center.x - boxCol2D.size.x / 2.0f, boxCol2D.center.y - boxCol2D.size.y / 2.0f);
		Vector3 vecB = transform.position + new Vector3 (boxCol2D.center.x + boxCol2D.size.x / 2.0f, boxCol2D.center.y + boxCol2D.size.y / 2.0f);
		Collider2D[] col2DList = Physics2D.OverlapAreaAll (vecA,vecB);
		foreach(Collider2D col2D in col2DList) {
			if (col2D.tag == "EnemyBody") {
				col2D.GetComponentInParent<EnemyMain>().cameraEnabled = true;
			}
		}
#endif		
	}

	protected override void FixedUpdateCharacter () {
		// 現在のステートを取得
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

		// 着地チェック
		if (jumped) {
			if ((grounded && !groundedPrev) || 
				(grounded && Time.fixedTime > jumpStartTime + 1.0f)) {
				animator.SetTrigger ("Idle");
				jumped 	  = false;
				jumpCount = 0;
				rigidbody2D.gravityScale = gravityScale;
			}
			if (Time.fixedTime > jumpStartTime + 1.0f) {
				if (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || 
				    stateInfo.nameHash == ANISTS_Run  || stateInfo.nameHash == ANISTS_Jump) {
					rigidbody2D.gravityScale = gravityScale;
				}
			}
		} else {
			jumpCount = 0;
			rigidbody2D.gravityScale = gravityScale;
		}

		// 攻撃中か？
		if (stateInfo.nameHash == ANISTS_ATTACK_A || 
		    stateInfo.nameHash == ANISTS_ATTACK_B || 
		    stateInfo.nameHash == ANISTS_ATTACK_C || 
		    stateInfo.nameHash == ANISTS_ATTACKJUMP_A || 
		    stateInfo.nameHash == ANISTS_ATTACKJUMP_B) {
			// 移動停止
			speedVx = 0;
		}

#if xxx
		// キャラの方向（攻撃中やジャンプ中に振り向き禁止にする）
		if (stateInfo.nameHash != ANISTS_ATTACK_A && 
		    stateInfo.nameHash != ANISTS_ATTACK_B && 
		    stateInfo.nameHash != ANISTS_ATTACK_C && 
		    stateInfo.nameHash != ANISTS_ATTACKJUMP_A && 
		    stateInfo.nameHash != ANISTS_ATTACKJUMP_B) {
			transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);
		}
#else
		// キャラの方向
		transform.localScale = new Vector3 (basScaleX * dir, transform.localScale.y, transform.localScale.z);
#endif

		// ジャンプ中の横移動減速
		if (jumped && !grounded && groundCheck_OnMoveObject == null) {
			if (breakEnabled) {
				breakEnabled = false;
				speedVx *= 0.9f;
			}
		}

		// 移動停止（減速）処理
		if (breakEnabled) {
			speedVx *= groundFriction;
		}
	}

	// === コード（アニメーションイベント用コード） ===============
	public void EnebleAttackInput() {
		atkInputEnabled = true;
	}
	
	public void SetNextAttack(string name) {
		if (atkInputNow == true) {
			atkInputNow = false;
			animator.Play(name);
		}
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
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || stateInfo.nameHash == ANISTS_Run || 
		    (stateInfo.nameHash == ANISTS_Jump && rigidbody2D.gravityScale >= gravityScale)) {
			switch(jumpCount) {
			case 0 :
				if (grounded) {
					animator.SetTrigger ("Jump");
					//rigidbody2D.AddForce (new Vector2 (0.0f, 1500.0f));	// Bug
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
			//Debug.Log(string.Format("Jump 1 {0} {1} {2} {3}",jumped,transform.position,grounded,groundedPrev));
			//Debug.Log(groundCheckCollider[1].name);
			AppSound.instance.SE_MOV_JUMP.Play ();
		}
	}

	public void ActionAttack() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || stateInfo.nameHash == ANISTS_Run || 
		    stateInfo.nameHash == ANISTS_Jump || stateInfo.nameHash == ANISTS_ATTACK_C) {

			animator.SetTrigger ("Attack_A");
			if (stateInfo.nameHash == ANISTS_Jump || stateInfo.nameHash == ANISTS_ATTACK_C) {
				rigidbody2D.velocity     = new Vector2(0.0f,0.0f);
				rigidbody2D.gravityScale = 0.1f;
			}
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	public void ActionAttackJump() {
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (grounded && 
		    (stateInfo.nameHash == ANISTS_Idle || stateInfo.nameHash == ANISTS_Walk || stateInfo.nameHash == ANISTS_Run ||
		     stateInfo.nameHash == ANISTS_ATTACK_A || stateInfo.nameHash == ANISTS_ATTACK_B)) {
			animator.SetTrigger ("Attack_C");
			jumpCount = 2;
		} else {
			if (atkInputEnabled) {
				atkInputEnabled = false;
				atkInputNow 	= true;
			}
		}
	}

	public void ActionEtc() {
		Collider2D[] otherAll = Physics2D.OverlapPointAll (groundCheck_C.position);
		foreach (Collider2D other in otherAll) {
			if (other.tag == "EventTrigger") {
				StageTrigger_Link link = other.GetComponent<StageTrigger_Link>();
				if (link != null) {
					link.Jump();
				}
			} else
			if (other.tag == "KeyDoor") {
				StageObject_KeyDoor keydoor = other.GetComponent<StageObject_KeyDoor>();
				keydoor.OpenDoor();
			} else
			if (other.name == "Stage_Switch_Body") {
				StageObject_Switch sw = other.transform.parent.GetComponent<StageObject_Switch>();
				sw.SwitchTurn();
			}
		}
	}

	public void ActionDamage(float damage) {
		// Debug:無敵モード
		if (SaveData.debug_Invicible) {
			return;
		}
		// ダメージ処理をしてもいいか？
		if (!activeSts) {
			return;
		}

#if xxx
		// ランダムにヒット音を再生
		switch(Random.Range(0,3)) {
		case 0 : AppSound.instance.SE_HIT_A1.Play (); break;
		case 1 : AppSound.instance.SE_HIT_A2.Play (); break;
		case 2 : AppSound.instance.SE_HIT_A3.Play (); break;
		}
#else
		// ヒット音を再生
		AppSound.instance.SE_HIT_A1.Play ();
#endif

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID)
		Handheld.Vibrate();
#endif

		animator.SetTrigger ("DMG_A");
		speedVx = 0;
		rigidbody2D.gravityScale = gravityScale;

		// Combo Reset
		comboCount = 0;
		comboTimer = 0.0f;

		if (jumped) {
			damage *= 1.5f;
		}

		if (SetHP(hp - damage,hpMax)) {
			Dead(true); // 死亡
		}
	}

	// === コード（その他） ====================================
	public override void Dead(bool gameOver) {
		// 死亡処理をしてもいいか？
		AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
		if (!activeSts || stateInfo.nameHash == ANISTS_DEAD) {
			return;
		}

		base.Dead (gameOver);

		zFoxFadeFilter.instance.FadeOut (Color.black, 2.0f);

		if (gameOver) {
			SetHP(0,hpMax);
			Invoke ("GameOver", 3.0f);
		} else {
			SetHP(hp / 2,hpMax);
			Invoke ("GameReset", 3.0f);
		}

		GameObject.Find ("HUD_Dead").GetComponent<MeshRenderer> ().enabled = true;
		GameObject.Find ("HUD_DeadShadow").GetComponent<MeshRenderer> ().enabled = true;
		if (GameObject.Find ("VRPad") != null) {
			GameObject.Find ("VRPad").SetActive(false);
		}
	}

	public void GameOver() {
		SaveData.SaveHiScore(score);
		PlayerController.score = 0;
		PlayerController.nowHp = PlayerController.checkPointHp;
		SaveData.SaveGamePlay ();

		AppSound.instance.fm.Stop ("BGM");
		if (SaveData.newRecord > 0) {
			AppSound.instance.BGM_HISCORE_RANKIN.Play ();
		} else {
			AppSound.instance.BGM_HISCORE.Play ();
		}

		Application.LoadLevel("Menu_HiScore");
	}

	void GameReset() {
		SaveData.SaveGamePlay ();
		Application.LoadLevel(Application.loadedLevelName);
	}

	public override bool SetHP(float _hp,float _hpMax) {
		if (_hp > _hpMax) {
			_hp = _hpMax;
		}
		nowHp 		= _hp;
		nowHpMax 	= _hpMax;
		return base.SetHP (_hp, _hpMax);
	}

	public void AddCombo() {
		comboCount ++;
		comboTimer += 1.0f;
		hudCombo.text = string.Format("Combo {0}",comboCount);
	}
}


