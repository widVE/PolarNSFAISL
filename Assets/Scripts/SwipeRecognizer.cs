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
    public GameObject pointsTemplate;
    public int neutrinoScore = 0;
    public int neutrinoCount = 0;
    public float goalAccuracy = .5f; // percentage
    private float initialSwipeAccuracy = 0.5f; // percentage

    public const float DELTA_GOAL_ACCURACY = 0.05f; // percentage
    public const float MAX_GOAL_ACCURACY = 0.95f; // percentage

    public AudioSource collectSound;

    private bool okToResolve = false;

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
    private Vector3[] screenStartEnd = new Vector3[2];

	public bool inResolveMode = false;

	private enum ResolveBounds {top, side, front, none};

	public Camera topCamera;
	public Camera sideCamera;
	public Camera frontCamera;

    public GameObject topPanel;
    public GameObject sidePanel;
    public GameObject frontPanel;
    public GameObject refinePanel;
    public GameObject congratsPanel;

    public float helperNumStages = 2f;

    private Vector3 totalVector = Vector3.zero;
    private Vector3 totalScore = Vector3.zero;

    private bool topSwiped;
    private bool frontSwiped;
    private bool sideSwiped;

    private bool isExiting = false;

    //public string name;

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

        topSwiped = false;
        frontSwiped = false;
        sideSwiped = false;

        UpdateRefinePanelGoal();
        updateScore();
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
		
        //gradients causing alpha to goto 0..
		// Set the gradient appropriately
		switch (type) {
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
		}

        TouchTableLine t = lines[index].GetComponent<TouchTableLine>();
		// Reset the timer so the line will begin fading after 2 seconds
		t.lineTimer = 2f;
        t.lineDrawn = true;
        t.lineFading = false;
        t.camToUse = cameraToUse;
        t.startEnd[0] = screenStartEnd[0];
        t.startEnd[1] = screenStartEnd[1];
        /*if(cameraToUse.orthographic)
        {
            t.startEnd[0].z = 0f;
            t.startEnd[1].z = 0f;
        }*/
		// Draw the line
        //Debug.Log(startEnd[0]);
        //Debug.Log(startEnd[1]);
        //lines[index].GetComponent<LineRenderer>().SetPositions(startEnd);
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

        Vector2 swipeVector = swipeGesture.ScreenFlicks[swipeGesture.recognizedId];
        //Debug.Log(swipeVector.magnitude);
        //need to swipe at least 100 pixels distance
        if(swipeVector.magnitude <= 100f)
        {
            return;
        }

		if (inResolveMode) {
            if (okToResolve)
            {
                handleResolveSwipe();
            }
		} else {
            totalVector = Vector3.zero;
            totalScore = Vector3.zero;
            okToResolve = false;
            handleCaptureSwipe ();
        }
	}

	private void handleCaptureSwipe() 
    {
		SwipeCalculation (Camera.main);

        if(inResolveMode)
        {
            //if we successfully captured an initial swipe, time delay for a second before we handle resolve swipes
            //to avoid accidentally swiping middle resolve window...
            StartCoroutine(ResolveDelay(1f));
        }
	}

    private IEnumerator ResolveDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        okToResolve = true;
    }

	private void handleResolveSwipe() {

		ResolveBounds currentBound = findBounds ();

        switch (currentBound) 
        {
            case ResolveBounds.top:
            {
                if (!topSwiped)
                {
                    //Debug.Log("Resolve top");
                    swipeGameMode.SetSwipeTop(true);
                    SwipeCalculation(topCamera);
                    topSwiped = true;
                    topPanel.transform.GetChild(1).gameObject.SetActive(!isExiting);
                }
                break;
            }
            case ResolveBounds.side:
            {
                if (!sideSwiped)
                {
                    //Debug.Log("Resolve side");
                    swipeGameMode.SetSwipeSide(true);
                    SwipeCalculation(sideCamera);
                    sideSwiped = true;
                    sidePanel.transform.GetChild(1).gameObject.SetActive(!isExiting);
                }
                break;
            }
            case ResolveBounds.front:
            {
                if (!frontSwiped)
                {
                    //Debug.Log("Resolve front");
                    swipeGameMode.SetSwipeFront(true);
                    SwipeCalculation(frontCamera);
                    frontSwiped = true;
                    frontPanel.transform.GetChild(1).gameObject.SetActive(!isExiting);
                }
                break;
            }
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

        //Debug.Log("End: " + end);
        //Debug.Log("Start: " + start);
        //Debug.Log("Swipe: " + swipeVector);
        //Debug.Log("Screen Min: " + Screen.height * 0.3);
        //Debug.Log("Screen Max: " + Screen.height * 0.6);

        //just add a decent amount of fudge to this to allow people to begin and end their swipe
        //off of the panel...judge which panel based on the shortest screen-space distance of the center of their 
        //swipe with the center of the panel
        const float FUDGE_AMOUNT = 0.075f;

        if (start.y < Screen.height * (0.2824 - FUDGE_AMOUNT) || start.y > Screen.height * (0.6157 + FUDGE_AMOUNT)) {
			//Debug.LogWarning ("Bad y on start");
			return ResolveBounds.none;
		}

		// Y values are in bounds, now check x values
        

		ResolveBounds startBounds;

        if (start.x > Screen.width * (0.169f - FUDGE_AMOUNT) && start.x < Screen.width * (0.3572 + FUDGE_AMOUNT))
        {
			startBounds = ResolveBounds.top;
        }
        else if (start.x > Screen.width * (0.40625 - FUDGE_AMOUNT) && start.x < Screen.width * (0.59375f + FUDGE_AMOUNT))
        {
			startBounds = ResolveBounds.front;
		} else if (start.x > Screen.width * (0.6315f - FUDGE_AMOUNT) && start.x < Screen.width * (0.8190f + FUDGE_AMOUNT)) {
			startBounds = ResolveBounds.side;
		} else {
			//Debug.LogWarning ("Bad x on start");
			return ResolveBounds.none;
		}

        if (startBounds != ResolveBounds.none)
        {
			return startBounds;
		}
        else
        {
			//Debug.LogWarning ("StartEnd mismatch");
			return ResolveBounds.none;
		}
	}

	public void EnterResolveMode() 
    {
		inResolveMode = true;
        //helpSwipe.SetActive(false);
        gameObject.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().enabled = true;

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

	public void ExitResolveMode(bool success=false) 
    {
        isExiting = false;
        inResolveMode = false;
        gameObject.GetComponent<UnityEngine.PostProcessing.PostProcessingBehaviour>().enabled = false;

        topSwiped = false;
        frontSwiped = false;
        sideSwiped = false;
        topPanel.transform.GetChild(1).gameObject.SetActive(false);
        sidePanel.transform.GetChild(1).gameObject.SetActive(false);
        frontPanel.transform.GetChild(1).gameObject.SetActive(false);

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

	private void SwipeCalculation(Camera cameraToUse) 
    {
		Vector2 prev = swipeGesture.PreviousPos[swipeGesture.recognizedId];
		Vector2 swipeVector = swipeGesture.ScreenFlicks[swipeGesture.recognizedId];

		// The end of the flick gesture (really the beginning, I think these are backwards but it doesn't affect anything)
		Vector2 next = prev - swipeVector;
		//Debug.LogError("Swipe Detected - Direction: " + swipeVector);
		//Debug.Log ("Start: " + start);

		// If we should show the line, then calculate where the screen-coordinate end points lie in world coordinates
		// We do this because line renderers only work with positions in 3D, not screen coordinates
        screenStartEnd[0].Set(prev.x, prev.y, cameraToUse.nearClipPlane + 10f);
        screenStartEnd[1].Set(next.x, next.y, cameraToUse.nearClipPlane + 10f);
		startEnd[0] = cameraToUse.ScreenToWorldPoint(new Vector3(prev.x, prev.y, cameraToUse.nearClipPlane + 10));
		startEnd[1] = cameraToUse.ScreenToWorldPoint(new Vector3(next.x, next.y, cameraToUse.nearClipPlane + 10));
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

                        Vector2 v = new Vector2(change.x, change.y);
                        float vTest = 0.0f;

                        if (cameraToUse == Camera.main)
                        {
                            //totalVector = swipeVector.normalized;
                            //for main camera to comparison in world space...
                            vTest = Mathf.Abs(Vector2.Dot(swipeVector.normalized, v.normalized));
                        }
                        else 
                        {
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

                            /* Accuracy modulation */
                            // Assume goalAccuracy = [0.0 .. 1.0] and initialSwipeAccuracy = 0.5,
                            // then exponent = [1.0 .. 4.0]
                            int exp = Mathf.RoundToInt(((goalAccuracy - initialSwipeAccuracy) * 6f) + 1f);
                            if (exp > 4) exp = 4; // max accuracy is 1 so exp should not exceed 4
                            // Compute (goalAccuracy ^ exp) in for loop due to no float exponent support
                            float initialScore = vTest;
                            for (int i = 0; i < exp; i++)
                            {
                                vTest = vTest * initialScore;
                            }
                            // Debug.Log("Score modulated from " + initialScore + " to " + vTest);
                            
                            if(refinePanel != null)
                            {
                                string txt = refinePanel.GetComponent<UnityEngine.UI.Text>().text;
                                int percentIdx = txt.IndexOf('%');
                                string subTxt = txt.Substring(percentIdx - 5, 5);
                                string accuracy = (vTest*100f).ToString("F2");
                                if (vTest * 100f < 10f)
                                {
                                    accuracy = "0" + accuracy;
                                }
                                else if(vTest *100f >= 100f)
                                {
                                    accuracy = "99.99";
                                }
                                string newTxt = txt.Replace(subTxt, accuracy);
                                refinePanel.GetComponent<UnityEngine.UI.Text>().text = newTxt;
                            }

                            //change this to just be a gradient from red to green based on vTest.
                            if(cameraToUse == frontCamera)
                            {
                                totalScore.z = vTest;
                                //Debug.Log(vTest);
                                //TODO: add event to summary panel
                                frontPanel.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);
                                frontPanel.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);

                                if (!swipeGameMode.isSoftTutorial())
                                {
                                    //always add points
                                    int score = (int)(vTest * 10) * 10;
                                    //EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                    //    screenStart, screenEnd, summaryColor, score, false);
                                    Debug.Log("Added: " + score + " points.");
                                    neutrinoScore += score;
                                    updateScore();
                                    spawnPoints(score, frontPanel);
                                }
                                else
                                {
                                    collectSound.Play();
                                }
                            } 
                            else if(cameraToUse == sideCamera)
                            {
                                totalScore.y = vTest;
                                sidePanel.transform.GetChild(1).GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);
                                sidePanel.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);

                                if (!swipeGameMode.isSoftTutorial())
                                {
                                    //always add points
                                    int score = (int)(vTest * 10) * 10;
                                    //EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                    //    screenStart, screenEnd, summaryColor, score, false);
                                    //Debug.Log("Added: " + score + " points.");
                                    neutrinoScore += score;
                                    updateScore();
                                    spawnPoints(score, sidePanel);
                                }
                                else
                                {
                                    collectSound.Play();
                                }
                            }
                            else if(cameraToUse == topCamera)
                            {
                                totalScore.x = vTest;
                                topPanel.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);
                                topPanel.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.Lerp(UnityEngine.Color.red, UnityEngine.Color.green, vTest);

                                if (!swipeGameMode.isSoftTutorial())
                                {
                                    //always add points
                                    int score = (int)(vTest * 10) * 10;
                                    //EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                    //    screenStart, screenEnd, summaryColor, score, false);
                                    //Debug.Log("Added: " + score + " points.");
                                    neutrinoScore += score;
                                    updateScore();
                                    spawnPoints(score, topPanel);
                                }
                                else
                                {
                                    collectSound.Play();
                                }
                            }
                        }

                        if (cameraToUse == Camera.main)
                        {
                            if (vTest < initialSwipeAccuracy)
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
                                        StartCoroutine(DelayedResolve(0.5f, false));
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

						// Now calculate a few more things and add the event to the panel

						if (!currentEvents.eventsPlaying[ev].isDetected && !inResolveMode)
						{
                            //first swipe complete, entering resolve mode with 3 cameras
                            //add event to summary panel, increment score based on accuracy
                            
                            //TODO:
                            if (summaryPanel != null)
                            {
                                if (!swipeGameMode.isSoftTutorial())
                                {
                                    EventPanelManager epm = summaryPanel.GetComponent<EventPanelManager>();
                                    if (epm != null)
                                    {
                                        //TODO: add bonus
                                        //Debug.Log(currentEvents.lastEventNumber + currentEvents.events[currentEvents.lastEventNumber].eventSource.name);
                                        Color summaryColor = new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
                                        //add in empty event to init score to 0
                                        //EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        //    screenStart, screenEnd, summaryColor, 0, false);
                                        //add in event again with actual score
                                        int score = 100;// (int)(vTest * 10) * 10;
                                        //e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        //    screenStart, screenEnd, summaryColor, score, false);
                                        Debug.Log("Added: " + score + " points.");
                                        neutrinoScore += score;
                                       
                                        updateScore();

                                        //what value is this?
                                        /// spawnPoints(score, new Vector3(3520, 1800, 0));
                                        spawnPoints(score, frontPanel);
                                    }
                                }
                                else
                                {
                                    collectSound.Play();
                                }
                            }

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
                            StartCoroutine(DelayedResolve(swipeGameMode.isSoftTutorial() ? 0.5f : 4f, true));

                            //add 100 point bonus
                            if (summaryPanel != null)
                            {
                                if (!swipeGameMode.isSoftTutorial())
                                {
                                    EventPanelManager epm = summaryPanel.GetComponent<EventPanelManager>();
                                    if (epm != null)
                                    {
                                        //TODO: add bonus
                                        //Debug.Log(currentEvents.lastEventNumber + currentEvents.events[currentEvents.lastEventNumber].eventSource.name);
                                        Color summaryColor2 = new Color(Random.Range(0.3f, 1f), Random.Range(0.3f, 1f), Random.Range(0.3f, 1f));
                                        //add in empty event to init score to 0
                                        EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                            screenStart, screenEnd, summaryColor2, 0, false);
                                        //add in event again with actual score
                                        //e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        //    screenStart, screenEnd, summaryColor2, 1000, false);
                                        Debug.Log("Added bonus: " + 100 + " points.");
                                        neutrinoScore += 100;
                                        neutrinoCount++;
                                        updateScore();
                                        spawnPoints(100, frontPanel);
                                    }
                                }
                            }
                            
                            if (congratsPanel != null)
                            {
                                swipeGameMode.DisableCameras();

                                if (!swipeGameMode.isSoftTutorial())
                                {
                                    name = currentEvents.events[currentEvents.lastEventNumber].eventSource.name;
                                    congratsPanel.SetActive(true);
                                    congratsPanel.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("congrats") + LocalizationManager.instance.GetLocalizedValue(name); 
                                }
                            }

                            Color summaryColor = Color.white;
                            if (!swipeGameMode.isSoftTutorial())
                            {
                                if (eventPanel != null)
                                {
                                    EventPanelManager epm = eventPanel.GetComponent<EventPanelManager>();
                                    //Debug.Log(currentEvents.lastEventNumber + currentEvents.events[currentEvents.lastEventNumber].eventSource.name);
                                    EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        screenStart, screenEnd, Color.white, (int)vTest * 100);

                                    if (e != null)
                                    {
                                        summaryColor = e.gameObject.GetComponent<UnityEngine.UI.Image>().color;
                                        earthView.AddDetectedEvent(currentEvents.events[currentEvents.lastEventNumber].startPos,
                                               currentEvents.events[currentEvents.lastEventNumber].endPos + (currentEvents.events[currentEvents.lastEventNumber].endPos - currentEvents.events[currentEvents.lastEventNumber].startPos).normalized * 500000f, summaryColor, vTest, goalAccuracy);                                        
                                    }
                                }
                            }
                            
                            /*  if (summaryPanel != null)
                            {
                                EventPanelManager epm = summaryPanel.GetComponent<EventPanelManager>();
                                if (epm != null)
                                {
                                    //TODO: add 100 point bonus
                                    //Debug.Log(currentEvents.lastEventNumber + currentEvents.events[currentEvents.lastEventNumber].eventSource.name);
                                    EventInfo e = epm.addEvent(currentEvents.events[currentEvents.lastEventNumber].eventSource.name, currentEvents.totalEnergy, vStart, vEnd,
                                        screenStart, screenEnd, summaryColor, (int)vTest * 100, false);
                                }
                            } */

                            //assuming just one event here for now..
                            //add point to sphere map...
                            if (!swipeGameMode.isSoftTutorial())
                            {
                                if (sphereMap != null)
                                {
                                    Vector3 dir = currentEvents.events[currentEvents.lastEventNumber].startPos - currentEvents.events[currentEvents.lastEventNumber].endPos;
                                    dir = dir.normalized;
                                    float longitude = (2 * Mathf.Acos(Vector3.Dot(dir, Vector3.up)) / Mathf.PI) - 1;
                                    float lat = (2 * Mathf.Acos(Vector3.Dot(dir, Vector3.forward)) / Mathf.PI) - 1;
                                    sphereMap.PlotPoint(lat, longitude, summaryColor);
                                }
                            }

                            /*if(scorePanel != null)
                            {
                                //neutrinoScore++;
                                string countTxt = "Score: " + neutrinoScore.ToString() + " Points";
                                scorePanel.GetComponent<UnityEngine.UI.Text>().text = countTxt;
                            } */
                        }
					}
				}
			} else if (cameraToUse.Equals(Camera.main)) {
				// Else then no events playing, so draw an idle swipe line
				DrawSwipeLine(SwipeType.idle, swipeGesture.recognizedId%10, cameraToUse);
			}
		}
	}

    public void updateScore()
    {
        if (scorePanel != null)
        {
            string countTxt = LocalizationManager.instance.GetLocalizedValue("score") + " " + neutrinoScore.ToString() +
                " " + LocalizationManager.instance.GetLocalizedValue("points"); 
            scorePanel.GetComponent<UnityEngine.UI.Text>().text = countTxt;
        }
    }

    private IEnumerator DelayedResolve(float waittime, bool success)
    {
        swipeGameMode.DisableCameras();
        currentEvents.scaleArray(3f);
        swipeGameMode.EventResolved(success);
        //Debug.Log("Has Resolved: " + success);
        // Increase difficulty on success
        if (success && !swipeGameMode.isSoftTutorial() 
            && DELTA_GOAL_ACCURACY > 0f)
        {
            goalAccuracy += DELTA_GOAL_ACCURACY;
            if (goalAccuracy > MAX_GOAL_ACCURACY)
                goalAccuracy = MAX_GOAL_ACCURACY;

            UpdateRefinePanelGoal();
            // This chage will also increase time penalty
            // for missing event on SwipeGameMode class
            swipeGameMode.SetTimePenaltyByPercent(
                2 * (goalAccuracy - initialSwipeAccuracy));

            currentEvents.truePath.enabled = false;

            /*Color newColor = currentEvents.truePath.material.color;
            if (newColor.a > 0f && currentEvents.truePath.enabled)
            {
                newColor.a -= 1f / helperNumStages; // alpha decreases linearly as function of success
                if (newColor.a < 0.1f) // If success >= 6
                {
                    currentEvents.truePath.enabled = false;
                    newColor.a = 1f;
                }
            }
            currentEvents.truePath.material.color = newColor;*/
        }
        isExiting = true;
        yield return new WaitForSeconds(waittime);
        ExitResolveMode(success);
    }

    public void spawnPoints (int points, GameObject refObj)
    {
        if (pointsTemplate != null)
        {
            GameObject text = Instantiate(pointsTemplate, refObj.transform);
            Vector3[] corners = new Vector3[4];
            refObj.GetComponent<RectTransform>().GetWorldCorners(corners);
            float objHeight = corners[1].y - corners[0].y;
            text.transform.Translate(new Vector3(0, objHeight, 0));
            text.GetComponent<UnityEngine.UI.Text>().text = points.ToString();

            collectSound.pitch = .5f +  (points / 100f);
            collectSound.Play();
        }   
    }

    public void ResetGoalAccuracy ()
    {
        goalAccuracy = initialSwipeAccuracy;
        UpdateRefinePanelGoal();
    }

    public void UpdateRefinePanelGoal ()
    {
        if (refinePanel != null)
        {
            string txt = refinePanel.GetComponent<UnityEngine.UI.Text>().text;
            int percentIdx = txt.LastIndexOf('%');
            string subTxt = txt.Substring(percentIdx - 2, 2);
            string accuracy = Mathf.CeilToInt(100 * goalAccuracy).ToString();
            string newTxt = txt.Replace(subTxt, accuracy);
            refinePanel.GetComponent<UnityEngine.UI.Text>().text = newTxt;
        }
    }
}
