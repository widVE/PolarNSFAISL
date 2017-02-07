using UnityEngine;
using System.Collections;

public class ParticleMovement : MonoBehaviour {
	
	private bool move = false;
	private Vector3 target;
	private Vector3 direction;
	private float startingDistance;
	private bool setUp = false;
	[SerializeField]
	private float speed = 0.25f;

	private GameObject colorManObj;
	private ColorEventManager colorMan;

	private IEnumerator coroutine;

	void Start() {
		colorManObj = GameObject.Find ("ColorEventScript");
		if (colorManObj == null) {
			Debug.LogError ("Couldn't find ColorEventScript object in ParticleMovement");
		}
		colorMan = colorManObj.GetComponent<ColorEventManager> ();
		if (colorMan == null) {
			Debug.LogError ("Couldn't find ColorEventManager component in ParticleMovement");
		}

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

		Destroy (this.gameObject);
		colorMan.numActiveParticles--;
		colorMan.resetGame ();
	}
}
