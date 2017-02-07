using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class VisualizeEvent : MonoBehaviour {

    public string EventFile;
    //public GameObject DOMTest;
    public bool eventPlaying = false;
    private int currIndex = 0;
    private const float BELOW_ICE = -1950.0f;
    private GameObject eventSphere;
    public float playSpeed = 0.01f;
    private float eventStartTime = 0.0f;
    private float eventEndTime = 0.0f;
    private float playStartTime = 0.0f;
    private float playEndTime = 0.0f;
    private bool advancedIndex = false;

    public struct EventData
    {
        public int str;
        public int dom;
        public Vector3 pos;
        public float charge;
        public float time;
    };

    public List<EventData> eventData;

    public static int IntParseFast(string value)
    {
        int result = 0;
        for (int i = 0; i < value.Length; i++)
        {
            char letter = value[i];
            result = 10 * result + (letter - 48);
        }
        return result;
    }

	// Use this for initialization
	void Start () {

        eventData = new List<EventData>();

        StreamReader sr = new StreamReader(EventFile, Encoding.Default);
        string s = sr.ReadLine();
        int lineCount = 0;
        while(s != null)
        {
            if(lineCount < 2)
            {
                string[] data = s.Split(' ');
                float x = (float)double.Parse(data[0]);
                float y = (float)double.Parse(data[1]);
                float z = (float)double.Parse(data[2]);
            }
            else 
            {
                string[] data = s.Split('\t');
                EventData d;
                d.str = IntParseFast(data[0]);
                d.dom = IntParseFast(data[1]);
                d.pos.x = (float)double.Parse(data[2]);
                d.pos.y = BELOW_ICE + (float)double.Parse(data[4]);
                d.pos.z = (float)double.Parse(data[3]);
                d.charge = (float)double.Parse(data[5]);
                d.time = (float)double.Parse(data[6]);
                eventData.Add(d);
            }

            lineCount++;
            s = sr.ReadLine();
        }

        eventData.Sort((s1, s2) => s1.time.CompareTo(s2.time));
	}
	
	// Update is called once per frame
	void Update () {
        
        float t = UnityEngine.Time.time;

        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R))
        {
            Debug.Log("Playing event!");
            eventPlaying = true;
            currIndex = 0;
            eventStartTime = eventData[0].time;
            playStartTime = t;
            eventEndTime = eventData[eventData.Count - 1].time;
            advancedIndex = true;

            if (transform.childCount > 0)
            {
                Transform rc = transform.GetChild(0);
                while (transform.childCount > 0)
                {
                    rc.parent = null;
                    Destroy(rc.gameObject);
                    if (transform.childCount > 0)
                    {
                        rc = transform.GetChild(0);
                    }
                }
            }
        }

        
	    if(eventPlaying)
        {
            if (currIndex < eventData.Count && advancedIndex)
            {
                eventSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //eventSphere = (GameObject)Instantiate(DOMTest);
                eventSphere.transform.position = eventData[currIndex].pos;
                eventSphere.transform.SetParent(transform);
                float sphereRadius = Mathf.Log(1000.0f * eventData[currIndex].charge * eventData[currIndex].charge);
                //float sphereRadius = (eventData[currIndex].charge * eventData[currIndex].charge);
                eventSphere.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
                float fTimeFrac = (eventData[currIndex].time - eventStartTime) / (eventEndTime - eventStartTime);
                float fColorFrac = 1.0f / 7.0f;

                if (fTimeFrac < fColorFrac)
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.red;
                }
                else if (fTimeFrac < 2.0f * fColorFrac)
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(1.0f, 0.5f, 0.0f, 1.0f);
                }
                else if (fTimeFrac < 3.0f * fColorFrac)
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.yellow;
                }
                else if (fTimeFrac < 4.0f * fColorFrac)
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.green;
                }
                else if (fTimeFrac < 5.0f * fColorFrac)
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.blue;
                }
                else if (fTimeFrac < 6.0f * fColorFrac)
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = UnityEngine.Color.magenta;
                }
                else
                {
                    eventSphere.GetComponent<MeshRenderer>().material.color = new UnityEngine.Color(0.5f, 0.0f, 1.0f, 1.0f);
                }
            }
            
            //advance index depending on timing...
            if (currIndex < eventData.Count - 1)
            {
                if ((eventData[currIndex + 1].time - eventStartTime) > (t - playStartTime) * playSpeed)
                {
                    currIndex++;
                }
                else
                {
                    //spin the existing spheres?
                    //fade out option?
                    advancedIndex = false;
                }
            }
            
            if(currIndex >= eventData.Count-1)
            {
                Debug.Log("Stopped playing");
                currIndex = 0;
                eventPlaying = false;
                advancedIndex = false;
                playStartTime = 0.0f;
                playEndTime = 0.0f;
                eventStartTime = 0.0f;
                eventEndTime = 0.0f;
            }
        }
	}
}
