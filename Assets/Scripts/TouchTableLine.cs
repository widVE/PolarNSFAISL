using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTableLine : MonoBehaviour {

    private LineRenderer ren;
    public float lineTimer = 3.0f;
    public bool lineDrawn = false;
    public bool lineFading = false;
    //public Vector3[] startEnd = new Vector3[2];

	// Use this for initialization
	void Start () {
		ren = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        // If the line is drawn on screen and is solid (not fading)
        if (lineDrawn)
        {
            // Decrement the timer and check if it has been on screen long enough
            lineTimer -= Time.deltaTime;

            // Begin fading if the timer is done
            if (lineTimer <= 0f)
            {
                lineTimer = 1.5f;
                lineDrawn = false;
                lineFading = true;
            }
            // Else if the line is currently fading, continue fading until the line is invisible
        }
        else if (lineFading)
        {

            // Update the gradient by decrementing its alpha keys
            Gradient currGradient = ren.colorGradient;
            GradientAlphaKey[] currAlphas = currGradient.alphaKeys;
            float newAlpha = currAlphas[0].alpha;
            newAlpha -= 0.5f * Time.deltaTime;

            // If alpha hits zero, then the line is invisible so we are done fading
            if (newAlpha < 0)
            {
                newAlpha = 0;
                Debug.Log("Done fading");
                lineFading = false;
            }

            // Update all alpha keys for the gradient
            for (int i = 0; i < currAlphas.Length; i++)
            {
                currAlphas[i].alpha = newAlpha;
            }

            // Update the line renderer and draw the new line
            currGradient.SetKeys(currGradient.colorKeys, currAlphas);
            ren.colorGradient = currGradient;
            //ren.SetPositions(startEnd);
        }	
	}
}
