using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkInfoDump : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		if (isServer) {
			Debug.Log ("isServer is true");
			Debug.Log (Network.connections);
		} else {
			Debug.Log ("isServer is false");
			Debug.Log (Network.connections);
		}

		if (Network.isServer) {
			Debug.Log ("Network.isServer is true");
		} else {
			Debug.Log ("Network.isServer is false");
		}




	}
}
