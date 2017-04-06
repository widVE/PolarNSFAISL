using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

public class SwipeRecognizer : MonoBehaviour {

	public FlickGesture swipeGesture;
	public bool showLine = true;
	public LineRenderer ren;
    public VisualizeEvent currentEvents;
    public GameObject collectionText = null;
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
		Vector2 swipeVector = swipeGesture.ScreenFlickVector;
        Vector2 next = prev - swipeVector;
        Debug.Log("Swipe Detected - Direction: " + swipeVector);
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
            if(currentEvents.IsEventPlaying())
            {
                for (int ev = 0; ev < currentEvents.eventsPlaying.Length; ++ev)
                {
                    if (currentEvents.eventsPlaying[ev].isPlaying)
                    {
                        Vector3 vStart = currentEvents.events[ev].startPos;
                        Vector3 vEnd = currentEvents.events[ev].endPos;

                        Vector3 screenStart = Camera.main.WorldToScreenPoint(vStart);
                        Vector3 screenEnd = Camera.main.WorldToScreenPoint(vEnd);

                        Vector2 screenMid;
                        screenMid.x = screenStart.x + 0.5f * (screenEnd.x - screenStart.x);
                        screenMid.y = screenStart.y + 0.5f * (screenEnd.y - screenStart.y);

                        Vector2 swipeMid = startEnd[0] + 0.5f * (startEnd[1] - startEnd[0]);

                        /*float positionDiff = Vector2.Distance(swipeMid, screenMid);

                        // TODO: If they are within a certain distance, it passes (this may change based on the display, may need adjusting for tabletop)
                        if (positionDiff <= Mathf.Min((Screen.height / 2f), (Screen.width) / 2f))
                        {
                            Debug.Log("Distance Check Passed, distance was: " + positionDiff);
                        }
                        else
                        {
                            Debug.Log("Distance Check failed, distance apart was: " + positionDiff);
                            return;
                        }*/

                        float swipeAngle = Mathf.Atan2(swipeVector.y, swipeVector.x) * Mathf.Rad2Deg;
                        Debug.Log("Swipe angle: " + swipeAngle);

                        //Vector3 start = Camera.main.WorldToScreenPoint(((0.1f) * points[0]));
                        //Vector3 end = Camera.main.WorldToScreenPoint(((0.1f) * points[1]));

                        Vector3 change = screenEnd - screenStart;
                        float trailAngle = Mathf.Atan2(change.y, change.x) * Mathf.Rad2Deg;

                        Debug.Log("Trail Angle: " + trailAngle);

                        float angleDiff = Mathf.Abs(Mathf.Abs(swipeAngle) - Mathf.Abs(trailAngle));

                        // Give 20-degree lenience
                        if (angleDiff < 30.0f)
                        {
                            Debug.Log("Angle Check Passed, angle difference was: " + angleDiff);
                        }
                        else
                        {
                            Debug.Log("Angle Checked Failed, angle difference was: " + angleDiff);
                            return;
                        }

                        // Length Check - just see if the swipe is long enough
                        /*float swipeLength = Vector3.Magnitude(swipeVector);

                        // As long as the swipeVector swipes 1/2 the screen, it passes
                        // TODO: This may need to be adjusted based on the display used
                        if (swipeLength > Screen.width / 4f)
                        {
                            Debug.Log("Length Check Passed, length was: " + swipeLength);
                        }
                        else
                        {
                            Debug.Log("Length Checked Failed, length was: " + swipeLength);
                            return;
                        }*/

                        if (collectionText != null)
                        {
                            collectionText.GetComponent<UnityEngine.UI.Text>().text = "Cosmic Phenomena Collection:\n" + currentEvents.events[ev].eventSource.name;
                        }
                        /*Vector2 screenDir = new Vector2(screenEnd.x - screenStart.x, screenEnd.y - screenStart.y);
                        float mag = screenDir.magnitude;
                        //check if flickVector is close to screenDir..
                        //probably a better way to do this than compare distances...
                        Debug.Log("Event screen space direction: " + screenDir);
                        float dist = Vector2.Distance(screenDir, flickVector);
                        float dist2 = Vector2.Distance(-screenDir, flickVector);
                        Debug.Log(dist);
                        Debug.Log(dist2);
                        if (dist < 250.0f || dist2 < 250.0f)
                        {
                            //add source phenomena to our list...
                            //trigger next steps...
                            Debug.Log("Flicked well!");
                            //add the item to a collection list...
                            if (collectionText != null)
                            {
                                collectionText.GetComponent<UnityEngine.UI.Text>().text = "Cosmic Phenomena Collection:\n" + currentEvents.events[ev].eventSource.name;
                            }
                        }
                        else
                        {
                            Debug.Log("You were off");
                        }*/
                    }
                }
            }
        }
	}
}
