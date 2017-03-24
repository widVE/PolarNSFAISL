using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class SwipeRecognizer : MonoBehaviour {

	public FlickGesture swipeGesture;
	public bool showLine = true;
	public LineRenderer ren;
    public VisualizeEvent currentEvents;
	//private ParticleTrail trail;
    private Vector3[] startEnd = new Vector3[2];

	void Start () {
		if (swipeGesture == null) {
			Debug.LogError ("No Flick Gesture assigned for DetectSwipe component on " + this.gameObject.name);
		}
	}

	private void OnEnable() {
		swipeGesture.Flicked += swipeHandler;
	}

	private void OnDisable() {
		swipeGesture.Flicked -= swipeHandler;
	}

	private void swipeHandler(object sender, System.EventArgs e) {
		Vector2 prev = swipeGesture.PreviousScreenPosition;
		Vector2 flickVector = swipeGesture.ScreenFlickVector;
        Vector2 next = prev - flickVector;
        Debug.Log("Swipe Detected - Direction: " + flickVector);
		//Debug.Log ("Start: " + start);
        
		if (showLine) {
            startEnd[0] = Camera.main.ScreenToWorldPoint(new Vector3(prev.x, prev.y, Camera.main.nearClipPlane));
            startEnd[1] = Camera.main.ScreenToWorldPoint(new Vector3(next.x, next.y, Camera.main.nearClipPlane));
            //Debug.Log("Line Drawn: " + startEnd[0] + " to " + startEnd[1]);
            ren.SetPositions(startEnd);
		}

        //let's instead convert any active events to screen space and test there...
        if(currentEvents != null)
        {
            if(currentEvents.eventPlaying)
            {
                Vector3 vStart = currentEvents.events[currentEvents.currEvent].startPos;
                Vector3 vEnd = currentEvents.events[currentEvents.currEvent].endPos;
                Vector3 screenStart = Camera.main.WorldToScreenPoint(vStart);
                Vector3 screenEnd = Camera.main.WorldToScreenPoint(vEnd);
                Vector2 screenDir = new Vector2(screenEnd.x-screenStart.x, screenEnd.y-screenStart.y);
                float mag = screenDir.magnitude;
                //check if flickVector is close to screenDir..
                //probably a better way to do this than compare distances...
                Debug.Log("Event screen space direction: " + screenDir);
                float dist = Vector2.Distance(screenDir, flickVector);
                float dist2 = Vector2.Distance(-screenDir, flickVector);
                Debug.Log(dist);
                Debug.Log(dist2);
                if(dist < 250.0f || dist2 < 250.0f)
                {
                    //add source phenomena to our list...
                    //trigger next steps...
                    Debug.Log("Flicked well!");
                }
                else
                {
                    Debug.Log("You were off");
                }
            }
        }
	}
}
