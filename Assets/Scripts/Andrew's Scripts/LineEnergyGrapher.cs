using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEnergyGrapher : MonoBehaviour {

	private LineRenderer linRen;
	private float zDistance;
	private Vector3[] points;

	[SerializeField]
	private bool randomizeData = false;

	private EventPlayer visEvent;
	// Use this for initialization
	void Start () {

		zDistance = Camera.main.nearClipPlane + 10;
       
		InitializePoints ();

		GameObject array = GameObject.Find ("DomArray");
		if (array != null) {
			visEvent = array.GetComponent<EventPlayer>();
		}
			
		linRen = GetComponent<LineRenderer> ();
		linRen.material = new Material (Shader.Find ("Particles/Additive"));
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePoints ();
		linRen.SetPositions (points);
	}

	private void InitializePoints() {
		points = new Vector3[100];
		for (int i = 0; i < 100; i++) {
			float x = (i * 5f);
			points [i] = this.transform.TransformPoint (new Vector3 (x, 0f, zDistance));
            points[i].y = transform.position.y;
            //points[i].z = zDistance;
		}
	}

	private void UpdatePoints() {
		//Debug.Log ("Update Points started");
		for (int i = 0; i < points.Length; i++) {
            float x = (i * 5f);
            points[i] = this.transform.TransformPoint(new Vector3(x, 0f, zDistance));
            if (i < points.Length - 1)
            {
                points[i].y = points[i + 1].y;
            }
		}
		// Either randomize or use VisualizeEvent totalEnergy
		if (randomizeData) {
			points[points.Length - 1].y = Random.value;
		} else {

			float newValue = visEvent.totalEnergy;
			//if (newValue > 1) {
			//	newValue = 1;
			//}

            //points[points.Length - 3].y = transform.position.y + newValue * 0.5f;
            //points[points.Length - 2].y = transform.position.y + newValue;
            points[points.Length - 1].y = transform.position.y + newValue;
		}
	}
}
