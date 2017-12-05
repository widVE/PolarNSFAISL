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
    public float summaryPanelLength = 10f;
    private bool countDown = false;
    private bool paused = false;
    private bool hasStarted = false;
    public GameObject score;
    public GameObject summaryPanel;
    public GameObject tutorial;
    public GameObject startButton;
    public GameObject eventPanelManager;
    public GameObject swipeRecognizer;

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
                    string countTxt = "Time left: " + timeLeft.ToString();
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                    GetComponent<UnityEngine.UI.Text>().color = Color.white;

                    if (timeLeft <= 5)
                    {
                        GetComponent<UnityEngine.UI.Text>().color = Color.red;
                        GetComponent<AudioSource>().volume = 1;
                        GetComponent<AudioSource>().pitch = .9f;
                        GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    countDown = false;
                    int tempCount = swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoCount;
                    eventPanelManager.GetComponent<EventPanelManager>().panels.Clear();
                    //trigger a restart of the game...
                    //high score list?
                    timeLeft = gameTime;
                    string countTxt = "Time left: " + timeLeft.ToString();
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                    int tempScore = swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore;

                    if(swipeGame != null)
                    {
                        if (summaryPanel != null)
                        {
                            if (tempCount == 1)
                            {
                                summaryPanel.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Game Summary: You detected " + tempCount + " neutrino source for a score of: " + tempScore;
                            }
                            else
                            {
                                summaryPanel.transform.GetChild(4).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Game Summary: You detected " + tempCount + " neutrino sources for a score of: " + tempScore;
                            }

                            int numTimesTouched = Camera.main.GetComponent<TouchScript.Gestures.MultiFlickGesture>().numTouches;

                            //write out log file here
                            StreamWriter w;

                            string d = BuildDate.ToString();// DateTime.Now.ToString();
                            d = d.Replace("/", "_");
                            d = d.Replace(" ", "_");
                            d = d.Replace(":", "_");

                            using (w = File.AppendText(d + ".log"))
                            {
                                string time = DateTime.Now.ToShortTimeString();
                                string date = DateTime.Now.ToShortDateString();

                                w.WriteLine(date + ", " + time + ", " + tempCount + ", " + tempScore + ", " + numTimesTouched);
                                w.Close();
                            }

                            StreamWriter w2;
                            try
                            {
                                w2 = new StreamWriter("S:\\research_data\\Polar\\" + d + ".log", true);
                                string time = DateTime.Now.ToShortTimeString();
                                string date = DateTime.Now.ToShortDateString();
                                Debug.Log("updated scores");
                                w2.WriteLine(date + ", " + time + ", " + tempCount + ", " + tempScore + ", " + numTimesTouched);
                                w2.Close();
                            } catch (Exception e)
                            {
                                if(e is IOException || e is System.IO.IsolatedStorage.IsolatedStorageException)
                                //just don't write it
                                Debug.Log("Couldn't update scores log to cave shared");
                            }  
                            
							Camera.main.GetComponent<TouchScript.Gestures.MultiFlickGesture> ().numTouches = 0;
                            summaryPanel.SetActive(true);
                        }

                        if (swipeGame != null)
                        {
                            swipeGame.GetComponent<SwipeGameMode>().StopGame();
                        }

                        StartCoroutine(DelayedResolve(summaryPanelLength, false));
                    }

                    if (score != null)
                    {
                        //reset score..
                        swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore = 0;
                        swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoCount = 0;
                        score.GetComponent<UnityEngine.UI.Text>().text = "Score: 0 Points";
                        GetComponent<UnityEngine.UI.Text>().color = Color.white;
                    }

                }
            }
        }
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
    }
}
