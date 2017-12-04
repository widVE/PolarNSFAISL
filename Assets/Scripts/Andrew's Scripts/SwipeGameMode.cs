using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeGameMode : MonoBehaviour {

	public EventPlayer eventPlayer;
    public GameObject countdownTimer;
	public SwipeRecognizer swipeRecognizer;
    public GameObject mainCamera;
    public GameObject tutorial;
    public GameObject refinePanel;
    public GameObject congratsPanel;
    public GameObject liveHelp;
    public GameObject coneParent;
    public GameObject eventPanel;
    public GameObject summaryPanel;
    public GameObject startButton;
    public GameObject softTutorialText;
    public int highScore;
    public GameObject highScorePanel;
    public GameObject countdownBeep;

    public Strings domStrings;

    private bool isGame;

    private bool isSoft;

	[SerializeField]
	private GameObject topCamera;
    [SerializeField]
    private GameObject topPanel;
	[SerializeField]
	private GameObject sideCamera;
    [SerializeField]
    private GameObject sidePanel;
	[SerializeField]
	private GameObject frontCamera;
    [SerializeField]
    private GameObject frontPanel;

    public GameObject panelParent;

    private bool swipedTop;
    private bool swipedSide;
    private bool swipedFront;

    private float timeStarted;
    private float softTutorialTime;

    [SerializeField]
    private float softTimeout = 30f;

	// Use this for initialization
	void Start () {
        isGame = false;
        isSoft = false;
        swipedTop = false;
        swipedSide = false;
        swipedFront = false;
        highScore = 0;
	}

    void Update()
    {
        float timeSinceStart = UnityEngine.Time.time - timeStarted;
        if (isSoft)
        {
            softTutorialTime = timeSinceStart;
            if (softTutorialTime > softTimeout)
            {
                //if sitting in the soft tutorial for over X seconds, restart...
                //StopSoftTutorial();
                Debug.Log("Stopping soft tutorial.");
                if (startButton != null)
                {
                    startButton.SetActive(true);
                }

                if (softTutorialText != null)
                {
                    softTutorialText.SetActive(false);
                }

                if (tutorial != null)
                {
                    tutorial.GetComponent<Tutorial>().playTutorial = true;
                }

                if (softTutorialText != null)
                {
                    softTutorialText.SetActive(false);
                }

                StopGame();
            }

            if (!isGame)
            {
                timeStarted = UnityEngine.Time.time;
            }
        }
        else
        {
            timeStarted = UnityEngine.Time.time;
        }
    }

    public bool isGamePlaying() { return isGame; }
    public bool isSoftTutorial() { return isSoft; }

    public void summaryDone()
    {
        //reset main camera
        mainCamera.transform.localPosition = new Vector3(300.2444f, 342.7338f, -3306.132f);
        mainCamera.transform.localEulerAngles = new Vector3(23.002f, -5.327f, 0);
         if (tutorial != null)
         {
             tutorial.GetComponent<Tutorial>().playTutorial = true;
         }

         if (startButton != null)
         {
             startButton.SetActive(true);
         }

         if (summaryPanel != null)
         {
             summaryPanel.SetActive(false);
 
             foreach (Transform child in summaryPanel.transform)
             {
                 if (child.gameObject.name.StartsWith("Event:"))
                 {
                     Destroy(child.gameObject);
                 }
             }
 
             summaryPanel.GetComponent<EventPanelManager>().panels.Clear();
         }
    }

    public void StartGame()
    {
        if (!isGame)
        {
            isSoft = true;

            AudioSource[] aSources = GameObject.Find("Sound Effects").GetComponents<AudioSource>();
            if (aSources != null)
            {
                AudioSource background = aSources[4];
                if (background != null)
                {
                    background.Play();
                }
            }

            GameObject.Find("startClick").GetComponent<AudioSource>().Play();
            isGame = true;
            
            timeStarted = UnityEngine.Time.time;

            if (liveHelp != null)
            {
                liveHelp.SetActive(true);
            }

            if (tutorial != null)
            {
                tutorial.GetComponent<Tutorial>().playTutorial = false;
                tutorial.GetComponent<Tutorial>().ClearTutorial();
            }

            if (swipeRecognizer != null)
            {
                swipeRecognizer.ExitResolveMode(false);
            }

            if (eventPlayer != null)
            {
                eventPlayer.GetComponent<EventPlayer>().keepPlaying = true;
                eventPlayer.GetComponent<EventPlayer>().StopCurrentEvent();
                eventPlayer.GetComponent<EventPlayer>().StopTutorialEvent();
            }

            //in case summary panel hadn't been cleared yet...
            foreach (Transform child in summaryPanel.transform)
            {
                if (child.gameObject.name.StartsWith("Event:"))
                {
                    Destroy(child.gameObject);
                }
            }

            summaryPanel.GetComponent<EventPanelManager>().panels.Clear();

            //because these can be on during tutorial
            if (panelParent != null)
            {
                panelParent.SetActive(false);
            }

            if(startButton != null)
            {
                startButton.SetActive(false);
            }

            if(softTutorialText != null)
            {
                softTutorialText.SetActive(true);
            }
        }
    }

    private void CountdownSoft(int count)
    {
        softTutorialText.GetComponent<UnityEngine.UI.Text>().text = "Now let's try for real: ";
        softTutorialText.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = count.ToString();
        if (countdownBeep != null)
        {
            countdownBeep.GetComponent<AudioSource>().volume = .6f;
            countdownBeep.GetComponent<AudioSource>().pitch = .6f;
            countdownBeep.GetComponent<AudioSource>().Play();
        }
    }

    public IEnumerator InvokeMethod(float interval, int invokeCount)
    {
        softTutorialText.transform.GetChild(0).gameObject.SetActive(true);
        for (int i = 0; i < invokeCount; ++i)
        {
            CountdownSoft(invokeCount-i);
            yield return new WaitForSeconds(interval);
        }
        softTutorialText.transform.GetChild(0).gameObject.SetActive(false);

        if (isSoft)
        {
            if (countdownTimer != null)
            {
                countdownTimer.GetComponent<Countdown>().StartCountdown();
            }

            if (mainCamera != null)
            {
                mainCamera.GetComponent<CameraRotate>().spin = true;
            }

            if (softTutorialText != null)
            {
                softTutorialText.SetActive(false);
            }

            /*AudioSource[] aSources = GameObject.Find("Sound Effects").GetComponents<AudioSource>();
            if (aSources != null)
            {
                AudioSource background = aSources[4];
                if (background != null)
                {
                    background.Play();
                }
            }*/

            eventPlayer.ResumePlaying();

            isSoft = false;
        }
    }

    public void StopSoftTutorial()
    {
        if(isSoft)
        {
            StartCoroutine(InvokeMethod(1f, 3));
        }

        softTutorialTime = 0f;
    }

    public void StopGame()
    {
        isGame = false;
        isSoft = false;

        swipedTop = false;
        swipedFront = false;
        swipedSide = false;

        /*if(startButton != null)
        {
            startButton.SetActive(true);
        }*/

        if (highScorePanel != null)
        {
            if (swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore > highScore)
            {
                string scoreTxt = "High Score: " + swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore.ToString() + " Points";
                highScorePanel.GetComponent<UnityEngine.UI.Text>().text = scoreTxt;
                Debug.Log("new high score");
            }
            else
            {
                highScorePanel.GetComponent<UnityEngine.UI.Text>().text = "High Score: " + highScore + " Points";
            }
        }

        if (eventPlayer != null)
        {
            //eventPlayer.GetComponent<EventPlayer>().PlayTutorialEvent();
            eventPlayer.GetComponent<EventPlayer>().StopCurrentEvent();
            eventPlayer.ResumePlaying();
            eventPlayer.scaleArray(3f);
        }

        if (mainCamera != null)
        {
            mainCamera.GetComponent<CameraRotate>().spin = false;
        }

        /*if (tutorial != null)
        {
            tutorial.GetComponent<Tutorial>().playTutorial = true;
        }*/

        if(swipeRecognizer != null)
        {
            swipeRecognizer.ExitResolveMode(false);
        }

        softTutorialText.GetComponent<UnityEngine.UI.Text>().text = "Let's do one for practice.";
        
        AudioSource[] aSources = GameObject.Find("Sound Effects").GetComponents<AudioSource>();
        if (aSources != null)
        {
            AudioSource background = aSources[4];
            if (background != null)
            {
                background.Stop();
            }
        }

        if(refinePanel != null)
        {
            refinePanel.SetActive(false);
        }

        if(congratsPanel != null)
        {
            congratsPanel.SetActive(false);
        }

        if(liveHelp != null)
        {
            liveHelp.SetActive(false);
        }

        if(eventPanel != null)
        {
            foreach(Transform child in eventPanel.transform)
            {
                if(child.gameObject.name.StartsWith("Event:"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        if(coneParent != null)
        {
            //coneParent.GetComponent<EarthView>().ClearCones();

            foreach (Transform child in coneParent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        //because these can be on during tutorial
        if (panelParent != null)
        {
            panelParent.SetActive(false);
        }

        //position top, front, side cameras for tutorial...
        EnableCameras(true, false);

        if (frontPanel != null)
        {
            frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
            frontPanel.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (sidePanel != null)
        {
            sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
            sidePanel.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        if (topPanel != null)
        {
            topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
            topPanel.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void SetSwipeTop(bool swipe) { swipedTop = swipe; }
    public void SetSwipeSide(bool swipe) { swipedSide = swipe; }
    public void SetSwipeFront(bool swipe) { swipedFront = swipe; }

	public void EventSwiped() {

		// Freeze playing, display full event
		eventPlayer.FreezePlaying (true);

		// Activate cameras
		EnableCameras();

		// Add bounds for swiping
		swipeRecognizer.EnterResolveMode();

        //earthView.gameObject.transform.Find("EarthModel").GetComponent<SpinFree>().spin = false;
        Camera.main.GetComponent<CameraRotate>().spin = false;
	}

	public void EventResolved(bool success=false) 
    {
        swipedTop = false;
        swipedFront = false;
        swipedSide = false;
        
        StopSoftTutorial();

        if(success)
        {
            AudioSource a = gameObject.GetComponent<AudioSource>();
            if(a != null && a.isActiveAndEnabled)
            {
                a.Play();
            }
        }

        //earthView.gameObject.transform.FindChild("EarthModel").GetComponent<SpinFree>().spin = true;
        
        if (!isSoft)
        {
            eventPlayer.ResumePlaying();
            Camera.main.GetComponent<CameraRotate>().spin = true;
        }
	}

    public bool SwipedAllThree() { return swipedTop && swipedFront && swipedSide; }

    public float transitionDuration = 2f;
    
    IEnumerator Transition(Vector3 from, Vector3 to, Quaternion fromQ, Quaternion toQ, GameObject toObject)
    {
        float startTime = Time.time;
        float t = 0f;
        while (t < 1.0f)
        {
            //Debug.Log(t);
            t = (Time.time - startTime) / transitionDuration;
            toObject.transform.position = Vector3.Lerp(from, to, t);
            toObject.transform.rotation = Quaternion.Slerp(fromQ, toQ, t);
            //Debug.Log(from);
            //Debug.Log(to);
            //Debug.Log(toObject.transform.position);
            yield return 0;
        }
    }

	private void EnableCameras(bool tutorial=false, bool makeVisible=true) {

        if(domStrings != null)
        {
            domStrings.cam = topCamera.transform;
            //domStrings.scaleMultiplier = 0.0005f;
        }

		Vector3 eventCenterPos = eventPlayer.GetEventCenterpoint (tutorial ? 0 : -1);
        Bounds b = eventPlayer.GetEventBounds(eventCenterPos, tutorial ? 0 : -1);

        // Front Camera
        //frontCamera.transform.position = Camera.main.transform.position;
        frontCamera.transform.rotation = Camera.main.transform.rotation;

        frontCamera.transform.RotateAround(eventCenterPos, UnityEngine.Camera.main.transform.right.normalized, Mathf.Rad2Deg * (Mathf.PI / 2f - Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up))));
        frontCamera.transform.position = b.center - frontCamera.transform.forward.normalized * b.extents.z;

        //forward = frontCamera.transform.forward.normalized;
        //endQ = frontCamera.transform.rotation;

        //frontCamera.transform.position = Camera.main.transform.position;
        //frontCamera.transform.rotation = Camera.main.transform.rotation;

        frontCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.x, b.extents.y) + 70.0f;

        //StartCoroutine(Transition(Camera.main.transform.position, b.center - forward * (Mathf.Tan((topCamera.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) * 0.5f) * Mathf.Max(b.extents.x, b.extents.y)) - forward * b.extents.z,
        //    Camera.main.transform.rotation, endQ, frontCamera));
        
		// Top Camera
        //topCamera.transform.position = Camera.main.transform.position;
        topCamera.transform.rotation = Camera.main.transform.rotation;

        topCamera.transform.RotateAround(eventCenterPos, UnityEngine.Camera.main.transform.right.normalized, Mathf.Rad2Deg * (Mathf.PI - Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up))));
        topCamera.transform.position = b.center + new Vector3(0f, b.extents.y, 0f);

        Quaternion endQ = topCamera.transform.rotation;

        //topCamera.transform.position = Camera.main.transform.position;
        //topCamera.transform.rotation = Camera.main.transform.rotation;

        topCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.x, b.extents.z) + 100.0f;

        if (makeVisible)
        {
            panelParent.SetActive(true);
        }

        StartCoroutine(Transition(frontCamera.transform.position, b.center + new Vector3(0f, b.extents.y, 0f), 
            frontCamera.transform.rotation, endQ, topCamera));

        //StartCoroutine(Transition(Camera.main.transform.position, b.center + 
        //    new Vector3(0f, Mathf.Tan((topCamera.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) * 0.5f) * Mathf.Max(b.extents.x, b.extents.z) + b.extents.y, 0f), 
        //    Camera.main.transform.rotation, endQ, topCamera));
        
		// Side Camera
       
        //sideCamera.transform.position = Camera.main.transform.position;
        sideCamera.transform.rotation = Camera.main.transform.rotation;
        
        sideCamera.transform.RotateAround(eventCenterPos, UnityEngine.Camera.main.transform.right.normalized, Mathf.Rad2Deg * (Mathf.PI/2f - Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up))));
        sideCamera.transform.RotateAround(eventCenterPos, Vector3.up, 90f);

        endQ = sideCamera.transform.rotation;

        sideCamera.transform.position = b.center - sideCamera.transform.forward.normalized * b.extents.x;
        
        //Vector3 forward = sideCamera.transform.forward.normalized;

        //sideCamera.transform.position = Camera.main.transform.position;
        //sideCamera.transform.rotation = Camera.main.transform.rotation;

        sideCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.y, b.extents.z) + 100.0f;

        StartCoroutine(Transition(frontCamera.transform.position, b.center - sideCamera.transform.forward.normalized * b.extents.x,
            frontCamera.transform.rotation, endQ, sideCamera));
        
        //StartCoroutine(Transition(Camera.main.transform.position, b.center -  forward * (Mathf.Tan((topCamera.GetComponent<Camera>().fieldOfView * Mathf.Deg2Rad) * 0.5f) * Mathf.Max(b.extents.y, b.extents.z)) - forward * b.extents.x,
        //    Camera.main.transform.rotation, endQ, sideCamera));

        if (isSoft)
        {
            softTutorialText.GetComponent<UnityEngine.UI.Text>().text = "Good, now do the same in the top, front and side views.";
        }

	}

	public void DisableCameras() {
        panelParent.SetActive(false);
        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
	}
}
