using UnityEngine;
using UnityEditor;
using System.Collections;

public sealed class zFoxColliderGizmoEditor : Editor {

	public static bool colliderViewEnabled = false;
	
	[MenuItem("zFoxTools/ColliderGizmo/ColliderGizmo On")]
	static void ColliderGizomoOn() {
		colliderViewEnabled = true;
	}

	[MenuItem("zFoxTools/ColliderGizmo/ColliderGizmo Off")]
	static void ColliderGizomoOff() {
		colliderViewEnabled = false;
	}

	[DrawGizmo(GizmoType.NotSelected | GizmoType.Active)]
	static void ColliderGizmoDraw(GameObject cv2,GizmoType gt) {

		if (!colliderViewEnabled) {
			return;
		}
		
		Gizmos.color = Color.blue;
		Gizmos.DrawCube (cv2.transform.position, new Vector3 (0.1f, 0.1f, 0.1f));
		
		Handles.Label (cv2.transform.position, "[Collider] ON");
		
		BoxCollider2D[] bcs = cv2.GetComponents<BoxCollider2D> ();
		foreach (BoxCollider2D bc in bcs) {
			Vector3 pos = new Vector3 (bc.center.x, bc.center.y, 0.0f);
			
			Vector3 p1 = cv2.transform.position + pos + new Vector3 (-bc.size.x / 2.0f, -bc.size.y / 2.0f, 0.0f);
			Vector3 p2 = cv2.transform.position + pos + new Vector3 (+bc.size.x / 2.0f, -bc.size.y / 2.0f, 0.0f);
			Vector3 p3 = cv2.transform.position + pos + new Vector3 (+bc.size.x / 2.0f, +bc.size.y / 2.0f, 0.0f);
			Vector3 p4 = cv2.transform.position + pos + new Vector3 (-bc.size.x / 2.0f, +bc.size.y / 2.0f, 0.0f);
			
			Gizmos.color = Color.red;
			Gizmos.DrawLine (p1, p2);
			Gizmos.DrawLine (p2, p3);
			Gizmos.DrawLine (p3, p4);
			Gizmos.DrawLine (p4, p1);
		}
		
		CircleCollider2D[] bcs2 = cv2.GetComponents<CircleCollider2D> ();
		foreach (CircleCollider2D bc in bcs2) {
			Vector3 pos = new Vector3 (bc.center.x, bc.center.y, 0.0f);
			
			//Gizmos.color = Color.red;
			//Gizmos.DrawWireSphere (transform.position + pos, bc.radius);
			
			Gizmos.color = Color.red;
			int cmax = 16;
			for(int i = 0;i < cmax;i ++) {
				Vector3 p1 = cv2.transform.position + pos + Quaternion.Euler(0.0f,0.0f,360.0f / cmax * (i + 0)) * new Vector3 (bc.radius,0.0f,0.0f);
				Vector3 p2 = cv2.transform.position + pos + Quaternion.Euler(0.0f,0.0f,360.0f / cmax * (i + 1)) * new Vector3 (bc.radius,0.0f,0.0f);
				Gizmos.DrawLine (p1, p2);
			}
		}
		
		EdgeCollider2D[] bcs3 = cv2.GetComponents<EdgeCollider2D> ();
		foreach (EdgeCollider2D bc in bcs3) {
			for(int i = 0;i < bc.pointCount - 1;i ++) {
				Vector3 p1 = cv2.transform.position + new Vector3(bc.points[i + 0].x,bc.points[i + 0].y);
				Vector3 p2 = cv2.transform.position + new Vector3(bc.points[i + 1].x,bc.points[i + 1].y);
				Gizmos.DrawLine (p1, p2);
			}
		}
		
		PolygonCollider2D[] bcs4 = cv2.GetComponents<PolygonCollider2D> ();
		foreach (PolygonCollider2D bc in bcs4) {
			for(int i = 0;i < bc.pathCount - 1;i ++) {
				Vector3 p1 = cv2.transform.position + new Vector3(bc.points[i + 0].x,bc.points[i + 0].y);
				Vector3 p2 = cv2.transform.position + new Vector3(bc.points[i + 1].x,bc.points[i + 1].y);
				Gizmos.DrawLine (p1, p2);
			}
		}
	}

}
