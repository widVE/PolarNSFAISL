using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class used to store information of an event
/// </summary>
public class EventInfo : MonoBehaviour {

	private string name;
	private float cumulative_energy;
	private Vector2 coordinates;
	private DateTime date_captured;

	public EventInfo(string nameP, float energy, Vector2 coords) {
		this.name = nameP;
		this.cumulative_energy = energy;
		this.coordinates = coords;
		this.date_captured = DateTime.Now;
	}
}
