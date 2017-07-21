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
		swipeRecognizer.ActivateBounds();
	}

	public void EventResolved() {
		eventPlayer.ResumePlaying ();
	}

	private void EnableCameras() {
		topCamera.SetActive (true);
		sideCamera.SetActive(true);
		frontCamera.SetActive (true);
	}

	private void DisableCameras() {
		topCamera.SetActive (false);
		sideCamera.SetActive(false);
		frontCamera.SetActive (false);
	}
}
