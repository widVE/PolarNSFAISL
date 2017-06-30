using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class SwipeRecognizer : MonoBehaviour {

	public GameObject PuzzleCamera;
	public FlickGesture swipeGesture;
	public bool showLine = true;
	public LineRenderer ren;
	private Gradient missedGradient;
	private Gradient foundGradient;
	private Gradient idleGradient;
	private enum SwipeType {missed, found, idle};
	private float lineTimer = 3.0f;
	private bool lineDrawn = false;
	private bool lineFading = false;
    public VisualizeEvent currentEvents;
    public GameObject collectionText = null;
	//private ParticleTrail trail;
    private Vector3[] startEnd = new Vector3[2];

	void Start () {
		if (swipeGesture == null) {
			Debug.LogError ("No Flick Gesture assigned for DetectSwipe component on " + this.gameObject.name);
		}
		if (PuzzleCamera == null) {
			Debug.LogError ("Puzzle camera reference not set in inspector");
		}

		BuildGradients ();
	}

	// Only used for line fading
	void Update () {

		// If the line is drawn on screen and is solid (not fading)
		if (lineDrawn) {

			// Decrement the timer and check if it has been on screen long enough
			lineTimer -= Time.deltaTime;

			// Begin fading if the timer is done
			if (lineTimer <= 0f) {
				lineTimer = 1.5f;
				lineDrawn = false;
				lineFading = true;
			}
		// Else if the line is currently fading, continue fading until the line is invisible
		} else if (lineFading) {

			// Update the gradient by decrementing its alpha keys
			Gradient currGradient = ren.colorGradient;
			GradientAlphaKey[] currAlphas = currGradient.alphaKeys;
			float newAlpha = currAlphas [0].alpha;
			newAlpha -= 0.5f * Time.deltaTime;

			// If alpha hits zero, then the line is invisible so we are done fading
			if (newAlpha < 0) {
				newAlpha = 0;
				Debug.Log ("Done fading");
				lineFading = false;
			} 

			// Update all alpha keys for the gradient
			for (int i = 0; i < currAlphas.Length; i++) {
				currAlphas [i].alpha = newAlpha;
			}

			// Update the line renderer and draw the new line
			currGradient.SetKeys (currGradient.colorKeys, currAlphas);
			ren.colorGradient = currGradient;
			ren.SetPositions (startEnd);
		}

	}

	/// <summary>
	/// Builds the gradients used by the line renderer for different swipe types
	/// </summary>
	void BuildGradients () {

		// Gradient for a swipe that detected an event successfully
		foundGradient = new Gradient();
		GradientColorKey[] gck = new GradientColorKey[2];
		GradientAlphaKey[] gak = new GradientAlphaKey[2];
		gck[0].color = Color.green;
		gck[0].time = 0.0f;
		gck[1].color = Color.green;
		gck[1].time = 1.0f;

		gak[0].alpha = 1.0f;
		gak[0].time = 0.0f;
		gak[1].alpha = 1.0f;
		gak[1].time = 1.0f;

		foundGradient.SetKeys(gck, gak);

		// Gradient for a swipe that missed an event that was playing
		missedGradient = new Gradient();
		gck = new GradientColorKey[2];
		gak = new GradientAlphaKey[2];
		gck[0].color = Color.red;
		gck[0].time = 0.0f;
		gck[1].color = Color.red;
		gck[1].time = 1.0f;

		gak[0].alpha = 1.0f;
		gak[0].time = 0.0f;
		gak[1].alpha = 1.0f;
		gak[1].time = 1.0f;

		missedGradient.SetKeys(gck, gak);

		// Gradient for a swipe that was made when no event was playing
		idleGradient = new Gradient();
		gck = new GradientColorKey[2];
		gak = new GradientAlphaKey[2];
		gck[0].color = Color.gray;
		gck[0].time = 0.0f;
		gck[1].color = Color.gray;
		gck[1].time = 1.0f;

		gak[0].alpha = 1.0f;
		gak[0].time = 0.0f;
		gak[1].alpha = 1.0f;
		gak[1].time = 1.0f;

		idleGradient.SetKeys(gck, gak);
	}

	private void OnEnable() {
		swipeGesture.Flicked += swipeHandler;
	}

	private void OnDisable() {
		swipeGesture.Flicked -= swipeHandler;
	}


	/// <summary>
	/// Function for drawing the player's swipe based on when/how the swipe was made
	/// </summary>
	/// <param name="type">The type of swipe</param>
	private void DrawSwipeLine(SwipeType type) {
		
		AudioSource[] aSources = GetComponents<AudioSource>();
		AudioSource toPlay = null;

		// Set the gradient appropriately
		switch (type) {
		case SwipeType.missed:
			ren.colorGradient = missedGradient;
			toPlay = aSources [0];
			break;
		case SwipeType.found:
			ren.colorGradient = foundGradient;
			toPlay = aSources [2];
			break;
		case SwipeType.idle:
			ren.colorGradient = idleGradient;
			toPlay = aSources [1];
			break;
		default:
			break;
		}

		// Reset the timer so the line will begin fading after 2 seconds
		lineTimer = 1.5f;
		lineDrawn = true;
		lineFading = false;
		// Draw the line
		ren.SetPositions(startEnd);
		Debug.Log ("Swipe was drawn");
		toPlay.Play ();
	}

	private bool InSwipeBounds(Vector2 start, Vector2 end) {

		// Check start
		if ((start.x > Screen.width/2) || (end.x > Screen.width/2)) {
			return false;
		}
		return true;
	}

	private void swipeHandler(object sender, System.EventArgs e) {
		Vector2 prev = swipeGesture.PreviousScreenPosition;
		Vector2 swipeVector = swipeGesture.ScreenFlickVector;
        Vector2 next = prev - swipeVector;
        //Debug.Log("Swipe Detected - Direction: " + swipeVector);
		//Debug.Log ("Start: " + start);
        
		if (!InSwipeBounds(next, prev)) {
			Debug.Log ("Swipe was out of bounds\nScreen x: " + Screen.width + "\tprevX" + prev.x + "\tnextX: " + next.x);
			return;
		} else {
			Debug.Log ("Swipe in bounds\nScreen x: " + Screen.width + "\tprevX" + prev.x + "\tnextX: " + next.x);
		}


		if (showLine) {
            startEnd[0] = Camera.main.ScreenToWorldPoint(new Vector3(prev.x, prev.y, Camera.main.nearClipPlane + 1));
            startEnd[1] = Camera.main.ScreenToWorldPoint(new Vector3(next.x, next.y, Camera.main.nearClipPlane + 1));
            //Debug.Log("Line Drawn: " + startEnd[0] + " to " + startEnd[1]);
            //ren.SetPositions(startEnd);
		} else {
			Debug.Log ("showLine was false");
		}


        //let's instead convert any active events to screen space and test there...
        if(currentEvents != null)
        {
            if(currentEvents.IsEventPlaying())
            {
                for (int ev = 0; ev < currentEvents.eventsPlaying.Length; ++ev)
                {
					if (currentEvents.eventsPlaying[ev].isPlaying)
                    {
                        Vector3 vStart = currentEvents.events[ev].startPos;
                        Vector3 vEnd = currentEvents.events[ev].endPos;
						//Debug.Log ("Start: " + vStart + " End: " + vEnd);

                        Vector3 screenStart = Camera.main.WorldToScreenPoint(vStart);
                        Vector3 screenEnd = Camera.main.WorldToScreenPoint(vEnd);

                        Vector2 screenMid;
                        screenMid.x = screenStart.x + 0.5f * (screenEnd.x - screenStart.x);
                        screenMid.y = screenStart.y + 0.5f * (screenEnd.y - screenStart.y);

						/*
                        GlobalScript g = GetComponent<GlobalScript>();
                        if (g != null)
                        {
                            g.displayingTarget = true;
                            Vector3 dir = (Vector3.right * Mathf.Cos(currentEvents.events[ev].theta) + Vector3.up * Mathf.Sin(currentEvents.events[ev].phi));
                            dir.Normalize();
                            g.targetLabel.transform.position = g.camera_house.transform.position + dir * g.dome.transform.localScale.x * 0.5f;
                            g.targetLabel.transform.rotation = Quaternion.LookRotation(-dir);
                        }*/


                        Vector2 swipeMid = next + 0.5f * (prev - next);


                        float positionDiff = Vector2.Distance(swipeMid, screenMid);

                        // TODO: If they are within a certain distance, it passes (this may change based on the display, may need adjusting for tabletop)
                        if (positionDiff <= Mathf.Min((Screen.height / 4f), (Screen.width) / 4f))
                        {
                            //Debug.Log("Distance Check Passed, distance was: " + positionDiff);
                        }
                        else
                        {
                            //Debug.Log("Distance Check failed, distance apart was: " + positionDiff);
							DrawSwipeLine(SwipeType.missed);
							return;
                        }

                        float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
                        //Debug.Log("Swipe angle: " + swipeAngle);

                        //Vector3 start = Camera.main.WorldToScreenPoint(((0.1f) * points[0]));
                        //Vector3 end = Camera.main.WorldToScreenPoint(((0.1f) * points[1]));

                        Vector3 change = screenEnd - screenStart;
                        float trailAngle = Mathf.Atan2(change.y, change.x) * Mathf.Rad2Deg;

                        if(swipeAngle < 0.0f)
                        {
                            swipeAngle = 180.0f + swipeAngle;
                        }

                        if(trailAngle < 0.0f)
                        {
                            trailAngle = 180.0f + trailAngle;
                        }

                        //Debug.Log("Trail Angle: " + trailAngle);

                        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(swipeAngle, trailAngle));

                        // Give 30-degree lenience
                        if (angleDiff < 30.0f)
                        {
                            //Debug.Log("Angle Check Passed, angle difference was: " + angleDiff);
                        }
                        else
                        {
                           // Debug.Log("Angle Checked Failed, angle difference was: " + angleDiff);
							DrawSwipeLine(SwipeType.missed);
                            return;
                        }

                        // Length Check - just see if the swipe is long enough
                        //float swipeLength = Vector3.Magnitude(swipeVector);

                        // As long as the swipeVector swipes 1/2 the screen, it passes
                        // This may need to be adjusted based on the display used
                        //correlate this to the length of the event projection...
                        /*if (swipeLength > Screen.width / 4f)
                        {
                            Debug.Log("Length Check Passed, length was: " + swipeLength);
                        }
                        else
                        {
                            Debug.Log("Length Checked Failed, length was: " + swipeLength);
                            return;
                        }*/


						// ----- EVENT DETECTED SUCCESSFULLY - Let the user know
						DrawSwipeLine(SwipeType.found);

						// EDIT for puzzle game - calculate and store the puzzle camera transform so that we can use it later

						//TODO: Need to get the actual event values and place them in the list instead of dummy values
						// EDIT - now this panel must also store the puzzle camera transform
						//Vector3 puzzleCameraLocation = FindPuzzleCameraLocation(currentEvents.events[ev]);
						//Vector3 eventMid = FindTargetLocation(currentEvents.events[ev]);
						EventInfo newEventInfo = GameObject.Find("EventPanel").GetComponent<EventPanelManager>().addEvent(currentEvents.events[ev].eventSource.name, currentEvents.getEnergy(), vStart, vEnd, currentEvents.eventsPlaying[ev].ActivatedDoms);

						// For testing, automatically move the camera after swiping
						PuzzleCamera.GetComponent<PuzzleCameraController>().MoveCamera(newEventInfo);




                        //if (collectionText != null)
                        //{
                        //    collectionText.GetComponent<UnityEngine.UI.Text>().text = "Cosmic Phenomena Collection:\n" + currentEvents.events[ev].eventSource.name;
                        //}
                    }
                }
			} else {
				// No events playing, so draw an idle swipe line
				DrawSwipeLine (SwipeType.idle);
			}
        }
	}

	/*
	private Vector3 FindPuzzleCameraLocation(VisualizeEvent.EventVis swipedEvent) {

		Vector3 vStart = swipedEvent.startPos;
		Vector3 vEnd = swipedEvent.endPos;

		Vector3 lookPosition = (vStart + vEnd) / 2f;

		Vector3 cameraPosition = lookPosition - new Vector3 (0, 0, 1000f);

		return cameraPosition;
	}*/


}
