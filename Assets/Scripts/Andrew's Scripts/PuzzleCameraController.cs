using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCameraController : MonoBehaviour {

	private bool isMoving = false;
	private Vector3 destPos;
	private Quaternion facingDir;

	void Start() {
		facingDir = Quaternion.LookRotation(new Vector3(0,0,1f));
	}


	// Update is called once per frame
	void Update () {
		if (isMoving) {
			if (Vector3.Distance(this.transform.position, destPos) > 50f) {
				Vector3 translationVector = destPos - this.transform.position;
				this.transform.Translate (translationVector * Time.deltaTime, Space.World);
				this.transform.rotation = Quaternion.Slerp (this.transform.rotation, facingDir, Time.deltaTime);
			} else {
				isMoving = false;
			}

		} 
	}

	public void MoveCamera(Vector3 targetPos) {
		destPos = (targetPos - new Vector3 (0, 0, 1000f));
		isMoving = true;
	}
}
