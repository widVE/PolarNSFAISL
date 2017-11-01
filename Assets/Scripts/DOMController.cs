using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DOMController : MonoBehaviour {

    public bool on = false;
    public float lastCharge = 0.0f;
    public int stringNum = 0;
    public int domNum = 0;

    private GameObject eventSphere = null;
    private MeshRenderer domGlobe = null;
    private MeshRenderer domGlobe2 = null;

    private float oldScale = 1.0f;

    private Color defaultColor = new UnityEngine.Color(0.6666f, 0.6666f, 0.6666f, 1.0f);
    private Color defaultColor2 = new UnityEngine.Color(0.8f, 0.6f, 0.0f, 1.0f);

	// Use this for initialization
	void Start () {

        eventSphere = gameObject;
        domGlobe = eventSphere.transform.Find("group_1").gameObject.GetComponent<MeshRenderer>();
        domGlobe2 = eventSphere.transform.Find("group_2").gameObject.GetComponent<MeshRenderer>();
        defaultColor = domGlobe.material.color;
        defaultColor2 = domGlobe2.material.color;

        oldScale = eventSphere.transform.localScale.x;
	}

    public void Fade(float fTimeFrac)
    {
        if (eventSphere != null)
        {
            UnityEngine.Color c = domGlobe.material.color;
            c.a = fTimeFrac;

            domGlobe.material.SetColor("_Color", c);
            domGlobe.material.SetColor("_EmissionColor", c);

            domGlobe2.material.SetColor("_Color", c);
            domGlobe2.material.SetColor("_EmisisonColor", c);
        }
    }

    public void TurnOn(float fTimeFrac, float fRadius)
    {
        //Debug.Log("Turning on");
        //change a material on the globe so that it glows...
        //except this causes material memory leak - so don't want to do a material:  http://answers.unity3d.com/questions/548420/material-memory-leak.html

       /* if(domGlobe != null)
        {
            domGlobe.material = glowMaterial;
        }

        if (domGlobe2 != null)
        {
            domGlobe2.material = glowMaterial;
        }*/

        if (eventSphere != null)
        {
            eventSphere.transform.localScale = new Vector3(fRadius, fRadius, fRadius);
            float h = (fTimeFrac * 0.69f) - 0.02f;
            //Debug.Log(h.ToString("F4"));
            if(h < 0f)
            {
                h = 1f + h;
            }

            UnityEngine.Color c = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
            c.a = 1.0f;

            domGlobe.material.SetColor("_Color", c);
            domGlobe.material.SetColor("_EmissionColor", c);

            domGlobe2.material.SetColor("_Color", c);
            domGlobe2.material.SetColor("_Color", c);
        }

        on = true;
    }

    public void TurnOff()
    {
        if(eventSphere != null)
        {
            eventSphere.transform.localScale = new Vector3(oldScale, oldScale, oldScale);

            domGlobe.material.SetColor("_Color", defaultColor);
            domGlobe2.material.SetColor("_Color", defaultColor2);
        }

        on = false;
    }
}
