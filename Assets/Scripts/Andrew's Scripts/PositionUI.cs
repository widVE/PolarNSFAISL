using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionUI : MonoBehaviour {

	[SerializeField]
	private float horizontalAnchorOffset;

	[SerializeField]
	private float verticalAnchorOffset;

	// Use this for initialization
	void Start () {

		this.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (Screen.width * horizontalAnchorOffset, Screen.height * verticalAnchorOffset);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
