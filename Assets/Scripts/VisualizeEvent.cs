using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class VisualizeEvent : MonoBehaviour {

    public string eventDirectory;
    //public GameObject particle;
    public float playSpeed = 0.01f;
    public float eventFrequency = 15.0f;
    public GameObject totalEnergyText = null;

    public float totalEnergy = 0.0f;
    
    private const float BELOW_ICE = -1950.0f;
    private float lastPlayTime = 0.0f;
    private DomData domData;

    public struct EventData
    {
        public int str;
        public int dom;
        public Vector3 pos;
        public float charge;
        public float time;
    };

    public struct EventVis
    {
        public List<EventData> eventData;
        public string fileName;
        public Vector3 startPos;
        public Vector3 endPos;
        public GameObject eventSource;
    };

    public struct EventPlayback
    {
        public bool isPlaying;
        public bool advancedIndex;
        public int eventIndex;
        public int eventStartFrame;
        public int eventEndFrame;
        public float eventStartTime;
        public float eventEndTime;
        public float playStartTime;
        public float newPlayTime;
    };

    public EventPlayback[] eventsPlaying;

    public List<EventVis> events = new List<EventVis>();

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

        if(eventDirectory.Length > 0)
        {
            string[] files = System.IO.Directory.GetFiles(eventDirectory);
            GameObject[] sources = GameObject.FindGameObjectsWithTag("NeutrinoSource");
            int numSources = sources.Length;

            foreach (string file in files)
            {
                if (file.EndsWith(".txt"))
                {
                    EventVis e = new EventVis();
                    e.eventData = new List<EventData>();
                    e.fileName = file;
                    Debug.Log(file);
                    StreamReader sr = new StreamReader(e.fileName, Encoding.Default);
                    string s = sr.ReadLine();
                    int lineCount = 0;
                    while (s != null)
                    {
                        if (lineCount < 2)
                        {
                            string[] data = s.Split(' ');
                            float x = (float)double.Parse(data[0]);
                            float y = (float)double.Parse(data[1]);
                            float z = (float)double.Parse(data[2]);
                            if(lineCount < 1)
                            {
                                e.startPos = new Vector3(x * 0.3048f, y * 0.3048f, z * 0.3048f);
                            }
                            else 
                            {
                                e.endPos = new Vector3(x * 0.3048f, y * 0.3048f, z * 0.3048f);
                            }
                        }
                        else
                        {
                            string[] data = s.Split('\t');
                            if(data.Length != 7)
                            {
                                data = s.Split(' ');
                            }
                            EventData d;
                            d.str = IntParseFast(data[0])-1;
                            d.dom = IntParseFast(data[1])-1;
                            d.pos.x = (float)double.Parse(data[2]);
                            d.pos.y = BELOW_ICE + (float)double.Parse(data[4]);
                            d.pos.z = (float)double.Parse(data[3]);
                            d.charge = (float)double.Parse(data[5]);
                            d.time = (float)double.Parse(data[6]);
                            e.eventData.Add(d);
                        }

                        lineCount++;
                        s = sr.ReadLine();
                    }

                    e.eventData.Sort((s1, s2) => s1.time.CompareTo(s2.time));
                    if (sources.Length > 0)
                    {
                        e.eventSource = sources[UnityEngine.Random.Range(0, numSources - 1)];
                    }
                    events.Add(e);
                }
            }

            if(events.Count > 0)
            {
                eventsPlaying = new EventPlayback[events.Count];
                for(int e = 0; e < events.Count; ++e)
                {
                    eventsPlaying[e].isPlaying = false;
                    eventsPlaying[e].eventIndex = -1;
                    eventsPlaying[e].eventStartTime = 0.0f;
                    eventsPlaying[e].eventEndTime = 0.0f;
                    eventsPlaying[e].playStartTime = 0.0f;
                    eventsPlaying[e].advancedIndex = false;
                    eventsPlaying[e].newPlayTime = 0.0f;
                }
            }
        }
	}
	
    public bool IsEventPlaying()
    {
        for (int i = 0; i < eventsPlaying.Length; ++i)
        {
            if(eventsPlaying[i].isPlaying)
            {
                return true;
            }
        }
        
        return false;
    }

	// Update is called once per frame
	void Update () {
        
        float t = UnityEngine.Time.time;

        totalEnergy = 0.0f;

        //r or every 60 seconds
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.R) || t - lastPlayTime > eventFrequency)
        {
            Debug.Log("Playing event!");
			lastPlayTime = t;
            
			//todo - don't allow same event to replay until it's done...
            int currEvent = UnityEngine.Random.Range(0, events.Count);

            eventsPlaying[currEvent].newPlayTime = t;
            eventsPlaying[currEvent].eventStartTime = events[currEvent].eventData[0].time;
            eventsPlaying[currEvent].eventStartFrame = UnityEngine.Time.frameCount;
            Debug.Log("Start Frame:" + eventsPlaying[currEvent].eventStartFrame);
            
            eventsPlaying[currEvent].eventEndFrame = UnityEngine.Time.frameCount + (int)((float)events[currEvent].eventData.Count / playSpeed);
            Debug.Log("End Frame:" + eventsPlaying[currEvent].eventEndFrame);

            eventsPlaying[currEvent].playStartTime = t;
            eventsPlaying[currEvent].eventEndTime = events[currEvent].eventData[events[currEvent].eventData.Count - 1].time;
            eventsPlaying[currEvent].advancedIndex = true;
            eventsPlaying[currEvent].eventIndex = 0;
            eventsPlaying[currEvent].isPlaying = true;

            Debug.Log("Source: " + events[currEvent].eventSource.name);
        }

	    if(IsEventPlaying())
        {
            for (int e = 0; e < eventsPlaying.Length; ++e)
            {
                if (eventsPlaying[e].isPlaying)
                {
                    //todo - to handle sped up play back, need to potentially loop ahead here, until we calculate a frame count beyond the current...
                    if (eventsPlaying[e].eventIndex < events[e].eventData.Count && eventsPlaying[e].advancedIndex)
                    {
                        if (domData == null)
                        {
                            domData = gameObject.GetComponent<DomData>();
                        }

                        GameObject d = domData.DOMArray[events[e].eventData[eventsPlaying[e].eventIndex].dom, events[e].eventData[eventsPlaying[e].eventIndex].str];

                        float fTimeFrac = 0.0f;
                        if (d != null)
                        {
                            totalEnergy += events[e].eventData[eventsPlaying[e].eventIndex].charge;
                            totalEnergyText.GetComponent<UnityEngine.UI.Text>().text = "Total Energy: " + totalEnergy;
                            fTimeFrac = (events[e].eventData[eventsPlaying[e].eventIndex].time - eventsPlaying[e].eventStartTime) / (eventsPlaying[e].eventEndTime - eventsPlaying[e].eventStartTime);
                            DOMController dc = d.GetComponent<DOMController>();
                            if (dc != null)
                            {
                                dc.TurnOn(fTimeFrac, Mathf.Log(20000.0f * events[e].eventData[eventsPlaying[e].eventIndex].charge * events[e].eventData[eventsPlaying[e].eventIndex].charge));
                                AudioSource asource = dc.GetComponent<AudioSource>();
                                if (asource != null && asource.isActiveAndEnabled)
                                {
                                    asource.Play();
                                }
                            }
                        }

                        //Vector3 dir = (events[e].endPos - events[e].startPos);
                        //float mag = (events[e].endPos - events[e].startPos).magnitude;
                        //particle.transform.position = events[e].startPos + dir * fTimeFrac;
                    }

                    //advance index depending on timing...
                    if (eventsPlaying[e].eventIndex < events[e].eventData.Count - 1)
                    {
                        //time scale here is probably off...
                        //these time values are in nanoseconds, so really huge, so this will probably be true every frame...
                        //instead let's do this based on frame count..
                        //given the event's time and our frame range, we can figure map the single event's time to which frame it should play...
                        float timeFrac = (events[e].eventData[eventsPlaying[e].eventIndex].time - eventsPlaying[e].eventStartTime) / (eventsPlaying[e].eventEndTime - eventsPlaying[e].eventStartTime);
                        int frameRange = eventsPlaying[e].eventEndFrame - eventsPlaying[e].eventStartFrame;
                        //Debug.Log("Range: " + frameRange);
                        float frameFrac = (float)eventsPlaying[e].eventStartFrame + (timeFrac * (float)frameRange);
                        //Debug.Log("Fraction: " + frameFrac);
                        //Debug.Log("Frame Count: " + UnityEngine.Time.frameCount);
                        //Debug.Log("Event Index: " + eventsPlaying[e].eventIndex);
                        //need to double check this...
                        if (frameFrac < (float)UnityEngine.Time.frameCount)
                        {
                            eventsPlaying[e].eventIndex++;
                            eventsPlaying[e].advancedIndex = true;
                        }
                        else if(UnityEngine.Time.frameCount > eventsPlaying[e].eventEndFrame)
                        {
                            StopPlaying(e);
                            //eventsPlaying[e].eventIndex++;
                        }
                        else
                        {
                            //spin the existing spheres?
                            //fade out option?
                            eventsPlaying[e].advancedIndex = false;
                        }
                    }

                    if (eventsPlaying[e].eventIndex >= events[e].eventData.Count - 1)
                    {
                        StopPlaying(e);
                    }
                }
            }
        }
	}

    void StopPlaying(int e)
    {
        Debug.Log("Stopped playing");
        eventsPlaying[e].eventIndex = 0;
        eventsPlaying[e].isPlaying = false;
        eventsPlaying[e].advancedIndex = false;
        eventsPlaying[e].playStartTime = 0.0f;
        eventsPlaying[e].eventStartTime = 0.0f;
        eventsPlaying[e].eventEndTime = 0.0f;
        eventsPlaying[e].eventStartFrame = 0;
        eventsPlaying[e].eventEndFrame = 0;

        //turn off all event visualization?
        for(int i = 0; i < events[e].eventData.Count; ++i)
        {
            GameObject d = domData.DOMArray[events[e].eventData[i].dom, events[e].eventData[i].str];
            if(d != null)
            {
                d.GetComponent<DOMController>().TurnOff();
            }
        }
    }
}
