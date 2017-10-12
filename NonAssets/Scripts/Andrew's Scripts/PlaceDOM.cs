using UnityEngine;
using System.Collections;

public class PlaceDOM : MonoBehaviour {

	[SerializeField]
	private GameObject domPrefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown("space")) {
			Vector3 domLocation = new Vector3 (0, 0, 0);
			domLocation.x = ((int)(this.transform.position.x) / 5) * 5;
			domLocation.z = ((int)(this.transform.position.z) / 5) * 5;
			Instantiate (domPrefab, domLocation, Quaternion.identity);
		}
	}
}
