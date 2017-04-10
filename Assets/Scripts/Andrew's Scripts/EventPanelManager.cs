using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelManager : MonoBehaviour {

	[SerializeField]
	private GameObject panel;
	private Text mytext;
	private int numEvents = 0;
	private List<EventInfo> events = new List<EventInfo>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addEvent(string name, float cumulative_energy, Vector2 coordinates) {
		if (numEvents > 20) {
			return;
		}
		numEvents++;
		EventInfo newEvent = new EventInfo (name, cumulative_energy, coordinates);
		GameObject newPanel = Instantiate (panel);
		newPanel.transform.SetParent (this.transform, false);
		newPanel.transform.localPosition = new Vector3 (0, 0, 0);
		newPanel.name = "EventBox " + numEvents; 

		Vector2 pos = new Vector2 (0f, (-100) * (numEvents - 1) - 50);
		//Debug.Log ("Position " + numEvents + ": " + pos);
		newPanel.GetComponent<RectTransform>().anchoredPosition = pos;
		newPanel.GetComponent<Image> ().color = new Color (Random.Range (0.3f, 1f), Random.Range (0.3f, 1f), Random.Range (0.3f, 1f));




		newPanel.transform.Find ("Text").GetComponent<Text> ().text = "Event " + numEvents;

	}
}
