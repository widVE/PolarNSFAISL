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
	private Vector3 pathStart;
	private Vector3 pathEnd;

	private Vector3 eventCenterPosition;
	private List<VisualizeEvent.DomSnapShot> eventSnapshot;

	void Start() {
		man = this.transform.parent.GetComponent<EventPanelManager> ();
		GameObject puzzleCamera = GameObject.Find ("Puzzle Camera");
		if (puzzleCamera == null) {
			Debug.LogError ("Puzzle camera not present in scene");
		}

		puzzleCameraController = puzzleCamera.GetComponent<PuzzleCameraController> ();
	}

	public EventInfo(string nameP, float energy, Vector2 coords, Vector3 eventCenterPosition, Vector3 pathStart, Vector3 pathEnd) {
		this.name = nameP;
		this.peak_energy = energy;
		this.coordinates = coords;
		this.date_captured = DateTime.Now;
		this.eventCenterPosition = eventCenterPosition;
		this.eventSnapshot = new List<VisualizeEvent.DomSnapShot> ();
		this.pathStart = pathStart;
		this.pathEnd = pathEnd;
	}

	public EventInfo(EventInfo other) {
		if (other != null) {
			this.name = other.name;
			this.peak_energy = other.peak_energy;
			this.coordinates = other.coordinates;
			this.date_captured = other.date_captured;
			this.eventCenterPosition = other.eventCenterPosition;
			this.eventSnapshot = other.eventSnapshot;
			this.pathStart = pathStart;
			this.pathEnd = pathEnd;
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


	public void setEventCenterPosition(Vector3 eventCenterPosition) {
		this.eventCenterPosition = eventCenterPosition;
	}

	public Vector3 getEventCenterPosition() {
		return this.eventCenterPosition;
	}

	public void setSnapshot(List<VisualizeEvent.DomSnapShot> snapshot) {
		this.eventSnapshot = snapshot;
	}

	public List<VisualizeEvent.DomSnapShot> getSnapshot() {
		return this.eventSnapshot;
	}

	public void setStart(Vector3 vStart) {
		this.pathStart = vStart;
	}

	public Vector3 getStart() {
		return this.pathStart;
	}

	public void setEnd(Vector3 vEnd) {
		this.pathEnd = vEnd;
	}

	public Vector3 getEnd() {
		return this.pathEnd;
	}
		
	// Used for buttons on panels
	public void GoToPuzzleView() {
		puzzleCameraController.MoveCamera (this);
	}

	public void delete() {
		puzzleCameraController.CleanUp ();
		man.removeEvent (this.gameObject);
	}
}
