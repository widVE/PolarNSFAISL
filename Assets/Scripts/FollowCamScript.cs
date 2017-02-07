using UnityEngine;
using System.Collections;

public class FollowCamScript : MonoBehaviour {

	[SerializeField]
	private Transform carPosition;
	private Vector3 originalLocPos;
	private Quaternion originalLocRot;
	private enum camPos {above, below};
	private camPos currPos;
	private bool transitioning;
	float rotRate = 10;
	float transRate = 0.5f;

	// Use this for initialization
	void Start () {
		currPos = camPos.above;
		originalLocPos = this.transform.position;
		originalLocRot = this.transform.rotation;
		transitioning = false;
	}

	void Update() {
		if (transitioning) {
			Vector3 destinationPos = Vector3.zero;
			Quaternion destinationRot = Quaternion.identity;
			switch(currPos) {
				case camPos.above:
					destinationPos = originalLocPos + new Vector3 (0f, -5f, 0f);
					destinationRot = Quaternion.LookRotation((carPosition.position - new Vector3(0, 5f, 0)));
					break;
				case camPos.below:
					destinationPos = originalLocPos;
					destinationRot = originalLocRot;
					break;
				default:
					Debug.LogError ("How are you here......?");
					break;
			}

			// Destination position/rotation set up, now translate/slerp to them
			this.transform.Translate(destinationPos * transRate * Time.deltaTime, Space.World);
			this.transform.rotation = Quaternion.Slerp (this.transform.rotation, destinationRot, rotRate * Time.deltaTime);

			// End condition - rotation will continue to get close without going over, so we can
			// End by checking if translate made it close
			if (Vector3.Distance(this.transform.position, destinationPos) < 0.2) {
				// set currPos
				if (currPos.Equals(camPos.above)) {
					currPos = camPos.below;
				} else {
					currPos = camPos.above;
				}
				transitioning = false;
			}
		}
	}

	public void transitionCamera() {
		transitioning = true;
	}
}
