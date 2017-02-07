using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateProps : MonoBehaviour {

	[SerializeField]
	private float rate = 0.1f;
	
	// Update is called once per frame
	void Update () {
		this.transform.Rotate (new Vector3 (0, 0, 1) * rate * Time.deltaTime);
	}
}
