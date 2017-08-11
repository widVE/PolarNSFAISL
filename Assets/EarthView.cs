using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthView : MonoBehaviour {

    public GameObject lineObject;

    private List<GameObject> detectedEvents = new List<GameObject>();
    private List<Vector3> lineData = new List<Vector3>();

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

        Transform trans = this.transform.Find("EarthModel").transform.Find("IceCubeLocation");

        for(int i = 0; i < detectedEvents.Count; ++i)
        {
            LineRenderer r = detectedEvents[i].GetComponent<LineRenderer>();
            Vector3[] v = new Vector3[2];
            v[0] = lineData[i * 2];
            v[1] = lineData[i * 2 + 1];
            v[0] = trans.TransformPoint(v[0]);
            v[1] = trans.TransformPoint(v[1]);

            v = extendLine(v);
            r.SetPosition(0, v[0]);
            r.SetPosition(1, v[1]);
        }
	}

    public void AddDetectedEvent(Vector3 start, Vector3 end)
    {
        GameObject g = Instantiate(lineObject, transform);

        LineRenderer r = g.GetComponent<LineRenderer>();
        //r.gameObject.AddComponent(r);
        r.startWidth = 3.0f;
        r.endWidth = 3.0f;
        
        Vector3[] v = new Vector3[2];
        v[0] = start;
        v[1] = end;

        lineData.Add(start);
        lineData.Add(end);

        Transform trans = this.transform.Find("EarthModel").transform.Find("IceCubeLocation");

        v[0] = trans.TransformPoint(v[0]);
        v[1] = trans.TransformPoint(v[1]);

        v = extendLine(v);
        r.SetPosition(0, v[0]);
        r.SetPosition(1, v[1]);

        detectedEvents.Add(g);
    }

	private Vector3[] extendLine(Vector3[] endPoints) {

		Vector3 direction = endPoints [1] - endPoints [0];
		endPoints [0] += (direction * 10);
		endPoints [1] -= (direction * 10);
		return endPoints;
	}
}
