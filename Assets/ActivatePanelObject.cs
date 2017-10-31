using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatePanelObject : MonoBehaviour {

    public GameObject objectToActivate;
    public bool deactivateOnClear = true;
    public bool deactivateOnSwitch = true;
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

            if(objectToActivate.GetComponent<EventPlayer>() != null)
            {
                objectToActivate.GetComponent<EventPlayer>().keepPlaying = true;
                objectToActivate.GetComponent<EventPlayer>().StopCurrentEvent();
                objectToActivate.GetComponent<EventPlayer>().PlayTutorialEvent();
            }

            if(objectToActivate.name == "FullDOMView")
            {
                GameObject.Find("DOMArrow").SetActive(true);
            }
        }
    }

    public void DeactivateObject()
    {
        if (objectToActivate != null)
        {
            if (objectToActivate.GetComponent<EventPlayer>() != null)
            {
                objectToActivate.GetComponent<EventPlayer>().StopCurrentEvent();
                objectToActivate.GetComponent<EventPlayer>().StopTutorialEvent();
            }
            else
            {
                objectToActivate.SetActive(false);
            }


            if (objectToActivate.name == "FullDOMView")
            {
                GameObject g = GameObject.Find("DOMArrow");
                if (g != null)
                {
                    g.SetActive(false);
                }
            }
        }
    }
}
