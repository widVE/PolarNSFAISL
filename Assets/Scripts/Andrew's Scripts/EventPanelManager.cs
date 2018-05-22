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

    public GameObject congratsPanel;

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
        Vector2 swipeStart, Vector2 swipeEnd, Color inColor, int score, bool eventPanel=true) {

        if (inColor != Color.white)
        {
            for(int i = 0; i < panels.Count; ++i)
            {
                if(panels[i].name == "Event: " + name)
                {
                    //Debug.Log("Duplicate found, incrementing score");
                    //if already there, increment score
                    int p;
                    int.TryParse(panels[i].transform.Find("Score").GetComponent<Text>().text, out p);
                    //p += score;
                    p++;
                    panels[i].transform.Find("Score").GetComponent<Text>().text = p.ToString();
                    return null;
                }
            }
        }

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
        newPanel.GetComponent<EventInfo>().setScore(score);
        // Setting the panel in the scene
        newPanel.transform.SetParent (this.transform, false);
		newPanel.transform.localPosition = new Vector3 (0, 0, 0);
		newPanel.name = "Event: " + name; 

        //set up image that goes along with event
        switch (name)
        {
            case "Gamma Ray Burst":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = grb;
                if (congratsPanel)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = grb;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("gamma_ray_burst");
                break;
            case "Black Hole":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = blackHole;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = blackHole;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("blackhole");
                break;
            case "Radio Galaxy":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = radioGalaxy;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = radioGalaxy;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("radio_galaxy");
                break;
            case "Blazar":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = blazar;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = blazar;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("blazar");
                break;
            case "Quasar":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = quazar;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = quazar;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("quasar");
                break;
            case "Supernova":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = supernova;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = supernova;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("supernova");
                break;
            case "Active Galactic Nucleus":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = agn;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = agn;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("active_galaxy_nucleus");
                break;
            case "X-Ray Binary":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = xrayBinary;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = xrayBinary;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("xray_binary");
                break;
            case "Supernova Remnant":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = snr;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = snr;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("supernova_remnant");
                break;
            case "Magnetar":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = magnetar;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = magnetar;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("magnetar");
                break;
            case "No Source":
                newPanel.transform.GetChild(0).GetComponent<Image>().sprite = noSource;
                if (congratsPanel != null)
                {
                    congratsPanel.transform.GetChild(0).GetComponent<Image>().sprite = noSource;
                }
                newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue("no_source");
                break;
            default:
                break;
        }
       
        if(congratsPanel)
        {
            //adding name underneath congrats image.
            congratsPanel.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue(name);
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
            pos = new Vector2(0f, (-118) * (panels.Count - 1) - 50);   
        }
        else 
        {
            pos = new Vector2(0f, (-200) * (panels.Count - 1) - 350);   
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
            newPanel.transform.Find("Text").GetComponent<Text>().text = LocalizationManager.instance.GetLocalizedValue(name);
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
		//Debug.Log ("Event \"sent\" to Players");
	}
		
}
