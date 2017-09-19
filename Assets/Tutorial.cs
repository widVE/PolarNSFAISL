using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    private int currPanelIndex = 0;
    private float lastTime = 0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float currTime = Time.time;
        if (currTime - lastTime > 10f)
        {
            lastTime = currTime;
            int nextPanelIndex = currPanelIndex+1;
            if(nextPanelIndex == transform.childCount)
            {
                nextPanelIndex = 0;
            }

            transform.GetChild(currPanelIndex).gameObject.SetActive(false);
            ActivatePanelObject apo = transform.GetChild(currPanelIndex).gameObject.GetComponent<ActivatePanelObject>();
            if(apo != null)
            {
                apo.DeactivateObject();
            }
            transform.GetChild(nextPanelIndex).gameObject.SetActive(true);
            apo = transform.GetChild(nextPanelIndex).gameObject.GetComponent<ActivatePanelObject>();
            if (apo != null)
            {
                apo.ActivateObject();
            }

            currPanelIndex = nextPanelIndex;
        }
	}
}
