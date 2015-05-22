using UnityEngine;
using UnityEditor;
using System.Collections;

public class zFoxMenuGenerateUID {

	[MenuItem("zFoxTools/UID/Generate")]
	public static void GenerateUID () {
		int guidIndex = 0;
		
		if (!EditorUtility.DisplayDialog ("UID Generate", "Generate UID?", "Ok", "Cancel")) {
			return ;
		}
		
		Debug.Log ("\n");
		Debug.Log ("--- GenerateUID Begin ---");
		zFoxUID[] uidList = GameObject.Find ("Stage").GetComponentsInChildren<zFoxUID> ();
		foreach(zFoxUID uidItem in uidList) {
//			if (uidItem.uid != null && uidItem.uid == "(non)") { 
			if (uidItem.uid != null) { 
					switch(uidItem.type) {
				case zFOXUID_TYPE.NUMBER :
					uidItem.uid = guidIndex.ToString();
					guidIndex ++;
					break;
				case zFOXUID_TYPE.GUID 	:
					uidItem.uid = System.Guid.NewGuid ().ToString();
					Debug.Log (string.Format("{0} {1} <- {2}",uidItem.name,uidItem.transform.position,System.Guid.NewGuid ()));
					break;
				}
				EditorUtility.SetDirty(uidItem);
			}
		}
		Debug.Log ("--- GenerateUID End ---");
		Debug.Log ("\n");
	}
	
	[MenuItem("zFoxTools/UID/Delete")]
	public static void DeleteUID () {
		if (EditorUtility.DisplayDialog ("UID Delete", "Delete UID?", "Ok", "Cancel")) {
			zFoxUID[] uidList = GameObject.Find ("Stage").GetComponentsInChildren<zFoxUID> ();
			foreach(zFoxUID uidItem in uidList) {
				uidItem.uid = "(non)";
				EditorUtility.SetDirty(uidItem);
			}
		}
	}

}
