using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCameraController : MonoBehaviour {

	private bool isMoving = false;
	private Vector3 destPos;
	private Quaternion facingDir;

	private List<VisualizeEvent.DomSnapShot> prevSnapShots;

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
				foreach (VisualizeEvent.DomSnapShot curr in prevSnapShots) {
					curr.Dom.GetComponent<DOMController> ().TurnOn (curr.timeFrac, curr.charge);
					
				}
			} else {
				isMoving = false;
			}

		} 
	}

	public void MoveCamera(Vector3 targetPos, List<VisualizeEvent.DomSnapShot> snapShots) {

		if (prevSnapShots != null) {
			foreach (VisualizeEvent.DomSnapShot curr in prevSnapShots) {
				curr.Dom.GetComponent<DOMController> ().TurnOff ();
			}
		}

		destPos = (targetPos - new Vector3 (0, 0, 1000f));
		isMoving = true;

		foreach (VisualizeEvent.DomSnapShot curr in snapShots) {
			curr.Dom.GetComponent<DOMController> ().TurnOn (curr.timeFrac, curr.charge);
		}

		prevSnapShots = snapShots;
	}
}
