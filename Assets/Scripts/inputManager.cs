﻿using System.Collections;
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

		if (Input.GetKeyDown ("1")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].DashPathStart ();
					playerArray [i].moveType = "dash";
				}
			}
		}

		if (Input.GetKeyDown ("2")) {
			for (int i = 0; i < playerArray.Count; i++) {
				if (playerArray [i].activePlayer) {
					playerArray [i].stopPathing ();
					playerArray [i].JumpPathStart ();
					playerArray [i].moveType = "jump";
				}
			}
		}

		}

	void resolveInput(Monkey fedPlayer) {
		if (fedPlayer.projectingValidMove) {
			fedPlayer.moveTarget = fedPlayer.projectedTarget;
			fedPlayer.moveLocked = true;
			//move target marker
			if (!moveTargetMarker.activeInHierarchy) {
				moveTargetMarker.SetActive (true);
			}
			moveTargetMarker.transform.position = fedPlayer.moveTarget;
		}
	}

}
