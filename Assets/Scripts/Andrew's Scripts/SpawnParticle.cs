using UnityEngine;
using System.Collections;

public class SpawnParticle : MonoBehaviour {

	public GameObject particlePrefab;
	private GameObject currParticle;
	
	public Vector3 target;
	private bool targetSet = false;
	public bool throwingParticle = false;
    public bool throwOnStart = false;
	
    private EventPlayer eventPlayer;
	
	private bool counting = false;
	private ParticleTrail trail;

	// Use this for initialization
	void Start () {
		
		eventPlayer = GameObject.Find("DomArray").GetComponent<EventPlayer> ();
	}
	
	// Update is called once per frame
	void Update () {

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

	public void startThrowing() 
    {
		throwingParticle = true;
	}

	public void stopThrowing() 
    {
		throwingParticle = false;
	}
}
