using UnityEngine;
using System.Collections;

public class DetectDOMSpot : MonoBehaviour {

	void OnTriggerEnter(Collider other) {
		Debug.Log ("Trigger Entered on " + other.gameObject.name);
		if (other.gameObject.tag.Equals("DomSpot")) {
			Debug.Log ("Updating Domspot");
			GetComponentInParent<DomSpotManager> ().SetDomSpot (other.transform);
		}
	}

	void OnTriggerExit(Collider other) {
		//Debug.Log ("Trigger Exited on " + other.gameObject.name);
	}
}
