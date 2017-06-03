using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Class for handling messages across a single Tabletop (IceCube Array) instance and an arbitrary number of
/// Oculus rift players (or as many as Unity limits....10?)
/// Only one instanced of this script is needed per client (in their scene), and use SetupClient -OR- SetupTableTop functions here to.....well....set it up.
/// All functions needed to create and handle messages are within this class.
/// </summary>
public class EventDataSystem : MonoBehaviour {

	// Reference to the current client - EVERY instance of this game needs a client
	public NetworkClient myClient = null;

	// Port to send messages through, arbitrarily chosen
	private int port = 5555;

	// Flag for if this client is the TableTop (server exists in same program instance)
	private bool isTabletopClient = false;

	// Server-side connectionID for the Tabletop - ASSUMES the Tabletop is set up first, so always
	// start server before starting Oculus players!!!!
	private int tableTopID = 1;

	/// <summary>
	/// Subclass to hold message ID's, used for client and server handlers
	/// </summary>
	public class MyMessageTypes {
		public static short EventRequestID = MsgType.Highest + 1;
		public static short EventDataID = (short)(EventRequestID + 1);
		public static short EventResultID = (short)(EventDataID + 1);
	};

	/// <summary>
	/// Message to be sent between the server and the clients, holding event data
	/// </summary>
	public class EventDataMessage : MessageBase{
		public Vector2 coordinates;
		public float energy;
		public int destinationClientID;
	};


	/// <summary>
	/// Message used to request event data from the table top
	/// </summary>
	public class EventRequestMessage : MessageBase{
		public int sourceClientID;
	};

	/// <summary>
	/// Message used to request results found from an Oculus Rift player
	/// </summary>
	public class EventResultMessage : MessageBase{
		public string phenomenaName;
		public int sourceClientID;
	};

	/// <summary>
	/// UNUSED - Sends event data to the server.
	/// Should only be called from the tabletop, but is unused since the tabletop only sends data
	/// in response to a request (see ClientRequestReceived)
	/// </summary>
	/// <param name="coordinates">Coordinates.</param>
	/// <param name="energy">Energy.</param>
	/// <param name="source">Source.</param>
	/*public void SendData(Vector2 coordinates, float energy, string source) {

		if (!isTabletopClient) {
			Debug.LogError ("SendData was called from Oculus client - did you mean to use SendResults to send the results of a discovery?");
		}

		EventDataMessage msg = new EventDataMessage ();
		msg.energy = energy;
		msg.coordinates = coordinates;
		msg.destinationClientID = -1;

		myClient.Send (MyMessageTypes.EventDataID, msg);

		//NetworkServer.SendToClient (clientID, MyMessageTypes.EventDataID, msg);
	}*/

	/// <summary>
	/// Sends a request for data to the tabletop - ONLY Oculus clients should call this!
	/// </summary>
	/// <param name="name">Name of the data requested (placeholder param - in case we want to specify an event name/type/location)</param>
	public void SendRequest() {
		if (isTabletopClient) {
			Debug.LogWarning ("You're the tabletop, you can't send requests because you already have all of the data!");
			return;
		}

		EventRequestMessage msg = new EventRequestMessage ();
		msg.sourceClientID = -1;

		Debug.Log ("Client sent the request to the server");
		myClient.Send (MyMessageTypes.EventRequestID, msg);
	}

	/// <summary>
	/// (For use when an oculus rift player discovers something)
	/// Sends the results to the server, which broadcasts the results to ALL
	/// clients (even the client who found the phenomena)
	/// </summary>
	/// <param name="resultName">Result name.</param>
	public void SendResults(string resultName) {
		EventResultMessage msg = new EventResultMessage ();
		msg.phenomenaName = resultName;
		msg.sourceClientID = -1;

		myClient.Send (MyMessageTypes.EventResultID, msg);
	}

	/// <summary>
	/// Creates a message client. This function should only be called outside "SetupTableTop" when
	/// we want to setup an oculus client. The tabletop client is made when the server is created.
	/// </summary>
	public void SetupClient() {

		if (myClient != null) {
			if (isTabletopClient) {
				Debug.LogWarning ("Process already setup as an TableTop (with a server), restart the program to setup as Oculus player (without a server)");
			} else {
				Debug.LogWarning ("Process already setup as an Oculus Client, restart the program to set it up again");
			}
		} else {
			myClient = new NetworkClient ();
			myClient.RegisterHandler (MsgType.Connect, OnConnected);
			myClient.RegisterHandler (MyMessageTypes.EventDataID, ClientDataReceived);
			myClient.RegisterHandler (MyMessageTypes.EventRequestID, ClientRequestReceived);
			myClient.RegisterHandler (MyMessageTypes.EventResultID, ClientResultReceived);
			myClient.Connect ("127.0.0.1", port);
		}

	}

