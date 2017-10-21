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

    //image sources for events
    public Sprite grb;
    public Sprite blackHole;
    public Sprite blazar;
    public Sprite supernova;
    public Sprite radioGalaxy;
    public Sprite quazar;
    public Sprite agn;
    public Sprite xrayBinary;
    public Sprite magnetar;
    public Sprite noSource;
    public Sprite snr;


    // List of EventBoxes currently representing swiped events
    public List<GameObject> panels = new List<GameObject> ();

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
        Vector2 swipeStart, Vector2 swipeEnd, Color inColor, bool eventPanel=true) {

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

        //set up image that goes along with event
        switch (name)
        {
            case "Gamma Ray Burst":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = grb;
                break;
            case "Black Hole":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = blackHole;
                break;
            case "Radio Galaxy":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = radioGalaxy;
                break;
            case "Blazar":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = blazar;
                newPanel.transform.Find("Text").GetComponent<Text>().text = "Blazar: An extragalactic source powered by a supermassive black hole at the center of a giant elliptical galaxy. They emit a relativistic jet that points to Earth.";
                break;
            case "Quasar":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = quazar;
                break;
            case "Supernova":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = supernova;
                break;
            case "Active Galactic Nucleus":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = agn;
                break;
            case "X-Ray Binary":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = xrayBinary;
                newPanel.transform.Find("Text").GetComponent<Text>().text = "X-ray Binary. A binary star produced by matter from one star to the other. The second one is usually a black hole or a neutron star.";
                break;
            case "Supernova Remnant":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = snr;
                newPanel.transform.Find("Text").GetComponent<Text>().text = "Supernova remnant (SNR): Either galactic or extragalactic, these sources are the result of a supernova explosion.";
                break;
            case "Magnetar":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = magnetar;
                newPanel.transform.Find("Text").GetComponent<Text>().text = "Magnetar. A neutron star with a very powerful magnetic field that could produce neutrinos, gamma rays and x-rays.";
                break;
            case "No Source":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = noSource;
                break;
            default:
                break;
        }
       

		// The panel has two buttons, so set up the OnClick functions for each button
		//newPanel.GetComponentsInChildren<Button> ()[0].onClick.AddListener (newPanel.GetComponent<EventInfo>().GoToPuzzleView);
		//newPanel.GetComponentsInChildren<Button> ()[1].onClick.AddListener (newPanel.GetComponent<EventInfo>().delete);

		// Add the event to our list of events
		panels.Add (newPanel);

		// Adjust the panel's position based on the number of events already swiped in the list
        //this needs to be adjusted for the summary panel...
		Vector2 pos;
        if(eventPanel)
        {
            pos = new Vector2(0f, (-100) * (panels.Count - 1) - 50);   
        }
        else 
        {
            pos = new Vector2(0f, (-200) * (panels.Count - 1) - 300);   
        }

		newPanel.GetComponent<RectTransform>().anchoredPosition = pos;

		// Randomly color it for visual appeal
        if (inColor != Color.white)
        {
            newPanel.GetComponent<Image>().color = inColor;
        }
        else
        {
            newPanel.GetComponent<Image>().color = new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
        }

		// Add name text and return it
        if (inColor == Color.white)
        {
        /*    newPanel.transform.Find("Text").GetComponent<Text>().text = name;
        }
        else
        {*/
            newPanel.transform.Find("Text").GetComponent<Text>().text = "Source: " + name;
        }

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
