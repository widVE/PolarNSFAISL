using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

    public GameObject swipeGame;
    public int gameTime = 90;
    private int timeLeft;
    private float oneSec = 0f;
    private bool countDown = false;
    public GameObject score;
    public GameObject summaryPanel;
    public GameObject tutorial;
    public GameObject startButton;
    public GameObject eventPanelManager;
    public GameObject swipeRecognizer;

    public void StartCountdown() { 
        countDown = true; 
    }

	void Start () {
        timeLeft = gameTime;
	}
	
	// Update is called once per frame
	void Update () {

        if (countDown)
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
                    GetComponent<UnityEngine.UI.Text>().alignment = TextAnchor.MiddleCenter;

                    if(timeLeft <= 5)
                    {
                        GetComponent<AudioSource>().Play();
                    }
                }
                else
                {
                    countDown = false;
                    int tempScore = swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore;
                    eventPanelManager.GetComponent<EventPanelManager>().panels.Clear();
                    //trigger a restart of the game...
                    //high score list?
                    timeLeft = gameTime;
                    string countTxt = "Time left: " + timeLeft.ToString();
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                    if(score != null)
                    {
                        //reset score..
                        swipeRecognizer.GetComponent<SwipeRecognizer>().neutrinoScore = 0;
                        score.GetComponent<UnityEngine.UI.Text>().text = "Score: 0 Neutrinos";
                    }

                    if(swipeGame != null)
                    {
                        if(summaryPanel != null)
                        {
                            if (tempScore == 1)
                            {
                                summaryPanel.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Game Summary: You detected " + tempScore + " neutrino source";
                            } else
                            {
                                summaryPanel.transform.GetChild(3).gameObject.GetComponent<UnityEngine.UI.Text>().text = "Game Summary: You detected " + tempScore + " neutrino sources";
                            }
                            
                            summaryPanel.SetActive(true);
                        }

                        if (swipeGame != null)
                        {
                            swipeGame.GetComponent<SwipeGameMode>().StopGame();
                        }

                        StartCoroutine(DelayedResolve(10f, false));
                    }
                }
            }
        }
	}

    private IEnumerator DelayedResolve(float waittime, bool success)
    {
        yield return new WaitForSeconds(waittime);

        if (tutorial != null)
        {
            tutorial.GetComponent<Tutorial>().playTutorial = true;
        }

        if(startButton != null)
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
