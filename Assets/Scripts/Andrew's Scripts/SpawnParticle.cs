using UnityEngine;
using System.Collections;

public class SpawnParticle : MonoBehaviour {

	[SerializeField]
	private GameObject particlePrefab;

	private GameObject currParticle;
	private float updateListInterval = 0;
	private GameObject[] domList;
	private Vector3 target;
	private bool targetSet = false;
	public bool throwingParticle = false;
    public bool throwOnStart = false;
	//private ColorEventManager colorMan;
    private EventPlayer eventPlayer;
	private float travelInterval = 0f;
	private bool counting = false;
	private ParticleTrail trail;

	// Use this for initialization
	void Start () {
		//UpdateDomList ();
		eventPlayer = GameObject.Find("DomArray").GetComponent<EventPlayer> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (counting) 
        {
			travelInterval += Time.deltaTime;
		}

		if (throwingParticle && currParticle == null) 
        {
			throwingParticle = false;
            currParticle = (GameObject)Instantiate(particlePrefab, eventPlayer.events[eventPlayer.lastEventNumber == -1 ? 0 : eventPlayer.lastEventNumber].endPos, Quaternion.identity);
			currParticle.transform.SetParent (this.transform);

            target = eventPlayer.events[eventPlayer.lastEventNumber == -1 ? 0 : eventPlayer.lastEventNumber].startPos;
			Debug.Log ("Spawner " + this.gameObject.name + " is targeting " + target);
			currParticle.GetComponent<ParticleMovement>().MoveParticle(target);
		}
	}

    void OnDisable()
    {
        if (currParticle != null)
        {
            Debug.Log("Disabling particle spawner");
            Destroy(currParticle.gameObject);
            currParticle = null;
            stopThrowing();
        }
    }

    void OnEnable()
    {
        if (throwOnStart)
        {
            startThrowing();
        }
    }

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
