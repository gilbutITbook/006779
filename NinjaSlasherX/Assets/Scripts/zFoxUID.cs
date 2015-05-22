using UnityEngine;
using System.Collections;

public enum zFOXUID_TYPE {
	NUMBER,
	GUID,
}

public class zFoxUID : MonoBehaviour {
	public zFOXUID_TYPE type 	= zFOXUID_TYPE.NUMBER;
	public string 		uid 	= "(non)";
}
