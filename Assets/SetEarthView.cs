using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEarthView : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Camera c = GetComponent<Camera>();
		if (c != null)
		{
			Rect rct = c.rect;
			rct.x = 0.01f;
			rct.width = (Screen.width / 7.0f) / Screen.width;
			rct.height = (Screen.width / 7.0f) / Screen.height;
			rct.y = 0.015f;
			c.rect = rct;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
