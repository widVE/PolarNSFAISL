using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotate : MonoBehaviour {

    public GameObject pivot;
    public bool spin=false;
    public float speed = 5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (spin)
        {
            gameObject.transform.RotateAround(pivot.transform.position, Vector3.up, speed * Time.deltaTime);
        }
	}
}
