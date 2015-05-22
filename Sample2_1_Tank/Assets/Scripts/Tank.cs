using UnityEngine;
using System.Collections;

public class Tank : MonoBehaviour {

	GameObject 	goShell = null;
	bool		action 	= false;

	// Use this for initialization
	void Start () {
		// 砲弾のゲームオブジェクト取得と砲弾の非表示設定
		goShell = transform.FindChild("Tank_Shell").gameObject;
		goShell.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		// ボタンが押されたか？
		if (Input.GetMouseButton(0)) {
			// タンクがクリックされたか？
			Vector2 tapPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Collider2D collition2d = Physics2D.OverlapPoint(tapPoint);
			if (collition2d) {
				if (collition2d.gameObject == gameObject) {
					// アクションを有効にする
					action = true;
				}
			}
			// ボタンが押されたままか？
			if (action) {
				// タンク移動
				rigidbody2D.AddForce (new Vector2(+30.0f, 0.0f));
			}
		} else
		// ボタンが離されたか？
		if (Input.GetMouseButtonUp(0) && action) {
			// 砲弾発射
			if (goShell)	{
				goShell.SetActive (true);
				goShell.rigidbody2D.AddForce (new Vector2(+300.0f,500.0f));
				Destroy(goShell.gameObject,3.0f);
			}
			action = false;
		}
	}
	
	void OnGUI() {
		GUI.TextField(new Rect(10,10,300,60), 
		              "[Unity2Dでゲームを作る本 Sample 2-1]\n" + 
		              "戦車をクリックすると加速\nはなすと発射！");
		if (GUI.Button(new Rect(10, 80, 100, 20), "リセット")) {
			Application.LoadLevel(Application.loadedLevelName);
		}
	}
}
