using UnityEngine;
using System.Collections;

public class StageTrigger_EventSound : MonoBehaviour {

	public string 	playGroup		= "BGM";
	public string 	playAudio		= "";
	public bool 	loop 			= true;

	public bool 	stopPlayGroup 	= true;

	void OnTriggerEnter2D_PlayerEvent (GameObject go) {
		if (stopPlayGroup) {
			if (!AppSound.instance.fm.FindAudioSource(playGroup, playAudio).isPlaying) {
				//AppSound.instance.fm.Stop (playGroup);
				AppSound.instance.fm.FadeOutVolumeGroup(playGroup,playAudio,0.0f,1.0f,false);
			}
		}
		if (playAudio != "") {
			//Debug.Log(string.Format("StageTrigger_EventSound : {0} {1}",playGroup,playAudio));
			AppSound.instance.fm.SetVolume(playGroup,playAudio,SaveData.SoundBGMVolume);
			AppSound.instance.fm.PlayDontOverride (playGroup, playAudio,loop);
		}
	}
}
