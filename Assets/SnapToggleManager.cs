using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the Toggle UI used for puzzle camera snapping
/// It ensures that only one toggle can be true at a time (like a radio button)
/// </summary>
public class SnapToggleManager : MonoBehaviour {

	// References to the toggles on the UI interface
	// Toggle for the top snap position
	public Toggle topToggle;
	// Toggle for the side snap position
	public Toggle sideToggle;
	// Toggle for the front snap position
	public Toggle frontToggle;

	// The toggle that is currently set to true, is set to one of the above three
	private Toggle trueToggle;

	// Just put the toggles in an array for ease of access
	private Toggle[] toggleArray = new Toggle[3];

	// Reference to the controller, so we can update its snap position
	public PuzzleCameraController puzzleCamController;

	/// <summary>
	/// Start - Set toggles to default values and set other variables
	/// </summary>
	void Start () {
		topToggle.isOn = false;
		sideToggle.isOn = false;
		frontToggle.isOn = true;
		trueToggle = frontToggle;

		toggleArray [0] = topToggle;
		toggleArray [1] = sideToggle;
		toggleArray [2] = frontToggle;
	}

	/// <summary>
	/// Update - used to check if the toggles have been changed, and update other toggle values
	/// I don't use toggle events here, since it creates infinite loops when we assign Toggle values
	/// </summary>
	void Update() {

		// if the true toggle is 'unchecked' we immediately check it again
		// One toggle has to be on at all times!
		if (!trueToggle.isOn) {
			trueToggle.isOn = true;
		}

		// Iterate through all toggles, and if we find a toggle that is on AND is not the trueToggle
		// from the last frame, then a player must have checked it, so we treat it as our new trueToggle
		foreach (Toggle curr in toggleArray) {
			if (curr.isOn && !curr.Equals(trueToggle)) {
				UpdateToggles (curr);
				break;
			}
		}
	}

	/// <summary>
	/// Update the toggles by setting all toggles that aren't newTrueToggle to false
	/// </summary>
	/// <param name="newTrueToggle">The toggle that was just set to true</param>
	void UpdateToggles(Toggle newTrueToggle) {

		// Turn all toggles that aren't newTrueToggle to false
		foreach (Toggle curr in toggleArray) {
			if (!curr.Equals(newTrueToggle)) {
				curr.isOn = false;
			}
		}

		// update which toggle is the trueToggle
		trueToggle = newTrueToggle;
	}

	/// <summary>
	/// Returns which SnapPosition the puzzle camera should use based on 
	/// </summary>
	/// <returns>The snap toggle setting to use</returns>
	public PuzzleCameraController.SnapPosition GetSnapToggleSetting() {

		// Just return the enum corresponding to which toggle trueToggle is
		if (trueToggle.Equals(topToggle)) {
			return PuzzleCameraController.SnapPosition.Top;
		} else if (trueToggle.Equals(sideToggle)) {
			return PuzzleCameraController.SnapPosition.Side;
		} else {
			return PuzzleCameraController.SnapPosition.Front;
		} 
	}

}

