using UnityEngine;
using System.Collections;

public class Effect_FadeObject : MonoBehaviour {

	public float fadeV 	= -1.0f;

	SpriteRenderer[] spriteFadeList;

	void Start () {
		spriteFadeList = GetComponentsInChildren<SpriteRenderer> ();
	}

	void Update() {
		foreach (SpriteRenderer sprite in spriteFadeList) {
			sprite.color = new Color(sprite.color.r,sprite.color.g,sprite.color.b,sprite.color.a + fadeV * Time.deltaTime);
		}
	}

	void FadeStart() {
		GameObject goEmpty = new GameObject ("FadeObject");
		goEmpty.AddComponent<Effect_FadeObject>();

		SpriteRenderer[] spriteList = GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer sprite in spriteList) {
				sprite.tag = "";
				Collider2D[] col2DList = GetComponentsInChildren<Collider2D>();
				foreach(Collider2D col2D in col2DList) {
					col2D.enabled = false;
				}
		}
		goEmpty.transform.position = transform.position;
	}
}
