using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class displayUI : MonoBehaviour {

	public GameObject camera;
	public Text myText;
	public string myString;
	public float fadeTime = 10;
	public float interactiveDistance = 12;
	public bool displayInfo;

	// Use this for initialization
	void Start () {
		/*myText.color = Color.black;
		myText = GameObject.Find ("Text").GetComponent<Text> ();
		myText.text = myString;
		myString = "Welcome to the South Pole. These flags represent the original 12 signatory nations to Antarctic Treaty.";*/
	}
	
	// Update is called once per frame
	void Update () {

//		FadeText ();
//		
		/*float dist = Vector3.Distance (transform.position, camera.transform.position);
//
//		Debug.Log ("Distance to Object" + dist);
		if (dist < interactiveDistance) {
			//Debug.Log ("YO!");
			myText.color = Color.Lerp (myText.color, Color.black, fadeTime * Time.deltaTime);
		} 
		if (dist > interactiveDistance) {
			//Debug.Log ("NO DISPLAY!");
			myText.color = Color.Lerp (myText.color, Color.clear, fadeTime * Time.deltaTime);
		}*/


//		if (displayInfo == true) {
//			myText.color = Color.Lerp (myText.color, Color.black, fadeTime * Time.deltaTime);
//		} else {
//			myText.color = Color.Lerp (myText.color, Color.clear, fadeTime * Time.deltaTime);
//		}
	}

//	void FadeText() {
//		if (displayInfo) {
//			myText.text = myString;
//			myString = "Welcome to the South Pole. These flags represent the original 12 signatory nations to Antarctic Treaty.";
//			myText.color = Color.Lerp (myText.color, Color.black, fadeTime * Time.deltaTime);
//		} 
//		else {
//			myText.color = Color.Lerp (myText.color, Color.clear, fadeTime * Time.deltaTime);
//		}
//	}

//	void OnMouseOver() {
//		displayInfo = true;
//	}
//
//	void OnMouseExit() {
//		displayInfo = false;
//	}

}
