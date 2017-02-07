using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTwoMovement : MonoBehaviour {

	[SerializeField]
	private float speed = 5f;
	
	// Update is called once per frame
	void Update () {

		float horizontalMove = Input.GetAxis ("Horizontal_P2") * speed * 0.1f;
		float verticalMove = Input.GetAxis ("Vertical_P2") * speed * 0.1f;

		this.transform.Translate (new Vector3 (horizontalMove, 0, verticalMove));
	}
}