	/// <summary>
	/// Creates a Server for the messages (background), AND creates a message client for the tabletop
	/// </summary>
	public void SetupTableTop() {

		if (myClient != null) {
			if (isTabletopClient) {
				Debug.LogWarning ("Process already setup as a tabletop");
			} else {
				Debug.LogWarning ("Process already setup as an Oculus Client (no server), restart the program to setup as tabletop (with server)");
			}

		} else {
			
			NetworkServer.RegisterHandler (EventDataSystem.MyMessageTypes.EventRequestID, ServerHandleRequest);
			NetworkServer.RegisterHandler (EventDataSystem.MyMessageTypes.EventDataID, ServerHandleData);
			NetworkServer.RegisterHandler (EventDataSystem.MyMessageTypes.EventResultID, ServerHandleResult);
			NetworkServer.Listen (port);
	
			// We started a server on the same program instance as this client, so this should be the tabletop process
			SetupClient();
			isTabletopClient = true;
		}

	}

	/// <summary>
	/// Just a handler for when a client connects to the server, called on the client
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	public void OnConnected(NetworkMessage netMsg) {
		Debug.Log("Connected to server");
		myClient.connection.isReady = true;
	}

	/// <summary>
	/// Handler for when a request is received - should only be triggered on the tabletop client.
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	public void ClientRequestReceived(NetworkMessage netMsg) {

		if (!isTabletopClient) {
			Debug.LogError ("Request received by non-host client");
			return;
		}
		EventRequestMessage msg = netMsg.ReadMessage<EventRequestMessage> ();
		Debug.Log("Client received a request");

		// Temporary - send blank message back to client
		EventDataMessage response = new EventDataMessage();
		response.coordinates = new Vector2 (0f, 0f);
		response.energy = 10f;
		response.destinationClientID = msg.sourceClientID;

	
		// TODO: Do stuff with this request, create a new EventDataMessage, and send it back to
		// whomever sent the request

		// Access data from our list

		// Make a response message

		// For now, just send a dummy data back
		myClient.Send (MyMessageTypes.EventDataID, response);
		Debug.Log ("Table Client sent a data response to the server");

		// Send it to the client that asked for it
	}

	/// <summary>
	/// Handler for when the client receives the data it requested.
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	public void ClientDataReceived(NetworkMessage netMsg) {

		EventDataMessage msg = netMsg.ReadMessage<EventDataMessage> ();

		Debug.Log ("Client received data message");

		// TODO: Parse the message and store it somewhere for use

		// FOR TESTING ONLY - immediately send a dummy result
		SendResults("Good data");
		Debug.Log("Client sent a result to the server");
	}
		
	/// <summary>
	/// Handler for when a client recieves a message stating a phenomena was found.
	/// Used in case we want to flash something on screen, like "A black hole was found by player X!"
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	public void ClientResultReceived(NetworkMessage netMsg) {

		EventResultMessage msg = netMsg.ReadMessage<EventResultMessage> ();

		Debug.Log ("Hurray, client " + msg.sourceClientID + " found a " + msg.phenomenaName + "!");
	}


	//------------------------SERVER SIDE FUNCTIONS-------------------------------

	/// <summary>
	/// Forwards a request for data made by the client to the tabletop
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	void ServerHandleRequest(NetworkMessage netMsg) {

		Debug.Log ("Server received request");
		EventRequestMessage msg = netMsg.ReadMessage<EventRequestMessage> ();
		msg.sourceClientID = netMsg.conn.connectionId;

		// Send this to host
		NetworkServer.SendToClient (tableTopID, MyMessageTypes.EventRequestID, msg);
	}

	/// <summary>
	/// Forwards data received from the tabletop to the client that requested it
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	void ServerHandleData(NetworkMessage netMsg) {

		Debug.Log ("Server received data");
		EventDataMessage msg = netMsg.ReadMessage<EventDataMessage> ();

		NetworkServer.SendToClient (msg.destinationClientID, MyMessageTypes.EventDataID, msg);
		Debug.Log ("Server forwarded the data to the requester");
	}

	/// <summary>
	/// Broadcasts a result received from a client to all other clients, including the original sending client
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	void ServerHandleResult(NetworkMessage netMsg) {

		Debug.Log ("Server received result from a client");
		EventResultMessage msg = netMsg.ReadMessage<EventResultMessage> ();
		msg.sourceClientID = netMsg.conn.connectionId;

		Debug.Log ("Server sent the result to ALL clients");
		NetworkServer.SendToAll (MyMessageTypes.EventResultID, msg);
	}

	/// <summary>
	/// Prints the list of current connections on the server (server-side ID's)
	/// Will not work if called on an Oculus client, since their processes don't own the server
	/// </summary>
	public void PrintServerConnectionInfo() {

		// If it is not called by the table top client, then it has no server!
		if (!isTabletopClient) {
			Debug.Log ("PrintServerConnectionInfo was called, but this client has no server");
		} else {
			Debug.Log ("---SERVER CONNECTIONS---\nNumber of server connections: " + NetworkServer.connections.Count);
			foreach (NetworkConnection curr in NetworkServer.connections) {
				if (curr == null) {continue;}
				Debug.Log (curr.ToString ());
			}
		}
	}

	/// <summary>
	/// Prints the connection info for the current client (client-side ID's)
	/// </summary>
	public void PrintClientConnectionInfo() {
		Debug.Log (myClient.connection.ToString ());
	}
}
