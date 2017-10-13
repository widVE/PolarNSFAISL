using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class SwipeRecognizer : MonoBehaviour {

	//----------VARIABLES----------

	public SwipeGameMode swipeGameMode;
    public EarthView earthView;
    public PlotSphereMap sphereMap;
    public GameObject scorePanel;
    public GameObject detectionCone;
    public GameObject helpSwipe;
    public GameObject eventPanel;
    public GameObject summaryPanel;

    public int neutrinoScore = 0;
    public float goalAccuracy = .9f;

	// TouchScript gesture that this script listens to
	public MultiFlickGesture swipeGesture;
    public GameObject lineObject;
    private GameObject[] lines = new GameObject[10];

	// Color gradients for the swipe line, depending on the state of the swipe
	// Gradient for if there was an event, but the swipe missed it (red line)
	private Gradient missedGradient;
	// Gradient for if there was an event and it was swiped and caputured correctly (green line)
	private Gradient foundGradient;
	// Gradient for if the user swipes, but no event was playing (grey line)
	private Gradient idleGradient;

	// Enumeration used for switching between swipe types
	private enum SwipeType {missed, found, idle};

	// Reference to the VisualizeEvent script, which handles all event playback
    public EventPlayer currentEvents;

	// Array used to store the endpoints of the swipe line in world coordinates, for use with the line renderer
    private Vector3[] startEnd = new Vector3[2];

	private bool inResolveMode = false;

	private enum ResolveBounds {top, side, front, none};

	public Camera topCamera;
	public Camera sideCamera;
	public Camera frontCamera;

    public GameObject topPanel;
    public GameObject sidePanel;
    public GameObject frontPanel;
    public GameObject refinePanel;
    public GameObject congratsPanel;

    private Vector3 totalVector = Vector3.zero;
    private Vector3 totalScore = Vector3.zero;

	//----------END VARIABLES----------

	/// <summary>
	/// Start - For variable initialization and gradient generation
	/// </summary>
	void Start () {

		if (swipeGesture == null) {
			Debug.LogError ("No Flick Gesture assigned for DetectSwipe component on " + this.gameObject.name);
		}

        if (lineObject != null)
        {
            for (int i = 0; i < 10; ++i)
            {
                lines[i] = GameObject.Instantiate(lineObject, Camera.main.transform);
            }
        }
		BuildGradients ();
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
	private void DrawSwipeLine(SwipeType type, int index=0, Camera cameraToUse=null) {
		

		// Set the gradient appropriately
		/*switch (type) {
		case SwipeType.missed:
            lines[index].GetComponent<LineRenderer>().colorGradient = missedGradient;
			break;
		case SwipeType.found:
            lines[index].GetComponent<LineRenderer>().colorGradient = foundGradient;
			break;
		case SwipeType.idle:
            lines[index].GetComponent<LineRenderer>().colorGradient = idleGradient;
			break;
		default:
			break;
		}*/

        TouchTableLine t = lines[index].GetComponent<TouchTableLine>();
		// Reset the timer so the line will begin fading after 2 seconds
		t.lineTimer = 5.5f;
        t.lineDrawn = true;
        t.lineFading = false;
		// Draw the line
        //Debug.Log(startEnd[0]);
        //Debug.Log(startEnd[1]);
        lines[index].GetComponent<LineRenderer>().SetPositions(startEnd);
        //lines[index].GetComponent<LineRenderer>().sortingOrder = -1;
        //ren.SetPositions(startEnd);
        //Debug.Log ("Swipe was drawn " + type);
        if (cameraToUse != null && cameraToUse == Camera.main)
        {
            AudioSource[] aSources = GameObject.Find("Sound Effects").GetComponents<AudioSource>();
            AudioSource toPlay = null;

            switch (type)
            {
                case SwipeType.missed:
                    toPlay = aSources[0];
                    break;
                case SwipeType.found:
                    toPlay = aSources[2];
                    break;
                case SwipeType.idle:
                    toPlay = aSources[1];
                    break;
                default:
                    break;
            }

            if (toPlay != null)
            {
                toPlay.Play();
            }
        }
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

        if(!swipeGameMode.GetComponent<SwipeGameMode>().isGamePlaying())
        {
            return;
        }

		if (inResolveMode) {
			handleResolveSwipe ();
		} else {
            totalVector = Vector3.zero;
            totalScore = Vector3.zero;
            handleCaptureSwipe ();
        }
	}

	private void handleCaptureSwipe() {
		SwipeCalculation (Camera.main);
	}

	private void handleResolveSwipe() {

		ResolveBounds currentBound = findBounds ();

		switch(currentBound) {
		case ResolveBounds.top:
            Debug.Log("Resolve top");
            swipeGameMode.SetSwipeTop(true);
            SwipeCalculation (topCamera);
			break;
		case ResolveBounds.side:
            Debug.Log("Resolve side");
            swipeGameMode.SetSwipeSide(true);
            SwipeCalculation (sideCamera);
            break;
		case ResolveBounds.front:
            Debug.Log("Resolve front");
            swipeGameMode.SetSwipeFront(true);
            SwipeCalculation (frontCamera);
			break; 
		case ResolveBounds.none:
			break;
		default:
			break;
		}

	}

	private ResolveBounds findBounds() {
		Vector2 end = swipeGesture.PreviousPos[swipeGesture.recognizedId];
		Vector2 swipeVector = swipeGesture.ScreenFlicks[swipeGesture.recognizedId];

		// The end of the flick gesture (really the beginning, I think these are backwards but it doesn't affect anything)
		Vector2 start = end - swipeVector;

		if (start.y < Screen.height * 0.3 || start.y > Screen.height * 0.6) {
			Debug.LogWarning ("Bad y on start");
			return ResolveBounds.none;
		}

		// Y values are in bounds, now check x values

		ResolveBounds startBounds;

		if (start.x > 0.1234375f && start.x < Screen.width * 0.2921875f) {
			startBounds = ResolveBounds.top;
		} else if (start.x > Screen.width * 0.415625 && start.x < Screen.width * 0.584375f) {
			startBounds = ResolveBounds.front;
		} else if (start.x > Screen.width * 0.7078125f) {
			startBounds = ResolveBounds.side;
		} else {
			Debug.LogWarning ("Bad x on start");
			return ResolveBounds.none;
		}

        if (startBounds != ResolveBounds.none)
        {
			return startBounds;
		} else {
			Debug.LogWarning ("StartEnd mismatch");
			return ResolveBounds.none;
		}
	}

	public void EnterResolveMode() {
		inResolveMode = true;
        
        if (refinePanel != null)
        {
            string txt = refinePanel.GetComponent<UnityEngine.UI.Text>().text;
            int percentIdx = txt.IndexOf('%');
            string subTxt = txt.Substring(percentIdx - 5, 5);
            string accuracy = "00.00";
            string newTxt = txt.Replace(subTxt, accuracy);
            refinePanel.GetComponent<UnityEngine.UI.Text>().text = newTxt;
            refinePanel.SetActive(true);

        }

        currentEvents.scaleArray(0.149815f);
    }

	public void ExitResolveMode(bool success=false) {
		inResolveMode = false;

        if (refinePanel != null)
        {
            refinePanel.SetActive(false);
        }
        if (congratsPanel != null)
        {
            congratsPanel.SetActive(false);
        }
        if(detectionCone != null)
        {
            detectionCone.SetActive(false);
        }
    }

	private void SwipeCalculation(Camera cameraToUse) {
		Vector2 prev = swipeGesture.PreviousPos[swipeGesture.recognizedId];
		Vector2 swipeVector = swipeGesture.ScreenFlicks[swipeGesture.recognizedId];

		// The end of the flick gesture (really the beginning, I think these are backwards but it doesn't affect anything)
		Vector2 next = prev - swipeVector;
		//Debug.LogError("Swipe Detected - Direction: " + swipeVector);
		//Debug.Log ("Start: " + start);

		// If we should show the line, then calculate where the screen-coordinate end points lie in world coordinates
		// We do this because line renderers only work with positions in 3D, not screen coordinates
		startEnd[0] = cameraToUse.ScreenToWorldPoint(new Vector3(prev.x, prev.y, cameraToUse.nearClipPlane + 1));
		startEnd[1] = cameraToUse.ScreenToWorldPoint(new Vector3(next.x, next.y, cameraToUse.nearClipPlane + 1));
		//Debug.Log("Line Drawn: " + startEnd[0] + " to " + startEnd[1]);

		// Begin event detection
		// Here we iterate through every actively playing event, and see if our swipe path matches with the neutrino event path
		
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
						Vector3 screenStart = cameraToUse.WorldToScreenPoint(vStart);
						Vector3 screenEnd = cameraToUse.WorldToScreenPoint(vEnd);
                        //Vector

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
                        //this position check doesn't need to be done for the front,side,top cameras.
                        if (cameraToUse == Camera.main)
                        {
                            if (positionDiff > Mathf.Min((Screen.height / 4f), (Screen.width / 4f)))
                            {
                                Debug.Log("Missed due to position");
                                DrawSwipeLine(SwipeType.missed, swipeGesture.recognizedId % 10, cameraToUse);
                                continue;
                            }
                        }

                        Vector3 worldEventVector = (vEnd - vStart).normalized;
                        Vector3 worldSwipeVector = (startEnd[0] - startEnd[1]).normalized;

                        Vector3 change = screenEnd - screenStart;

                        // Angle check next - check to see if the angles of the swipe and path are relatively similar
                        /*float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;

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
*/
                        Vector2 v = new Vector2(change.x, change.y);
                        float vTest = 0.0f;

                        if (cameraToUse == Camera.main)
                        {
                            //totalVector = swipeVector.normalized;
                            //for main camera to comparison in world space...
                            vTest = Mathf.Abs(Vector2.Dot(swipeVector.normalized, v.normalized));
                        }
                        else {
                            if (totalVector.magnitude > 0)
                            {
                                if (Vector3.Dot(totalVector.normalized, worldSwipeVector.normalized) < 0.0f)
                                {
                                    worldSwipeVector = -worldSwipeVector;
                                }
                            }
                            totalVector += worldSwipeVector;
                            totalVector = totalVector.normalized;
                            vTest = Mathf.Abs(Vector3.Dot(worldEventVector, totalVector));
                            if(refinePanel != null)
                            {
                                string txt = refinePanel.GetComponent<UnityEngine.UI.Text>().text;
                                int percentIdx = txt.IndexOf('%');
                                string subTxt = txt.Substring(percentIdx - 5, 5);
                                string accuracy = (vTest*100f).ToString("F2");
                                string newTxt = txt.Replace(subTxt, accuracy);
                                refinePanel.GetComponent<UnityEngine.UI.Text>().text = newTxt;
                            }

                            //change this to just be a gradient from red to green based on vTest.
                            if(cameraToUse == frontCamera)
                            {
                                totalScore.z = vTest;
                                frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);
                                /*if(totalScore.x == 0f && totalScore.y == 0f)
                                {
                                    frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                }
                                else if (totalScore.x > 0f && totalScore.y == 0f)
                                {
                                    if (totalScore.z > totalScore.x)
                                    {
                                        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }
                                else if (totalScore.x == 0f && totalScore.y > 0f)
                                {
                                    if (totalScore.z > totalScore.y)
                                    {
                                        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }
                                else if (totalScore.x > 0f && totalScore.y > 0f)
                                {
                                    if (totalScore.z > totalScore.x && totalScore.z > totalScore.y)
                                    {
                                        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }*/
                            } 
                            else if(cameraToUse == sideCamera)
                            {
                                totalScore.y = vTest;
                                sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);
                                /*if (totalScore.x == 0f && totalScore.z == 0f)
                                {
                                    sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                }
                                else if(totalScore.x > 0f && totalScore.z == 0f)
                                {
                                    if(totalScore.y > totalScore.x)
                                    {
                                        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else 
                                    {
                                        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }
                                else if(totalScore.x == 0f && totalScore.z > 0f)
                                {
                                    if (totalScore.y > totalScore.z)
                                    {
                                        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }
                                else if(totalScore.x > 0f && totalScore.z > 0f)
                                {
                                    if (totalScore.y > totalScore.x && totalScore.y > totalScore.z)
                                    {
                                        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }*/
                            }
                            else if(cameraToUse == topCamera)
                            {
                                totalScore.x = vTest;
                                topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);
                                /*if (totalScore.y == 0f && totalScore.z == 0f)
                                {
                                    topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                }
                                else if (totalScore.y > 0f && totalScore.z == 0f)
                                {
                                    if (totalScore.x > totalScore.y)
                                    {
                                        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }
                                else if (totalScore.y == 0f && totalScore.z > 0f)
                                {
                                    if (totalScore.x > totalScore.z)
                                    {
                                        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }
                                else if (totalScore.y > 0f && totalScore.z > 0f)
                                {
                                    if (totalScore.x > totalScore.y && totalScore.x > totalScore.z)
                                    {
                                        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.green;
                                    }
                                    else
                                    {
                                        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.red;
                                    }
                                }*/
                            }
                        }

                        //Debug.Log(worldEventVector.ToString("F4"));
                        //Debug.Log(worldSwipeVector.ToString("F4"));
                        //Debug.Log(totalVector.ToString("F4"));
                        Debug.Log(vTest);

                        // Give 30-degree lenience, and if over that, then it doesn't match
                        //if (angleDiff >= 30.0f)
                        if (cameraToUse == Camera.main)
                        {
                            if (vTest < goalAccuracy)
                            {
                                //if vTest < than prior vTest, mark red, else green...
                                DrawSwipeLine(SwipeType.missed, swipeGesture.recognizedId % 10, cameraToUse);
                                continue;
                            }
                        }
                        else
                        {
                            if (vTest < 0.999f)
                            {
                                //if vTest < than prior vTest, mark red, else green...
                                DrawSwipeLine(SwipeType.missed, swipeGesture.recognizedId % 10, cameraToUse);
                                if (swipeGameMode.SwipedAllThree())
                                {
                                    if (vTest < goalAccuracy)
                                    {
                                        StartCoroutine(DelayedResolve(2f, false));
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }

						// ----- EVENT DETECTED SUCCESSFULLY - Let the user know by drawing a green line
						DrawSwipeLine(SwipeType.found, swipeGesture.recognizedId%10, cameraToUse);
                        //Debug.Log(vTest);
						// Now calculate a few more things and add the event to the panel

						// Need to do some mathematical magic to get the swipe endpoints into proper "estimated" world coordinates, not relative to the Main Camera
						/*Vector3 pathCenterWorld = (vStart + vEnd) / 2f;

						// Center position relative to the Main Camera
						float centerpointCameraZValue = cameraToUse.transform.InverseTransformPoint(pathCenterWorld).z;

						// Start and end of the swipe in world coordinates relative to the main camera
						Vector3 startCamera = cameraToUse.transform.InverseTransformPoint (startEnd [0]);
						Vector3 endCamera = cameraToUse.transform.InverseTransformPoint (startEnd [1]);

						// EDIT for puzzle game - calculate and store the puzzle camera transform so that we can use it later

						// "Pushing the line out" so that it has a z value equal to the event's midpoint z value relative to the camera
						float startCoeff = centerpointCameraZValue / startCamera.z;
						startCamera *= startCoeff;

						float endCoeff = centerpointCameraZValue / endCamera.z;
						endCamera *= endCoeff;

						// startDirection and endDirection are now the positions of the swipe relative to the camera, convert them to world coordinates
						Vector3 swipeStartWorld = cameraToUse.transform.TransformPoint (startCamera);
						Vector3 swipeEndWorld = cameraToUse.transform.TransformPoint (endCamera);*/

						if (!currentEvents.eventsPlaying[ev].isDetected && !inResolveMode)
						{
							currentEvents.eventsPlaying[ev].isDetected = true;
                            /*if(cameraToUse == Camera.main)
                            {
                                if(detectionCone != null)
                                {
                                    detectionCone.SetActive(true);
                                    Quaternion q = detectionCone.transform.rotation;
                                    q.SetLookRotation(worldSwipeVector.normalized);
                                    detectionCone.transform.rotation = q;
                                }
                            }*/
							swipeGameMode.EventSwiped ();
						}

                        if(cameraToUse != Camera.main && inResolveMode)
                        {
                            //if we are here, we've successfully swiped the event..
                            //tell user good job or something, then accumulate event and return to game.
                            //Debug.Log("SUCCESS");

                            if(congratsPanel != null)
                            {
                                swipeGameMode.DisableCameras();
                                congratsPanel.SetActive(true);
                                congratsPanel.GetComponent<UnityEngine.UI.Text>().text = "Great Job!  You detected a neutrino from a: " + currentEvents.events[currentEvents.lastEventNumber].eventSource.name;
                            }

                            StartCoroutine(DelayedResolve(3f, true));
                            Color summaryColor = Color.white;

                            if (eventPanel != null)
                            {
                                EventPanelManager epm = eventPanel.GetComponent<EventPanelManager>();
                                if (epm != null)
                                {
                                    //Debug.Log(currentEvents.lastEventNumber + currentEvents.events[currentEvents.lastEventNumber].eventSource.name);
                                    EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        screenStart, screenEnd, Color.white);
                                    summaryColor = e.gameObject.GetComponent<UnityEngine.UI.Image>().color;
                                    earthView.AddDetectedEvent(currentEvents.events[currentEvents.lastEventNumber].startPos,
                                        currentEvents.events[currentEvents.lastEventNumber].endPos, summaryColor, vTest, goalAccuracy);
                                }
                            }

                            if (summaryPanel != null)
                            {
                                EventPanelManager epm = summaryPanel.GetComponent<EventPanelManager>();
                                if (epm != null)
                                {
                                    //Debug.Log(currentEvents.lastEventNumber + currentEvents.events[currentEvents.lastEventNumber].eventSource.name);
                                    EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        screenStart, screenEnd, summaryColor, false);
                                }
                            }

                            //assuming just one event here for now..
                            //add point to sphere map...
                            if (sphereMap != null)
                            {
                                Vector3 dir = currentEvents.events[currentEvents.lastEventNumber].startPos - currentEvents.events[currentEvents.lastEventNumber].endPos;
                                dir = dir.normalized;
                                float longitude = Mathf.Acos(Vector3.Dot(dir, Vector3.up));
                                float lat = Mathf.Acos(Vector3.Dot(dir, Vector3.forward));
                                //sphereMap.PlotPoint(new Vector2(lat, longitude));
                            }

                            if(scorePanel != null)
                            {
                                neutrinoScore++;
                                string countTxt = "Score: " + neutrinoScore.ToString() + " Neutrinos";
                                scorePanel.GetComponent<UnityEngine.UI.Text>().text = countTxt;
                            }
                        }
					}
				}
			} else if (cameraToUse.Equals(Camera.main)) {
				// Else then no events playing, so draw an idle swipe line
				DrawSwipeLine(SwipeType.idle, swipeGesture.recognizedId%10, cameraToUse);
			}
		}
	}

    private IEnumerator DelayedResolve(float waittime, bool success)
    {
        swipeGameMode.DisableCameras();
        currentEvents.scaleArray(3f);
        swipeGameMode.EventResolved(success);

        yield return new WaitForSeconds(waittime);
        ExitResolveMode(success);
    }
}
