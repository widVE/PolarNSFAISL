using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

/// <summary>
/// TouchScript handler for when a flick gesture is detected on the screen, used for "capturing"
/// the events as they occur in the main array
/// If the swipe either starts or ends on the right hand (puzzle) side of the screen, the swipe
/// ignored.
/// </summary>
public class SwipeRecognizer : MonoBehaviour {

	//----------VARIABLES----------

	// TouchScript gesture that this script listens to
	public FlickGesture swipeGesture;

	// Boolean to toggle whether or not the swipe lines should be drawn when the user's swipe
	// This is primarily a debug feature, but could be incorporated into a final build
	public bool showLine = true;

	// Reference to the line renderer for the swipe line drawing
	public LineRenderer ren;

	// Color gradients for the swipe line, depending on the state of the swipe
	// Gradient for if there was an event, but the swipe missed it (red line)
	private Gradient missedGradient;
	// Gradient for if there was an event and it was swiped and caputured correctly (green line)
	private Gradient foundGradient;
	// Gradient for if the user swipes, but no event was playing (grey line)
	private Gradient idleGradient;

	// Enumeration used for switching between swipe types
	private enum SwipeType {missed, found, idle};

	// Timer used for swipe line fading
	private float lineTimer = 3.0f;

	// Boolean flag to signify if a swipe line is currently drawn on the screen and is not fading
	private bool lineDrawn = false;

	// Boolean flag to signify if a swipe line is currently drawn on the screen and is currently fading
	private bool lineFading = false;

	// Reference to the VisualizeEvent script, which handles all event playback
    public VisualizeEvent currentEvents;

	// Array used to store the endpoints of the swipe line in world coordinates, for use with the line renderer
    private Vector3[] startEnd = new Vector3[2];

	//----------END VARIABLES----------

	/// <summary>
	/// Start - For variable initialization and gradient generation
	/// </summary>
	void Start () {
		if (swipeGesture == null) {
			Debug.LogError ("No Flick Gesture assigned for DetectSwipe component on " + this.gameObject.name);
		}

		BuildGradients ();
	}

	/// <summary>
	/// Update - Soley used for swipe line drawing, not swipe detection or calculation
	/// </summary>
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

	/// <summary>
	/// Subscribes to the flicked event when enabled
	/// </summary>
	private void OnEnable() {
		swipeGesture.Flicked += swipeHandler;
	}

	/// <summary>
	/// Unsubscribes to the flicked event when disabled
	/// </summary>
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

	/// <summary>
	/// Checks if the swipe was on the left-hand side of the array
	/// 
	/// </summary>
	/// <returns><c>true</c>true if the start and end of the line are on the left side of the screen,<c>false</c> otherwise.</returns>
	/// <param name="start">Start of the swipe line in screen coordinates</param>
	/// <param name="end">End of the swipe line in screen coordinates</param>
	private bool InSwipeBounds(Vector2 start, Vector2 end) {

		// Check bounds relative to screen pixel width
		if ((start.x > Screen.width/2) || (end.x > Screen.width/2)) {
			return false;
		}
		return true;
	}

