using UnityEngine;
using System.Collections;

public class DomSpot : MonoBehaviour {

	private int numDoms;
	private GameObject domString;

	// Use this for initialization
	void Start () {
		numDoms = 0;
		domString = null;
	}

	public int incrementNumDoms() {
		return numDoms++;
	}

	public int getNumDoms() {
		return numDoms;
	}

	public GameObject getString() {
		return domString;
	}

	public void setString(GameObject currString) {
		domString = currString;
	}
}
