using UnityEngine;
using System.Collections;

public class EnemySprite : MonoBehaviour {

	EnemyMain enemyMain;

	void Awake () {
		// EnemyMainを検索
		enemyMain = GetComponentInParent<EnemyMain> ();
	}
	
	void OnBecameVisible()
	{
		// カメラに写っている（このサンプルプログラムでは処理なし）
	}
	void OnBecameInvisible()
	{
		// カメラに写っていない（このサンプルプログラムでは処理なし）
	}

	void OnWillRenderObject() {
		if (Camera.current.tag == "MainCamera") {
			// 処理
			enemyMain.cameraEnabled = true;
		}
	}
}
