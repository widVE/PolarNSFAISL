using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveHelpTimer : MonoBehaviour {

    //last time that the screen was touched..
    public float pressTime;

	// Use this for initialization
	void Start () {
        pressTime = UnityEngine.Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
