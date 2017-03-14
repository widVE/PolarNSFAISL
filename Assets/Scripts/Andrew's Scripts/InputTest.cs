using UnityEngine;
using System.Collections;

public class InputTest : MonoBehaviour {

	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Fire1")) {
			Debug.Log ("Mousekey Pressed");
		}

	}
}
