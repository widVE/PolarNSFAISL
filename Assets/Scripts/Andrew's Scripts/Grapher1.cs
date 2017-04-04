using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapher1 : MonoBehaviour {

	[Range(10, 100)]
	public int resolution = 10;
	private int currentResolution;
	private ParticleSystem.Particle[] points;
	private float timer = 0f;

	// Use this for initialization
	void Start () {
		CreatePoints ();
	}
	
	// Update is called once per frame
	void Update () {

		CreatePoints ();

		this.GetComponent<ParticleSystem> ().SetParticles (points, points.Length);
	}


	/// <summary>
	/// Creates the points for the current graph
	/// Used in case resolution changes at runtime
	/// </summary>
	private void CreatePoints() {

		currentResolution = resolution;

		// This array also creates/instantiates all particles immediately
		points = new ParticleSystem.Particle[resolution];

		// Setting up graph points

		// Distance between points along x axis
		float increment = 1f / (resolution - 1);

		for (int i = 0; i < resolution; i++) {

			// Space between particles
			float x = i * increment;

			// Place the particle
			points [i].position = new Vector3 (x, sine(x), 0f);

			// We color the particle based on it's x value
			// Eventually make it based on slope?
			points[i].color = getColor(points[i].position.y);

			// Just so the particle isn't too large
			points[i].size = 0.1f;
		}
	}

	private float sine(float x) {

		timer += Time.deltaTime / 10f;
		if (timer >= 2 * Mathf.PI) {
			timer = 0f;
		}
		

		//Debug.Log ("Timer: " + timer);



		return 0.5f + 0.5f * Mathf.Sin (2 * Mathf.PI * x + timer);
	}

	private Color getColor(float y) {
		return new Color (y, 1f - y, 0);
	}
}
