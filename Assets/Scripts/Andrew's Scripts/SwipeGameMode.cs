using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeGameMode : MonoBehaviour {

	public EventPlayer eventPlayer;

	public SwipeRecognizer swipeRecognizer;

    public EarthView earthView;

	[SerializeField]
	private GameObject topCamera;
    [SerializeField]
    private GameObject topPanel;
	[SerializeField]
	private GameObject sideCamera;
    [SerializeField]
    private GameObject sidePanel;
	[SerializeField]
	private GameObject frontCamera;
    [SerializeField]
    private GameObject frontPanel;

    private bool swipedTop = false;
    private bool swipedSide = false;
    private bool swipedFront = false;

	// Use this for initialization
	void Start () {
		
	}

    public void SetSwipeTop(bool swipe) { swipedTop = swipe; }
    public void SetSwipeSide(bool swipe) { swipedSide = swipe; }
    public void SetSwipeFront(bool swipe) { swipedFront = swipe; }

	public void EventSwiped() {

		// Freeze playing, display full event
		eventPlayer.FreezePlaying ();

		// Activate cameras
		EnableCameras();

		// Add bounds for swiping
		swipeRecognizer.EnterResolveMode();
	}

	public void EventResolved(bool success=false) {
        swipedTop = false;
        swipedFront = false;
        swipedSide = false;
		eventPlayer.ResumePlaying ();
        if(success)
        {
            //assuming just one event here for now..
            earthView.AddDetectedEvent(swipeRecognizer.currentEvents.events[0].startPos, swipeRecognizer.currentEvents.events[0].endPos);
        }
	}

    public bool SwipedAllThree() { return swipedTop && swipedFront && swipedSide; }

	private void EnableCameras() {

		Vector3 eventCenterPos = eventPlayer.GetEventCenterpoint ();
        Bounds b = eventPlayer.GetEventBounds(eventCenterPos);
        
		// Top Camera
        topCamera.transform.position = b.center + new Vector3(0f, b.extents.y, 0f);
        topCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.x, b.extents.z) + 30.0f;
		topCamera.transform.LookAt (eventCenterPos);
		topCamera.SetActive (true);
        topPanel.SetActive(true);

		// Side Camera
        sideCamera.transform.position = b.center - new Vector3(b.extents.x, 0f, 0f);
        sideCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.y, b.extents.z) + 30.0f;
		sideCamera.transform.LookAt (eventCenterPos, Vector3.up);
		sideCamera.SetActive(true);
        sidePanel.SetActive(true);

		// Front Camera
        frontCamera.transform.position = b.center - new Vector3(0f, 0f, b.extents.z);
        frontCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.x, b.extents.y) + 30.0f;
		frontCamera.transform.LookAt (eventCenterPos, Vector3.up);
		frontCamera.SetActive (true);
        frontPanel.SetActive(true);
	}

	public void DisableCameras() {
		topCamera.SetActive (false);
		sideCamera.SetActive(false);
		frontCamera.SetActive (false);
        topPanel.SetActive(false);
        sidePanel.SetActive(false);
        frontPanel.SetActive(false);
        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
	}
}
