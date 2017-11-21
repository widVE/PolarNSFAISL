using UnityEngine;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;

public class EventPlayer : MonoBehaviour {

    public GameObject helpSwipe;
    public GameObject swipeGameMode;
    public GameObject tutorial;
    public string eventDirectory;
    public string newEventFile;
    public int newEventCutoff;
    public GameObject particle;   //used for debugging trajectory for now
    public GameObject sparks;
    List<GameObject> sparkList = new List<GameObject>();

    public float playSpeed = 0.01f;
    public float totalEnergy = 0.0f;
    public float secondsBeforeHelp = 10.0f;
    public float fadeTime = 3.0f;
    public float secondsBeforeDissappear = 10.0f;
    public int lastEventNumber = -1;
    public bool keepPlaying = false;

	private AudioSource alarm;
    private const float BELOW_ICE = -1950.0f;
    private float lastPlayTime = 0.0f;
    private float fadeStart = -1.0f;
	private DomArrayGenerator arrayGenerator;
	private int currEventNumber = -1;
    private float eventFrequency = 10.0f;
	private bool donePlaying = false;
    private bool firstPlay = true;
    private bool beginFade = false;
    private bool alreadyFaded = false;
	private bool isSwiped = false;
    private bool playingTutorial = true;
	private float timer = 2.0f;
    private int incrEventNumber = 5;

