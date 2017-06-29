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

	public EventInfo addEvent(string name, float cumulative_energy, Vector2 coordinates, Vector3 eventCenterPosition, List<VisualizeEvent.DomSnapShot> snapshot, Vector3 vStart, Vector3 vEnd) {

		GameObject newPanel = Instantiate (template);
		newPanel.GetComponent<EventInfo> ().setName (name);
		newPanel.GetComponent<EventInfo> ().setDate (System.DateTime.Now);
		newPanel.GetComponent<EventInfo> ().setEnergy (cumulative_energy);
		newPanel.GetComponent<EventInfo> ().setCoordinates (coordinates);
		newPanel.GetComponent<EventInfo> ().setEventCenterPosition (eventCenterPosition);
		newPanel.GetComponent<EventInfo> ().setSnapshot (snapshot);
		newPanel.GetComponent<EventInfo> ().setStart (vStart);
		newPanel.GetComponent<EventInfo> ().setEnd (vEnd);
		newPanel.transform.SetParent (this.transform, false);
		newPanel.transform.localPosition = new Vector3 (0, 0, 0);
		newPanel.name = "Event: " + name; 
		newPanel.GetComponentsInChildren<Button> ()[0].onClick.AddListener (newPanel.GetComponent<EventInfo>().GoToPuzzleView);
		newPanel.GetComponentsInChildren<Button> ()[1].onClick.AddListener (newPanel.GetComponent<EventInfo>().delete);
		panels.Add (newPanel);
		numEvents++;


		Vector2 pos = new Vector2 (0f, (-100) * (numEvents - 1) - 50);
		//Debug.Log ("Position " + numEvents + ": " + pos);
		newPanel.GetComponent<RectTransform>().anchoredPosition = pos;
		newPanel.GetComponent<Image> ().color = new Color (Random.Range (0.3f, 1f), Random.Range (0.3f, 1f), Random.Range (0.3f, 1f));




		newPanel.transform.Find ("Text").GetComponent<Text> ().text = "Event: " + name;
		return newPanel.GetComponent<EventInfo> ();
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
