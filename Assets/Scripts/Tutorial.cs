using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour {

    private int currPanelIndex = 0;
    private float lastTime = 0f;
    public bool playTutorial = true;
    public float tutorialSpeed = 10.0f;

	// Use this for initialization
	void Start () {
		
	}
	
    public void ClearTutorial()
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            ActivatePanelObject apo = transform.GetChild(i).GetComponent<ActivatePanelObject>();
            if(apo != null)
            {
                if (apo.deactivateOnClear)
                {
                    apo.DeactivateObject();
                }
            }

            transform.GetChild(i).gameObject.SetActive(false);
        }

        currPanelIndex = 0;
    }

	// Update is called once per frame
	void Update () {

        if (playTutorial)
        {
            float currTime = Time.time;
            if (currTime - lastTime > tutorialSpeed)
            {
                lastTime = currTime;
                int nextPanelIndex = currPanelIndex + 1;
                if (nextPanelIndex == transform.childCount)
                {
                    nextPanelIndex = 0;
                    GameObject ep = GameObject.Find("DomArray");
                    if(ep != null)
                    {
                        EventPlayer e = ep.GetComponent<EventPlayer>();
                        if(e != null)
                        {
                            e.StopCurrentEvent();
                            e.StopTutorialEvent();
                            //e.truePath.enabled = true;
                            e.truePath.enabled = false;
                            //e.scaleArray(3f);
                        }
                    }
                }

                /*if (currPanelIndex == 7)
                {
                    GameObject ep = GameObject.Find("DomArray");
                    if (ep != null)
                    {
                        EventPlayer e = ep.GetComponent<EventPlayer>();
                        if (e != null)
                        {
                            e.scaleArray(0.149815f);
                        }
                    }
                }*/

                transform.GetChild(currPanelIndex).gameObject.SetActive(false);
                ActivatePanelObject apo = transform.GetChild(currPanelIndex).gameObject.GetComponent<ActivatePanelObject>();
                if (apo != null)
                {
                    if (apo.deactivateOnSwitch)
                    {
                        apo.DeactivateObject();
                    }
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
}
