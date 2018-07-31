using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class Countdown : MonoBehaviour {

    public GameObject swipeGame;
    public int gameTime = 90;
    private int timeLeft;
    private float oneSec = 0f;
    public float summaryPanelLength = 60f;
    public float restartButtonDelay = 10f;
    private bool countDown = false;
    private bool paused = false;
    private bool hasStarted = false;
    public int tempCount;
    public int tempScore;
    
    public GameObject score;
    public GameObject summaryPanel;
    public GameObject tutorial;
    public GameObject startButton;
    public GameObject eventPanelManager;
    public GameObject swipeRecognizer;
    public Animator evalButton;

    public void StartCountdown() { 
        countDown = true;
    }
    public void PauseCountdown()
    {
        paused = true;
        oneSec = 0f; // Reset the current second recorded
    }
    public void ContinueCountdown()
    {
        paused = false;
    }

    void Start () {
        timeLeft = gameTime;
	}

	// Update is called once per frame
	void Update () {    
        if (swipeRecognizer.GetComponent<SwipeRecognizer>().congratsPanel.activeSelf)
        {
            PauseCountdown();
        }
        else
        {
            ContinueCountdown();
        }

        if (countDown && !paused)
        {
            oneSec += Time.deltaTime;
            if (oneSec >= 1f)
            {
                oneSec = 0f;
                timeLeft--;
                if (timeLeft >= 0)
                {
                    string countTxt = LocalizationManager.instance.GetLocalizedValue("countdown") + " " + timeLeft.ToString(); 
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                    GetComponent<UnityEngine.UI.Text>().color = Color.white;

                    if (timeLeft <= 5)
                    {
                        GetComponent<UnityEngine.UI.Text>().color = Color.red;
                        GetComponent<AudioSource>().volume = 0.4f;
                        GetComponent<AudioSource>().pitch = 0.6f;
                        GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    countDown = false;
                    tempCount = swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoCount;
                    eventPanelManager.GetComponent<EventPanelManager>().panels.Clear();
                    //trigger a restart of the game...
                    //high score list?
                    timeLeft = gameTime;
                    string countTxt = LocalizationManager.instance.GetLocalizedValue("countdown") + " " + timeLeft.ToString(); 
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                    tempScore = swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore;

                    if(swipeGame != null)
                    {
                        if (summaryPanel != null)
                        {
                            if (tempCount == 1)
                            {
                                summaryPanel.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("game_summary1") + tempCount + 
                                    LocalizationManager.instance.GetLocalizedValue("game_summary2") + "\n" + LocalizationManager.instance.GetLocalizedValue("game_summary3") + " " + tempScore;
                            }
                            else
                            {
                                summaryPanel.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("game_summary1") + tempCount + 
                                    LocalizationManager.instance.GetLocalizedValue("game_summary2") + "\n" + LocalizationManager.instance.GetLocalizedValue("game_summary3") + " " + tempScore;
                            }

                            int numTimesTouched = Camera.main.GetComponent<TouchScript.Gestures.MultiFlickGesture>().numTouches;
                            string lastTouchTime = Camera.main.GetComponent<TouchScript.Gestures.MultiFlickGesture>().lastTouchTime;
                            //write out log file here
                            StreamWriter w;

                            string d = BuildDate.ToString(); // DateTime.Now.ToString();
                            d = d.Replace("/", "_");
                            d = d.Replace(" ", "_");
                            d = d.Replace(":", "_");

                            using (w = File.AppendText(d + ".log"))
                            {
                                string time = DateTime.Now.ToLongTimeString();
                                string date = DateTime.Now.ToShortDateString();

                                w.WriteLine(date + ", " + time + ", " + tempCount + ", " + tempScore + ", " + numTimesTouched + ", " + lastTouchTime + ", " + swipeGame.GetComponent<SwipeGameMode>().viewedInstructions.ToString() + ", " + LocalizationManager.instance.spanish);
                                w.Close();
                            }

                            StreamWriter w2;
                            try
                            {
                                w2 = new StreamWriter("S:\\research_data\\Polar\\" + d + ".log", true);
                                string time = DateTime.Now.ToShortTimeString();
                                string date = DateTime.Now.ToShortDateString();
                                Debug.Log("updated scores");
                                w2.WriteLine(date + ", " + time + ", " + tempCount + ", " + tempScore + ", " + numTimesTouched + ", " + lastTouchTime + ", " + swipeGame.GetComponent<SwipeGameMode>().viewedInstructions.ToString() + ", " + LocalizationManager.instance.spanish);
                                w2.Close();
                            } catch (Exception e)
                            {
                                if(e is IOException || e is System.IO.IsolatedStorage.IsolatedStorageException)
                                //just don't write it
                                Debug.Log("Couldn't update scores log to cave shared");
                            }  
                            
							Camera.main.GetComponent<TouchScript.Gestures.MultiFlickGesture> ().numTouches = 0;
                            summaryPanel.SetActive(true);
                            if(evalButton != null)
                            {
                                 evalButton.Play("AnimateButton", -1, 0f);                               
                            }
                            SwipeRecognizer swipeRec = swipeRecognizer.GetComponent<SwipeRecognizer>();
                            swipeRec.ResetGoalAccuracy();
                        }

                        if (swipeGame != null)
                        {
                            swipeGame.GetComponent<SwipeGameMode>().StopGame();
                        }

                        StartCoroutine(DelayedResolve(summaryPanelLength, false));
                        StartCoroutine(RestartDelay(restartButtonDelay));
                    }
                    //if (score != null)
                    //{
                    //    //reset score..
                    //    swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore = 0;
                    //    swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoCount = 0;
                    //    score.GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("reset_score");
                    GetComponent<UnityEngine.UI.Text>().color = Color.white;
                    //}
                }
            }
        }
	}

    private IEnumerator RestartDelay(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        summaryPanel.transform.GetChild(0).gameObject.SetActive(true);
    }

    private IEnumerator DelayedResolve(float waittime, bool success)
    {
        yield return new WaitForSeconds(waittime);

        if (!swipeGame.GetComponent<SwipeGameMode>().isGamePlaying())
        {
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

            if(evalButton != null)
            {
                evalButton.gameObject.transform.GetChild(0).GetComponent<UnityEngine.UI.Button>().interactable = true;
                evalButton.transform.GetChild(0).transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = LocalizationManager.instance.GetLocalizedValue("survey_request_label");
            }
        }
    }

    public void DecrTimeLeftBy (int val)
    {
        if (val <= 0) return;
        // +1 so next time is updated,
        // time will be decreased by exactly val units
        timeLeft = timeLeft - val + 1;
        oneSec = 1; // Force instant update
    }
}
