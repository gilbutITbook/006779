using UnityEngine;
using System.Collections;

public enum ENEMYAISTS // --- 敵のAIステート ---
{
	ACTIONSELECT,		// アクション選択（思考）
	WAIT,				// 一定時間（止まって）待つ
	RUNTOPLAYER,		// 走ってプレイヤーに近づく
	JUMPTOPLAYER,		// ジャンプしてプレイヤーに近づく
	ESCAPE,				// プレイヤーから逃げる
	RETURNTODOGPILE,	// ドッグパイルに戻る
	ATTACKONSIGHT,		// その場から移動せずに攻撃する（遠距離攻撃用）
	FREEZ,				// 行動停止（ただし移動処理は継続する）
}

public class EnemyMain : MonoBehaviour {

	// === 外部パラメータ（インスペクタ表示） =====================
	public 		bool				cameraSwitch 			= true;
	public		bool				inActiveZoneSwitch		= false;
	public		bool				combatAIOerder			= true;
	public 		float 				dogPileReturnLength 	= 10.0f;

	public		int					debug_SelectRandomAIState = -1;

	// === 外部パラメータ ======================================
	[System.NonSerialized] public bool		  	cameraEnabled 	= false;
	[System.NonSerialized] public bool		  	inActiveZone	= false;
	[System.NonSerialized] public ENEMYAISTS 	aiState			= ENEMYAISTS.ACTIONSELECT;
	[System.NonSerialized] public GameObject	dogPile;

	// === キャッシュ ==========================================
	protected 	EnemyController 	enemyCtrl;
	protected 	GameObject		 	player;
	protected 	PlayerController 	playerCtrl;

	// === 内部パラメータ ======================================
	protected 	float				aiActionTimeLength		= 0.0f;
	protected 	float				aiActionTImeStart		= 0.0f;
	protected 	float				distanceToPlayer 		= 0.0f;
	protected 	float				distanceToPlayerPrev	= 0.0f;

	// === コード（Monobehaviour基本機能の実装） ================
	public virtual void Awake() {
		enemyCtrl 	 	= GetComponent <EnemyController>();
		player 			= PlayerController.GetGameObject ();
		playerCtrl 		= player.GetComponent<PlayerController>();
	}

