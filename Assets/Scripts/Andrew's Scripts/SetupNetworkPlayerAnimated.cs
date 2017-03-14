using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SetupNetworkPlayerAnimated : NetworkBehaviour {

	private Transform spawnPointHost;
	private Transform spawnPointClient;
	private int currSceneId;

	void Awake() {
		// Maintain the same prefab in game's existence, don't destroy/respawn
		DontDestroyOnLoad (this.gameObject);
		currSceneId = 0;
		// SceneManager.sceneLoaded += OnSceneChange;
	}

	//	void OnEnable() {
	//		SceneManager.sceneLoaded += OnSceneChange;
	//	}
	//
	//	void OnDisable() {
	//		SceneManager.sceneLoaded -= OnSceneChange;
	//	}

	// SceneManager.sceneLoaded is bugged and is really unreliable, best to use this deprecated function until Unity removes it
	// ISSUE - Apparently this is only being called on the local player object on the host, not on the other players within the 
	// 		   host's game, nor in any objects in the client's game
	// POSSIBLE SOLUTION - OnLevelWasLoaded is called too early, before other players enter the scene - replace or set timeout or something
	void levelSetup() {

		// Load into the main menu scene, disable the player prefab to just show scene camera
		if (SceneManager.GetActiveScene().buildIndex == 1) {

			// Disabling components to prevent player control, and compile errors
			//GetComponent<MeshRenderer>().enabled = false;
			GetComponentInChildren<Camera>().enabled = false;
			GetComponentInChildren<AudioListener>().enabled = false;
			GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl> ().enabled = false;
			GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter> ().enabled = false;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

			// Enable the Scene camera for menu or top down view
			GameObject sceneCamera = GameObject.FindGameObjectWithTag("SceneCamera");
			//Camera main = sceneCamera.GetComponent<Camera> ();
			if (sceneCamera != null) {
				sceneCamera.SetActive (true);
			}

			// Unnecessary return (?)
			return;
		} else {
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;
			// Move the player to the desired start locations based on the scene SpawnPoint
			spawnPointHost = GameObject.Find("SpawnPointHost").transform;
			spawnPointClient = GameObject.Find ("SpawnPointClient").transform;
			if (spawnPointHost == null || spawnPointClient == null) {
				Debug.LogError ("Couldn't find spawnPoints in scene");
			}
			if (!this.gameObject.GetComponent<NetworkIdentity>().isServer) {
				Debug.Log ("Client made it to the setup");
			}

			if (this.gameObject.GetComponent<NetworkIdentity>().isServer) {
				this.gameObject.transform.position = spawnPointHost.position;
				this.gameObject.transform.rotation = spawnPointHost.rotation;
			} else {
				this.gameObject.transform.position = spawnPointClient.position;
				this.gameObject.transform.rotation = spawnPointClient.rotation;
			}


			// In a playable scene, so enable the player object - possible set redundantly if values are already true
			if (isLocalPlayer) {
				GetComponentInChildren<Camera>().enabled = true;
				GetComponentInChildren<AudioListener> ().enabled = true;
				GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl> ().enabled = true;
				GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter> ().enabled = true;
			}

			// Enable the MeshRenderer on all player objects, not just the one with local authority
			//GetComponent<MeshRenderer>().enabled = true;

			// Find and disable the scene camera
			GameObject sceneCamera = GameObject.FindGameObjectWithTag("SceneCamera");
			//Camera main = sceneCamera.GetComponent<Camera> ();
			if (sceneCamera != null) {
				sceneCamera.SetActive (false);
			}
		}
	}
	// Only called once - when the Main Menu Scene is first loaded
	void Start () {

		// Handle these in any scene, on objects that aren't local player controlled
		if (!isLocalPlayer) {
			GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonUserControl> ().enabled = false;
			GetComponent<UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter> ().enabled = false;
			GetComponentInChildren<Camera> ().enabled = false;
			GetComponentInChildren<AudioListener> ().enabled = false;
		}

		// Reduncancy
		// GetComponent<MeshRenderer> ().enabled = false;
	}

	void Update () {

		if (sceneChanged()) {
			// Do all the stuff needed if we changed the scene we were in
			levelSetup();

			// Stupid falling glitch fix
			if (this.transform.position.y < 20) {
				this.transform.position = new Vector3 (this.transform.position.x, 20f, this.transform.position.z);
			}
		}
	}

	bool sceneChanged() {
		if (SceneManager.GetActiveScene().buildIndex != currSceneId) {
			currSceneId = SceneManager.GetActiveScene ().buildIndex;
			return true;
		}
		else {
			return false;
		}
	}
}
