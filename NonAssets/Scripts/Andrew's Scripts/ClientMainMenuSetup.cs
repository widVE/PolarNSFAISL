using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ClientMainMenuSetup : NetworkBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (SceneManager.GetActiveScene().buildIndex == 1) {
			foreach (GameObject curr in GameObject.FindGameObjectsWithTag("Player")) {
				if (curr.GetComponent<NetworkIdentity>().isLocalPlayer && !curr.GetComponent<NetworkIdentity>().isServer) {
					this.gameObject.SetActive (false);
				}
			}
		}
	}
}
