using UnityEngine;
using System.Collections;

public class StageTrigger_EventBrokenWorld : MonoBehaviour {

	public GameObject brokenObject;
	public GameObject destroyObject;

	GameObject	player;

	void Start () {
		player = PlayerController.GetGameObject();
	}
	
	void OnTriggerEnter2D_PlayerEvent (GameObject go) {
		GetComponent<BoxCollider2D> ().enabled = false;
		DestroyObject (destroyObject);
		Invoke("BrokenStepA",0.5f);
		Invoke("BrokenStepB",1.0f);
	}

	void BrokenStepA() {
		SpriteRenderer[] gos = brokenObject.transform.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer go in gos) {
			if (go.GetComponent<Rigidbody2D>() == null) {
				Rigidbody2D addRigidbody2D 	= go.gameObject.AddComponent<Rigidbody2D>();
				addRigidbody2D.gravityScale = Random.Range(-0.1f,-0.3f);
				addRigidbody2D.mass 		= 100.0f;
				addRigidbody2D.AddTorque(Random.Range(-100.0f,+100.0f));
				player.rigidbody2D.mass 	= 0.01f;
			}
		}
		AppSound.instance.SE_EXPLOSION.Play ();
	}

	void BrokenStepB() {
		zFoxFadeFilter.instance.FadeOut (Color.black, 3.0f);
		SpriteRenderer[] gos = brokenObject.transform.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer go in gos) {
			if (go.GetComponent<Rigidbody2D>() != null) {
				go.rigidbody2D.gravityScale = Random.Range(0.3f,1.0f);
				
			}
		}
	}
}
