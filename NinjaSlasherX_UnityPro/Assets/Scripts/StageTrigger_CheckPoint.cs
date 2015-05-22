using UnityEngine;
using System.Collections;

public class StageTrigger_CheckPoint : MonoBehaviour {

	public string				labelName 	= "";
	public bool					save 		= true;

	public CameraFollow.Param 	cameraParam;

	void OnTriggerEnter2D_PlayerEvent (GameObject go) {
		PlayerController.checkPointEnabled   = true;
		PlayerController.checkPointLabelName = labelName;
		PlayerController.checkPointSceneName = Application.loadedLevelName;
		PlayerController.checkPointHp 		 = PlayerController.nowHp;
		Camera.main.GetComponent<CameraFollow>().SetCamera(cameraParam);
		if (save) {
			//AppSound.instance.SE_CHECKPOINT.Play();
			SaveData.SaveGamePlay ();
		}
	}
}
