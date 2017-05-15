using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class EventDataSystem : MonoBehaviour {

	NetworkClient myClient;
	[SerializeField]
	private int port;

	public class MyMessageTypes {
		public static short EventDataID = MsgType.Highest + 1;
		public static short EventRequestID = (short)(EventDataID + 1);
	};

	/// <summary>
	/// Message to be sent between the server and the clients, holding event data
	/// </summary>
	public class EventDataMessage : MessageBase{
		public Vector2 coordinates;
		public float energy;
		public string source;
	};


	/// <summary>
	/// Message used to request event data from the table top
	/// </summary>
	public class EventRequestMessage : MessageBase{
		public string name;
	};


	public void SendData(Vector2 coordinates, float energy, string source, int clientID) {
		EventDataMessage msg = new EventDataMessage ();
		msg.source = source;
		msg.energy = energy;
		msg.coordinates = coordinates;

		NetworkServer.SendToClient (clientID, MyMessageTypes.EventDataID, msg);
	}

	public void SetupClient() {
		myClient = new NetworkClient ();
		myClient.RegisterHandler (MsgType.Connect, OnConnected);
		myClient.RegisterHandler (MyMessageTypes.EventDataID, OnDataReceived);
		myClient.RegisterHandler (MyMessageTypes.EventRequestID, OnRequestReceived);
		myClient.Connect ("127.0.0.1", port);
	}

	public void OnConnected(NetworkMessage netMsg) {
		Debug.Log("Connected to server");
	}

	public void OnDataReceived(NetworkMessage netMsg) {
		EventDataMessage msg = netMsg.ReadMessage<EventDataMessage> ();

		// Do stuff with the data now
		Debug.Log("Data Message received: " + msg.source);
	}

	public void OnRequestReceived(NetworkMessage netMsg) {
		EventRequestMessage msg = netMsg.ReadMessage<EventRequestMessage> ();
		// Do stuff with this request, create a new EventDataMessage, and send it back to
		// whomever sent the request
	}
}
