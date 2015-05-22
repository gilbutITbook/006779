using UnityEngine;
using System.Collections;

public static class SaveData {

	// Info
	const float SaveDataVersion = 0.30f;

	// === 外部パラメータ ======================================
	public static string 	SaveDate 		= "(non)";

	// HiScore
		   static int[] 	HiScoreInitData = new int[10] { 300000,100000,75000,50000,25000,10000,7500,5000,2500,1000 };
	public static int[] 	HiScore 		= new int[10] { 300000,100000,75000,50000,25000,10000,7500,5000,2500,1000 };

	// Option
	public static float 	SoundBGMVolume 	= 1.0f;
	public static float 	SoundSEVolume  	= 1.0f;
#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
	public static bool		VRPadEnabled 	= true;
#else
	public static bool		VRPadEnabled 	= false;
#endif

	// Etc(Don't Save)
	public static bool 		continuePlay 	= false;
	public static int		newRecord		= -1;
	public static bool		debug_Invicible	= false;

	// === コード（セーブデータチェック） ========================
	static void SaveDataHeader(string dataGroupName) {
		PlayerPrefs.SetFloat ("SaveDataVersion"	, SaveDataVersion);
		SaveDate = System.DateTime.Now.ToString("G");
		PlayerPrefs.SetString ("SaveDataDate"	, SaveDate);
		PlayerPrefs.SetString (dataGroupName	, "on");
	}

	static bool CheckSaveDataHeader(string dataGroupName) {
		if (!PlayerPrefs.HasKey ("SaveDataVersion")) {
			Debug.Log("SaveData.CheckData : No Save Data");
			return false;
		}
		if (PlayerPrefs.GetFloat ("SaveDataVersion") != SaveDataVersion) {
			Debug.Log("SaveData.CheckData : Version Error");
			return false;
		}
		if (!PlayerPrefs.HasKey (dataGroupName)) {
			Debug.Log("SaveData.CheckData : No Group Data");
			return false;
		}
		SaveDate = PlayerPrefs.GetString ("SaveDataDate");
		return true;
	}

	public static bool CheckGamePlayData() {
		return CheckSaveDataHeader ("SDG_GamePlay");
	}

	// === コード（プレイデータ・セーブロード） ===================
	public static bool SaveGamePlay() {
		try {
			Debug.Log("SaveData.SaveGamePlay : Start");

			// SaveDataInfo
			SaveDataHeader("SDG_GamePlay");
			{ // PlayerData
				zFoxDataPackString playerData = new zFoxDataPackString();
				playerData.Add ("Player_HPMax", PlayerController.nowHpMax);
				playerData.Add ("Player_HP"	  , PlayerController.nowHp);
				playerData.Add ("Player_Score", PlayerController.score);
				playerData.Add ("Player_checkPointEnabled"	, PlayerController.checkPointEnabled);
				playerData.Add ("Player_checkPointSceneName", PlayerController.checkPointSceneName);
				playerData.Add ("Player_checkPointLabelName", PlayerController.checkPointLabelName);
				playerData.PlayerPrefsSetStringUTF8 ("PlayerData", playerData.EncodeDataPackString ());
				//Debug.Log(playerData.EncodeDataPackString ());
			}
			{ // StageData
				zFoxDataPackString stageData = new zFoxDataPackString();
				zFoxUID[] uidList = GameObject.Find ("Stage").GetComponentsInChildren<zFoxUID> ();
				foreach(zFoxUID uidItem in uidList) {
					if (uidItem.uid != null && uidItem.uid != "(non)") { 
						stageData.Add (uidItem.uid,true);
					}
				}
				stageData.PlayerPrefsSetStringUTF8 ("StageData_" + Application.loadedLevelName, stageData.EncodeDataPackString ());
				//Debug.Log(stageData.EncodeDataPackString ());
			}
			{ // EventData
				zFoxDataPackString eventData = new zFoxDataPackString();
				eventData.Add ("Event_KeyItem_A", PlayerController.itemKeyA);
				eventData.Add ("Event_KeyItem_B", PlayerController.itemKeyB);
				eventData.Add ("Event_KeyItem_C", PlayerController.itemKeyC);
				eventData.PlayerPrefsSetStringUTF8 ("EventData", eventData.EncodeDataPackString ());
				//Debug.Log(playerData.EncodeDataPackString ());
			}
			// Save
			PlayerPrefs.Save ();

			Debug.Log("SaveData.SaveGamePlay : End");
			return true;

		} catch(System.Exception e) {
			Debug.LogWarning("SaveData.SaveGamePlay : Failed (" + e.Message + ")");
		}
		return false;
	}

