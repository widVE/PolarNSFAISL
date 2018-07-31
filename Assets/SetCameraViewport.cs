using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCameraViewport : MonoBehaviour {

	public GameObject panelRect;

	// Use this for initialization
	void Start () {
		if(panelRect != null)
		{
			Camera c = GetComponent<Camera>();
			if(c != null)
			{
				RectTransform t = panelRect.GetComponent<RectTransform>();
				Rect rct = c.rect;
				Vector3[] worldCorners = new Vector3[4];
				t.GetWorldCorners(worldCorners);
				//Rect rct = RectTransformUtility.PixelAdjustRect(t, panelRect.GetComponent<Canvas>());
				rct.x = worldCorners[0].x / Screen.width;
				rct.width = (worldCorners[3].x - worldCorners[0].x) / Screen.width;
				rct.height = (worldCorners[1].y - worldCorners[0].y) / Screen.height;
				rct.y = worldCorners[0].y / Screen.height;
				c.rect = rct;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