    public LineRenderer truePath;
    public GameObject lineGrapher;

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
        public Vector3 middlePos;
        public float theta;
        public float phi;
        public GameObject eventSource;
    };

	public struct DomState
	{
		public GameObject Dom;
		public float charge;
		public float timeFrac;
	};

    public struct EventPlayback
    {
        public bool isPlaying;
        public bool isDetected;
        public bool advancedIndex;
        public int eventIndex;
        public int eventStartFrame;
        public int eventEndFrame;
        public float eventStartTime;
        public float eventEndTime;
        public float playStartTime;
        public float newPlayTime;
		//public List<DomState> ActivatedDoms;
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

    private Vector3 SphericalToCartesian(float radius, float polar, float elevation)
    {
        Vector3 outCart = Vector3.zero;
        //float a = radius * Mathf.Sin(elevation);
        outCart.x = radius * Mathf.Cos(polar) * Mathf.Sin(elevation);
        outCart.y = radius * Mathf.Cos(elevation);
        outCart.z = radius * Mathf.Sin(polar) * Mathf.Sin(elevation);
        //Debug.Log(outCart.ToString("F4"));
        return outCart.normalized;
    }

	// Use this for initialization
	void Start () {

		arrayGenerator = GetComponent<DomArrayGenerator> ();
		//alarm = GameObject.Find("Sound Effects").GetComponents<AudioSource> () [3];

        if(eventDirectory.Length > 0)
        {
            string[] files = System.IO.Directory.GetFiles(eventDirectory);
            GameObject[] sources = GameObject.FindGameObjectsWithTag("NeutrinoSource");
            int numSources = sources.Length;
            Debug.Log("Num sources: " + numSources);
            int fileNum = 0;
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
								//Debug.LogWarning ("START SET");
                                e.startPos = new Vector3(x * 0.3048f, y * 0.3048f, z * 0.3048f);
                            }
                            else 
                            {
								//Debug.LogWarning ("END SET");
                                e.endPos = new Vector3(x * 0.3048f, y * 0.3048f, z * 0.3048f);
                            }

                            if(events.Count == 0)
                            {
                                //for tutorial - make start and end further apart.
                                Vector3 diff = e.startPos - e.endPos;
                                diff.Normalize();
                                e.startPos = e.startPos + diff * 1000.0f;
                                e.endPos = e.endPos - diff * 1000.0f;
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
                        e.eventSource = sources[fileNum];//sources[UnityEngine.Random.Range(0, numSources - 1)];
                    }
                    events.Add(e);
                    fileNum++;
                }
            }
        }

        if(newEventFile.Length > 0)
        {
            StreamReader sr = new StreamReader(newEventFile, Encoding.Default);
            string s = sr.ReadLine();

            bool first = true;

            GameObject[] sources = GameObject.FindGameObjectsWithTag("NeutrinoSource");
            int numSources = sources.Length;
            List<EventData> ed = new List<EventData>();
            float lastTheta = 0.0f;
            float lastPhi = 0.0f;
            Vector3 lastCenter = Vector3.zero;
            while (s != null)
            {
                if(s[0] != '#')
                {
                    string[] data = s.Split(' ');
                    if(data.Length == 2)
                    {
                        //theta, phi
                        lastTheta = (float)double.Parse(data[0]);
                        lastPhi = (float)double.Parse(data[1]);
                        //todo - derive a start and end point from theta and phi
                    }
                    else if(data.Length == 5)
                    {
                        lastTheta = (float)double.Parse(data[0]);
                        lastPhi = (float)double.Parse(data[1]);
                        lastCenter.x = (float)double.Parse(data[2]);
                        lastCenter.y = BELOW_ICE + (float)double.Parse(data[4]);
                        lastCenter.z = (float)double.Parse(data[3]);
                    }
                    else if(data.Length == 7)
                    { 
                        //string, dom, x, y, z, time, charge
                        EventData d;
                        d.dom = IntParseFast(data[0]) - 1;
                        d.str = IntParseFast(data[1]) - 1;
                        d.pos.x = (float)double.Parse(data[2]);
                        d.pos.y = BELOW_ICE + (float)double.Parse(data[4]);
                        d.pos.z = (float)double.Parse(data[3]);
                        d.charge = (float)double.Parse(data[6]);
                        d.time = (float)double.Parse(data[5]);
                        ed.Add(d);
                    }
                }
                else
                {
                    if(s[1] == 'e')
                    {
                        //new event...
                        //store old event...
                        if (!first)
                        {
                            if (ed.Count < newEventCutoff)
                            {
                                EventVis e = new EventVis();
                                e.eventData = new List<EventData>(ed);
                                e.theta = lastTheta;
                                e.phi = lastPhi;

                                Vector3 dir = SphericalToCartesian(1.0f, lastPhi, lastTheta);

                                /*Vector3 avgPos = UnityEngine.Vector3.zero;
                                for (int i = 0; i < ed.Count; ++i)
                                {
                                    avgPos += ed[i].pos;

                                }
                                avgPos /= (float)ed.Count;
                                UnityEngine.Bounds b = new UnityEngine.Bounds(avgPos, new Vector3(1.0f, 1.0f, 1.0f));
                                for (int i = 0; i < ed.Count; ++i)
                                {
                                    b.Encapsulate(ed[i].pos);
                                }*/

                                e.middlePos = lastCenter;
                                e.startPos = e.middlePos + dir * 1000f;// b.extents.magnitude;
                                e.endPos = e.middlePos - dir * 1000f;// b.extents.magnitude;

                                e.eventData.Sort((s1, s2) => s1.time.CompareTo(s2.time));
                                if (sources.Length > 0)
                                {
                                    e.eventSource = sources[UnityEngine.Random.Range(0, numSources - 1)];
                                }
                                events.Add(e);
                            }

                            ed.Clear();
                        }
                        first = false;
                    }
                }
                s = sr.ReadLine();
            }
        }

        if(events.Count > 0)
        {
            Debug.Log("Total Events: " + events.Count);
            eventsPlaying = new EventPlayback[events.Count];
            for(int e = 0; e < events.Count; ++e)
            {
                eventsPlaying[e].isPlaying = false;
                eventsPlaying[e].isDetected = false;
                eventsPlaying[e].eventIndex = -1;
                eventsPlaying[e].eventStartTime = 0.0f;
                eventsPlaying[e].eventEndTime = 0.0f;
                eventsPlaying[e].playStartTime = 0.0f;
                eventsPlaying[e].advancedIndex = false;
                eventsPlaying[e].newPlayTime = 0.0f;
				//eventsPlaying [e].ActivatedDoms = new List<DomState> ();
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

    public void PlayTutorialEvent()
    {
        totalEnergy = 0.0f;

        if (currEventNumber != -1)
        {
            StopPlaying(currEventNumber);
        }
        playingTutorial = true;
        firstPlay = true;
        currEventNumber = -1;
    }

    public void StopTutorialEvent()
    {
        totalEnergy = 0.0f;
        playingTutorial = false;
        firstPlay = true;
        donePlaying = false;
        currEventNumber = -1;
        lastPlayTime = -eventFrequency; //this forces an event to play right away.
        //Debug.Log(UnityEngine.Time.time);
    }

	// Update is called once per frame
	void Update () {

        if(!swipeGameMode.GetComponent<SwipeGameMode>().isGamePlaying() && !playingTutorial)
        {
            return;
        }
        
        for (int j = 0; j < sparkList.Count; ++j)
        {
            GameObject s = sparkList[j];
            sparkList[j].transform.Translate(0f, 20f, 0f);
            //-1127 is y value of ground - add this as a reference..
            if(sparkList[j].transform.position.y > -47f)
            {
                sparkList.RemoveAt(j);
                DestroyObject(s);
            }
        }

        if (donePlaying && !isSwiped)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = 2.0f;
                StopPlaying(currEventNumber);
                //set this back to -1 as we don't necessarily want to replay the same event if it wasn't swiped..
                lastEventNumber = currEventNumber;
                currEventNumber = -1;
                donePlaying = false;
            }
        }

		if (!keepPlaying) {
			return;
		}

        float t = UnityEngine.Time.time;
        
        totalEnergy = 0.0f;
        //r or every eventFrequency seconds
		if ((t - lastPlayTime) > eventFrequency && !IsEventPlaying())
        {
            if (firstPlay || (t - lastPlayTime) > eventFrequency + secondsBeforeHelp + secondsBeforeDissappear)
            {
                //Debug.Log("Playing new");
                firstPlay = false;

                if (playingTutorial)
                {
                    //always just play first event when not in game mode.
                    currEventNumber = 0;
                    lastEventNumber = currEventNumber;
                }
                else
                {
                    if (currEventNumber == -1)
                    {
                        currEventNumber = UnityEngine.Random.Range(0, events.Count);// incrEventNumber;
                        if (swipeGameMode.GetComponent<SwipeGameMode>().isSoftTutorial())
                        {
                            currEventNumber = 0;
                        }
                        
                        Debug.Log("Playing event: " + currEventNumber);
                        if(lineGrapher != null)
                        {
                            lineGrapher.GetComponent<LineEnergyGrapher>().ResetEnergy();
                        }

                        lastEventNumber = currEventNumber;
                        incrEventNumber++;
                    }
                }

                lastPlayTime = t;

                //todo - don't allow same event to replay until it's done...
                eventsPlaying[currEventNumber].newPlayTime = t;
                eventsPlaying[currEventNumber].eventStartTime = events[currEventNumber].eventData[0].time;
                eventsPlaying[currEventNumber].eventStartFrame = UnityEngine.Time.frameCount;

                eventsPlaying[currEventNumber].eventEndFrame = UnityEngine.Time.frameCount + (int)((float)events[currEventNumber].eventData.Count / playSpeed);

                eventsPlaying[currEventNumber].playStartTime = t;
                eventsPlaying[currEventNumber].eventEndTime = events[currEventNumber].eventData[events[currEventNumber].eventData.Count - 1].time;
                eventsPlaying[currEventNumber].advancedIndex = true;
                eventsPlaying[currEventNumber].eventIndex = 0;
                eventsPlaying[currEventNumber].isPlaying = true;
                eventsPlaying[currEventNumber].isDetected = false;
                
                if (alarm != null && alarm.isActiveAndEnabled)
                {
                    alarm.Play();
                }

                if (truePath != null)
                {
                    truePath.SetPosition(0, events[currEventNumber].startPos);
                    truePath.SetPosition(1, events[currEventNumber].endPos);
                }

                if(particle != null)
                {
                    /*if ((swipeGameMode.GetComponent<SwipeGameMode>().isSoftTutorial()))
                    {
                        particle.GetComponent<SpawnParticle>().particlePrefab.transform.GetChild(0).gameObject.SetActive(true);
                    }
                    else
                    {
                        particle.GetComponent<SpawnParticle>().particlePrefab.transform.GetChild(0).gameObject.SetActive(false);
                    }*/

                    if (swipeGameMode.GetComponent<SwipeGameMode>().isSoftTutorial())
                    {
                        particle.GetComponent<SpawnParticle>().startThrowing();
                        //Debug.Log("Throwing particle");
                    }
                }
            }
        }

	    if(IsEventPlaying())
        {
			//todo - to handle sped up play back, need to potentially loop ahead here, until we calculate a frame count beyond the current...
			if (eventsPlaying[currEventNumber].eventIndex < events[currEventNumber].eventData.Count && eventsPlaying[currEventNumber].advancedIndex)
			{
				GameObject d = arrayGenerator.DOMArray[events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].dom, events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].str];

				float fTimeFrac = 0.0f;
				if (d != null)
				{
					totalEnergy += events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].charge;

					//fTimeFrac = (events[e].eventData[eventsPlaying[e].eventIndex].time - eventsPlaying[e].eventStartTime) / (eventsPlaying[e].eventEndTime - eventsPlaying[e].eventStartTime);
					//Ross - changed coloring to just be always rainbow, not dependent on time stamps..
					fTimeFrac = (float)eventsPlaying[currEventNumber].eventIndex / (float)events[currEventNumber].eventData.Count;
					DOMController dc = d.GetComponent<DOMController>();
					if (dc != null)
					{
						float charge = Mathf.Log (60000.0f * events [currEventNumber].eventData [eventsPlaying [currEventNumber].eventIndex].charge * events [currEventNumber].eventData [eventsPlaying [currEventNumber].eventIndex].charge);
                        //Debug.Log(eventsPlaying[currEventNumber].eventIndex);
                        dc.TurnOn(fTimeFrac, charge);
						//DomState toAdd = new DomState ();
						//toAdd.charge = charge;
						//toAdd.timeFrac = fTimeFrac;
				
						//eventsPlaying [currEventNumber].ActivatedDoms.Add (toAdd);

                        if (sparks != null)
                        {
                            GameObject spark = Instantiate(sparks, transform);
                            spark.transform.position = d.transform.position;
                            spark.GetComponent<ParticleSystem>().Play();
                            sparkList.Add(spark);
                        }

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
			if (eventsPlaying[currEventNumber].eventIndex < events[currEventNumber].eventData.Count - 1)
			{
				//time scale here is probably off...
				//these time values are in nanoseconds, so really huge, so this will probably be true every frame...
				//instead let's do this based on frame count..
				//given the event's time and our frame range, we can figure map the single event's time to which frame it should play...
				//float timeFrac = (events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].time - eventsPlaying[currEventNumber].eventStartTime) / (eventsPlaying[currEventNumber].eventEndTime - eventsPlaying[currEventNumber].eventStartTime);
				//int frameRange = eventsPlaying[currEventNumber].eventEndFrame - eventsPlaying[currEventNumber].eventStartFrame;

				//float frameFrac = (float)eventsPlaying[currEventNumber].eventStartFrame + (timeFrac * (float)frameRange);

				eventsPlaying[currEventNumber].eventIndex++;
				eventsPlaying[currEventNumber].advancedIndex = true;
			}
            else
            {
                if(beginFade)
                {
                    if(fadeStart == -1.0f)
                    {
                        fadeStart = t;
                    }

                    if(t - fadeStart < fadeTime)
                    {
                        //loop through all DOMs...
                        for(int k = 0; k < events[currEventNumber].eventData.Count; ++k)
                        {
                            GameObject d = arrayGenerator.DOMArray[events[currEventNumber].eventData[k].dom, events[currEventNumber].eventData[k].str];
                            d.GetComponent<DOMController>().Fade(1.0f - ((t-fadeStart)/fadeTime));
                        }
                    }
                    else
                    {
                        beginFade = false;
                        alreadyFaded = true;
                        fadeStart = -1.0f;
                    }
                }
            }

			if (eventsPlaying[currEventNumber].eventIndex >= events[currEventNumber].eventData.Count - 1)
			{
                if(swipeGameMode.GetComponent<SwipeGameMode>().isSoftTutorial())
                    swipeGameMode.GetComponent<SwipeGameMode>().softTutorialText.GetComponent<UnityEngine.UI.Text>().text = "Swipe in the direction of the event.";

                if (!playingTutorial && !swipeGameMode.GetComponent<SwipeGameMode>().isSoftTutorial())
                {
                    if ((t - lastPlayTime) > eventFrequency + secondsBeforeHelp + secondsBeforeDissappear - fadeTime)
                    {
                        if (!alreadyFaded)
                        {
                            beginFade = true;
                        }
                    }
                    //Debug.Log(eventFrequency + secondsBeforeHelp);
                    if ((t - lastPlayTime) > eventFrequency + secondsBeforeHelp)
                    {
                        if (helpSwipe != null && !playingTutorial)
                        {
                            /*float pressTime = helpSwipe.GetComponent<LiveHelpTimer>().pressTime;
                            if (t - pressTime > secondsBeforeHelp)
                            {
                                Vector3 diff = (events[currEventNumber].startPos - events[currEventNumber].endPos).normalized;
                                //helpSwipe.transform.position = (events[currEventNumber].startPos + events[currEventNumber].endPos) / 2;
                                float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
                                //helpSwipe.transform.localRotation = Quaternion.Euler(helpSwipe.transform.localRotation.x, helpSwipe.transform.localRotation.y, angle);
                                helpSwipe.SetActive(true);
                                //TODO: set 2d rotation of helpSwipe to match current event
                            }
                            else
                            {
                                helpSwipe.SetActive(false);
                            }*/
                        }
                    }

                    // Event done playing
                    if ((t - lastPlayTime) > eventFrequency + secondsBeforeHelp + secondsBeforeDissappear)
                    {
                        donePlaying = true;
                        //also need to deactivate if successful swipe occurs...
                        /*if (helpSwipe != null)
                        {
                            helpSwipe.SetActive(false);
                        }*/
                    }
                }
			}
        }
	}

    public void StopCurrentEvent()
    {
        //Debug.Log("Stopping event" + currEventNumber);
        StopPlaying(currEventNumber);
    }

    void StopPlaying(int e)
    {
        //Debug.Log("Stopped playing" + e);
        if (e != -1)
        {
            eventsPlaying[e].eventIndex = 0;
            eventsPlaying[e].isPlaying = false;
            eventsPlaying[e].isDetected = false;
            eventsPlaying[e].advancedIndex = false;
            eventsPlaying[e].playStartTime = 0.0f;
            eventsPlaying[e].eventStartTime = 0.0f;
            eventsPlaying[e].eventEndTime = 0.0f;
            eventsPlaying[e].eventStartFrame = 0;
            eventsPlaying[e].eventEndFrame = 0;
            //eventsPlaying[e].ActivatedDoms = new List<DomState>();

            alreadyFaded = false;
            //turn off all event visualization - 11/16/2017 - Ross - yes, since we aren't doing multiple events anyways
            //fixes a random null reference that could occur in commented out code below.
            /*for (int i = 0; i < events[e].eventData.Count; ++i)
            {
                float lastTime = Time.time;
                //Debug.Log(lastTime);
                //while (Mathf.Abs(Time.time - lastTime) < 100)
                //{
                //    Debug.Log("x");
                //}

                GameObject d = arrayGenerator.DOMArray[events[e].eventData[i].dom, events[e].eventData[i].str];
                if (d != null)
                {
                    d.GetComponent<DOMController>().TurnOff();
                }
            }*/

            GameObject da = GameObject.Find("DomArray");
            for (int i = 0; i < DomArrayGenerator.NUM_STRINGS; ++i)
            {
                for (int j = 0; j < DomArrayGenerator.NUM_DOMS_PER_STRING; ++j)
                {
                    GameObject d = arrayGenerator.DOMArray[i, j];
                    if (d != null)
                    {
                        DOMController dc = d.GetComponent<DOMController>();
                        if (dc != null)
                        {
                            dc.GetComponent<DOMController>().TurnOff();
                        }
                    }
                }
            }

        }
    }

    public void scaleArray(float fScale)
    {
        GameObject da = GameObject.Find("DomArray");
        for(int i = 0; i < DomArrayGenerator.NUM_STRINGS; ++i)
        {
            for(int j = 0; j < DomArrayGenerator.NUM_DOMS_PER_STRING; ++j)
            {
                GameObject d = arrayGenerator.DOMArray[i, j];
                if (d != null)
                {
                    DOMController dc = d.GetComponent<DOMController>();
                    if (dc != null && !dc.on)
                    {
                        Vector3 v = d.transform.localScale;
                        v.Set(fScale, fScale, fScale);
                        d.transform.localScale = v;
                    }
                }
            }
        }
    }

	private IEnumerator DelayedReset(float waittime, int e) {
		yield return new WaitForSeconds (waittime);
		StopPlaying (e);
	}

	public float getEnergy() {
		return totalEnergy;
	}

	public void FreezePlaying(bool forceAllVisible=false) {
		timer = 2.0f;
		isSwiped = true;
		keepPlaying = false;
		FinishEvent (forceAllVisible);
	}

	public void ResumePlaying() {
        //Debug.Log("Resuming");
		keepPlaying = true;
		isSwiped = false;
		StopPlaying (currEventNumber);
        lastEventNumber = currEventNumber;
		currEventNumber = -1;
        lastPlayTime = -eventFrequency;
	}

    public float GetCurrentEnergy()
    {
        if (currEventNumber != -1)
        {
            return events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].charge;
        }
        else
        {
            return 0f;
        }
    }

    public int GetCurrentDOM()
    {
        if (currEventNumber != -1)
        {
            return eventsPlaying[currEventNumber].eventIndex;
        }
        else 
        {
            return 0;
        }
    }

    public float GetTotalDOMs()
    {
        if (currEventNumber != -1)
        {
            return (float)events[currEventNumber].eventData.Count;
        }
        else 
        {
            return 0f;
        }
    }

    /**
     * Gets the maximum length out of all events listed
     */
    public float GetMaxDOMs()
    {
        float total = 0f;
        for (int i = 0; i < events.Count; i++)
        {
            if (events[i].eventData.Count > total)
            {
                total = events[i].eventData.Count;
            }
        }
        return total;
    }

    public float GetTotalEventEnergy()
    {
        float maxEnergy = 0f;
        for (int i = 0; i < events.Count; i++)
        {
            float totalEnergy = 0f;
            for (int j = 0; j < events[i].eventData.Count; j++)
            {
                totalEnergy += events[i].eventData[j].charge;
            }

            if(maxEnergy < totalEnergy)
            {
                maxEnergy = totalEnergy;
            }
        }
        Debug.Log(maxEnergy);
        return maxEnergy;
    }

    private void FinishEvent(bool forceAllVisible) {

        if (!forceAllVisible)
        {
            while (eventsPlaying[currEventNumber].eventIndex < events[currEventNumber].eventData.Count - 1)
            {
                //todo - to handle sped up play back, need to potentially loop ahead here, until we calculate a frame count beyond the current...
                if (eventsPlaying[currEventNumber].eventIndex < events[currEventNumber].eventData.Count && eventsPlaying[currEventNumber].advancedIndex)
                {
                    GameObject d = arrayGenerator.DOMArray[events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].dom, events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].str];

                    float fTimeFrac = 0.0f;
                    if (d != null)
                    {
                        totalEnergy += events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].charge;

                        //fTimeFrac = (events[e].eventData[eventsPlaying[e].eventIndex].time - eventsPlaying[e].eventStartTime) / (eventsPlaying[e].eventEndTime - eventsPlaying[e].eventStartTime);
                        //Ross - changed coloring to just be always rainbow, not dependent on time stamps..
                        fTimeFrac = (float)eventsPlaying[currEventNumber].eventIndex / (float)events[currEventNumber].eventData.Count;
                        DOMController dc = d.GetComponent<DOMController>();
                        if (dc != null)
                        {
                            float charge = Mathf.Log(60000.0f * events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].charge * events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].charge);
                            dc.TurnOn(fTimeFrac, charge);

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
                if (eventsPlaying[currEventNumber].eventIndex < events[currEventNumber].eventData.Count - 1)
                {
                    //time scale here is probably off...
                    //these time values are in nanoseconds, so really huge, so this will probably be true every frame...
                    //instead let's do this based on frame count..
                    //given the event's time and our frame range, we can figure map the single event's time to which frame it should play...
                    float timeFrac = (events[currEventNumber].eventData[eventsPlaying[currEventNumber].eventIndex].time - eventsPlaying[currEventNumber].eventStartTime) / (eventsPlaying[currEventNumber].eventEndTime - eventsPlaying[currEventNumber].eventStartTime);
                    int frameRange = eventsPlaying[currEventNumber].eventEndFrame - eventsPlaying[currEventNumber].eventStartFrame;

                    float frameFrac = (float)eventsPlaying[currEventNumber].eventStartFrame + (timeFrac * (float)frameRange);

                    eventsPlaying[currEventNumber].eventIndex++;
                    eventsPlaying[currEventNumber].advancedIndex = true;
                }
            }
        }
        else
        {
            int i = 0;
            while (i < events[currEventNumber].eventData.Count)
            {
                GameObject d = arrayGenerator.DOMArray[events[currEventNumber].eventData[i].dom, events[currEventNumber].eventData[i].str];
                float fTimeFrac = (float)i / (float)events[currEventNumber].eventData.Count;
                DOMController dc = d.GetComponent<DOMController>();
                if (dc != null)
                {
                    float charge = Mathf.Log(60000.0f * events[currEventNumber].eventData[i].charge * events[currEventNumber].eventData[i].charge);
                    dc.TurnOn(fTimeFrac, charge);
                }

                i++;
            }
        }
	}

	public Vector3 GetEventCenterpoint(int index=-1) {
		EventVis curr = events [index == -1 ? currEventNumber : 0];
		return ((curr.startPos + curr.endPos) / 2);
	}

    public Bounds GetEventBounds(Vector3 centerPos, int index=-1)
    {
        Bounds b = new Bounds(centerPos, Vector3.zero);
        
        for(int i = 0; i <  events[index == -1 ? currEventNumber : 0].eventData.Count; ++i)
        {
            b.Encapsulate(events[index == -1 ? currEventNumber : 0].eventData[i].pos);
        }

        return b;
    }
}
