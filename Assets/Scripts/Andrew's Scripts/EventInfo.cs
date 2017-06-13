using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class used to store information of an event
/// </summary>
public class EventInfo : MonoBehaviour {

	private PuzzleCameraController puzzleCameraController;
	private  EventPanelManager man;
	private string name;
	private float peak_energy;
	private Vector2 coordinates;
	private DateTime date_captured;

	private Vector3 puzzleCameraPosition;

	void Start() {
		man = this.transform.parent.GetComponent<EventPanelManager> ();
		GameObject puzzleCamera = GameObject.Find ("Puzzle Camera");
		if (puzzleCamera == null) {
			Debug.LogError ("Puzzle camera not present in scene");
		}

		puzzleCameraController = puzzleCamera.GetComponent<PuzzleCameraController> ();
	}

	public EventInfo(string nameP, float energy, Vector2 coords, Vector3 puzzleCameraLocation) {
		this.name = nameP;
		this.peak_energy = energy;
		this.coordinates = coords;
		this.date_captured = DateTime.Now;
		this.puzzleCameraPosition = puzzleCameraLocation;
	}

	public EventInfo(EventInfo other) {
		if (other != null) {
			this.name = other.name;
			this.peak_energy = other.peak_energy;
			this.coordinates = other.coordinates;
			this.date_captured = other.date_captured;
			this.puzzleCameraPosition = other.puzzleCameraPosition;
		}
	}

	public void setName(string name) {
		this.name = name;
	}

	public void setEnergy(float energy) {
		this.peak_energy = energy;
	}

	public void setCoordinates(Vector2 coords) {
		this.coordinates = coords;
	}

	public void setDate(DateTime date) {
		this.date_captured = date;
	}


	public void setPuzzleCameraLocation(Vector3 puzzleCameraLocation) {
		this.puzzleCameraPosition = puzzleCameraLocation;
	}

	public Vector3 getPuzzleCameraLocation() {
		return this.puzzleCameraPosition;
	}
		
	public void GoToPuzzleView() {
		puzzleCameraController.MoveCamera (this.puzzleCameraPosition);
	}
}
