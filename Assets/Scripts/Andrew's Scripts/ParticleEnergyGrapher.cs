using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEnergyGrapher : MonoBehaviour {

    //this class will change to serve as a point plotting class for the sphere map.
    private List<ParticleSystem.Particle> points = new List<ParticleSystem.Particle>();
	
	// Use this for initialization
	void Start () {
        AddPoint(Vector2.zero);
	}

    public void AddPoint(Vector2 latLong)
    {
        ParticleSystem.Particle p = new ParticleSystem.Particle();
        //p.position = Vector3.zero;
        //convert latlong to particle position via mollweide projection conversion
        p.position.Set(0f, 0f, 0f);
        p.color = Color.red;
        p.size = 0.1f;
        points.Add(p);
        this.GetComponent<ParticleSystem>().SetParticles(points.ToArray(), points.Count);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
