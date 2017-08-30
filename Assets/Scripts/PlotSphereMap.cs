using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSphereMap : MonoBehaviour {

    public GameObject plotPoint;
	// Use this for initialization
	void Start () {
		
	}

    Vector2 CalculateMollweide(float lambda, float phi, float cx = 1f, float cy = 1f, float cp = Mathf.PI)
    {
        float nextPhi = MollweideBromleyTheta(cp, phi);
        return new Vector2(cx * lambda * Mathf.Cos(nextPhi), cy * Mathf.Sin(phi));
    }

    float MollweideBromleyTheta(float cp, float phi)
    {
        float cpsinPhi = cp * Mathf.Sin(phi);
        int i = 30;
        float delta;
        float p = phi;
        do
        {
            delta = (phi + Mathf.Sin(phi) - cpsinPhi) / (1f + Mathf.Cos(phi));
            p -= delta;
        } while (Mathf.Abs(delta) > 0.001f && --i > 0);

        return p / 2;
    }

    public void PlotPoint(Vector2 latLong)
    {
        Vector2 pXY = CalculateMollweide(latLong.x, latLong.y, Mathf.Sqrt(2f) / (Mathf.PI * 0.5f), Mathf.Sqrt(2f), Mathf.PI);
        Debug.Log("Mollweide: " + pXY);
        GameObject particlePoint = Instantiate(plotPoint);
        particlePoint.transform.SetParent(transform, true);
        //particlePoint.transform.position += transform.position;
        particlePoint.transform.localPosition = new Vector3(pXY.x * 395f, pXY.y * 200f, 0f);
    }

	// Update is called once per frame
	void Update () {
		
	}
}
