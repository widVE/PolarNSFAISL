using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEnergyGrapher : MonoBehaviour {

	private LineRenderer linRen;
	private float zDistance;
	private Vector3[] points;

    //this should be max # of DOMs
    public const int NUM_LINE_POINTS = 1000;

	[SerializeField]
	private bool randomizeData = false;

	private EventPlayer visEvent;
	// Use this for initialization
	void Start () {

		zDistance = Camera.main.nearClipPlane + 10;
       
		//InitializePoints ();

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
        if (points != null)
        {
            linRen.SetPositions(points);
        }
	}

    public void Reset()
    {

    }

	private void InitializePoints() {
        //this needs to be called every time a new DOM is played...
        points = new Vector3[(int)visEvent.GetTotalDOMs()];
        for (int i = 0; i < visEvent.GetTotalDOMs(); i++)
        {
            float x = (i * 5f);
            points[i] = this.transform.TransformPoint(new Vector3(x, 0f, zDistance));
            points[i].y = transform.position.y;
            //points[i].z = zDistance;
        }
	}

	private void UpdatePoints() {
        //Debug.Log(visEvent.GetTotalDOMs());
        if((points == null || points.Length == 0) && visEvent.GetTotalDOMs() > 0)
        {
            InitializePoints();
        }

        //for (int i = 0; i < visEvent.GetTotalDOMs(); i++)
        if (visEvent.GetTotalDOMs() > 0)
        {
            float x = (visEvent.GetCurrentDOM() * 5f);
            points[visEvent.GetCurrentDOM()] = this.transform.TransformPoint(new Vector3(x, 0f, zDistance));
            /*if (i < points.Length - 1)
            {
                points[i].y = points[i + 1].y;
            }*/
            float energy = 0f;
            if (visEvent.GetCurrentEnergy() > 100f)
            {
                energy = 100f;
            }
            else
            {
                energy = visEvent.GetCurrentEnergy();
            }

            points[visEvent.GetCurrentDOM()].y = transform.position.y + energy;
		}

		// Either randomize or use VisualizeEvent totalEnergy
		/*if (randomizeData) {
			points[points.Length - 1].y = Random.value;
		} else {

			float newValue = visEvent.totalEnergy;
			if (newValue > 100f) {
				newValue = 100f;
			}
            //Debug.Log(newValue);
            //points[points.Length - 3].y = transform.position.y + newValue * 0.5f;
            //points[points.Length - 2].y = transform.position.y + newValue;
            points[points.Length - 1].y = transform.position.y + newValue;
		}*/
	}
}
