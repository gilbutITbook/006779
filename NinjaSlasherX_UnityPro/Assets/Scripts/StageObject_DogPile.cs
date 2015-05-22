using UnityEngine;
using System.Collections;

public class StageObject_DogPile : MonoBehaviour {

	public GameObject[] enemyList;
	public GameObject[] destroyObjectList;

	public bool			sound 			= false;
	public string 		playGroup		= "BGM";
	public string 		playAudio		= "";
	public bool 		loop 			= false;
	
	public bool 		stopPlayGroup 	= true;

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
				destroyObject.AddComponent<Effect_FadeObject>();
				destroyObject.SendMessage("FadeStart");
				Destroy(destroyObject,1.0f);
			}
			// サウンドチェック＆サウンド再生
			if (sound) {
				if (stopPlayGroup) { 	// サウンド停止チェック
					if (playAudio == "") {
						AppSound.instance.fm.Stop (playGroup);
					} else
					if (!AppSound.instance.fm.FindAudioSource(playGroup, playAudio).isPlaying) {
						AppSound.instance.fm.Stop (playGroup);
					}
				}
				if (playAudio != "") {	// サウンド再生チェック
					AppSound.instance.fm.PlayDontOverride (playGroup, playAudio,loop);
				}
			}
			CancelInvoke("CheckEnemy");
		}
	}
}
