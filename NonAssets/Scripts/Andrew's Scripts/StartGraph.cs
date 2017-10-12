using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGraph : MonoBehaviour {

	private GameObject graphObject;

	// Use this for initialization
	void Start () {
		Instantiate (graphObject, this.transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
