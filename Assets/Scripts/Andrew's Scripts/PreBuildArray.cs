using UnityEngine;
using System.Collections;

public class PreBuildArray : MonoBehaviour {

	private GameObject[] spots;
	[SerializeField]
	private GameObject stringPrefab;
	// Use this for initialization
	void Start () {
		spots = GameObject.FindGameObjectsWithTag ("DomSpot");
		if (spots == null) {
			Debug.LogError ("Couldn't build an array of DomSpots");
		}
		Debug.Log ("Size of spots array: " + spots.Length);

		// Add a string and list of doms for each DomSpot
		foreach (GameObject currspot in spots) {

			// Made a string
			GameObject tempString = (GameObject)Instantiate (stringPrefab);
			tempString.transform.SetParent (currspot.transform);
			Vector3[] pos = new Vector3[2];
			pos [0] = currspot.transform.position;
			pos [1] = pos [0] - new Vector3 (0.0f, 20.0f, 0.0f);
			tempString.GetComponent<ICLString> ().SetStringPositions (pos);
			try {
				currspot.GetComponent<DomSpot> ().setString (tempString);
			} catch {
				Debug.Log ("Name that failed was: " + currspot.name);
			}

			// Add doms
			for (int i = 1; i < 6; i ++) {
				GameObject currentDOM = tempString.GetComponent<ICLString> ().AddDOM (currspot.transform.position - new Vector3 (0, i * 4, 0));
				tempString.GetComponent<ICLString> ().addDOMtoList(currentDOM);
			}
		}

		// Initialize Spawners in Random Locations around the Dom
		// Will eventually change for multiple rounds (multiple spawners)
	}

}
