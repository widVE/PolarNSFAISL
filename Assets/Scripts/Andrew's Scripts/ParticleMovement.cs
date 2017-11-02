using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleMovement : MonoBehaviour {
	
	private bool move = false;
	private Vector3 target;
	private Vector3 direction;
	private float startingDistance;
	private bool setUp = false;

	[SerializeField]
	private float speed = 0.2f;

	private IEnumerator coroutine;

	int index = 1;
	float interval = 0f;

	void Start() {
		coroutine = WaitForReset (0.1f);
	}

	// Update is called once per frame
	void Update () {
		if (move) {
			if(!setUp) {
				direction = target - this.transform.position;
				startingDistance = Vector3.Distance (this.transform.position, target);
				setUp = true;
			}
			this.transform.Translate (direction * speed * Time.deltaTime);
			if (Vector3.Distance(this.transform.position, target) >= startingDistance) {
				move = false;
				StartCoroutine (coroutine);
			}
		}
	}

	public void MoveParticle(Vector3 targetP) {
		target = targetP;
		move = true;
	}

	private IEnumerator WaitForReset(float waitTime) {
		yield return new WaitForSeconds (waitTime);

		//trail.setEnd (this.transform.position);
		//trail.enterTraceMode ();
		Destroy (this.gameObject);
	}
}
