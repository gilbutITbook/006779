using UnityEngine;
using System.Collections;

public class Core : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.name == "Tank_Shell") {
			Debug.Log (">>>>>>>>>>>>>> Hit!");
			transform.localScale += new Vector3 (1.0f, 1.0f, 1.0f);
			rigidbody2D.AddForce(new Vector2(1000.0f,-1000.0f));
		}
	}

}
