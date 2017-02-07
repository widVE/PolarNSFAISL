using UnityEngine;
using System.Collections;

public class ToggleCamera : MonoBehaviour {

	private enum position {above, below};
	private position currPos;
	private bool isMoving;
	private Vector3 abovePosition;
	private Vector3 belowPosition;

	void Start() {
		currPos = position.above;
		isMoving = false;
	}

	void Update() {
		if (isMoving) {
			
		}
	}




	public void MoveCamera() {
		isMoving = true;
	}
}
