using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// FOR TESTING MESSAGE SYSTEM ONLY - NOT FOR USE IN FINAL PROJECT
/// In reality the clients will be making messages using a button press or something,
/// they won't be autocreated like they are here in this script
/// </summary>
public class MessageCreator : NetworkBehaviour {

	EventDataSystem system;
	float timer = 5f;

	// Use this for initialization
	void Start () {
		system = GetComponent<EventDataSystem> ();
		/*system.SetupClient ();

		if (isServer) {
			system.SetupServer ();
			Debug.Log ("Server set up \nNumber of server connections: " + NetworkServer.connections.Count);
		}*/
	}
	
	// Update is called once per frame
	void Update () {

		timer -= Time.deltaTime;

		// Don't send requests from the table top
		if (isServer && timer <= 0f) {
			//PrintConnections ();
			timer = 5f;
			return;
		}


		/*
		if (timer <= 0f) {
			timer = 5f;

			system.SendRequest ();
			Debug.Log ("Client sent request message to server");

		} */
	}


}
