using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class used to store information of an event
/// </summary>
public class EventInfo : MonoBehaviour {

	private  EventPanelManager man;
	private string name;
	private float peak_energy;
	private Vector2 coordinates;
	private DateTime date_captured;

	void Start() {
		man = this.transform.parent.GetComponent<EventPanelManager> ();
	}

	public EventInfo(string nameP, float energy, Vector2 coords) {
		this.name = nameP;
		this.peak_energy = energy;
		this.coordinates = coords;
		this.date_captured = DateTime.Now;
	}

	public EventInfo(EventInfo other) {
		if (other != null) {
			this.name = other.name;
			this.peak_energy = other.peak_energy;
			this.coordinates = other.coordinates;
			this.date_captured = other.date_captured;
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

	public void SendEventToOculus() {
		man.removeEvent (this.gameObject);
	}
}
