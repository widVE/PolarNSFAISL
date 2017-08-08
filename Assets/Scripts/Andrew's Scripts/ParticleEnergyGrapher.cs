using UnityEngine;

public class ParticleEnergyGrapher : MonoBehaviour {

	[Range(10, 500)]
	public int resolution = 10;
	private ParticleSystem.Particle[] points;
	float timer = 0f;
	private EventPlayer visEvent;

	[SerializeField]
	private bool randomizeData = true;

	// Use this for initialization
	void Start () {
		points = new ParticleSystem.Particle[resolution];
		InitializePoints ();
        visEvent = GameObject.Find("DomArray").GetComponent<EventPlayer>();
	}

	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer > 0.01) {
			timer = 0f;
			UpdateGraph ();
		}

		this.GetComponent<ParticleSystem> ().SetParticles (points, points.Length);
	}


	/// <summary>
	/// Creates the points for the current graph
	/// Used in case resolution changes at runtime
	/// </summary>
	private void UpdateGraph() {
		
		// Setting up graph points
		for (int i = 0; i < points.Length - 1; i++) {
			Vector3 currPos = points [i].position;
			Vector3 temp2 = points [i + 1].position;
			currPos.y = temp2.y;
			points [i].position = currPos;

			points [i].color = getColor (points [i].position.y);
			points[i].size = 0.1f;
		}

		Vector3 temp = points [points.Length - 1].position;

		// Either randomize or use VisualizeEvent totalEnergy
		if (randomizeData) {
			temp.y = Random.value;
		} else {
            temp.y = visEvent.totalEnergy*0.001f;
		}

		points [points.Length - 1].position = temp;
	}
		
	private void InitializePoints() {
		float increment = 1f / (resolution - 1);
		for (int i = 0; i < points.Length; i++) {
			float x = i * increment;
			points [i].position = new Vector3 (x, 0, 0);
		}
	}

	private Color getColor(float y) {
		float green;
		if (y < 0.5f) {
			green = 1f;
		} else {
			green = 2 - (2 * y);
		}
		float red;
		if (y > 0.5f) {
			red = 1f;
		} else {
			red = 2 * y;
		}


		return new Color (red, green, 0);
	}
}
