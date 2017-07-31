using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeGameMode : MonoBehaviour {

	public EventPlayer eventPlayer;

	public SwipeRecognizer swipeRecognizer;

	[SerializeField]
	private GameObject topCamera;
	[SerializeField]
	private GameObject sideCamera;
	[SerializeField]
	private GameObject frontCamera;

	// Use this for initialization
	void Start () {
		
	}

	public void EventSwiped() {

		// Freeze playing, display full event
		eventPlayer.FreezePlaying ();

		// Activate cameras
		EnableCameras();

		// Add bounds for swiping
		swipeRecognizer.EnterResolveMode();
	}

	public void EventResolved() {
		eventPlayer.ResumePlaying ();
	}

	private void EnableCameras() {

		Vector3 eventCenterPos = eventPlayer.GetEventCenterpoint ();
        Bounds b = eventPlayer.GetEventBounds(eventCenterPos);
        //Debug.Log(b.min);
        //Debug.Log(b.max);
        //Debug.Log(b.extents);
		// Top Camera
		topCamera.transform.position = eventCenterPos + new Vector3(0f, b.extents.y, 0f);
		topCamera.transform.LookAt (eventCenterPos);
		topCamera.SetActive (true);

		// Side Camera
		sideCamera.transform.position = eventCenterPos - new Vector3 (b.extents.x, 0f, 0f);
		sideCamera.transform.LookAt (eventCenterPos, Vector3.up);
		sideCamera.SetActive(true);

		// Front Camera
		frontCamera.transform.position = eventCenterPos - new Vector3 (0f, 0f, b.extents.z);
		frontCamera.transform.LookAt (eventCenterPos, Vector3.up);
		frontCamera.SetActive (true);
	}

	public void DisableCameras() {
		topCamera.SetActive (false);
		sideCamera.SetActive(false);
		frontCamera.SetActive (false);
	}
}
