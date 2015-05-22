using UnityEngine;
using System.Collections;

public class ApplicationStartup : MonoBehaviour {
	void Start () {
		Debug.Log ("=== Application Startup [Ninja SlasherX] ===");
		SaveData.LoadOption ();
	}
}
