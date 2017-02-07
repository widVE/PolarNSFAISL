using UnityEngine;
using System.Collections;



public class ParticleCollision : MonoBehaviour {

	private float prevdistance = 100000f;

	void OnTriggerStay(Collider other) {
		if (!other.gameObject.tag.Equals("Particle")) {
			//Debug.LogError ("DOM collided with something not a particle!");
			return;
		}
		//Debug.Log ("Collided with particle!");
		// Collided with the particle, get the distance and scale the dom for now
		float distance = Vector3.Distance(this.transform.position, other.transform.position);
		if (distance != prevdistance) {
			prevdistance = distance;
			float coeff = Mathf.Sqrt(10f / distance);
			this.transform.localScale = new Vector3(1f,1f,1f) * coeff;
		}

	}

	void OnTriggerExit(Collider other) {
		if (!other.gameObject.tag.Equals("Particle")) {
			//Debug.LogError ("DOM exited collider of something not a particle!");
			return;
		}
		this.transform.localScale = new Vector3 (1f, 1f, 1f);
		//Debug.Log ("Particle exited nicely");
	}
}
