using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class SwipeRecognizer : MonoBehaviour {

    public FlickGesture swipeGesture;
	// Use this for initialization
	void Start () {
		
	}

    private void OnEnable()
    {
        swipeGesture.Flicked += swipeRecognize;
        
    }

    private void OnDisable()
    {
        swipeGesture.Flicked -= swipeRecognize;
    }

	// Update is called once per frame
	void Update () {
		
	}

    private void swipeRecognize(object sender, System.EventArgs e)
    {
        //compare the swipe in screen space to the active event trajectory in screen space..
    }
}
