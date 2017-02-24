using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DOMController : MonoBehaviour {

    public bool on = false;
    public float lastCharge = 0.0f;
    public int stringNum = 0;
    public int domNum = 0;
    private MeshRenderer domGlobe = null;
    private MeshRenderer domGlobe2 = null;
    private float oldScale = 1.0f;

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
        //only if camera has moved...
        if (UnityEngine.Camera.main.velocity.magnitude > 0.0f)
        {
            //resize line widths...
            LineRenderer r = GetComponent<LineRenderer>();
            if (r != null)
            {
                float w = Vector3.Distance(UnityEngine.Camera.main.transform.position, transform.position) / 1000.0f;
                r.startWidth = w;
                r.endWidth = w;
            }
        }
	}

    public void TurnOn(float fTimeFrac, float fRadius)
    {
        //Debug.Log("Turning on");
        //change a material on the globe so that it glows...
        if(domGlobe != null)
        {
            domGlobe.materials[0].shader = Shader.Find("Particles/Additive");
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = Shader.Find("Particles/Additive");
        }

        GameObject eventSphere = transform.FindChild("Sphere").gameObject;
        if (eventSphere != null)
        {
            oldScale = eventSphere.transform.localScale.x;
            eventSphere.transform.localScale = new Vector3(fRadius, fRadius, fRadius);

            float fColorFrac = 1.0f / 7.0f;

            if (fTimeFrac < fColorFrac)
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.red;
            }
            else if (fTimeFrac < 2.0f * fColorFrac)
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(1.0f, 0.5f, 0.0f, 1.0f);
            }
            else if (fTimeFrac < 3.0f * fColorFrac)
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.yellow;
            }
            else if (fTimeFrac < 4.0f * fColorFrac)
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.green;
            }
            else if (fTimeFrac < 5.0f * fColorFrac)
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.blue;
            }
            else if (fTimeFrac < 6.0f * fColorFrac)
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.magenta;
            }
            else
            {
                eventSphere.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(0.5f, 0.0f, 1.0f, 1.0f);
            }
        }

        on = true;
    }

    public void TurnOff()
    {
        if (domGlobe != null)
        {
            domGlobe.materials[0].shader = Shader.Find("Standard");
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = Shader.Find("Standard");
        }

        GameObject eventSphere = transform.FindChild("Sphere").gameObject;
        if(eventSphere != null)
        {
            eventSphere.transform.localScale = new Vector3(oldScale, oldScale, oldScale);
            eventSphere.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 0.25f);
        }

        on = false;
    }
}
