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
	// Use this for initialization

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
                }
                else
                {
                    countDown = false;
                    //trigger a restart of the game...
                    //high score list?
                    timeLeft = gameTime;
                    string countTxt = "Time left: " + timeLeft.ToString();
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                    if(score != null)
                    {
                        //reset score..
                        score.GetComponent<UnityEngine.UI.Text>().text = "Score: 0 Neutrinos";
                    }

                    if(swipeGame != null)
                    {
                        if(summaryPanel != null)
                        {
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
        }
    }
}
