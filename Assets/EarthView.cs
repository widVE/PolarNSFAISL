using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthView : MonoBehaviour {

	//public PuzzleLineAdjuster lineAdjuster;

	//public PuzzleCameraController puzzleCamController;

	private Vector3 puzzleArrayOffset = new Vector3 (3000f, 0, 0);

	//public LineRenderer adjustRen;
	//public LineRenderer pathRen;

    private List<LineRenderer> detectedEvents;

	// Use this for initialization
	void Start () {
	
		// Ensure the line renderers aren't using world space, to prevent us having to do another transformation
		//adjustRen.useWorldSpace = true;
		//pathRen.useWorldSpace = true;
	}
	
	// Update is called once per frame
	void Update () {

		// If an event is being viewed, render the current adjustable line and the actual line
		/*if (puzzleCamController.isViewingEvent()) {

			// World positions of the endpoints, relative to world coordinates
			Vector3 currentPathStart = lineAdjuster.getCurrentPathStart();
			Vector3 currentPathEnd = lineAdjuster.getCurrentPathEnd();

			// Convert them to local (main array) coordinates so we can place them correctly on the miniature earth array
			currentPathStart = lineAdjuster.transform.InverseTransformPoint(currentPathStart);
			currentPathEnd = lineAdjuster.transform.InverseTransformPoint(currentPathEnd);

			// World positions of the adjustable line, remove the offset to place it in main array instead of puzzle array
			Vector3 adjustLineStart = lineAdjuster.getAdjustLineStart ();
			Vector3 adjustLineEnd = lineAdjuster.getAdjustLineEnd ();

			// Again convert them to local (main array) space
			adjustLineStart = lineAdjuster.transform.InverseTransformPoint(adjustLineStart);
			adjustLineEnd = lineAdjuster.transform.InverseTransformPoint (adjustLineEnd);


			Transform trans = this.transform.Find ("IceCubeLocation").transform;

			currentPathStart = trans.TransformPoint (currentPathStart);
			currentPathEnd = trans.TransformPoint (currentPathEnd);

			adjustLineStart = trans.TransformPoint (adjustLineStart);
			adjustLineEnd = trans.TransformPoint (adjustLineEnd);

			Vector3[] adjustPos = new Vector3[2];
			adjustPos [0] = adjustLineStart;
			adjustPos [1] = adjustLineEnd;

			Vector3[] pathPos = new Vector3[2];
			pathPos [0] = currentPathStart;
			pathPos [1] = currentPathEnd;

			adjustPos = extendLine (adjustPos);
			pathPos = extendLine (pathPos);

			adjustRen.SetPositions (adjustPos);
			pathRen.SetPositions (pathPos);
		} else {
			adjustRen.SetPositions (new Vector3[2]);
			pathRen.SetPositions (new Vector3[2]);
		}*/
	}

    public void AddDetectedEvent(Vector3 start, Vector3 end)
    {
        LineRenderer r = new LineRenderer();
        //r.gameObject.AddComponent(r);
        r.startWidth = 5.0f;
        r.endWidth = 5.0f;
        r.useWorldSpace = true;
        r.SetPosition(0, start);
        r.SetPosition(1, end);

        detectedEvents.Add(r);
    }

	private Vector3[] extendLine(Vector3[] endPoints) {

		Vector3 direction = endPoints [1] - endPoints [0];
		endPoints [0] += (direction * 10);
		endPoints [1] -= (direction * 10);
		return endPoints;
	}
}
