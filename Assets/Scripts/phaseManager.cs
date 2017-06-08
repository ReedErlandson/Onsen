using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class phaseManager : MonoBehaviour {
	//sloppy
	public static phaseManager instance;
	// Use this for initialization
	void Start () {
		//sloppy
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void resolvePhases() {
		StartCoroutine (resolvePhasesCoroutine ());
	}

	IEnumerator resolvePhasesCoroutine() {
		dashExecute ();
		yield return new WaitForSeconds (1f);
		slamExecute ();
		yield return new WaitForSeconds (1f);
		throwExecute ();
		yield return new WaitForSeconds (1f);
		jumpExecute ();
		yield return new WaitForSeconds (1f);
		roundReset ();
	}

	void dashExecute() {
		for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
			if (inputManager.instance.playerArray [i].moveType == "dash") {
				inputManager.instance.playerArray [i].dashGoCall();
			}
		}
	}

	void slamExecute() {
		for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
			if (inputManager.instance.playerArray [i].moveType == "slam") {
				inputManager.instance.playerArray [i].slamGo();
			}
		}
	}

	void throwExecute() {
		for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
			if (inputManager.instance.playerArray [i].moveType == "throw") {
				inputManager.instance.playerArray [i].throwGoCall();
			}
		}
	}

	void jumpExecute() {
		for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
			if (inputManager.instance.playerArray [i].moveType == "jump") {
				inputManager.instance.playerArray [i].jumpGoCall();
			}
		}
	}

	void roundReset() {
		for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
			inputManager.instance.playerArray [i].activePlayer = false;
			inputManager.instance.playerArray [i].hasMoved = false;
			inputManager.instance.playerArray [i].moveLocked = false;
			inputManager.instance.playerArray [i].isShook = false;
			if (inputManager.instance.playerArray [i].playerNo == 1) {
				inputManager.instance.playerArray [i].startPlotting();
			}
		}
	}
}
