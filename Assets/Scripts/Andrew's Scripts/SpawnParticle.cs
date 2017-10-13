using UnityEngine;
using System.Collections;

public class SpawnParticle : MonoBehaviour {

	[SerializeField]
	private GameObject particlePrefab;
	[SerializeField]
	private bool repeatTarget = false;
	//[SerializeField]
	//private int seed;
	private GameObject currParticle;
	private float updateListInterval = 0;
	private GameObject[] domList;
	private Vector3 target;
	private bool targetSet = false;
	private bool throwingParticle = true;
	//private ColorEventManager colorMan;
    private EventPlayer eventPlayer;
	private float travelInterval = 0f;
	private bool counting = false;
	private ParticleTrail trail;

	//private bool particleSpawned = false;
	// Use this for initialization
	void Start () {
		//UpdateDomList ();
		eventPlayer = GameObject.Find("DomArray").GetComponent<EventPlayer> ();
		//trail = GetComponent<ParticleTrail> ();
		//if (colorMan == null) {
		//	Debug.LogError ("Couldn't find ColorEventManager component in ParticleMovement");
		//}
	}
	
	// Update is called once per frame
	void Update () {

		if (counting) {
			travelInterval += Time.deltaTime;
		}

		/*updateListInterval += Time.deltaTime;
		if (updateListInterval >= 3f) {
			updateListInterval = 0;
			UpdateDomList ();
		}*/
		/*if (domList.Length == 0) {
			return;
		}*/
		if (throwingParticle && currParticle == null) {
			throwingParticle = false;
			//colorMan.numActiveParticles++;
            currParticle = (GameObject)Instantiate(particlePrefab, eventPlayer.events[0].endPos, Quaternion.identity);
			currParticle.transform.SetParent (this.transform);

			// Find a random dom to shoot at
			//Random.InitState(seed * (int) System.DateTime.Now.Millisecond);
			if (repeatTarget) {
				// If we haven't made a target yet (first iteration), get one
				if (!targetSet) {
					//int index = Random.Range(0,domList.Length);
					//target = domList [index].transform.position;
                    target = eventPlayer.events[0].endPos;
					Debug.Log ("Spawner " + this.gameObject.name + " is targeting " + target);
					//trail.setStart (currParticle.transform.position);
					targetSet = true;
				}
				// Move it
				currParticle.GetComponent<ParticleMovement>().MoveParticle(target);
			} else {
				// Every new particle gets a new target
				//int index = Random.Range(0,domList.Length);
                target = eventPlayer.events[0].startPos;
				Debug.Log ("Spawner " + this.gameObject.name + " is targeting " + target);
				currParticle.GetComponent<ParticleMovement>().MoveParticle(target);
			}
		}
	}

	/*private void UpdateDomList() {
		domList = GameObject.FindGameObjectsWithTag ("DOM");
	}*/
		
	public void startThrowing() {
		throwingParticle = true;
	}

	public void stopThrowing() {
		throwingParticle = false;
	}

	public float getTravelInterval() {
		if (travelInterval == 0f) {
			counting = true;
			return 0f;
		}
		return travelInterval;
	}

	public void resetTravelInterval() {
		counting = false;
		travelInterval = 0f;
	}
		
}
