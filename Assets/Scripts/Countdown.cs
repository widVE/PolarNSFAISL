using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

    
    public int gameTime = 90;
    private int timeLeft;
    private float oneSec = 0f;
    private bool countDown = false;
    public GameObject score;
	// Use this for initialization

    public void StartCountdown() { countDown = true; Debug.Log("Starting countdown"); }

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
                }
            }
        }
	}
}
