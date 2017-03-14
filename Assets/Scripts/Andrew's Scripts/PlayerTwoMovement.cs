using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTwoMovement : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	[SerializeField]
	private float sensitivity = 5f;
	
	// Update is called once per frame
	void Update () {

		float horizontalMove = Input.GetAxis ("Horizontal_P2") * speed * 0.1f;
		float forwardMove = Input.GetAxis ("Vertical_P2") * speed * 0.1f;
		float verticalMove = 0;
		if (Input.GetButton("Ascend")) {
			verticalMove += speed * 0.05f;
		}
		if (Input.GetButton("Descend")) {
			verticalMove -= speed * 0.05f;
		}
		this.transform.Translate (new Vector3 (horizontalMove, verticalMove, forwardMove));

		float horizRot = Input.GetAxis ("LookHorizontal_P2") * sensitivity * 0.1f;
		//float yRot = Input.GetAxis ("LookVertical_P2") * sensitivity * -0.1f;

		this.transform.Rotate (new Vector3 (0, horizRot, 0));
	}
}
