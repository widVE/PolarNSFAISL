using UnityEngine;
using System.Collections;

public class examplescene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RenderSettings.skybox = (Material)Resources.Load("Skybox3");
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		int x = 50;
		int y = 50;
		int dy = 40;
		int cnt = 0;
		int sx = 300;
		int sy = 30;
		if (GUI.Button(new Rect(x, y+dy*cnt++, sx, sy), "Skybox 1 - hubble deep field")) {
			RenderSettings.skybox = (Material)Resources.Load("Skybox1");
		}
		if (GUI.Button(new Rect(x, y+dy*cnt++, sx, sy), "Skybox 2 - Semi-close galaxy")) {
			RenderSettings.skybox = (Material)Resources.Load("Skybox2");
		}
		if (GUI.Button(new Rect(x, y+dy*cnt++, sx, sy), "Skybox 3 - Large blueish galaxy")) {
			RenderSettings.skybox = (Material)Resources.Load("Skybox3");
		}
		if (GUI.Button(new Rect(x, y+dy*cnt++, sx, sy), "Skybox 4 - center large blue galaxy")) {
			RenderSettings.skybox = (Material)Resources.Load("Skybox4");
		}
		if (GUI.Button(new Rect(x, y+dy*cnt++, sx, sy), "Skybox 5 - Neutral galaxies")) {
			RenderSettings.skybox = (Material)Resources.Load("Skybox5");
		}
		if (GUI.Button(new Rect(x, y+dy*cnt++, sx, sy), "Skybox 6 - red galaxy")) {
			RenderSettings.skybox = (Material)Resources.Load("Skybox6");
		}
		
	}
}
