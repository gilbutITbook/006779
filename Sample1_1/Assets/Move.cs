using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// ここから……
		float vx = Input.GetAxis ("Horizontal");
		float vy = Input.GetAxis ("Vertical");
		transform.Translate(new Vector3 (vx, vy, 0.0f));
		// ここまでを追加する
	}
}
