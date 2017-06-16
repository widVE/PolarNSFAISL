using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventPanelManager : MonoBehaviour {


	[SerializeField]
	private GameObject template;
	private Text mytext;
	private int numEvents = 0;
	private List<GameObject> panels = new List<GameObject> ();
	private AudioSource removeSound;
	// Use this for initialization
	void Start () {
		removeSound = GetComponent<AudioSource> ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void addEvent(string name, float cumulative_energy, Vector2 coordinates, Vector3 eventCenterPosition, List<VisualizeEvent.DomSnapShot> snapshot) {
		if (numEvents > 20) {
			return;
		}

		GameObject newPanel = Instantiate (template);
		newPanel.GetComponent<EventInfo> ().setName (name);
		newPanel.GetComponent<EventInfo> ().setDate (System.DateTime.Now);
		newPanel.GetComponent<EventInfo> ().setEnergy (cumulative_energy);
		newPanel.GetComponent<EventInfo> ().setCoordinates (coordinates);
		newPanel.GetComponent<EventInfo> ().setEventCenterPosition (eventCenterPosition);
		newPanel.GetComponent<EventInfo> ().setSnapshot (snapshot);
		newPanel.transform.SetParent (this.transform, false);
		newPanel.transform.localPosition = new Vector3 (0, 0, 0);
		newPanel.name = "Event: " + name; 
		newPanel.GetComponentInChildren<Button> ().onClick.AddListener (newPanel.GetComponent<EventInfo>().GoToPuzzleView);

		panels.Add (newPanel);
		numEvents++;


		Vector2 pos = new Vector2 (0f, (-100) * (numEvents - 1) - 50);
		//Debug.Log ("Position " + numEvents + ": " + pos);
		newPanel.GetComponent<RectTransform>().anchoredPosition = pos;
		newPanel.GetComponent<Image> ().color = new Color (Random.Range (0.3f, 1f), Random.Range (0.3f, 1f), Random.Range (0.3f, 1f));




		newPanel.transform.Find ("Text").GetComponent<Text> ().text = "Event: " + name;

	}

	public void removeEvent(GameObject eventToRemove) {

		removeSound.Play ();

		if (panels.Remove (eventToRemove)) {
			numEvents--;
			foreach (GameObject curr in panels) {
				if (curr.GetComponent<RectTransform>().anchoredPosition.y < eventToRemove.GetComponent<RectTransform>().anchoredPosition.y) {
					curr.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (0f, 100f);
				}
			}
		}

		// Send the EventInfo to the Oculus Player
		// this.sendInfo(eventToRemove.getComponent<EventInfo>());
		Destroy(eventToRemove);
		Debug.Log ("Event \"sent\" to Oculus");
	}
		
}
