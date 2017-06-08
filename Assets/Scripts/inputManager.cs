using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inputManager : MonoBehaviour {
	public List<Monkey> playerArray;
	public GameObject moveTargetMarker;
	//sloppy
	public static inputManager instance;

	// Use this for initialization
	void Awake () {
		//sloppy
		instance = this;
		playerArray = new List<Monkey> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					resolveInput (playerArray [i]);
				}
			}

			}

		if (Input.GetKeyDown ("l")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					if (playerArray [i].moveLocked) {
						gameManager.instance.advancePhase ();
					}
				}
			}
		}

		//dash
		if (Input.GetKeyDown ("1")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].DashPathStart ();
					playerArray [i].moveType = "dash";
				}
			}
		}

		//jump
		if (Input.GetKeyDown ("2")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].JumpPathStart ();
					playerArray [i].moveType = "jump";
				}
			}
		}

		//throw
		if (Input.GetKeyDown ("3")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].ThrowPathStart ();
					playerArray [i].moveType = "throw";
				}
			}
		}

		//slam
		if (Input.GetKeyDown ("4")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].SlamPathStart ();
					playerArray [i].moveType = "slam";
				}
			}
		}

		//block
		if (Input.GetKeyDown ("5")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].BlockPathStart ();
					playerArray [i].moveType = "block";
				}
			}
		}

		}

	void resolveInput(Monkey fedPlayer) {
		if (fedPlayer.projectingValidMove) {
			fedPlayer.moveTarget = fedPlayer.projectedTarget;
			fedPlayer.moveLocked = true;
			//move target marker
			if (fedPlayer.moveType == "dash" || fedPlayer.moveType == "jump" || fedPlayer.moveType == "throw" || fedPlayer.moveType == "block") {
				if (!moveTargetMarker.activeInHierarchy) {
					moveTargetMarker.SetActive (true);
				}
				moveTargetMarker.transform.position = fedPlayer.moveTarget;
				if (fedPlayer.moveType == "block") {
					fedPlayer.blockProjector.SetActive (true);
				}
			} else if (fedPlayer.moveType == "slam") {
				if (!gameManager.instance.slamProjector.activeInHierarchy) {
					gameManager.instance.slamProjector.SetActive (true);
				}
				gameManager.instance.slamProjector.transform.position = fedPlayer.transform.position;
				gameManager.instance.slamProjector.transform.forward = fedPlayer.moveTarget - fedPlayer.transform.position;
				gameManager.instance.slamProjector.transform.Rotate (90, 0, 0);
			}
		}
	}
}
