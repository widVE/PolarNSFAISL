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
    private Shader partAdd = null;
    private float oldScale = 1.0f;

    private Color orange = new UnityEngine.Color(1.0f, 0.5f, 0.0f, 1.0f);
    private Color purple = new UnityEngine.Color(0.5f, 0.0f, 1.0f, 1.0f);
    private Color defaultColor = new UnityEngine.Color(0.5f, 0.5f, 0.5f, 1.0f);

	// Use this for initialization
	void Start () {
        /*standard = Shader.Find("Standard");
        partAdd = Shader.Find("Particles/Additive");
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
        }*/

        eventSphere = transform.FindChild("low_poly_sphere").gameObject;
        oldScale = eventSphere.transform.localScale.x;
	}

    public void TurnOn(float fTimeFrac, float fRadius)
    {
        //Debug.Log("Turning on");
        //change a material on the globe so that it glows...
       /* if(domGlobe != null)
        {
            domGlobe.materials[0].shader = partAdd;
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = partAdd;
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

            //float h = (fTimeFrac * 0.75f);
            UnityEngine.Color c = UnityEngine.Color.HSVToRGB(h, 1f, 1f);
           // UnityEngine.Color c2 = UnityEngine.Color.HSVToRGB(h, 0.75f, 0.75f);
            eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", c);
            eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", c);
            
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
        /*if (domGlobe != null)
        {
            domGlobe.materials[0].shader = standard;
        }

        if (domGlobe2 != null)
        {
            domGlobe2.materials[2].shader = standard;
        }*/

        if(eventSphere != null)
        {
            eventSphere.transform.localScale = new Vector3(oldScale, oldScale, oldScale);
            eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowColor", defaultColor);
            eventSphere.GetComponent<MeshRenderer>().material.SetColor("_MKGlowTexColor", defaultColor);
        }

        on = false;
    }
}
