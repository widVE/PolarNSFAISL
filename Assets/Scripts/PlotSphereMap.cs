using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSphereMap : MonoBehaviour {

    public GameObject plotPoint;
    private Vector2 quadSize;
    private List<GameObject> plottedPoints = new List<GameObject>();
	// Use this for initialization
	void Start ()
    {
        RectTransform objectRectTransform = gameObject.GetComponent<RectTransform>();
        // Get size of sphere map in pixels
        quadSize = new Vector2(
            objectRectTransform.rect.width,
            objectRectTransform.rect.height);
    }

    Vector2 CalculateMollweide(float lambda, float phi, float cx = 1f, float cy = 1f, float cp = Mathf.PI)
    {
        float nextPhi = MollweideBromleyTheta(cp, phi);
        return new Vector2(cx + cx * lambda * Mathf.Cos(nextPhi), cy * Mathf.Sin(phi) - cy);
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

    /**
     * Adds point to the sphere map.
     * latitude and longitude range should be [-1, 1]
     */
    public void PlotPoint(float lat, float longi, Color c)
    {
        RectTransform objectRectTransform = gameObject.GetComponent<RectTransform>();
        // Get size of sphere map in pixels
        quadSize = new Vector2(
            objectRectTransform.rect.width,
            objectRectTransform.rect.height);
        // Project latitude and longitude into sphere map
        Vector2 pXY = CalculateMollweide(lat, longi, quadSize.x / 2f, quadSize.y / 2f, Mathf.PI);
        GameObject particlePoint = Instantiate(plotPoint);
        particlePoint.GetComponent<UnityEngine.UI.Image>().color = c;
        particlePoint.transform.SetParent(transform, true);
        particlePoint.transform.localPosition = new Vector3(pXY.x, pXY.y, 0f);
        plottedPoints.Add(particlePoint);
    }

    public void ClearPoints()
    {
        foreach(GameObject p in plottedPoints)
        {
            DestroyObject(p);
        }

        plottedPoints.Clear();
    }

	// Update is called once per frame
	void Update () {
		
	}
}
