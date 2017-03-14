using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class SetupLocalPlayer : NetworkBehaviour {

	[SyncVar]
	public string pname = "player";

	void OnGUI() {
		if (isLocalPlayer) {
			pname = GUI.TextField (new Rect (25, Screen.height - 40, 100, 30), pname);
			if (GUI.Button (new Rect (130, Screen.height - 40, 80, 30), "Change")) {
				changeName ();
			}
		}
			
		
	}

	[Command]
	public void CmdChangeName(string newName)
	{
		pname = newName;
	}

	void Start () {
		if (isLocalPlayer) {
			GetComponent<NetworkMovement>().enabled = true;
		}
	}

	void Update() {
		if (isLocalPlayer)
			this.GetComponentInChildren<TextMesh>().text = pname;
	}

	[Client]
	void changeName()
	{
		CmdChangeName (pname);
	}

}
