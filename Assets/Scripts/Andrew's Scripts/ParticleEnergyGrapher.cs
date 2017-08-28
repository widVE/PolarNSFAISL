using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEnergyGrapher : MonoBehaviour {

    //this class will change to serve as a point plotting class for the sphere map.

	//[Range(10, 500)]
	//public int resolution = 10;
	//private ParticleSystem.Particle[] points;
    private List<ParticleSystem.Particle> points = new List<ParticleSystem.Particle>();
	//float timer = 0f;
	private EventPlayer visEvent;

	//[SerializeField]
	//private bool randomizeData = true;

	// Use this for initialization
	void Start () {
		//points = new ParticleSystem.Particle[resolution];
		//InitializePoints ();
        //AddPoint(Vector2.zero);
        AddPoint(new Vector2(Mathf.Deg2Rad * 15f, Mathf.Deg2Rad * 10f));
        visEvent = GameObject.Find("DomArray").GetComponent<EventPlayer>();
	}

    Vector2 CalculateMollweide(float lambda, float phi, float cx=1f, float cy=1f, float cp=Mathf.PI) {
        float nextPhi = MollweideBromleyTheta(cp, phi);
        return new Vector2(cx * lambda * Mathf.Cos(nextPhi), cy * Mathf.Sin(phi));
    }
     
    float MollweideBromleyTheta(float cp, float phi) {
        float cpsinPhi = cp * Mathf.Sin(phi);
        int i = 30;
        float delta;
        float p = phi;
        do {
            delta = (phi + Mathf.Sin(phi) - cpsinPhi) / (1f + Mathf.Cos(phi));
            p -= delta;
        } while (Mathf.Abs(delta) > 0.001f && --i > 0);

        return p / 2;
    }

    public void AddPoint(Vector2 latLong)
    {
        ParticleSystem.Particle p = new ParticleSystem.Particle();
        //p.position = Vector3.zero;
        //convert latlong to particle position via mollweide projection conversion
        Vector2 pXY = CalculateMollweide(latLong.x, latLong.y, Mathf.Sqrt(2f) / (Mathf.PI * 0.5f), Mathf.Sqrt(2f), Mathf.PI);
        Debug.Log("Mollweide: " + pXY);
        p.position.Set(pXY.x*792f, pXY.y*900f, 0f);
        p.color = Color.red;
        p.size = 0.1f;
        points.Add(p);
    }

	// Update is called once per frame
	void Update () {
		/*timer += Time.deltaTime;
		if (timer > 0.01) {
			timer = 0f;
			UpdateGraph ();
		}*/

		this.GetComponent<ParticleSystem> ().SetParticles (points.ToArray(), points.Count);
	}

	/// <summary>
	/// Creates the points for the current graph
	/// Used in case resolution changes at runtime
	/// </summary>
	private void UpdateGraph() {
		
		// Setting up graph points
		/*for (int i = 0; i < points.Count - 1; i++) {
			Vector3 currPos = points [i].position;
			Vector3 temp2 = points [i + 1].position;
			currPos.y = temp2.y;
			points.[i].position = currPos;

            points[i].color = Color.red;//getColor (points [i].position.y);
			points[i].size = 0.1f;
		}

		Vector3 temp = points [points.Count - 1].position;

		// Either randomize or use VisualizeEvent totalEnergy
		if (randomizeData) {
			temp.y = Random.value;
		} else {
            temp.y = visEvent.totalEnergy*0.001f;
		}

		points [points.Count - 1].position = temp;*/
	}
		
	/*private void InitializePoints() {
		float increment = 1f / (resolution - 1);
		for (int i = 0; i < points.Length; i++) {
			float x = i * increment;
			points [i].position = new Vector3 (x, 0, 0);
		}
	}*/

	/*private Color getColor(float y) {
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
	}*/
}
