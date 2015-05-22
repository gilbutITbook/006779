using UnityEngine;
using System.Collections;

public class zFoxDontDestroyOnLoad : MonoBehaviour {

	public bool DontDestroyEnabled = true;

	void Start () {
		if (DontDestroyEnabled) {
			DontDestroyOnLoad (this);
		}
	}
}
