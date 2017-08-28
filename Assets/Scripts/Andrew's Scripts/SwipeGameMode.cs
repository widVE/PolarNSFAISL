using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeGameMode : MonoBehaviour {

	public EventPlayer eventPlayer;

	public SwipeRecognizer swipeRecognizer;

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

    private bool swipedTop = false;
    private bool swipedSide = false;
    private bool swipedFront = false;

	// Use this for initialization
	void Start () {
		
	}

    public void SetSwipeTop(bool swipe) { swipedTop = swipe; }
    public void SetSwipeSide(bool swipe) { swipedSide = swipe; }
    public void SetSwipeFront(bool swipe) { swipedFront = swipe; }

	public void EventSwiped() {

		// Freeze playing, display full event
		eventPlayer.FreezePlaying ();

		// Activate cameras
		EnableCameras();

		// Add bounds for swiping
		swipeRecognizer.EnterResolveMode();

        //earthView.gameObject.transform.Find("EarthModel").GetComponent<SpinFree>().spin = false;
        Camera.main.GetComponent<CameraRotate>().spin = false;
	}

	public void EventResolved(bool success=false) {
        swipedTop = false;
        swipedFront = false;
        swipedSide = false;
		eventPlayer.ResumePlaying ();
        if(success)
        {
            //assuming just one event here for now..
            
            AudioSource a = gameObject.GetComponent<AudioSource>();
            if(a != null && a.isActiveAndEnabled)
            {
                //Debug.Log("Playing audio");
                a.Play();
            }
        }

        //earthView.gameObject.transform.FindChild("EarthModel").GetComponent<SpinFree>().spin = true;
        Camera.main.GetComponent<CameraRotate>().spin = true;
	}

    public bool SwipedAllThree() { return swipedTop && swipedFront && swipedSide; }

    public float transitionDuration = 10f;
    
    IEnumerator Transition(Vector3 from, Vector3 to, Quaternion fromQ, Quaternion toQ, GameObject toObject)
    {
        float t = 0.0f;
        while (t < 1.0f)
        {
            //Debug.Log(t);
            t += Time.deltaTime * (Time.timeScale / transitionDuration);
            toObject.transform.position = Vector3.Lerp(from, to, t);
            toObject.transform.rotation = Quaternion.Slerp(fromQ, toQ, t);
            //Debug.Log(from);
            //Debug.Log(to);
            //Debug.Log(toObject.transform.position);
            yield return 0;
        }
    }

	private void EnableCameras() {

		Vector3 eventCenterPos = eventPlayer.GetEventCenterpoint ();
        Bounds b = eventPlayer.GetEventBounds(eventCenterPos);

		// Top Camera
        topCamera.transform.position = Camera.main.transform.position;
        topCamera.transform.rotation = Camera.main.transform.rotation;

        topCamera.transform.RotateAround(eventCenterPos, UnityEngine.Camera.main.transform.right.normalized, Mathf.Rad2Deg * (Mathf.PI - Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up))));
        //topCamera.transform.position = b.center + new Vector3(0f, b.extents.y, 0f);

        Quaternion endQ = topCamera.transform.rotation;

        topCamera.transform.position = Camera.main.transform.position;
        topCamera.transform.rotation = Camera.main.transform.rotation;

        topCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.x, b.extents.z) + 60.0f;

		topCamera.SetActive(true);
        topPanel.SetActive(true);

        StartCoroutine(Transition(Camera.main.transform.position, b.center + new Vector3(0f, b.extents.y, 0f), 
            Camera.main.transform.rotation, endQ, topCamera));
        
		// Side Camera
       
        sideCamera.transform.position = Camera.main.transform.position;
        sideCamera.transform.rotation = Camera.main.transform.rotation;
        
        sideCamera.transform.RotateAround(eventCenterPos, UnityEngine.Camera.main.transform.right.normalized, Mathf.Rad2Deg * (Mathf.PI/2f - Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up))));
        sideCamera.transform.RotateAround(eventCenterPos, Vector3.up, 90f);

        endQ = sideCamera.transform.rotation;

        //sideCamera.transform.position = b.center - sideCamera.transform.forward.normalized * b.extents.x;
        
        Vector3 forward = sideCamera.transform.forward.normalized;

        sideCamera.transform.position = Camera.main.transform.position;
        sideCamera.transform.rotation = Camera.main.transform.rotation;

        sideCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.y, b.extents.z) + 60.0f;

		sideCamera.SetActive(true);
        sidePanel.SetActive(true);

        StartCoroutine(Transition(Camera.main.transform.position, b.center - forward * b.extents.x,
            Camera.main.transform.rotation, endQ, sideCamera));
        

		// Front Camera
        frontCamera.transform.position = Camera.main.transform.position;
        frontCamera.transform.rotation = Camera.main.transform.rotation;
       
        frontCamera.transform.RotateAround(eventCenterPos, UnityEngine.Camera.main.transform.right.normalized, Mathf.Rad2Deg * (Mathf.PI / 2f - Mathf.Acos(Vector3.Dot(Camera.main.transform.forward.normalized, Vector3.up))));
        //frontCamera.transform.position = b.center - frontCamera.transform.forward.normalized * b.extents.z;

        forward = frontCamera.transform.forward.normalized;
        endQ = frontCamera.transform.rotation;

        frontCamera.transform.position = Camera.main.transform.position;
        frontCamera.transform.rotation = Camera.main.transform.rotation;

        frontCamera.GetComponent<Camera>().orthographicSize = Mathf.Max(b.extents.x, b.extents.y) + 60.0f;

        frontCamera.SetActive (true);
        frontPanel.SetActive(true);

        StartCoroutine(Transition(Camera.main.transform.position, b.center - forward * b.extents.z,
            Camera.main.transform.rotation, endQ, frontCamera));
        
	}

	public void DisableCameras() {
		topCamera.SetActive (false);
		sideCamera.SetActive(false);
		frontCamera.SetActive (false);
        topPanel.SetActive(false);
        sidePanel.SetActive(false);
        frontPanel.SetActive(false);
        frontPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        sidePanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
        topPanel.GetComponent<UnityEngine.UI.Image>().color = UnityEngine.Color.cyan;
	}
}
