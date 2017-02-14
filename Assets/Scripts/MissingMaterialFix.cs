using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissingMaterialFix : MonoBehaviour {

	[SerializeField]
	private Material sphereColor;

	// Use this for initialization
	void Start () {
		GameObject[] spheres = GameObject.FindGameObjectsWithTag ("DOM");
		Debug.Log ("Number of Spheres: " + spheres.Length);
		if (spheres.Length != 125) {
			Debug.LogError ("Nope");
		}

		foreach (GameObject curr in spheres) {
			
			curr.transform.Find("sphere").GetComponent<MeshRenderer> ().material = sphereColor;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
