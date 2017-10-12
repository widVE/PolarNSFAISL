using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignDom : MonoBehaviour {

	private float xRatio;
	private float yRatio;
	private float zRatio;

	void Start() {
		xRatio = this.transform.localPosition.x;
		yRatio = this.transform.localPosition.y;
		zRatio = this.transform.localPosition.z;
	}
		
	public void setPosition(float xAlign, float yAlign, float zAlign, Vector3 refPos) {
		Vector3 nextPos = new Vector3 ();
		nextPos.x = ((xAlign - refPos.x) * xRatio) + refPos.x;
		nextPos.y = ((yAlign - refPos.y) * yRatio) + refPos.y;
		nextPos.z = ((zAlign - refPos.z) * zRatio) + refPos.z;
		this.transform.localPosition = nextPos;
	}
}

/*
 * Backup of old method that works
 * [SerializeField]
	private Transform parentTransform;
	[SerializeField]
	private Transform lowerPoint;
	[SerializeField]
	private Transform upperX;
	[SerializeField]
	private Transform upperY;
	[SerializeField]
	private Transform upperZ;

	private float xRatio;
	private float yRatio;
	private float zRatio;

	void Start() {
		xRatio = this.transform.localPosition.x;
		yRatio = this.transform.localPosition.y;
		zRatio = this.transform.localPosition.z;
	}
		
	public void setPosition(float xAlign, float yAlig, float zAlign, Vector3 refPos) {
		Vector3 nextPos = new Vector3 ();
		nextPos.x = ((upperX.localPosition.x - lowerPoint.localPosition.x) * xRatio) + lowerPoint.localPosition.x;
		nextPos.y = ((upperY.localPosition.y - lowerPoint.localPosition.y) * yRatio) + lowerPoint.localPosition.y;
		nextPos.z = ((upperZ.localPosition.z - lowerPoint.localPosition.z) * zRatio) + lowerPoint.localPosition.z;

		this.transform.localPosition = nextPos;
	}
 * */
