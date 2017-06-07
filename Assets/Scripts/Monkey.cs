using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour {
	public GameObject dashRangeDisplay;
	public GameObject stoneObject;
	public LineRenderer actionRenderer;
	public float dashRange;
	public float jumpSlamRadius;
	public float jumpSlamMagnitude;
	public float dashTime;
	public float jumpTime;
	public float recoilBoopMagnitude;
	public float throwBoopMagnitude;
	private float recoilMultiplier;
	private bool isDashPathing = false;
	private bool isJumpPathing = false;
	private bool isThrowPathing = false;
	private bool isStoic = false;
	private Coroutine coroutineCall;
	[HideInInspector]
	public bool isDashing, isJumping, isDashRecoiling, isJumpRecoiling, isBoopRecoiling, isThrowRecoiling, isShook, projectingValidMove, moveLocked, activePlayer, hasMoved, thrownStoneHit;
	[HideInInspector]
	public Vector3 dashingVector, throwingVector, jumpingVector, recoilingVector, moveTarget, projectedTarget;
	[HideInInspector]
	public string moveType = "dash";
	[HideInInspector]
	public int playerNo;
	public LayerMask raycastLayer, throwRaycastLayer;
	void Start () {
		inputManager.instance.playerArray.Add (this);
		playerNo = inputManager.instance.playerArray.Count;
		if (playerNo == 1) {
			DashPathStart ();
			activePlayer = true;
		}
	}
 	public void Update () {
		if (isDashPathing) {
			dashMoveParse ();
		} else if (isJumpPathing) {
			jumpMoveParse ();
		} else if (isThrowPathing) {
			throwMoveParse ();
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Monker")) {
			if (isDashing) {
				if (other.GetComponent<Monkey> ().isDashing) {
					dashRecoilCall (other.GetComponent<Monkey> ().dashingVector);
				}
				other.GetComponent<Monkey> ().dashRecoilCall (dashingVector);
				other.GetComponent<Monkey> ().isShook = true;
			} else if (isDashRecoiling || isJumpRecoiling || isBoopRecoiling) {
				if (!other.GetComponent<Monkey> ().isDashRecoiling && !other.GetComponent<Monkey> ().isJumpRecoiling && !other.GetComponent<Monkey> ().isBoopRecoiling && !other.GetComponent<Monkey>().isJumping && !other.GetComponent<Monkey>().isStoic) {
					other.GetComponent<Monkey> ().recoilBoopCall (recoilingVector);
					other.GetComponent<Monkey> ().isShook = true;
				}
			}
		}
	}

	void dashMoveParse() {
		//mouse to playfield raycast
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast(Camera.main.ScreenPointToRay (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100)), out hitInfo, 1000, raycastLayer);
		projectingValidMove = false;
		if (hitInfo.collider != null) {
			Vector3 cursorPos = hitInfo.point;
			cursorPos.y += 0.2f;
			if (Vector3.Distance (cursorPos, this.transform.position) <= dashRange) {
				projectedTarget = cursorPos;
				projectedTarget.y = 1;
				//draw dash line
				Vector3[] dashLineArray = new Vector3[] { this.transform.position, cursorPos };
				actionRenderer.positionCount = 2;
				actionRenderer.SetPositions (dashLineArray);
				//valid move range
				projectingValidMove = true;
			}
		}
	}

	void jumpMoveParse() {
		//mouse to playfield raycast
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast(Camera.main.ScreenPointToRay (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100)), out hitInfo, 1000, raycastLayer);
		projectingValidMove = false;
		if (hitInfo.collider != null) {
			Vector3 cursorPos = hitInfo.point;
			cursorPos.y = 1.2f;
			projectedTarget = cursorPos;
			projectedTarget.y = 1.2f;
			//draw jump line
			Vector3[] jumpLineArray = new Vector3[] { this.transform.position, cursorPos };
			actionRenderer.positionCount = 2;
			actionRenderer.SetPositions (jumpLineArray);
			//valid move range
			projectingValidMove = true;
		}
	}

	void throwMoveParse() {
		//mouse to playfield raycast
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast(Camera.main.ScreenPointToRay (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100)), out hitInfo, 1000, raycastLayer);
		projectingValidMove = false;
		if (hitInfo.collider != null) {
			Vector3 cursorPos = hitInfo.point;
			cursorPos.y =1.2f;
			projectedTarget = cursorPos;
			projectedTarget.y = 1.2f;
			//draw jump line
			Vector3[] jumpLineArray = new Vector3[] { this.transform.position, cursorPos };
			actionRenderer.positionCount = 2;
			actionRenderer.SetPositions (jumpLineArray);
			//valid move range
			projectingValidMove = true;
		}
	}

	public void DashPathStart() {
		isDashPathing = true;
		//show dash range
		if (!dashRangeDisplay.activeSelf) {
			dashRangeDisplay.SetActive (true);
		}
		dashRangeDisplay.transform.localScale = new Vector3 (dashRange*2,0.1f,dashRange*2);
	}

	public void JumpPathStart() {
		isJumpPathing = true;
		//show jump range
		if (!gameManager.instance.jumpRangeDisplay.activeSelf) {
			gameManager.instance.jumpRangeDisplay.SetActive (true);
		}
	}

	public void ThrowPathStart() {
		isThrowPathing = true;
		//show jump range
		if (!gameManager.instance.jumpRangeDisplay.activeSelf) {
			gameManager.instance.jumpRangeDisplay.SetActive (true);
		}
	}

	public void dashGoCall() {
		dashingVector = moveTarget - this.transform.position;
		coroutineCall = StartCoroutine (dashGo ());
	}

	public void jumpGoCall() {
		jumpingVector = moveTarget - this.transform.position;
		coroutineCall = StartCoroutine (jumpGo ());
	}

	public void throwGoCall() {
		throwingVector = moveTarget - this.transform.position;
		coroutineCall = StartCoroutine (throwGo ());
	}

	public IEnumerator dashGo() {
		isDashing = true;
		Vector3 startPos = this.transform.position;
		float startTime = Time.time;
		while (Time.time <= startTime + dashTime) {
			this.transform.position = Vector3.Lerp (startPos,moveTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		this.transform.position = moveTarget;
		isDashing = false;
	}

	public IEnumerator throwGo() {
		bool thrownStoneOnTarget = false;
		thrownStoneHit = false;
		Vector3 startPos = this.transform.position;
		RaycastHit stoneHitInfo;
		Physics.Linecast (startPos, moveTarget, out stoneHitInfo, throwRaycastLayer);
		if (stoneHitInfo.collider != null) {
			moveTarget = stoneHitInfo.transform.position;
			thrownStoneHit = true;
			thrownStoneOnTarget = true;
		}
		Vector3 stoneStartVector = this.transform.position;
		stoneStartVector.y = 2;
		GameObject thrownStone = Instantiate (stoneObject, stoneStartVector, Quaternion.identity);
		thrownStone.GetComponent<stoneScript> ().parentMonkey = this;
		float startTime = Time.time;
		while (Time.time <= startTime + dashTime) {
			thrownStone.transform.position = Vector3.Lerp (stoneStartVector,moveTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		if (thrownStoneHit) {
			stoneHitInfo.collider.transform.root.GetComponent<Monkey>().throwRecoilCall (throwingVector);
		}
		if (thrownStoneHit || !thrownStoneOnTarget) {
			Destroy (thrownStone);
		}
	}

	public IEnumerator jumpGo() {
		if (!isShook) {
			isJumping = true;
			Vector3 startPos = this.transform.position;
			float startTime = Time.time;
			while (Time.time <= startTime + jumpTime) {
				this.transform.position = Vector3.Lerp (startPos, moveTarget, (Time.time - startTime) / jumpTime);
				yield return null;
			}
			this.transform.position = moveTarget;
			isStoic = true;
			isJumping = false;
			for (int i = 0; i < inputManager.instance.playerArray.Count; i++) {
				if (inputManager.instance.playerArray[i]!=this && Vector3.Distance (inputManager.instance.playerArray [i].transform.position, this.transform.position) <= jumpSlamRadius) {
					inputManager.instance.playerArray [i].jumpRecoilCall (jumpingVector);
				}
			}
		}
	}

	public void jumpRecoilCall(Vector3 fedVector) {
		if (!isJumpRecoiling) {
			recoilingVector = ((fedVector.normalized * jumpSlamMagnitude) + this.transform.position) - this.transform.position;
			coroutineCall = StartCoroutine (jumpRecoil(fedVector));
			isJumpRecoiling = true;
		}
	}

	public void dashRecoilCall(Vector3 fedVector) {
		if (!isDashRecoiling) {
			recoilMultiplier = 1f;
			if (isDashing) {
				recoilMultiplier = 1.5f;
				StopCoroutine (coroutineCall);
				isDashing = false;
			}
			recoilingVector = fedVector * recoilMultiplier;
			coroutineCall = StartCoroutine (dashRecoil (fedVector));
			isDashRecoiling = true;
		}
	}

	public void recoilBoopCall(Vector3 fedVector) {
		if (!isBoopRecoiling) {
			recoilingVector = ((fedVector.normalized * recoilBoopMagnitude) + this.transform.position) - this.transform.position;
			coroutineCall = StartCoroutine (boopRecoil (fedVector));
			isBoopRecoiling = true;
		}
	}

	public void throwRecoilCall(Vector3 fedVector) {
		if (!isThrowRecoiling) {
			recoilingVector = ((fedVector.normalized * throwBoopMagnitude) + this.transform.position) - this.transform.position;
			coroutineCall = StartCoroutine (throwRecoil (fedVector));
			isThrowRecoiling = true;
		}
	}

	public IEnumerator dashRecoil(Vector3 dasherVector) {
		Debug.Log ("DRC " + this.playerNo + dasherVector);
		Vector3 knockTarget = (dasherVector * recoilMultiplier) + this.transform.position;
		Vector3 startPos = this.transform.position;
		float startTime = Time.time;
		while (Time.time <= startTime + dashTime) {
			this.transform.position = Vector3.Lerp (startPos,knockTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		this.transform.position = knockTarget;
		isDashRecoiling = false;
	}

	public IEnumerator jumpRecoil(Vector3 jumperVector) {
		Vector3 knockTarget = (jumperVector.normalized * jumpSlamMagnitude) + this.transform.position;
		Vector3 startPos = this.transform.position;
		float startTime = Time.time;
		while (Time.time <= startTime + dashTime) {
			this.transform.position = Vector3.Lerp (startPos,knockTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		this.transform.position = knockTarget;
		isJumpRecoiling = false;
	}

	public IEnumerator boopRecoil(Vector3 booperVector) {
		Vector3 knockTarget = (booperVector.normalized * recoilBoopMagnitude) + this.transform.position;
		Vector3 startPos = this.transform.position;
		float startTime = Time.time;
		while (Time.time <= startTime + dashTime) {
			this.transform.position = Vector3.Lerp (startPos,knockTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		this.transform.position = knockTarget;
		isBoopRecoiling = false;
	}

	public IEnumerator throwRecoil(Vector3 throwerVector) {
		Vector3 knockTarget = (throwerVector.normalized * throwBoopMagnitude) + this.transform.position;
		Vector3 startPos = this.transform.position;
		float startTime = Time.time;
		while (Time.time <= startTime + dashTime) {
			this.transform.position = Vector3.Lerp (startPos,knockTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		this.transform.position = knockTarget;
		isThrowRecoiling = false;
	}

	public void startPlotting() {
		activePlayer = true;
	}

	public void stopPathing() {
		isDashPathing = false;
		isJumpPathing = false;
		isThrowPathing = false;
		dashRangeDisplay.SetActive (false);
		inputManager.instance.moveTargetMarker.SetActive (false);
		gameManager.instance.jumpRangeDisplay.SetActive (false);
		actionRenderer.positionCount = 0;
	}

	public void endPlotting() {
		actionRenderer.positionCount = 0;
		hasMoved = true;
		activePlayer = false;
		stopPathing ();
	}
}
