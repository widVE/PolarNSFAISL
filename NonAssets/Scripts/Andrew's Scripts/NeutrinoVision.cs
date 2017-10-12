using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class NeutrinoVision : MonoBehaviour {

	public LayerMask neutrinoMask;
	private int oldMask;

	private GameObject OculusObj;
	private GameObject tableObj;

	private Camera playerCam;
	private Camera tableCam;
	bool isOn = false;

	private VignetteAndChromaticAberration oculusEffect;
	private VignetteAndChromaticAberration tableEffect;

	// Use this for initialization
	void Start () {
		OculusObj = this.transform.Find ("OculusCamera").gameObject;
		playerCam = OculusObj.GetComponent<Camera> ();

		tableObj = this.transform.Find ("TableRenderCamera").gameObject;
		tableCam = tableObj.GetComponent<Camera> ();

		oldMask = playerCam.cullingMask;
		oculusEffect = OculusObj.GetComponent<VignetteAndChromaticAberration> ();
		tableEffect = tableObj.GetComponent<VignetteAndChromaticAberration> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown("Neutrino_Vision")) {
			if (isOn) {
				isOn = false;
				playerCam.cullingMask = oldMask;
				tableCam.cullingMask = oldMask;
				oculusEffect.enabled = false;
				tableEffect.enabled = false;
			} else {
				isOn = true;
				playerCam.cullingMask = neutrinoMask.value;
				tableCam.cullingMask = neutrinoMask.value;
				oculusEffect.enabled = true;
				tableEffect.enabled = true;
			}
		} 
		
	}
}
