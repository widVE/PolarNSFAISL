using UnityEngine;
using System.Collections;

public class BlinkStation : MonoBehaviour {

    private bool toWhite = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        MeshRenderer m = GetComponent<MeshRenderer>();
        if(m != null)
        {
            Color c = m.material.color;
            if (!toWhite)
            {
                //interpolate towards red..
                float gb = UnityEngine.Mathf.Lerp(c.g, 0.0f, 0.2f);
                c.g = gb;
                c.b = gb;
            }
            else
            {
                float gb = UnityEngine.Mathf.Lerp(c.g, 1.0f, 0.2f);
                c.g = gb;
                c.b = gb;
            }

            if (c == Color.white)
            {
                toWhite = false;
            } 
            else if(c == Color.red)
            {
                toWhite = true;
            }

            m.material.color = c;
        }
	}
}
