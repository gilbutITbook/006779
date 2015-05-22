using UnityEngine;
using UnityEditor;
using System.Collections;


public class NinjaSlasherX_GameDebugInfo {
	
	public static bool debugViewEnabled = false;
	
	[MenuItem("zFoxTools/NinjaSlasherX/ColliderGizmo On")]
	static void ColliderGizomoOn() {
		debugViewEnabled = true;
	}
	
	[MenuItem("zFoxTools/NinjaSlasherX/ColliderGizmo Off")]
	static void ColliderGizomoOff() {
		debugViewEnabled = false;
	}
	
	[DrawGizmo(GizmoType.NotSelected | GizmoType.Active)]
	static void ColliderGizmoDraw(GameObject go,GizmoType gt) {
		if (!debugViewEnabled) {
			return;
		}

		if (go.tag == "Player") {
			// Scaler
			for (int x = -10;x < 10;x ++) {
				Vector3 vecA = go.transform.position + Vector3.left * x;
				Vector3 vecB = vecA + Vector3.up;
				Gizmos.color = (Mathf.Abs(x) < 5) ? Color.red : Color.white;
				Gizmos.DrawLine (vecA,vecB);
			}
			for (int x = -10;x < 10;x ++) {
				Vector3 vecA = go.transform.position + Vector3.up * x;
				Vector3 vecB = vecA + Vector3.left;
				Gizmos.color = (Mathf.Abs(x) < 5) ? Color.red : Color.white;
				Gizmos.DrawLine (vecA,vecB);
			}
		} else
		if (go.tag == "Enemy") {
			// AI Status
			Gizmos.color = Color.red;
			Gizmos.DrawCube (go.transform.position, new Vector3 (0.1f, 0.1f, 0.1f));
			GUIStyle style = new GUIStyle();
			style.active.textColor 	= Color.red;
			style.normal.textColor 	= Color.red;
			style.hover.textColor 	= Color.red;
			style.focused.textColor	= Color.red;

			EnemyMain em = go.GetComponent<EnemyMain>();
			if (em == null) {
				return;
			}

			if (em.cameraEnabled) {
				Handles.Label (go.transform.position,
				               string.Format("{0} {1} {2}",
				              em.aiState,
				              em.GetComponent<EnemyController>().grounded,
				              em.GetDistanePlayer()) );
			}

			// Dog Pile
			Gizmos.color = Color.red;
			StageObject_DogPile[] dogPileList = GameObject.FindObjectsOfType<StageObject_DogPile>();
			foreach(StageObject_DogPile dogPile in dogPileList) {
				foreach(GameObject enemy in dogPile.enemyList) {
					if (go == enemy) {
						Gizmos.DrawLine (go.transform.position, dogPile.transform.position);
						return;
					}
				}
			}
			
		}
	}
	
}