	public virtual void Start () {
		// Dog Pile Set
		StageObject_DogPile[] dogPileList = GameObject.FindObjectsOfType<StageObject_DogPile>();
		foreach(StageObject_DogPile findDogPile in dogPileList) {
			foreach(GameObject go in findDogPile.enemyList) {
				if (gameObject == go) {
					dogPile = findDogPile.gameObject;
					break;
				}
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
	}
	void OnTriggerStay2D(Collider2D other) {
		// 状態チェック
		if (enemyCtrl.grounded && CheckAction ()) {
			if (other.name == "EnemyJumpTrigger_L") {
				if (enemyCtrl.ActionJump ()) {
					enemyCtrl.ActionMove(-1.0f);
				}
			} else
			if (other.name == "EnemyJumpTrigger_R") {
				if (enemyCtrl.ActionJump ()) {
					enemyCtrl.ActionMove(+1.0f);
				}
			} else
			if (other.name == "EnemyJumpTrigger") {
				enemyCtrl.ActionJump ();
			} 

			//Debug.Log ("Enemy OnTriggerEnter2D : " + other.name);
		}
	}
	void OnTriggerExit2D(Collider2D other) {
	}
	
	public virtual void Update () {
		cameraEnabled = false;
	}

	public virtual void FixedUpdate () {
		if (BeginEnemyCommonWork ()) {
			FixedUpdateAI();
			EndEnemyCommonWork ();
		}
	}

	public virtual void FixedUpdateAI () {
	}


	// === コード（基本AI動作処理） =============================
	public bool BeginEnemyCommonWork () {
		// 生きているか?
		if (enemyCtrl.hp <= 0) {
			return false;
		}

		// アクティブゾーンに入っているか？
		if (inActiveZoneSwitch) {
			inActiveZone = false;
			Vector3 vecA = player.transform.position + playerCtrl.enemyActiveZonePointA;
			Vector3 vecB = player.transform.position + playerCtrl.enemyActiveZonePointB;
			if (transform.position.x > vecA.x && transform.position.x < vecB.x && 
			    transform.position.y > vecA.y && transform.position.y < vecB.y) {
				inActiveZone  = true;
			}
		}

		// 空中は強制実行（空中設置敵・エリアル対応）
		if (enemyCtrl.grounded) {
			// カメラ内に入っているか？
			if (cameraSwitch && !cameraEnabled && !inActiveZone) {
				// カメラに映っていない
				enemyCtrl.ActionMove (0.0f);
				enemyCtrl.cameraRendered 	= false;
				enemyCtrl.animator.enabled 	= false;
				rigidbody2D.Sleep ();
				enemyCtrl.groundCheckWork = false;
				return false;
			}
		}
		enemyCtrl.animator.enabled 	= true;
		enemyCtrl.cameraRendered 	= true;


		// 状態チェック
		if (!CheckAction ()) {
			return false;
		}

		#if xxx
		// 本当は、この処理が必要だが、斜めジャンプができなくなるので、DMG_Bのアニメーションの長さで調整
		if (!enemyCtrl.grounded) {
			enemyCtrl.ActionMove (0.0f);
			return false;
		}
		#endif		

		// ドッグパイル
		if (dogPile != null) {
			if (GetDistaneDogPile() > dogPileReturnLength) {
				aiState = ENEMYAISTS.RETURNTODOGPILE;
			}
		}

		return true;
	}

	public void EndEnemyCommonWork() {
		// アクションのリミット時間をチェック
		float time = Time.fixedTime - aiActionTImeStart;
		if (time > aiActionTimeLength) {
			aiState = ENEMYAISTS.ACTIONSELECT;
		}
	}

	public bool CheckAction() {
		// 状態チェック
		AnimatorStateInfo stateInfo = enemyCtrl.animator.GetCurrentAnimatorStateInfo(0);

		if (stateInfo.tagHash  == EnemyController.ANITAG_ATTACK ||
		    stateInfo.nameHash == EnemyController.ANISTS_DMG_A ||
		    stateInfo.nameHash == EnemyController.ANISTS_DMG_B ||
		    stateInfo.nameHash == EnemyController.ANISTS_Dead) {
			return false;
		}

#if xxx
		if (enemyCtrl.freez) {
			if (enemyCtrl.freezStartTime + 3.0f < Time.fixedTime) {
				enemyCtrl.freezStartTime = 0.0f;
				enemyCtrl.freez = false;
			}
			return false;
		}
#endif

		return true;
	}

	public int SelectRandomAIState() {
#if UNITY_EDITOR
		if (debug_SelectRandomAIState >= 0) {
			return debug_SelectRandomAIState;
		}
#endif
		return Random.Range (0, 100 + 1);
	}

	public void SetAIState(ENEMYAISTS sts,float t) {
		aiState 			= sts;
		aiActionTImeStart  	= Time.fixedTime;
		aiActionTimeLength 	= t;
	}
	
	public virtual void SetCombatAIState(ENEMYAISTS sts) {
		aiState 		  = sts;
		aiActionTImeStart = Time.fixedTime;
		enemyCtrl.ActionMove (0.0f);
	}

	// === コード（AIスクリプトサポート関数） ====================
	public float GetDistanePlayer() {
		distanceToPlayerPrev = distanceToPlayer;
		distanceToPlayer = Vector3.Distance (transform.position, playerCtrl.transform.position);
		return distanceToPlayer;
	}

	public bool IsChangeDistanePlayer(float l) {
		return (Mathf.Abs(distanceToPlayer - distanceToPlayerPrev) > l);
	}

	public float GetDistanePlayerX() {
		Vector3 posA = transform.position;
		Vector3 posB = playerCtrl.transform.position;
		posA.y = 0; posA.z = 0;
		posB.y = 0; posB.z = 0;
		return Vector3.Distance (posA, posB);
	}
	
	public float GetDistanePlayerY() {
		Vector3 posA = transform.position;
		Vector3 posB = playerCtrl.transform.position;
		posA.x = 0; posA.z = 0;
		posB.x = 0; posB.z = 0;
		return Vector3.Distance (posA, posB);
	}

	public float GetDistaneDogPile() {
		return Vector3.Distance (transform.position, dogPile.transform.position);
	}
}
