using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChange : MonoBehaviour {

	[SerializeField]
	private Material hoverColor;
	[SerializeField]
	private Material standardColor;
	private MeshRenderer rend;


	void Start() {
		rend = GetComponent<MeshRenderer> ();
		if (rend == null) {
			Debug.LogError ("No MeshRenderer found on point");
		}
	}

	void OnMouseEnter() {
		rend.material = hoverColor;
	}

	void OnMouseExit() {
		rend.material = standardColor;
	}
}