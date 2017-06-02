using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventDataSystem : NetworkBehaviour {

	public NetworkClient myClient;

	private int port = 5555;

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

	public class EventResultMessage : MessageBase{
		public string phenomenaName;
		public int sourceClientID;
	};

	/// <summary>
	/// Sends event data - should only be called from the tabletop!
	/// </summary>
	/// <param name="coordinates">Coordinates.</param>
	/// <param name="energy">Energy.</param>
	/// <param name="source">Source.</param>
	/*public void SendData(Vector2 coordinates, float energy, string source) {

		if (!isServer) {
			Debug.LogError ("SendData was called from non-host client - did you mean to use SendResults?");
		}

		EventDataMessage msg = new EventDataMessage ();
		msg.source = source;
		msg.energy = energy;
		msg.coordinates = coordinates;
		msg.clientID = -1;

		myClient.Send (MyMessageTypes.EventDataID, msg);

		//NetworkServer.SendToClient (clientID, MyMessageTypes.EventDataID, msg);
	}*/

	/// <summary>
	/// Sends a request for data to the tabletop - ONLY Oculus clients should call this!
	/// </summary>
	/// <param name="name">Name.</param>
	public void SendRequest() {
		EventRequestMessage msg = new EventRequestMessage ();
		msg.sourceClientID = -1;

		myClient.Send (MyMessageTypes.EventRequestID, msg);
	}

	public void SendResults(string resultName) {
		EventResultMessage msg = new EventResultMessage ();
		msg.phenomenaName = name;
		msg.sourceClientID = -1;

		myClient.Send (MyMessageTypes.EventResultID, msg);
	}

	public void SetupClient() {
		myClient = new NetworkClient ();
		myClient.RegisterHandler (MsgType.Connect, OnConnected);
		myClient.RegisterHandler (MyMessageTypes.EventDataID, ClientDataReceived);
		myClient.RegisterHandler (MyMessageTypes.EventRequestID, ClientRequestReceived);
		myClient.RegisterHandler (MyMessageTypes.EventResultID, ClientResultReceived);
		myClient.Connect ("127.0.0.1", port);
	}

	public void SetupServer() {
		NetworkServer.Listen (port);
		NetworkServer.RegisterHandler (EventDataSystem.MyMessageTypes.EventRequestID, ServerHandleRequest);
		NetworkServer.RegisterHandler (EventDataSystem.MyMessageTypes.EventDataID, ServerHandleData);
		NetworkServer.RegisterHandler (EventDataSystem.MyMessageTypes.EventResultID, ServerHandleRequest);
	}

	public void OnConnected(NetworkMessage netMsg) {
		Debug.Log("Connected to server");
		myClient.connection.isReady = true;
	}

	/// <summary>
	/// Handler for when a request is received - should only be triggered on the tabletop.
	/// </summary>
	/// <param name="netMsg">Net message.</param>
	public void ClientRequestReceived(NetworkMessage netMsg) {


		if (!isServer) {
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

		myClient.Send (MyMessageTypes.EventDataID, response);

		// NetworkServer.SendToClient (netMsg.conn.connectionId, MyMessageTypes.EventDataID, response);
		// Do stuff with this request, create a new EventDataMessage, and send it back to
		// whomever sent the request

		// Access data from our list

		// Make a response message


		// Send it to the client that asked for it
	}

	public void ClientDataReceived(NetworkMessage netMsg) {

		EventDataMessage msg = netMsg.ReadMessage<EventDataMessage> ();

		Debug.Log ("Client received data message");

		// Do stuff with the message, like look at it

		// TESTING ONLY - immediately send a response
		SendResults("Good data");
	}

	public void ClientResultReceived(NetworkMessage netMsg) {

		EventResultMessage msg = netMsg.ReadMessage<EventResultMessage> ();

		if (isServer) {
			Debug.Log ("Storing results in our list - Hurray client " + msg.sourceClientID + " found a " + msg.phenomenaName + "!");
		} else {
			Debug.Log("Hurray client " + msg.sourceClientID + " found a " + msg.phenomenaName + "!");
		}

	}

	//------------------------SERVER SIDE-----------------------------------


	// Forward to host
	void ServerHandleRequest(NetworkMessage netMsg) {

		Debug.Log ("Server received request");
		EventRequestMessage msg = netMsg.ReadMessage<EventRequestMessage> ();
		msg.sourceClientID = netMsg.conn.connectionId;

		// Send this to host
		NetworkServer.SendToClient(0, MyMessageTypes.EventRequestID, msg);
		Debug.Log ("Server sent request to client id: 0");
		Debug.Log ("Number of server connections: " + NetworkServer.connections.Count);
	}

	// Forward to client
	void ServerHandleData(NetworkMessage netMsg) {

		Debug.Log ("Server received data");
		EventDataMessage msg = netMsg.ReadMessage<EventDataMessage> ();

		NetworkServer.SendToClient (msg.destinationClientID, MyMessageTypes.EventDataID, msg);

	}

	void ServerHandleResult(NetworkMessage netMsg) {

		Debug.Log ("Server received result");
		EventResultMessage msg = netMsg.ReadMessage<EventResultMessage> ();
		msg.sourceClientID = netMsg.conn.connectionId;

		NetworkServer.SendToAll (MyMessageTypes.EventResultID, msg);
	}

	public void PrintConnections() {
		for (int i = 0; i < NetworkServer.connections.Count; i++) {
			Debug.Log(NetworkServer.connections[i].ToString());
		}
	}
}
