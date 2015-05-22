using UnityEngine;
using System.Collections;
public class Player_NonPhysics2D : MonoBehaviour {
	// --- 宣言 ---------------------------------------------------------------
	// Inspectorで調整するためのプロパティ
	public float speed = 15.0f; // プレイヤーキャラの速度
	public Sprite[] run; // プレイヤーキャラの走りプライト
	public Sprite[] jump; // プレイヤーキャラのジャンプスプライト
	// 内部で扱う変数
	float jumpVy; // プレイヤーキャラの上昇速度
	int animIndex; // プレイヤーキャラのアニメ再生インデックス
	bool goalCheck; // ゴールチェック
	// --- メッセージに対応したコード -----------------------------------------
	// コンポーネントの実行開始
	void Start() {
		// 初期化
		jumpVy = 0.0f;
		animIndex = 0;
		goalCheck = false;
	}
	// プレイヤーキャラのコリジョンに他のゲームオブジェクトのコリジョンが入った
	void OnCollisionEnter2D(Collision2D col) {
		// ゴールチェック
		if(col.gameObject.name == "Stage_Gate") {
			// ゴール！！
			goalCheck = true;
			return;
		}
		// ゴール以外だったらステージを再読み込みしてリセット
		Application.LoadLevel(Application.loadedLevelName);
	}
	// フレームの描き換え
	void Update() {
		if(goalCheck) { // ゴールチェック
			return; // ゴールだったら処理停止
		}
		// 現在のプレイヤーキャラの高さを計算
		float height = transform.position.y + jumpVy;
		// 接地チェック（高さが0なら接地している）
		if(height <= 0.0f) {
			// ジャンプ初期化
			height = 0.0f;
			jumpVy = 0.0f;
			
			// ジャンプチェック
			if(Input.GetButtonDown("Fire1")) {
				// ジャンプ処理
				jumpVy = +1.3f;
				// ジャンプスプライト画像に切り替え
				GetComponent<SpriteRenderer>().sprite = jump[0];
			} else {
				// 走り処理
				animIndex ++;
				if(animIndex >= run.Length) {
					animIndex = 0;
				}
				// 走りスプライト画像に切り替え
				GetComponent<SpriteRenderer>().sprite = run[animIndex];
			}
		} else {
			// ジャンプ後の降下中
			jumpVy -= 0.2f;
			// jumpVy -= 6.0f * Time.deltaTime; // 正しい処理はこちら
		}
		// プレイヤーキャラの移動（座標設定）
		transform.position = new Vector3(
			transform.position.x + speed * Time.deltaTime, height, 0.0f);
		// 下記のように相対移動で記述してもよい
		// transform.Translate(speed * Time.deltaTime, jumpVy, 0.0f);
		// transform.position += new Vector3(speed * Time.deltaTime, jumpVy, 0.0f);
		// ただし次の書き方では動かないので注意
		// transform.position.Set(
		// transform.position.x + speed * Time.deltaTime, height, 0.0f);
		// カメラの移動（座標の相対移動）
		GameObject goCam = GameObject.Find("Main Camera");
		goCam.transform.Translate(speed * Time.deltaTime, 0.0f, 0.0f);
	}
	// UnityGUIの表示
	void OnGUI() {
		// デバッグテキスト
		GUI.TextField(new Rect(10, 10, 300, 60),
		              "[Unity 2D Sample 3-1 A]\nマウスの左ボタンを押すと加速\nはなすとジャンプ！");
		// リセットボタン
		if(GUI.Button(new Rect(10, 80, 100, 20), "リセット")) {
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}