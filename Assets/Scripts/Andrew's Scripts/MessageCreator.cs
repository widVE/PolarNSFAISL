using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// FOR TESTING MESSAGE SYSTEM ONLY - NOT FOR USE IN FINAL PROJECT
/// </summary>
public class MessageCreator : NetworkBehaviour {

	EventDataSystem system;
	float timer = 5f;

	// Use this for initialization
	void Start () {
		system = new EventDataSystem ();
		system.SetupClient ();

		if (isServer) {
			system.SetupServer ();
			Debug.Log ("Server set up");
			Debug.Log ("Number of server connections: " + NetworkServer.connections.Count);
		}
	}
	
	// Update is called once per frame
	void Update () {

		// Don't send requests from the table top
		if (isServer) {
			return;
		}

		timer -= Time.deltaTime;

		if (timer <= 0f) {
			timer = 5f;

			system.SendRequest ();
			Debug.Log ("Client sent request message to server");

		}
	}
}
