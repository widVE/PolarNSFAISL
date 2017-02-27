using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeutrinoViewToggle : MonoBehaviour {

    public bool neutrinoViewOn = false;
    public Material neutrinoSkybox;
    public Material defaultSkybox;
	// Use this for initialization
	void Start () {
		
	}

	public void ToggleNeutrinoView() {
		neutrinoViewOn = !neutrinoViewOn;
		//currSkybox = UnityEngine.Scene
		if(neutrinoViewOn)
		{
			UnityEngine.Camera.main.GetComponent<Skybox>().material = neutrinoSkybox;
		}
		else
		{
			UnityEngine.Camera.main.GetComponent<Skybox>().material = defaultSkybox;
		}
	}

	// Update is called once per frame
	void Update () {
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.N))
        {
			ToggleNeutrinoView ();
        }		
	}
}
