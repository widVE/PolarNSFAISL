using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

    public int timeLeft = 60;
    private float oneSec = 0f;
    private bool countDown = true;
	// Use this for initialization
	void Start () {
		
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
                    string countTxt = GetComponent<UnityEngine.UI.Text>().text;
                    countTxt = "Time left: " + timeLeft.ToString();
                    GetComponent<UnityEngine.UI.Text>().text = countTxt;
                }
                else
                {
                    //trigger a restart of the game...
                }
            }
        }
	}
}
