using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour {
	//sloppy
	public static gameManager instance;
	public GameObject jumpRangeDisplay;

	string currentPhase = "plotting";
	// Use this for initialization
	void Start () {
		//sloppy
		instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void advancePhase() {
		Monkey currentPlayer = null;
		Monkey nextPlayer = null;
		for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
			if (inputManager.instance.playerArray [i].activePlayer) {
				currentPlayer = inputManager.instance.playerArray [i];
			} else if (!inputManager.instance.playerArray [i].hasMoved) {
				if (nextPlayer == null || inputManager.instance.playerArray [i].playerNo < nextPlayer.playerNo) {
					nextPlayer = inputManager.instance.playerArray [i];
				}
			}
		}
		if (currentPhase == "plotting") {
			if (nextPlayer == null) {
				currentPlayer.endPlotting ();
				phaseManager.instance.resolvePhases ();
			} else {
				currentPlayer.endPlotting ();
				nextPlayer.startPlotting ();
			}
			for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
				inputManager.instance.playerArray [i].moveLocked = false;
			}
		}
	}

}