	public static  bool LoadGamePlay(bool allData) {
		try {
			// SaveDataInfo
			if (CheckSaveDataHeader("SDG_GamePlay")) {
				Debug.Log("SaveData.LoadGamePlay : Start");
				SaveDate = PlayerPrefs.GetString ("SaveDataDate");
				if (allData) { // PlayerData
					zFoxDataPackString playerData = new zFoxDataPackString();
					playerData.DecodeDataPackString(playerData.PlayerPrefsGetStringUTF8 ("PlayerData"));
					//Debug.Log(playerData.PlayerPrefsGetStringUTF8 ("PlayerData"));
					PlayerController.nowHpMax 			 = (float)playerData.GetData ("Player_HPMax");
					PlayerController.nowHp 				 = (float)playerData.GetData ("Player_HP");
					PlayerController.score 				 = (int)playerData.GetData ("Player_Score");
					PlayerController.checkPointEnabled 	 = (bool)playerData.GetData ("Player_checkPointEnabled");
					PlayerController.checkPointSceneName = (string)playerData.GetData ("Player_checkPointSceneName");
					PlayerController.checkPointLabelName = (string)playerData.GetData ("Player_checkPointLabelName");
				}
				// StageData
				if (PlayerPrefs.HasKey("StageData_" + Application.loadedLevelName)) {
					zFoxDataPackString stageData = new zFoxDataPackString();
					stageData.DecodeDataPackString(stageData.PlayerPrefsGetStringUTF8 ("StageData_" + Application.loadedLevelName));
					//Debug.Log(stageData.PlayerPrefsGetStringUTF8 ("StageData_" + Application.loadedLevelName));
					zFoxUID[] uidList = GameObject.Find ("Stage").GetComponentsInChildren<zFoxUID> ();
					foreach(zFoxUID uidItem in uidList) {
						if (uidItem.uid != null && uidItem.uid != "(non)") { 
							if (stageData.GetData (uidItem.uid) == null) {
								uidItem.gameObject.SetActive(false);
							}
						}
					}
				}
				if (allData) { // EventData
					zFoxDataPackString eventData = new zFoxDataPackString();
					eventData.DecodeDataPackString(eventData.PlayerPrefsGetStringUTF8 ("EventData"));
					//Debug.Log(playerData.PlayerPrefsGetStringUTF8 ("PlayerData"));
					PlayerController.itemKeyA 			= (bool)eventData.GetData ("Event_KeyItem_A");
					PlayerController.itemKeyB 			= (bool)eventData.GetData ("Event_KeyItem_B");
					PlayerController.itemKeyC 			= (bool)eventData.GetData ("Event_KeyItem_C");
				}
				Debug.Log("SaveData.LoadGamePlay : End");
				return true;
			}
		} catch(System.Exception e) {
			Debug.LogWarning("SaveData.LoadGamePlay : Failed (" + e.Message + ")");
		}
		return false;
	}

	public static string LoadContinueSceneName() {
		if (CheckSaveDataHeader("SDG_GamePlay")) {
			zFoxDataPackString playerData = new zFoxDataPackString();
			playerData.DecodeDataPackString(playerData.PlayerPrefsGetStringUTF8 ("PlayerData"));
			return (string)playerData.GetData ("Player_checkPointSceneName");
		}

		continuePlay = false;
		return "StageA";
	}

