using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]

//Base Script Aquired from Codetastic  http://answers.unity3d.com/questions/392606/line-drawing-how-can-i-interpolate-between-points.html
//ModifedforVR Purposes

public class LineArcSystem : MonoBehaviour
{
    public LineRenderer myLine;
    Vector3[] thePoints;
    public Vector2 curveOffset;
    [Range(0, 50)]
    public float smoothness;
    [Range(0, 1)]
    public float AppogeePoint;
    [Range(0, 1)]
    public float Alpha;
    [Header("-Test Mode-")]
    public bool TestMode;
    public Transform LineStart;
    public Transform LineEnd;
    public Color TestColor;
    // Use this for initialization
    void Start()
    {
        thePoints = new Vector3[3];
        myLine = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if(TestMode)
        {
            CreateLine(LineStart.position, LineEnd.position, TestColor);
        }

    }


    public void CreateLine(Vector3 start, Vector3 end,Color yourColor)
    {
        myLine.enabled = true;
        yourColor.a = Alpha;
        transform.position = start;
        thePoints[0] = transform.position;
        thePoints[1] = LerpByDistance(start, end, AppogeePoint);
        thePoints[1].y += curveOffset.y;
        thePoints[2] = end;
        Vector3[] LineHold = MakeSmoothCurve(thePoints, smoothness);
        myLine.SetVertexCount(LineHold.Length);
        myLine.GetComponent<Renderer>().material.color = yourColor;
        myLine.SetPositions(LineHold);
    }

    public void HideLine()
    {
        myLine.enabled = false;
    }


    public Vector3 LerpByDistance(Vector3 start, Vector3 end, float percent)
    {
        return (start + percent * (end - start));
    }

    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Length;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }
}