	/// <summary>
	/// Handler for when a flicked gesture is detected by TouchScript
	/// It checks the bounds of the swipe line, and if it is in bounds checks how it aligns
	/// with the neutrino paths for all currently playing events. If the swipe line matches up to an event path,
	/// the event is "captured" and added to the captured events panel
	/// </summary>
	/// <param name="sender">Sender of the event, unused</param>
	/// <param name="e">Unity Event arguments object, unused</param>
	private void swipeHandler(object sender, System.EventArgs e) {

		// The start of the flick gesture
		Vector2 prev = swipeGesture.PreviousScreenPosition;

		// The screen vector representing the flick
		Vector2 swipeVector = swipeGesture.ScreenFlickVector;

		// The end of the flick gesture (really the beginning, I think these are backwards but it doesn't affect anything)
        Vector2 next = prev - swipeVector;

		// If we should show the line, then calculate where the screen-coordinate end points lie in world coordinates
		// We do this because line renderers only work with positions in 3D, not screen coordinates
		if (showLine) {
            startEnd[0] = Camera.main.ScreenToWorldPoint(new Vector3(prev.x, prev.y, Camera.main.nearClipPlane + 1));
            startEnd[1] = Camera.main.ScreenToWorldPoint(new Vector3(next.x, next.y, Camera.main.nearClipPlane + 1));
		}

       	// Begin event detection
		// Here we iterate through every actively playing event, and see if our swipe path matches with the neutrino event path
		// All calculations are done in screen coordinates
        if(currentEvents != null)
        {
            if(currentEvents.IsEventPlaying())
            {
				// Iterate through actively playing events
                for (int ev = 0; ev < currentEvents.eventsPlaying.Length; ++ev)
                {
					if (currentEvents.eventsPlaying[ev].isPlaying)
                    {

						// Get the start and end positions of the neutrino path for this event, in world coordinates
                        Vector3 vStart = currentEvents.events[ev].startPos;
                        Vector3 vEnd = currentEvents.events[ev].endPos;

						// Convert them to screen coordinates
                        Vector3 screenStart = Camera.main.WorldToScreenPoint(vStart);
                        Vector3 screenEnd = Camera.main.WorldToScreenPoint(vEnd);

						// First check - distance check
						// See if the midpoints of both paths are relatively close

						// path midpoint
                        Vector2 screenMid;
                        screenMid.x = screenStart.x + 0.5f * (screenEnd.x - screenStart.x);
                        screenMid.y = screenStart.y + 0.5f * (screenEnd.y - screenStart.y);

						// swipe midpoint
                        Vector2 swipeMid = next + 0.5f * (prev - next);

						// Distance between them
                        float positionDiff = Vector2.Distance(swipeMid, screenMid);

     					// If the difference is too large, draw the missed line and return immediately - ya missed it!
						// Move on to next event
                        if (positionDiff > Mathf.Min((Screen.height / 4f), (Screen.width) / 4f))
                        {
							DrawSwipeLine(SwipeType.missed);
							continue;
                        }
                       

						// Angle check next - check to see if the angles of the swipe and path are relatively similar
                        float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;

                        Vector3 change = screenEnd - screenStart;
                        float trailAngle = Mathf.Atan2(change.y, change.x) * Mathf.Rad2Deg;

						// Keeping the angles positive
                        if(swipeAngle < 0.0f)
                        {
                            swipeAngle = 180.0f + swipeAngle;
                        }

                        if(trailAngle < 0.0f)
                        {
                            trailAngle = 180.0f + trailAngle;
                        }

						// Get the difference
                        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(swipeAngle, trailAngle));

                        // Give 30-degree lenience, and if over that, then it doesn't match
                        if (angleDiff >= 30.0f)
                        {
							DrawSwipeLine(SwipeType.missed);
                            continue;
                        }

						// ----- EVENT DETECTED SUCCESSFULLY - Let the user know by drawing a green line
						DrawSwipeLine(SwipeType.found);

						// Now calculate a few more things and add the event to the panel

						// Need to do some mathematical magic to get the swipe endpoints into proper "estimated" world coordinates, not relative to the Main Camera
						Vector3 pathCenterWorld = (vStart + vEnd) / 2f;

						// Center position relative to the Main Camera
						float centerpointCameraZValue = Camera.main.transform.InverseTransformPoint(pathCenterWorld).z;

						// Start and end of the swipe in world coordinates relative to the main camera
						Vector3 startCamera = Camera.main.transform.InverseTransformPoint (startEnd [0]);
						Vector3 endCamera = Camera.main.transform.InverseTransformPoint (startEnd [1]);

						// "Pushing the line out" so that it has a z value equal to the event's midpoint z value relative to the camera
						float startCoeff = centerpointCameraZValue / startCamera.z;
						startCamera *= startCoeff;

						float endCoeff = centerpointCameraZValue / endCamera.z;
						endCamera *= endCoeff;


						// startDirection and endDirection are now the positions of the swipe relative to the camera, convert them to world coordinates
						Vector3 swipeStartWorld = Camera.main.transform.TransformPoint (startCamera);
						Vector3 swipeEndWorld = Camera.main.transform.TransformPoint (endCamera);


						// Add the event to the event panel list, nice and cleanly
						EventInfo newEventInfo = GameObject.Find("EventPanel").GetComponent<EventPanelManager>().addEvent(currentEvents.events[ev].eventSource.name, currentEvents.getEnergy(), vStart, vEnd, swipeStartWorld, swipeEndWorld, currentEvents.eventsPlaying[ev].ActivatedDoms);
                    }
                }
			} else {
				// Else then no events playing, so draw an idle swipe line
				DrawSwipeLine (SwipeType.idle);
			}
        }
	}


}