	// === コード（ハイスコアデータ・セーブロード） ================
	public static bool SaveHiScore(int playerScore) {

		LoadHiScore ();

		try {
			Debug.Log("SaveData.SaveHiScore : Start");
			// Hiscore Set & Sort
			newRecord = 0;
			int[] scoreList = new int [11];
			HiScore.CopyTo (scoreList, 0);
			scoreList[10] = playerScore;
			System.Array.Sort(scoreList);
			System.Array.Reverse(scoreList);
			for(int i = 0;i < 10;i ++) {
				HiScore[i] = scoreList[i];
				if (playerScore == HiScore[i]) {
					newRecord = i + 1;
				}
			}

			// Hiscore Save
			SaveDataHeader("SDG_HiScore");
			zFoxDataPackString hiscoreData = new zFoxDataPackString();
			for(int i = 0;i < 10;i ++) {
				hiscoreData.Add ("Rank" + (i + 1), HiScore[i]);
			}
			hiscoreData.PlayerPrefsSetStringUTF8 ("HiScoreData", hiscoreData.EncodeDataPackString ());

			PlayerPrefs.Save ();
			Debug.Log("SaveData.SaveHiScore : End");
			return true;
		} catch(System.Exception e) {
			Debug.LogWarning("SaveData.SaveHiScore : Failed (" + e.Message + ")");
		}

		return false;
	}

	public static bool LoadHiScore() {
		try {
			if (CheckSaveDataHeader("SDG_HiScore")) {
				Debug.Log("SaveData.LoadHiScore : Start");
				zFoxDataPackString hiscoreData = new zFoxDataPackString();
				hiscoreData.DecodeDataPackString(hiscoreData.PlayerPrefsGetStringUTF8 ("HiScoreData"));
				//Debug.Log(hiscoreData.PlayerPrefsGetStringUTF8 ("HiScoreData"));
				for(int i = 0;i < 10;i ++) {
					HiScore[i] = (int)hiscoreData.GetData ("Rank" + (i + 1));
				}
				Debug.Log("SaveData.LoadHiScore : End");
			}
			return true;
		} catch(System.Exception e) {
			Debug.LogWarning("SaveData.LoadHiScore : Failed (" + e.Message + ")");
		}
		return false;
	}

	// === コード（オプションデータ・セーブロード） ================
	public static bool SaveOption() {
		try {
			Debug.Log("SaveData.SaveOption : Start");
			// Option Data
			SaveDataHeader("SDG_Option");

			PlayerPrefs.SetFloat ("SoundBGMVolume", SoundBGMVolume);
			PlayerPrefs.SetFloat ("SoundSEVolume" , SoundSEVolume);
			PlayerPrefs.SetInt 	 ("VRPadEnabled"  , (VRPadEnabled ? 1 : 0));

			// Save
			PlayerPrefs.Save ();
			Debug.Log("SaveData.SaveOption : End");
			return true;
		} catch(System.Exception e) {
			Debug.LogWarning("SaveData.SaveOption : Failed (" + e.Message + ")");
		}
		return false;
	}

	public static bool LoadOption() {
		try {
			if (CheckSaveDataHeader("SDG_Option")) {
				Debug.Log("SaveData.LoadOption : Start");

				SoundBGMVolume = PlayerPrefs.GetFloat ("SoundBGMVolume");
				SoundSEVolume  = PlayerPrefs.GetFloat ("SoundSEVolume");
				VRPadEnabled   = (PlayerPrefs.GetInt ("VRPadEnabled") > 0) ? true : false;

				Debug.Log("SaveData.LoadOption : End");
			}
		} catch(System.Exception e) {
			Debug.LogWarning("SaveData.LoadOption : Failed (" + e.Message + ")");
		}
		return false;
	}

	// === コード（セーブロードの削除・初期化） ==---==============
	public static  void DeleteAndInit(bool init) {
		Debug.Log("SaveData.DeleteAndInit : DeleteAll");
		PlayerPrefs.DeleteAll ();

		if (init) {
			Debug.Log("SaveData.DeleteAndInit : Init");
			SaveDate 		= "(non)";
			SoundBGMVolume  = 1.0f;
			SoundSEVolume   = 1.0f;

#if !UNITY_EDITOR && (UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8)
			VRPadEnabled 	= true;
#else
			VRPadEnabled 	= false;
#endif

			HiScoreInitData.CopyTo(HiScore,0);
		}
	}
}

// Windows
// (ComputerName)\HKEY_CURRENT_USR\Software\(CompanyName:DefaultCompany)\(AppName)
// http://docs-jp.unity3d.com/Documentation/ScriptReference/PlayerPrefs.html

