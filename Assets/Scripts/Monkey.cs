using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monkey : MonoBehaviour {
	public GameObject dashRangeDisplay;
	public float dashRange;
	public float dashTime;
	public float jumpTime;
	private bool isDashPathing = false;
	private bool isJumpPathing = false;
	public bool isDashing = false;
	public bool isJumping = false;
	public bool isDashRecoiling = false;
	public bool isShook = false;
	public Vector3 dashingVector;
	public LineRenderer actionRenderer;
	public Vector3 moveTarget;
	public Vector3 projectedTarget;
	public bool projectingValidMove = false;
	public bool moveLocked = false;
	public bool activePlayer = false;
	public bool hasMoved = false;
	public string moveType = "dash";
	public int playerNo;

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
		}
	}

	void OnTriggerEnter(Collider other) {
		Debug.Log ("collision!");
		if (isDashing) {
			other.GetComponent<Monkey> ().dashRecoilCall (dashingVector);
			other.GetComponent<Monkey> ().isShook = true;
		}
	}

	void dashMoveParse() {
		//mouse to playfield raycast
		RaycastHit hitInfo = new RaycastHit ();
		Physics.Raycast(Camera.main.ScreenPointToRay (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100)), out hitInfo);
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
		Physics.Raycast(Camera.main.ScreenPointToRay (new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100)), out hitInfo);
		projectingValidMove = false;
		if (hitInfo.collider != null) {
			Vector3 cursorPos = hitInfo.point;
			cursorPos.y += 0.2f;
			projectedTarget = cursorPos;
			projectedTarget.y = 1;
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
		dashRangeDisplay.transform.localScale = new Vector3 (dashRange*2,0.1f,dashRange*2);
	}

	public void dashGoCall() {
		dashingVector = moveTarget - this.transform.position;
		StartCoroutine (dashGo ());
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

	public void jumpGoCall() {
		StartCoroutine (jumpGo ());
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
			isJumping = false;
		}
	}

	public void dashRecoilCall(Vector3 fedVector) {
		if (!isDashRecoiling) {
			StartCoroutine (dashRecoil (fedVector));
		}
	}

	public IEnumerator dashRecoil(Vector3 dasherVector) {
		float recoilMultiplier = 1f;
		if (isDashing) {
			recoilMultiplier = 1.5f;
		}
		Vector3 knockTarget = (dasherVector * recoilMultiplier) + this.transform.position;
		Vector3 startPos = this.transform.position;
		float startTime = Time.time;
		isDashRecoiling = true;
		while (Time.time <= startTime + dashTime) {
			this.transform.position = Vector3.Lerp (startPos,knockTarget,(Time.time-startTime)/dashTime);
			yield return null;
		}
		this.transform.position = knockTarget;
		isDashRecoiling = false;
	}

	public void startPlotting() {
		activePlayer = true;
	}

	public void stopPathing() {
		isDashPathing = false;
		isJumpPathing = false;
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
