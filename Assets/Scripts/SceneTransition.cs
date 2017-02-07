using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;

public class SceneTransition : NetworkBehaviour {

	public Texture2D fadeOutTexture; // the texture that will overlay the screen (black screen)
	public float fadeSpeed = 0.8f; // fade speed
	private NetworkManager netMan;
	private int drawDepth = -1000; // the texture's order in the draw hierarchy; low number means it renders on top
	private float alpha = 1.0f; // texture starts completely visable;
	private int fadeDir = -1; // the direction to fade: om = -1 or out = 1

	// Draws the fade texture to screen, OnGUI is called every frame (or even multiple times per frame)
	void OnGUI() {
		// fade in/out the alpha value using a direction, a speed, and a deltatime to convert to seconds
		alpha += fadeDir * fadeSpeed * Time.deltaTime;
		// clamp the number between 0 and 1 because GUI.color uses alpha values between 0 and 1
		alpha = Mathf.Clamp01(alpha);

		// set color of our GUI (in this case our texture). All color values remain the same and the Alpha is set to the alpha variable

		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha); // setting the alpha
		GUI.depth = drawDepth; // ensure the black texture renders on top

		GUI.DrawTexture (new Rect (0,0, Screen.width, Screen.height), fadeOutTexture); // draw the texture
	}

	void Awake() {
		netMan = GameObject.Find ("LobbyManager").GetComponent<NetworkLobbyManager> ();
		if (netMan == null) {
			Debug.LogError("Couldn't find LobbyManager in the scene - did you start the game in Lobby Scene?");
		}
	}
	//Used to create the fade
	public float BeginFade(int direction) {
		fadeDir = direction;
		return fadeSpeed;
	}
		
	// OBSOLETE - Need to use SceneManager.SceneLoaded event
	/*void OnLevelWasLoaded() {
		BeginFade (-1);
	}*/

	// Public wropper to call function from button OnClick event
	public void onButtonPress(string toLoad) {
		BeginFade (1);
		StartCoroutine(changeScene(toLoad));
	}

	// Delay for the fade to complete before transitioning
	IEnumerator changeScene(string toLoad) {
		yield return new WaitForSeconds (fadeSpeed);
		netMan.ServerChangeScene(toLoad);
	}

	// After scene loads, wait for 1 second and then remove black overlay
	IEnumerator openScene() {
		yield return new WaitForSeconds (1.0f);
		BeginFade (-1);
	}
}
