
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMirroring : MonoBehaviour {

	// Use this for initialization
	void Start () {
        UnityEngine.VR.VRSettings.showDeviceView = false;
	}
	
	// Update is called once per frame
	void Update () {
        //UnityEngine.VR.VRSettings.showDeviceView = false;
	}
}
