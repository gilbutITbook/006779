using UnityEngine;
using System.Collections;

public class Player_Physics2DAndMecanim : MonoBehaviour {

	// --- 宣言 ------------------------------------------------------------
	
	// インスペクタで調整するためのプロパティ
	public float 	speed 		= 12.0f;	// プレイヤーキャラの速度
	public float 	jumpPower 	= 1600.0f;	// プレイヤーをジャンプさせるときのパワー

	// 内部で使う変数
	bool 			grounded;		// 接地チェック
	bool 			goalCheck;		// ゴールチェック
	float 			goalTime;		// ゴールタイム

	// --- メッセージに対応したコード -----------------------------------------
	
	// コンポーネントの実行開始
	void Start () {
		// 初期化
		grounded 	= false;
		goalCheck 	= false;
	}

	// プレイヤーキャラのコリジョンに他のゲームオブジェクトのコリジョンが入った
	void OnCollisionEnter2D(Collision2D col) {
		// ゴールチェック
		if (col.gameObject.name == "Stage_Gate") {
			// ゴール！！
			goalCheck 	= true;
			goalTime 	= Time.timeSinceLevelLoad;
		}
	}

	// フレームの書き換え
	void Update () {
		// 地面チェック
		Transform 	groundCheck = transform.Find ("GroundCheck");
		grounded = (Physics2D.OverlapPoint (groundCheck.position) != null) ? true : false;  
		if (grounded) {
			// ジャンプボタンチェック
			if (Input.GetButtonDown ("Fire1")) {
				// ジャンプ処理
				GetComponent<Rigidbody2D>().AddForce ( new Vector2 (0.0f,jumpPower) );
			}
			// 走りアニメーションを設定
			GetComponent <Animator>().SetTrigger("Run");
		} else {
			// ジャンプアニメーションを設定
			GetComponent <Animator>().SetTrigger("Jump");
		}
		// 穴に落ちたか？
		if (transform.position.y < -10.0f) {
			// 穴に落ちたらステージを再読み込みしてリセット
			Application.LoadLevel (Application.loadedLevelName);
		}
	}

	// フレームの書き換え
	void FixedUpdate () {
		// 移動計算
		GetComponent<Rigidbody2D>().velocity = new Vector2 (speed, GetComponent<Rigidbody2D>().velocity.y);
		// カメラ移動
		GameObject goCam = GameObject.Find ("Main Camera");
		goCam.transform.position = new Vector3 (transform.position.x + 5.0f, goCam.transform.position.y, goCam.transform.position.z);
	}
	
	// UnityGUIの表示
	void OnGUI() {
		// デバッグテキスト
		GUI.TextField(new Rect(10,10,300,60), "[Unity2D Sample 3-1 C]\nマウスの左ボタンを押すとジャンプ！");
		if (goalCheck) {
			GUI.TextField (new Rect (10, 100, 330, 60), string.Format ("***** Goal!! *****\nTime {0}", goalTime));
		}
		// リセットボタン
		if ( GUI.Button( new Rect(10, 80, 100, 20), "リセット" ) ) {
			Application.LoadLevel (Application.loadedLevelName);
		}
		// メニューに戻る
		if ( GUI.Button ( new Rect(10,110, 100, 20), "メニュー" ) ) {
			Application.LoadLevel ("SelectMenu");
		}
	}

}
