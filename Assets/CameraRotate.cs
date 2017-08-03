using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {

    public GameObject pivot;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.RotateAround(pivot.transform.position, Vector3.up, 5 * Time.deltaTime);
	}
}
