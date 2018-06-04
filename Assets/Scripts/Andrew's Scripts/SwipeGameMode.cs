using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeGameMode : MonoBehaviour {

	public EventPlayer eventPlayer;
    public GameObject instructionChoice;
    public GameObject instructions;
    public GameObject countdownTimer;
	public SwipeRecognizer SwipeRecognizer;
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
    public int highScore = 0;
    public GameObject highScorePanel;
    public GameObject countdownBeep;
    public GameObject sphereMap;
    

    public Strings domStrings;

    private bool isGame;

    private bool isSoft;

    private float instructionStarted = 0f;
    public float instructionTimeout = 60f;
    public bool viewedInstructions = false;

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

    private int instructionPanelIndex = 0;

    private int timePenalty = 0;
    public const int MAX_TIME_PENALTY = 5;

    public GameObject evalButton;

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
        HighScore();
	}

    public void LoadSurvey()
    {
        if(evalButton != null)
        {
            evalButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            evalButton.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("loading_survey");
        }

        if (LocalizationManager.instance.spanish)
        {
            Debug.Log("spanish survey");
            Application.OpenURL("https://uwmadison.co1.qualtrics.com/jfe/form/SV_beDHLF64cs7Wr4N");
        }
        else
        {
            Debug.Log("en survey");
            Application.OpenURL("https://uwmadison.co1.qualtrics.com/jfe/form/SV_eyujvZNGHehJ2sJ");
        }
        
    }

    public void StartInstructions()
    {
        if (instructions != null)
        {
            instructions.SetActive(true);
        }

        instructionPanelIndex = 0;
        instructionStarted = UnityEngine.Time.time;
        viewedInstructions = true;
    }

    public void ShowInstructionPanel()
    {
        if(instructions != null)
        {
            //Debug.Log(instructionPanelIndex);
            
            int numPanels = instructions.transform.GetChild(2).transform.childCount;
            //Debug.Log(numPanels);

            instructions.transform.GetChild(2).transform.GetChild(instructionPanelIndex).gameObject.SetActive(false);
            instructionPanelIndex++;
            if (instructionPanelIndex < numPanels)
            {
                instructionStarted = UnityEngine.Time.time;
                instructions.transform.GetChild(2).transform.GetChild(instructionPanelIndex).gameObject.SetActive(true);
            }
            else
            {
                instructionStarted = 0f;
                instructionPanelIndex = 0;
                SkipInstructions();
            }
        }
    }

    public void SkipInstructions()
    {
        if(instructions != null)
        {
            instructions.SetActive(false);
        }

        InstructionChoice(false);
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
                mainCamera.transform.localPosition = new Vector3(300.2444f, 342.7338f, -3306.132f);
                mainCamera.transform.localEulerAngles = new Vector3(23.002f, -5.327f, 0);

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

                if (eventPlayer != null)
                {
                    eventPlayer.truePath.enabled = false;
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

            if(instructionStarted != 0f)
            {
                if(timeStarted - instructionStarted > instructionTimeout)
                {
                    mainCamera.transform.localPosition = new Vector3(300.2444f, 342.7338f, -3306.132f);
                    mainCamera.transform.localEulerAngles = new Vector3(23.002f, -5.327f, 0);

                    Debug.Log("Stopping instructions.");
                    if (startButton != null)
                    {
                        startButton.SetActive(true);
                    }

                    if (tutorial != null)
                    {
                        tutorial.GetComponent<Tutorial>().playTutorial = true;
                    }

                    if(instructionChoice != null)
                    {
                        instructionChoice.SetActive(false);
                    }

                    if(instructions != null)
                    {
                        instructions.SetActive(false);
                    }

                    StopGame();
                    instructionStarted = 0f;
                }
            }
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
            summaryPanel.transform.GetChild(0).gameObject.SetActive(false);
            foreach (Transform child in summaryPanel.transform)
            {
                if (child.gameObject.name.StartsWith("Event:"))
                {
                    Destroy(child.gameObject);
                }
            }
 
            summaryPanel.GetComponent<EventPanelManager>().panels.Clear();
        }

        if (evalButton != null)
        {
            evalButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            evalButton.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("survey_request_label");
        }

        if (eventPlayer != null)
        {
            eventPlayer.truePath.enabled = false;
            Color newColor = eventPlayer.truePath.material.color;
            newColor.a = 1f;
            eventPlayer.truePath.material.color = newColor;
        }
    }

    public void InstructionChoice(bool yes)
    {
        //viewedInstructions = yes;

        //add instruction choice here...
        if (instructionChoice != null)
        {
            instructionChoice.SetActive(false);
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

        if(yes)
        {
            instructionStarted = UnityEngine.Time.time;
            //show instruction panels...
            StartInstructions();
            //ShowInstructionPanel();
        }
        else
        {
            instructionStarted = 0f;

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

            isGame = true;

            timeStarted = UnityEngine.Time.time;

            /*if (liveHelp != null)
            {
                liveHelp.SetActive(true);
            }*/

            if (softTutorialText != null)
            {
                softTutorialText.SetActive(true);
            }
        }
    }

    public void StartGame()
    {
        viewedInstructions = false;

        if (!isGame)
        {
            GameObject.Find("startClick").GetComponent<AudioSource>().Play();

            if (startButton != null)
            {
                startButton.SetActive(false);
            }

            if (tutorial != null)
            {
                tutorial.GetComponent<Tutorial>().playTutorial = false;
                tutorial.GetComponent<Tutorial>().ClearTutorial();
            }

            if (SwipeRecognizer != null)
            {
                SwipeRecognizer.ExitResolveMode(false);
                SwipeRecognizer.neutrinoScore = 0;
                SwipeRecognizer.neutrinoCount = 0;
                countdownBeep.GetComponent<Countdown>().score.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("reset_score");
            }

            //add instruction choice here...
            //need a time out if someone doesn't choose an instruction item...
            if(instructionChoice != null)
            {
                instructionStarted = UnityEngine.Time.time;
                instructionChoice.SetActive(true);
            }
        }
    }

    private void CountdownSoft(int count)
    {
        softTutorialText.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("soft_tutorial_text1");
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

    public void HighScore()
    {
        if (highScorePanel != null)
        {
            string scoreTxt = LocalizationManager.instance.GetLocalizedValue("high_score") + " " + highScore +
                    " " + LocalizationManager.instance.GetLocalizedValue("points");
            highScorePanel.GetComponent<UnityEngine.UI.Text>().text = scoreTxt;
        }
    }

    public void StopGame()
    {
        //call update hs
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
            if (SwipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore > highScore)
            {
                string scoreTxt = LocalizationManager.instance.GetLocalizedValue("high_score") + " " + SwipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore.ToString() + 
                    " " + LocalizationManager.instance.GetLocalizedValue("points");
                highScorePanel.GetComponent<UnityEngine.UI.Text>().text = scoreTxt;
                Debug.Log("new high score");
                highScore = SwipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore;
            }
            else
            {
                highScorePanel.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("high_score") + " " + highScore + " " + LocalizationManager.instance.GetLocalizedValue("points");
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

        if(SwipeRecognizer != null)
        {
            SwipeRecognizer.ExitResolveMode(false);
        }

        softTutorialText.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("soft_tutorial_text2");
        
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

        /*if(liveHelp != null)
        {
            liveHelp.SetActive(false);
        }*/

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

        if(sphereMap != null)
        {
            sphereMap.GetComponent<PlotSphereMap>().ClearPoints();
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
		SwipeRecognizer.EnterResolveMode();

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
        else if(!isSoftTutorial()) // If failed to resolve
        {
            countdownTimer.GetComponent<Countdown>().DecrTimeLeftBy(timePenalty);
            SwipeRecognizer.spawnPoints(-timePenalty, new Vector3(1915, 1900, 0));
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
            softTutorialText.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("soft_tutorial_text3");
        }

	}

	public void DisableCameras() {
        panelParent.SetActive(false);
        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
	}

    public void SetTimePenaltyByPercent(float percent)
    {
        if (percent < 0f) percent = 0f;
        else if (percent > 1f) percent = 1f;

        timePenalty = Mathf.RoundToInt(percent * MAX_TIME_PENALTY);
    }
}
