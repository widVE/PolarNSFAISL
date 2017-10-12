using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawUILine : MonoBehaviour {

	[SerializeField]
	private Texture2D point;
	private Vector2 pos = Vector2.zero;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 loc = Input.mousePosition;

		pos = new Vector2 (loc.x, loc.y);

	}

	void OnGUI()
	{
		Vector2 start = new Vector3(500f, 500f);
		Vector2 end = pos;
		int width = 10;
		Vector2 d = end - start;
		float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
		if (d.x < 0)
			a += 180;

		int width2 = (int) Mathf.Ceil(width / 2);

		GUIUtility.RotateAroundPivot(a, start);
		GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), point);
		GUIUtility.RotateAroundPivot(-a, start);
	}
}
