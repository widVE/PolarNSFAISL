using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePanelObject : MonoBehaviour {

    public GameObject objectToActivate;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void ActivateObject()
    {
        if(objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }

    public void DeactivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(false);
        }
    }
}
