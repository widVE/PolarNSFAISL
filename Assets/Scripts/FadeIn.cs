using UnityEngine;
using System.Collections;

public class FadeIn : MonoBehaviour {

    private bool startFade = false;
    private bool cameraMoveStart = false;
    public void SetStartFade() { startFade = true; }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        if (startFade)
        {
            if (gameObject.GetComponent<MeshRenderer>().material.color.a < 1.0f)
            {
                Color c = gameObject.GetComponent<MeshRenderer>().material.color;
                c.a += 0.001f;
                gameObject.GetComponent<MeshRenderer>().material.color = c;
            }
            else
            {
                //start next camera movement...

            }
        }
	}
}
