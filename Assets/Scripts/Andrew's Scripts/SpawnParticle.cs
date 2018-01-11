using UnityEngine;
using System.Collections;

public class SpawnParticle : MonoBehaviour {

	public GameObject particlePrefab;
    public GameObject label;
	private GameObject currParticle;
	
	public Vector3 target;
    public Vector3 start;

	private bool targetSet = false;
	public bool throwingParticle = false;
    public bool throwOnStart = false;
    public bool followEvent = true;

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
            if (followEvent)
            {
                currParticle = (GameObject)Instantiate(particlePrefab, eventPlayer.events[eventPlayer.lastEventNumber == -1 ? 0 : eventPlayer.lastEventNumber].endPos, Quaternion.identity);
                currParticle.transform.SetParent(this.transform);
                label.SetActive(true);
                label.transform.localPosition = new Vector3(299, 247, 0);
                target = eventPlayer.events[eventPlayer.lastEventNumber == -1 ? 0 : eventPlayer.lastEventNumber].startPos;
                //Debug.Log("Spawner " + this.gameObject.name + " is targeting " + target);
                currParticle.GetComponent<ParticleMovement>().MoveParticle(target);
                //label.transform.Translate(Vector3.down);
            }
            else 
            {
                currParticle = (GameObject)Instantiate(particlePrefab, start, Quaternion.identity);
                currParticle.transform.SetParent(this.transform);

                //Debug.Log("Spawner " + this.gameObject.name + " is targeting " + target);
                currParticle.GetComponent<ParticleMovement>().MoveParticle(target);
                //label.transform.Translate(Vector3.down);
            }
		} else if (currParticle != null)
        {
            label.transform.Translate(Vector3.down * 3.5f);
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
