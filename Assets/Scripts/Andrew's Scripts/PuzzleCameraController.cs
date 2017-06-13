using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveCamera(Vector3 pos) {
		this.transform.position = pos;
		this.transform.LookAt ((pos + new Vector3(0,0,1000f)));
	}
}
