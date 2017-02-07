using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DOMController : MonoBehaviour {

    public bool on = false;
    public float lastCharge = 0.0f;
    private VisualizeEvent currentEvent = null;
    private MeshRenderer domGlobe = null;
    private MeshRenderer domGlobe2 = null;

	// Use this for initialization
	void Start () {
        GameObject externalParts = transform.FindChild("s1DOM_ExternalParts").gameObject;
        if (externalParts != null)
        {
            domGlobe = externalParts.GetComponent<MeshRenderer>();
            //r.materials[0].shader = Shader.Find("Particles/Additive");
            //need to change this on all LOD levels as well too...
            GameObject shell = externalParts.transform.FindChild("Outer").gameObject;
            if (shell != null)
            {
                domGlobe2 = shell.GetComponent<MeshRenderer>();
                //r2.materials[2].shader = Shader.Find("Particles/Additive");
            }
        } 
	}
	
	// Update is called once per frame
	void Update () 
    {
        if(currentEvent == null)
        {
            GameObject e = GameObject.Find("EventData");
            if(e != null)
            {
                currentEvent = e.GetComponent<VisualizeEvent>();
            }
        }

	    if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Tab))
        {
            if (on)
            {
                TurnOff();
            }
			else
            {
                TurnOn();
            }
        }

        if (currentEvent != null && currentEvent.eventPlaying)
        {
            //check if this DOM is near any of the visualize event spheres...
            if (!on)
            {
                for (int i = 0; i < currentEvent.eventData.Count; ++i)
                {
                    if (Vector3.Distance(transform.position, currentEvent.eventData[i].pos) < 0.5f)
                    {
                        TurnOn();
                        break;
                    }
                }
            }
        }
	}

    void TurnOn()
    {
        Debug.Log("Turning on");
        //change a material on the globe so that it glows...
        if(domGlobe != null)
        {
            domGlobe.materials[0].shader = Shader.Find("Particles/Additive");
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = Shader.Find("Particles/Additive");
        }

        on = true;
    }

    void TurnOff()
    {
        if (domGlobe != null)
        {
            domGlobe.materials[0].shader = Shader.Find("Standard");
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = Shader.Find("Standard");
        }

        on = false;
    }
}
