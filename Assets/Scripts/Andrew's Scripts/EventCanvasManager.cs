using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCanvasManager : MonoBehaviour {

	private Text mytext;
	private int numEvents = 0;
	// Use this for initialization
	void Start () {
		mytext = this.transform.Find ("Text").GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addEvent(string info) {

		if (numEvents > 10) {
			return;
		}
		numEvents++;
		mytext.text += info + " " + numEvents + "\n\n";

	}
}
