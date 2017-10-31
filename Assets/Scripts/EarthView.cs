using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthView : MonoBehaviour
{

    public GameObject lineObject;
    public GameObject swipeGame;

    struct eventCone
    {
        public GameObject cone;
        public GameObject point;

        public eventCone(GameObject cone, GameObject point)
        {
            this.cone = cone;
            this.point = point;
        }
    }

    private List<eventCone> cones;

    //private List<GameObject> detectedEvents = new List<GameObject>();
    //private List<Vector3> lineData = new List<Vector3>();

    // Use this for initialization
    void Start()
    {
        cones = new List<eventCone>();
    }

    // Update is called once per frame
    void Update()
    {

        foreach (eventCone cone in cones)
        {
            cone.cone.transform.LookAt(cone.point.transform);
        }

        if (swipeGame != null)
        {
            if (!swipeGame.GetComponent<SwipeGameMode>().isGamePlaying())
            {
                if (cones.Count > 0)
                {
                    cones.Clear();
                }
            }
        }
        /*Transform trans = this.transform.Find("EarthModel").transform.Find("IceCubeLocation");
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
        }*/
    }

    public void AddDetectedEvent(Vector3 start, Vector3 end, Color color, float accuracy, float goal)
    {
        GameObject g = Instantiate(lineObject, transform.Find("IceCubeLocation"));
        g.layer = 11;
        Quaternion q = g.transform.rotation;
        q.SetLookRotation((end - start).normalized);
        g.transform.rotation = q;
        g.GetComponent<MeshRenderer>().material.color = color;

        //scale x and z based on accuracy
        //g.transform.localScale = new Vector3(5 + (1 - accuracy) * 50.0f, g.transform.localScale.y, 5 + (1 - accuracy) * 50.0f); //not based on goal
        g.transform.localScale = new Vector3(1 + (1 - ((accuracy - goal) / goal)) * 15.0f, g.transform.localScale.y, 1 + (1 - ((accuracy - goal) / goal)) * 15.0f); //based on goal
        //Vector3 endPoint = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), Random.Range(-200, 200));
        //Vector3 testPoint = (start - end) / 10;
        GameObject lookAtPoint = new GameObject();
        lookAtPoint.transform.position = end;
        cones.Add(new eventCone(g, lookAtPoint));
        /*LineRenderer r = g.GetComponent<LineRenderer>();
        //r.gameObject.AddComponent(r);
        r.startWidth = 3.0f;
        r.endWidth = 3.0f;
        r.startColor = color;
        r.endColor = color;
        
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
        detectedEvents.Add(g);*/
    }

    /*private Vector3[] extendLine(Vector3[] endPoints)
    {

        Vector3 direction = endPoints[1] - endPoints[0];
        endPoints[0] += (direction * 10);
        endPoints[1] -= (direction * 10);
        return endPoints;
    }*/
}