using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleLineAdjuster : MonoBehaviour {

	private Vector3 currentPathStart;
	private Vector3 currentPathEnd;

	[SerializeField]
	private GameObject startNode;
	[SerializeField]
	private GameObject endNode;

	public Camera puzzleCamera;

	private Vector3 puzzleOffset;

	[SerializeField]
	private LineRenderer lineRen;

	// Use this for initialization
	void Start () {
		puzzleOffset = new Vector3(3000f, 0, 0);
	}
	
	// Update is called once per frame
	void Update () {

		if (startNode.activeInHierarchy && endNode.activeInHierarchy) {
			Vector3[] lineRendPositions = new Vector3[2];
			lineRendPositions [0] = startNode.transform.position;
			lineRendPositions [1] = endNode.transform.position;

			lineRen.SetPositions (lineRendPositions);
		}

	}
		
	public void SetupLine(Vector3 pathStart, Vector3 pathEnd, Vector3 swipeStart, Vector3 swipeEnd) {

		// Remember to use the puzzle array offset of 3000f, so the nodes get placed in the second array
		currentPathStart = pathStart + puzzleOffset;
		currentPathEnd = pathEnd + puzzleOffset;

		// Move the start and end nodes to the swipe positions, estimated in world coordinates
		Vector3 pathCenterPos = (currentPathEnd + currentPathStart) / 2f;

		// Also need to apply offset to the swipe endpoints, since they correspond to the world coordinates in the origninal array
		swipeStart += puzzleOffset;
		swipeEnd += puzzleOffset;

		// Now move the two nodes to these positions to begin the adjustment game
		startNode.transform.position = swipeStart;
		endNode.transform.position = swipeEnd;

		EnableLine ();

	}

	public void DisableLine() {
		startNode.SetActive (false);
		endNode.SetActive (false);
		lineRen.SetPositions (new Vector3[2]);

	}

	private void EnableLine() {
		startNode.SetActive (true);
		endNode.SetActive (true);

		// Don't worry about setting positions, the Update function above will update the lineRenderer every frame
	}
}
