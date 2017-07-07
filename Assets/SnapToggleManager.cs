using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SnapToggleManager : MonoBehaviour {

	public Toggle topToggle;
	public Toggle sideToggle;
	public Toggle frontToggle;
	public Toggle customToggle;

	private Toggle trueToggle;
	private Toggle[] toggleArray = new Toggle[4];
	public PuzzleCameraController puzzleCamController;

	// Use this for initialization
	void Start () {
		topToggle.isOn = false;
		//topToggle.onValueChanged.AddListener ((value) => {UpdateTogglesTop(value);});
		sideToggle.isOn = false;
		//sideToggle.onValueChanged.AddListener ((value) => {UpdateTogglesSide(value);});
		frontToggle.isOn = false;
		//frontToggle.onValueChanged.AddListener ((value) => {UpdateTogglesFront(value);});
		customToggle.isOn = true;
		trueToggle = customToggle;
		//customToggle.onValueChanged.AddListener ((value) => {UpdateTogglesCustom(value);});

		toggleArray [0] = topToggle;
		toggleArray [1] = sideToggle;
		toggleArray [2] = frontToggle;
		toggleArray [3] = customToggle;
	}

	void Update() {

		if (!trueToggle.isOn) {
			trueToggle.isOn = true;
		}

		foreach (Toggle curr in toggleArray) {
			if (curr.isOn && !curr.Equals(trueToggle)) {
				UpdateToggles (curr);
				break;
			}
		}
	}

	void UpdateToggles(Toggle newTrueToggle) {
		
		foreach (Toggle curr in toggleArray) {
			if (!curr.Equals(newTrueToggle)) {
				curr.isOn = false;
			}
		}


		trueToggle = newTrueToggle;
	}

//	public void UpdateTogglesTop(bool value) {
//		if (value == false) {
//			topToggle.isOn = true;
//		} else {
//			sideToggle.isOn = false;
//			frontToggle.isOn = false;
//			customToggle.isOn = false;
//			puzzleCamController.SnapCamera (PuzzleCameraController.SnapPosition.Top);
//		}
//	}
//
//	public void UpdateTogglesSide(bool value) {
//		if (value == false) {
//			sideToggle.isOn = true;
//		} else {
//			topToggle.isOn = false;
//			frontToggle.isOn = false;
//			customToggle.isOn = false;
//			puzzleCamController.SnapCamera (PuzzleCameraController.SnapPosition.Side);
//		}
//	}
//
//	public void UpdateTogglesFront(bool value) {
//		if (value == false) {
//			frontToggle.isOn = true;
//		} else {
//			sideToggle.isOn = false;
//			topToggle.isOn = false;
//			customToggle.isOn = false;
//			puzzleCamController.SnapCamera (PuzzleCameraController.SnapPosition.Front);
//		}
//	}
//
//	public void UpdateTogglesCustom(bool value) {
//		if (value == false) {
//			customToggle.isOn = true;
//		} else {
//			sideToggle.isOn = false;
//			topToggle.isOn = false;
//			frontToggle.isOn = false;
//			puzzleCamController.SnapCamera (PuzzleCameraController.SnapPosition.Custom);
//		}
//	}
}
