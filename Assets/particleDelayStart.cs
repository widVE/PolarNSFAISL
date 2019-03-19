using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleDelayStart : MonoBehaviour {
    ParticleSystem system;

	// Use this for initialization
	void Start () {
        system = gameObject.transform.GetChild(2).GetComponent<ParticleSystem>();
        system.Stop();
        StartCoroutine(Wait(1.5f));
        //system.Play();

        Debug.Log(gameObject.transform.GetChild(2).name);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator Wait(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        system.Play();
    }
}
