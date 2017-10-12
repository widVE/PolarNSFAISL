using UnityEngine;
using System.Collections;

public class DriveTruck : MonoBehaviour {

	private Transform truckTransform;
	private const float step = 11.3f;
	private Vector2 gridPosition;
	private enum state {idle, rotating, translating};
	private enum direction {up, right, down, left};
	private state currentState;
	private direction currentDirection, targetDirection;
	private Vector3 oldPosition;

	// For efficiency, only allocate a vector once per movement
	private Vector3 targetDirVector;
	// Use this for initialization
	void Start () {
		currentState = state.idle;
		truckTransform = this.transform;
		gridPosition = new Vector2 (0, 0);
	}
	
	// Update is called once per frame
	void Update () {

		switch(currentState) {
			case state.idle:
			oldPosition = truckTransform.position;
			if (Input.GetKeyDown(KeyCode.W)) {
				targetDirection = direction.up;
				targetDirVector = new Vector3 (0, 0, 1);
				currentState = state.rotating;
				StartMovement ();
			} else if (Input.GetKeyDown(KeyCode.A)) {
				targetDirection = direction.left;
				targetDirVector = new Vector3 (1, 0, 0);
				currentState = state.rotating;
				StartMovement ();
			} else if (Input.GetKeyDown(KeyCode.S)) {
				targetDirection = direction.down;
				targetDirVector = new Vector3 (0, 0, -1);
				currentState = state.rotating;
				StartMovement ();
			} else if (Input.GetKeyDown(KeyCode.D)) {
				targetDirection = direction.right;
				targetDirVector = new Vector3 (-1, 0, 0);
				currentState = state.rotating;
				StartMovement ();
			}
				break;
		case state.rotating:
			RotateTruck ();
				break;
		case state.translating:
			MoveTruck ();
				break;
			default:
				break;
		}
	}

	void StartMovement() {
		
		// Check if we should rotate or translate
		if (!CheckDirection()) {
			RotateTruck ();
		} else {
			currentState = state.translating;
			MoveTruck ();
		}
	}

	// Returns true if we are facing the target direction
	bool CheckDirection() {
		return targetDirection.Equals (currentDirection);
	}

	void RotateTruck() {
		Vector3 newDir = Vector3.RotateTowards (-truckTransform.forward, targetDirVector, Time.deltaTime, 0.0f);
		truckTransform.rotation = Quaternion.LookRotation(newDir);
		if (Vector3.Angle(-truckTransform.forward, targetDirVector) == 0f) {
			currentDirection = targetDirection;
			//targetDirVector = null;
			currentState = state.translating;
		}
	}

	void MoveTruck() {

		// If at boundary, set state to idle and return
		Vector3 newPos = Vector3.Lerp(truckTransform.position, truckTransform.position + (targetDirVector * step), Time.deltaTime);
		truckTransform.Translate (newPos);
		if (truckTransform.position == (oldPosition + (targetDirVector * step))) {
			currentState = state.idle;

		}

	}
}
