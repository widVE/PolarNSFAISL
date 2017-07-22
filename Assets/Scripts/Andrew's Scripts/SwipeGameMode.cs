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

		// Top Camera
		topCamera.transform.position = eventCenterPos + new Vector3(0f, 1000f, 0f);
		topCamera.transform.LookAt (eventCenterPos);
		topCamera.SetActive (true);

		// Side Camera
		sideCamera.transform.position = eventCenterPos - new Vector3 (1000f, 0f, 0f);
		sideCamera.transform.LookAt (eventCenterPos, Vector3.up);
		sideCamera.SetActive(true);

		// Front Camera
		frontCamera.transform.position = eventCenterPos - new Vector3 (0f, 0f, 1000f);
		frontCamera.transform.LookAt (eventCenterPos, Vector3.up);
		frontCamera.SetActive (true);
	}

	private void DisableCameras() {
		topCamera.SetActive (false);
		sideCamera.SetActive(false);
		frontCamera.SetActive (false);
	}
}
