using UnityEngine;
using System.Collections;

public class Stage_Shadow : MonoBehaviour {

	public bool 	shadowEnabled  		= true;
	public bool		updateEnabled		= true;
	public Vector3 	offsetPosition 		= new Vector3 (-0.2f,0.0f,0.1f);
	public string	sortingLayerName 	= "Char";
	public int 		sortingOrder		= 0;
	public Color	shadowColor    		= new Color (0.0f, 0.0f, 0.0f, 0.5f);

	SpriteRenderer 	spriteSrc;
	SpriteRenderer 	spriteCopy;

	void Start () {
		spriteSrc = GetComponent<SpriteRenderer> ();

		GameObject goEmpty 			= new GameObject ("Shadow");
		spriteCopy 					= goEmpty.AddComponent<SpriteRenderer> ();
		spriteCopy = goEmpty.GetComponent<SpriteRenderer> ();
		goEmpty.transform.parent 	= transform;
		goEmpty.transform.localScale = Vector3.one;

		spriteCopy.tag			 	= "Shadow";
		spriteCopy.sortingLayerName = sortingLayerName;
		spriteCopy.sortingOrder 	= sortingOrder;
		spriteCopy.color 			= shadowColor;

		UpdateShadow ();
	}

	void Update () {
		if (updateEnabled) {
			UpdateShadow ();
		}
	}

	void UpdateShadow () {
		spriteCopy.transform.position = spriteSrc.transform.position;
		spriteCopy.transform.Translate(-0.2f,0.0f,0.1f,Space.Self);			
		spriteCopy.sprite = spriteSrc.sprite;
	}
}
