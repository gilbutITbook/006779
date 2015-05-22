using UnityEngine;
using System.Collections;

public class StageObject_DogPile : MonoBehaviour {

	public GameObject[] enemyList;
	public GameObject[] destroyObjectList;
	
	void Start () {
		InvokeRepeating ("CheckEnemy",0.0f, 1.0f);
	}

	void CheckEnemy () {
		// 登録されている敵リストから敵の生存状態を確認
		// （1秒に1回でもよい）
		bool flag = true;
		foreach (GameObject enemy in enemyList) {
			if (enemy != null) {
				flag = false;
			}
		}

		// すべての敵が倒されているか？
		if (flag) {
			// 登録されている破壊物リストのオブジェクトを削除
			foreach (GameObject destroyObject in destroyObjectList) {
				Destroy(destroyObject,1.0f);
			}
			CancelInvoke("CheckEnemy");
		}
	}
}
