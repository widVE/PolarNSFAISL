using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DOMController : MonoBehaviour {

    public bool on = false;
    
    public int stringNum = 0;
    public int domNum = 0;

    private MeshRenderer domGlobe = null;
    private MeshRenderer domGlobe2 = null;

    private float oldScale = 1.0f;

    private Color defaultColor = new UnityEngine.Color(0.6666f, 0.6666f, 0.6666f, 1.0f);
    private Color defaultColor2 = new UnityEngine.Color(0.8f, 0.6f, 0.0f, 1.0f);

	// Use this for initialization
	void Start () {
        oldScale = gameObject.transform.localScale.x;
        gameObject.SetActive(false);
	}

    public void Fade(float fTimeFrac)
    {
        UnityEngine.Color c = domGlobe.material.color;
        c.a = fTimeFrac;

        domGlobe.material.SetColor("_Color", c);
        domGlobe.material.SetColor("_EmissionColor", c);

        domGlobe2.material.SetColor("_Color", c);
        domGlobe2.material.SetColor("_EmissionColor", c);
    }

    public void TurnOn(float fTimeFrac, float fRadius)
    {
        //Debug.Log("Turning on: " + fRadius);
        gameObject.SetActive(true);
        if(domGlobe == null)
        {
            domGlobe = gameObject.transform.Find("group_1").gameObject.GetComponent<MeshRenderer>();
            defaultColor = domGlobe.material.color;
        }
        
        if (domGlobe2 == null)
        {
            domGlobe2 = gameObject.transform.Find("group_2").gameObject.GetComponent<MeshRenderer>();
            defaultColor2 = domGlobe2.material.color;
        }

        Vector3 v = gameObject.transform.localScale;
        v.Set(fRadius, fRadius, fRadius);
        gameObject.transform.localScale = v;

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

        on = true;
    }

    public void TurnOff()
    {
        Vector3 v = gameObject.transform.localScale;
        v.Set(oldScale, oldScale, oldScale);
        gameObject.transform.localScale = v;

        if (domGlobe != null)
        {
            domGlobe.material.SetColor("_Color", defaultColor);
        }

        if (domGlobe2 != null)
        {
            domGlobe2.material.SetColor("_Color", defaultColor2);
        }

        gameObject.SetActive(false);
        
        on = false;
    }
}
