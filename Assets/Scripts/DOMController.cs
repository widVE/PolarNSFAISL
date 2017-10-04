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
    private Shader standard = null;
    private Shader glow = null;
    private float oldScale = 1.0f;

    private Color defaultColor = new UnityEngine.Color(0.6666f, 0.6666f, 0.6666f, 1.0f);
    private Color defaultColor2 = new UnityEngine.Color(0.8f, 0.6f, 0.0f, 1.0f);

	// Use this for initialization
	void Start () {
        standard = Shader.Find("Standard");
        glow = Shader.Find("MK/MKGlow/Transparent/Diffuse");
        /*GameObject externalParts = transform.FindChild("s1DOM_ExternalParts").gameObject;
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
        }*/

        /*Transform t = transform.Find("low_poly_sphere");
        if (t != null)
        {
            eventSphere = t.gameObject;
        }
        else 
        {*/
            eventSphere = gameObject;
            domGlobe = eventSphere.transform.Find("group_1").gameObject.GetComponent<MeshRenderer>();
            domGlobe2 = eventSphere.transform.Find("group_2").gameObject.GetComponent<MeshRenderer>();
        //}
        
        oldScale = eventSphere.transform.localScale.x;
	}

    public void Fade(float fTimeFrac)
    {
        if (eventSphere != null)
        {
            UnityEngine.Color c = domGlobe.material.color;
            c.a = fTimeFrac;
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_Color", c);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", c);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", c);

            domGlobe.material.SetColor("_Color", c);
            domGlobe.material.SetColor("_MKGlowColor", c);
            domGlobe.material.SetColor("_MKGlowTexColor", c);

            domGlobe2.material.SetColor("_Color", c);
            domGlobe2.material.SetColor("_MKGlowColor", c);
            domGlobe2.material.SetColor("_MKGlowTexColor", c);
        }
    }

    public void TurnOn(float fTimeFrac, float fRadius)
    {
        //Debug.Log("Turning on");
        //change a material on the globe so that it glows...
        if(domGlobe != null)
        {
            domGlobe.materials[0].shader = glow;
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[0].shader = glow;
        }

        if (eventSphere != null)
        {
            eventSphere.transform.localScale = new Vector3(fRadius, fRadius, fRadius);
            float h = (fTimeFrac * 0.69f) - 0.02f;
            //Debug.Log(h.ToString("F4"));
            if(h < 0f)
            {
                h = 1f + h;
            }

            //float h = (fTimeFrac * 0.75f);
            UnityEngine.Color c = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
           // UnityEngine.Color c2 = UnityEngine.Color.HSVToRGB(h, 0.75f, 0.75f);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_Color", c);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", c);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", c);


            domGlobe.material.SetColor("_Color", c);
            domGlobe.material.SetColor("_MKGlowColor", c);
            domGlobe.material.SetColor("_MKGlowTexColor", c);

            domGlobe2.material.SetColor("_Color", c);
            domGlobe2.material.SetColor("_MKGlowColor", c);
            domGlobe2.material.SetColor("_MKGlowTexColor", c);
            
            /*float fColorFrac = 1.0f / 5.0f;
			if (fTimeFrac < fColorFrac)
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", UnityEngine.Color.red);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", UnityEngine.Color.red);
			}
			else if (fTimeFrac < 2.0f * fColorFrac)
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", orange);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", orange);
			}
			else if (fTimeFrac < 3.0f * fColorFrac)
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", UnityEngine.Color.yellow);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", UnityEngine.Color.yellow);
			}
			else if (fTimeFrac < 4.0f * fColorFrac)
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", UnityEngine.Color.green);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", UnityEngine.Color.green);
			}
			else
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", UnityEngine.Color.blue);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", UnityEngine.Color.blue);
			}*/
			/*else if (fTimeFrac < 6.0f * fColorFrac)
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", UnityEngine.Color.magenta);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", UnityEngine.Color.magenta);
			}
			else
			{
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", purple);
				eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", purple);
			}*/ 
        }

        on = true;
    }

    public void TurnOff()
    {
        if (domGlobe != null)
        {
            domGlobe.materials[0].shader = standard;
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[0].shader = standard;
        }

        if(eventSphere != null)
        {
            eventSphere.transform.localScale = new Vector3(oldScale, oldScale, oldScale);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_Color", defaultColor);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", defaultColor);
            //eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", defaultColor);


            domGlobe.material.SetColor("_Color", defaultColor);
            domGlobe.material.SetColor("_MKGlowColor", defaultColor);
            domGlobe.material.SetColor("_MKGlowTexColor", defaultColor);

            domGlobe2.material.SetColor("_Color", defaultColor2);
            domGlobe2.material.SetColor("_MKGlowColor", defaultColor2);
            domGlobe2.material.SetColor("_MKGlowTexColor", defaultColor2);
        }

        on = false;
    }
}
