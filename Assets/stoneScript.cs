using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class stoneScript : MonoBehaviour {
	public Monkey parentMonkey;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag ("Rock")) {
			parentMonkey.thrownStoneHit = false;
			Destroy (other.gameObject);
			Destroy (this.gameObject);
		}
	}
}
