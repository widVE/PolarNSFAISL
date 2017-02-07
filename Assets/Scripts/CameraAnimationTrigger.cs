using UnityEngine;
using System.Collections;

public class CameraAnimationTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void EndCameraMovement(float f)
    {
        //start the map fade ..
        GameObject g = GameObject.Find("AntarcticMapQuad");
        if (g != null)
        {
            FadeIn fadeScript = g.GetComponent<FadeIn>();
            fadeScript.SetStartFade();
        }

        //Application.UnloadLevel(0);
        //Application.LoadLevel(1);
    }
}
