using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zFoxGameObjectLoader : MonoBehaviour {
	
	// === 外部パラメータ（インスペクタ表示） =====================
	public GameObject[] LoadGameObjectList_Awake;
	public GameObject[] LoadGameObjectList_Start;
	public GameObject[] LoadGameObjectList_Update;
	public GameObject[] LoadGameObjectList_FixedUpdate;
	
	// === 外部パラメータ ======================================
	[System.NonSerialized] public Dictionary<string,GameObject> loadedGameObjectList_Awake = new Dictionary<string,GameObject>();
	[System.NonSerialized] public bool loaded_Awake = false;
	[System.NonSerialized] public Dictionary<string,GameObject> loadedGameObjectList_Start = new Dictionary<string,GameObject>();
	[System.NonSerialized] public bool loaded_Start = false;
	[System.NonSerialized] public Dictionary<string,GameObject> loadedGameObjectList_Update = new Dictionary<string,GameObject>();
	[System.NonSerialized] public bool loaded_Update = false;
	[System.NonSerialized] public Dictionary<string,GameObject> loadedGameObjectList_FixedUpdate = new Dictionary<string,GameObject>();
	[System.NonSerialized] public bool loaded_FixedUpdate = false;

	// === 内部パラメータ ======================================
	bool loaded = false;

	// === コード（Monobehaviour基本機能の実装） ================
	void Awake () {
		// --- 格ゲームオブジェクトのロードチェック
		bool loadedAll = false;
		GameObject[] gos = GameObject.FindObjectsOfType (typeof(GameObject)) as GameObject[];
		foreach (GameObject go in gos) {
			zFoxGameObjectLoader fol = go.GetComponent<zFoxGameObjectLoader>();
			if (fol) {
				if (fol.loaded) {
					loadedAll = true;
					break;
				}
			}
		}
		if (loadedAll) {
			Destroy(gameObject);
			return;
		}
		loaded = true;
	
		// ---- Awake処理実行
		if (!loaded_Awake) {
			loaded_Awake = true;
			LoadGameObject (LoadGameObjectList_Awake,loadedGameObjectList_Awake);
		}
	}

	void Start () {
		// ---- Start処理実行
		if (!loaded_Start) {
			loaded_Start = true;
			LoadGameObject (LoadGameObjectList_Start,loadedGameObjectList_Start);
		}
	}

	void Update() {
		// ---- Update処理実行
		if (!loaded_Update) {
			loaded_Update = true;
			LoadGameObject (LoadGameObjectList_Update,loadedGameObjectList_Update);
		}
	}

	void FixedUpdate() {
		// ---- FixedUpdate処理実行
		if (!loaded_FixedUpdate) {
			loaded_FixedUpdate = true;
			LoadGameObject (LoadGameObjectList_FixedUpdate,loadedGameObjectList_FixedUpdate);
		}
	}

	void LoadGameObject (GameObject[] loadGameObjectList,Dictionary<string,GameObject> loadedGameObjectList) {
		// このローダーがシーン間移動で削除されないように設定
		// （ロードするゲームオブジェクトは子に設定されるのでロードされたものも消えない）
		DontDestroyOnLoad (this);

		// 登録されているゲームオブジェクトをロード
		foreach (GameObject go in loadGameObjectList) {
			if (go) {
				if (loadedGameObjectList.ContainsKey(go.name)) {
					// ロード済み
				} else {
					// ロード
					GameObject goInstance = Instantiate(go) as GameObject;
					goInstance.name = go.name;
					goInstance.transform.parent = gameObject.transform;
					loadedGameObjectList.Add(go.name,goInstance);
					Debug.Log(string.Format("Loaded GameObject {0}",go.name));
				}
			}
		}
	}
}
