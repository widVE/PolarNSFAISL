using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxSwitch : MonoBehaviour {

	Material material1;
	Material material2;
	bool hey = false; 
	// Use this for initialization
	void Start () {

		RenderSettings.skybox = material1;
		
	}
	
	// Update is called once per frame
	void Update () {

		if (hey = true) {
			RenderSettings.skybox = material2; 
		}
		
	}
}
