using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class zFoxDataPackString {

	// === 外部パラメータ（インスペクタ表示） =====================
	public bool DebugLog = false;

	// === 内部パラメータ ======================================
	Dictionary<string,object> dataList = new Dictionary<string,object>();
	const string FDPSTRING_ID = "FDPS";

	// === コード（データ作成） =================================
	public void Clear() {
		dataList.Clear ();
	}

	public void Add(string key,object obj) {
		if (dataList.ContainsKey (key)) {
			dataList [key] = obj;
		} else {
			dataList.Add (key, obj);
		}
	}

	public void SetData(string key,object val) {		
		if (val is bool) {
		} else
		if (val is int) {
		} else
		if (val is float) {
		} else
		if (val is string) {
		} else {
			Debug.LogError("[FoxSaveLib] SetData Syntax Error");
		}

		dataList [key] = val;
	}
	
	public object GetData(string key) {
		if (dataList.ContainsKey (key)) {
			return dataList [key];
		}

		return null;
	}

	// === コード（エンコード・デコード） ========================
	public string EncodeDataPackString() {

		string rtnString = FDPSTRING_ID;
		foreach (KeyValuePair<string,object> data in dataList) {

			rtnString +=  "," + data.Key + "," + data.Value;

			if (data.Value is bool) {
				rtnString +=  ",b";
			} else
			if (data.Value is int) {
				rtnString +=  ",i";
			} else
			if (data.Value is float) {
				rtnString +=  ",f";
			} else
			if (data.Value is string) {
				rtnString +=  ",s";
			} else {
				Debug.LogError(string.Format("[FoxSaveLib] EncodeDataPackString Syntax Error {0},{1}",data.Key,data.Value));
			}

		}

		if (DebugLog) {
			Debug.Log ("[FoxSaveLib] " + rtnString);
		}

		return rtnString;
	}

	public bool DecodeDataPackString(string val) {
		string[] dataTip = val.Split (',');

		if (DebugLog) {
			Debug.Log (string.Format ("data {0}", dataTip.Length));
			//foreach(string str in dataTip) {
			//	Debug.Log(str);
			//}
		}

		if (dataTip [0] != FDPSTRING_ID) {
			return false;
		}

		for (int i = 1; i < dataTip.Length; i += 3) {
			switch(dataTip[i + 2][0]) {
			case 'b' : Add(dataTip[i + 0],bool.Parse(dataTip[i + 1]) ); break;
			case 'i' : Add(dataTip[i + 0],int.Parse(dataTip[i + 1]) ); break;
			case 'f' : Add(dataTip[i + 0],float.Parse(dataTip[i + 1]) ); break;
			case 's' : Add(dataTip[i + 0],dataTip[i + 1]); break;
			}
		}

		return true;
	}

	// === コード（文字列の書き込み・呼び出し） ===================
	public void PlayerPrefsSetStringUTF8(string key,string val) {
		string valBase64 = System.Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(val));
		PlayerPrefs.SetString (key, valBase64);
	}

	public string PlayerPrefsGetStringUTF8(string key) {
		string valBase64 = PlayerPrefs.GetString (key);
		return System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(valBase64));
	}
}
