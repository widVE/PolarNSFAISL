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
	//private ParticleTrail trail;

	//private ColorEventManager colorMan;

	private IEnumerator coroutine;
	//private Vector3[] linePositions = new Vector3[500];
	//private LineRenderer linRen;
	int index = 1;
	float interval = 0f;

	void Start() {
		//linRen = this.GetComponent<LineRenderer> ();
		//linRen.material = new Material(Shader.Find ("Particles/Additive"));
		//colorMan = GameObject.Find("DOMArray").GetComponent<ColorEventManager> ();
		//trail = this.transform.parent.GetComponent<ParticleTrail> ();
		/*if (colorMan == null) {
			Debug.LogError ("Couldn't find ColorEventManager component in ParticleMovement");
		}*/

		coroutine = WaitForReset (0.1f);

		//linePositions [0] = this.transform.position;
		//linRen.numPositions = 500;
		//linRen.SetPosition(0, this.transform.position);
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
				//move = false;
				//StartCoroutine (coroutine);
			}
            //Debug.Log(transform.position);
//			if (interval > 0.01 && index < 500) {
//				//linePositions [index] = this.transform.position;
//				linRen.SetPosition(index, this.transform.position);
//				index++;
//				Debug.Log (index);
//				interval = 0;
//			} else {
//				interval += Time.deltaTime;
//			}



		}
	}

	public void MoveParticle(Vector3 targetP) {
		target = targetP;
		move = true;
	}

	private IEnumerator WaitForReset(float waitTime) {
		yield return new WaitForSeconds (waitTime);


		//colorMan.numActiveParticles--;
		//colorMan.resetGame ();
		//trail.setEnd (this.transform.position);
		//trail.enterTraceMode ();
		Destroy (this.gameObject);
	}
}
