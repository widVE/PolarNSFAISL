using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that manages the swiped events panel. It instantiates EventBox prefabs to represent individual event panels,
/// and also removes them from the list when cleared. This class stores all swiped events
/// </summary>
public class EventPanelManager : MonoBehaviour {

	// ----------VARIABLES----------

	[SerializeField]
	// The EventBox prefab to instantiate for every swiped event
	private GameObject template;

	// List of EventBoxes currently representing swiped events
	private List<GameObject> panels = new List<GameObject> ();

	// Sound effect for when events are removed
	private AudioSource removeSound;

	// ----------END VARIABLES----------


	/// <summary>
	/// Used for initialization
	/// </summary>
	void Start () {
		removeSound = GetComponent<AudioSource> ();
	}

	/// <summary>
	/// Creates an EventBox representing the swiped event, and adds it to the list and UI ScrollView
	/// </summary>
	/// <returns>The EventInfo constructed for this event</returns>
	/// <param name="name">Name of the event</param>
	/// <param name="cumulative_energy">Cumulative energy of the event</param>
	/// <param name="vStart">The starting point of the neutrino path of this event</param>
	/// <param name="vEnd">The ending point of the neutrino path of this event</param>
	/// <param name="domStates">List of dom states (doms activated by this event, and their state params)</param>
	public EventInfo addEvent(string name, float cumulative_energy, Vector3 neutrinoStart, Vector3 neutrinoEnd, 
        Vector2 swipeStart, Vector2 swipeEnd) {//, List<EventPlayer.DomState> domStates) {

		// Instantiate a new UI panel for the event
		GameObject newPanel = Instantiate (template);

		// Setting EventInfo fields
		// We could just use the constructor, but I never got around to doing so...
		newPanel.GetComponent<EventInfo> ().setName (name);
		newPanel.GetComponent<EventInfo> ().setDate (System.DateTime.Now);
		newPanel.GetComponent<EventInfo> ().setEnergy (cumulative_energy);
		//newPanel.GetComponent<EventInfo> ().setCoordinates (coordinates);
		//newPanel.GetComponent<EventInfo> ().setEventCenterPosition (eventCenterPosition);
		//newPanel.GetComponent<EventInfo> ().setDomStates (domStates);
		newPanel.GetComponent<EventInfo> ().setNeutrinoPathStart (neutrinoStart);
		newPanel.GetComponent<EventInfo> ().setNeutrinoPathEnd (neutrinoEnd);
		newPanel.GetComponent<EventInfo> ().setSwipePathStart (swipeStart);
		newPanel.GetComponent<EventInfo> ().setSwipePathEnd (swipeEnd);
		// Setting the panel in the scene
		newPanel.transform.SetParent (this.transform, false);
		newPanel.transform.localPosition = new Vector3 (0, 0, 0);
		newPanel.name = "Event: " + name; 

		// The panel has two buttons, so set up the OnClick functions for each button
		//newPanel.GetComponentsInChildren<Button> ()[0].onClick.AddListener (newPanel.GetComponent<EventInfo>().GoToPuzzleView);
		//newPanel.GetComponentsInChildren<Button> ()[1].onClick.AddListener (newPanel.GetComponent<EventInfo>().delete);

		// Add the event to our list of events
		panels.Add (newPanel);

		// Adjust the panel's position based on the number of events already swiped in the list
		Vector2 pos = new Vector2 (0f, (-100) * (panels.Count - 1) - 50);
		newPanel.GetComponent<RectTransform>().anchoredPosition = pos;

		// Randomly color it for visual appeal
		newPanel.GetComponent<Image> ().color = new Color (Random.Range (0.3f, 1f), Random.Range (0.3f, 1f), Random.Range (0.3f, 1f));

		// Add name text and return it
		newPanel.transform.Find ("Text").GetComponent<Text> ().text = "Event: " + name;
		return newPanel.GetComponent<EventInfo> ();
	}

	/// <summary>
	/// Removes the event from the list of events, and adjusts the position of all other event panels so
	/// the list is still contiguous
	/// </summary>
	/// <param name="eventToRemove">Event to remove.</param>
	public void removeEvent(GameObject eventToRemove) {

		// Ensure not null
		if (eventToRemove == null) {
			return;
		}

		// Play sound
		removeSound.Play ();

		// If the element exists in the list and was successfully removed...
		if (panels.Remove (eventToRemove)) {

			//...for each panel in the list, if it is below the removed panel then move it up one panel
			foreach (GameObject curr in panels) {
				if (curr.GetComponent<RectTransform>().anchoredPosition.y < eventToRemove.GetComponent<RectTransform>().anchoredPosition.y) {
					curr.GetComponent<RectTransform> ().anchoredPosition += new Vector2 (0f, 100f);
				}
			}
		}

		// Destroy the removed panel!
		Destroy(eventToRemove);

		// TODO: If you wanted to send a notification to players or have some other event happen when
		//       an event source is "calculated" or "found" then here is where you would probably do it
		Debug.Log ("Event \"sent\" to Players");
	}
		
}
