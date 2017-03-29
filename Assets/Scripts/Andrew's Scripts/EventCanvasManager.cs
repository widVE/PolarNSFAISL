using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventCanvasManager : MonoBehaviour {

	[SerializeField]
	private GameObject panel;
	private Text mytext;
	private int numEvents = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addEvent(string info) {

		if (numEvents > 10) {
			return;
		}
		numEvents++;
		GameObject newPanel = Instantiate (panel, this.transform);
		newPanel.name = "EventBox " + numEvents; 

		Vector2 pos = new Vector2 (0f, -150 * (numEvents - 1) + 1025f);
		Debug.Log ("Position " + numEvents + ": " + pos);
		newPanel.GetComponent<Image> ().color = Random.ColorHSV();
		newPanel.GetComponent<RectTransform> ().anchoredPosition = pos;

		newPanel.transform.Find ("Text").GetComponent<Text> ().text = "Event " + numEvents;

	}
}
