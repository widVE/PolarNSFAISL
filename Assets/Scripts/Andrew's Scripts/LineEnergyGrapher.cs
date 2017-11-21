using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineEnergyGrapher : MonoBehaviour {

	private LineRenderer linRen;
	private Vector3[] points;

    //this should be max # of DOMs
    public const int NUM_LINE_POINTS = 1000;

    public enum MODE {
        INCREMENT, // Stacks energy levels at every index on new event
        OVERWRITE, // Starts new graph on new event
        ACCUMULATE
    };

    [Tooltip("The height of the energy graph in pixels")]
    public float graphHeight = 425f;
    public float graphWidth = 375f;
    [Tooltip("The mode to display energy: " 
        + "\n- INCREMENT: Stack energy levels at every point on every event"
        + "\n- OVERWRITE: Discard previous graph and generate new one for new event")]
    public MODE displayMode = MODE.INCREMENT;

	[SerializeField]
    [Tooltip("Currently not supported")]
    private bool randomizeData = false;

    private float maxEnergy = 0f;
    private const float START_ENERGY = 0.00000001f;
    private int prevIdx;
    private int prevEvent = 0;
    private float totalEventEnergy = 0f;

	private EventPlayer visEvent;
	// Use this for initialization
	void Start () {
        
        maxEnergy = START_ENERGY;
        prevIdx = -1;

        GameObject array = GameObject.Find ("DomArray");
		if (array != null) {
			visEvent = array.GetComponent<EventPlayer>();
		}
			
		linRen = GetComponent<LineRenderer> ();
		linRen.material = new Material (Shader.Find ("Particles/Additive"));
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePoints ();
        if (points != null)
        {
            linRen.SetPositions(points);
        }
	}

    /** 
     * Initializes the points to be displayed on the energy graph.
     * points length initialized to the maximum event length.
     */
	public void InitializePoints() {
        // Make array of size = max event length
        points = new Vector3[(int)visEvent.GetMaxDOMs()];
        // initialize point values
        for (int i = 0; i < points.Length; i++)
        {
            // Set X distance between adjacent points
            float x = (i * 5f);
            // Set initial energy (Y) to 0
            // Set Z = 0 because graph is relative to the GameObject with this component
            points[i] = new Vector3(x, 0f, 0f);
        }
	}

    public void ResetEnergy()
    {
        maxEnergy = 0f;
        if (points != null)
        {
            for (int i = 0; i < points.Length; i++)
            {
                // Set X distance between adjacent points
                float x = (i * 5f);
                // Set initial energy (Y) to 0
                // Set Z = 0 because graph is relative to the GameObject with this component
                points[i] = new Vector3(x, 0f, 0f);
            }
        }
    }

	private void UpdatePoints() {
        // Debug.Log(visEvent.GetTotalDOMs());
        // Initialize points if haven't done so
        if((points == null || points.Length == 0) && visEvent.GetTotalDOMs() > 0)
        {
            InitializePoints();
        }

        if(totalEventEnergy == 0f)
        {
            totalEventEnergy = visEvent.GetTotalEventEnergy();
        }

        // Get current event index
        int idx = visEvent.GetCurrentDOM();
        // Do not update graph if for some reason it tries to update the 
        // same index more than once in sequence.
        if (visEvent.GetTotalDOMs() > 0 && idx != prevIdx)
        {
            prevIdx = idx; // For checking purposes
            // Get new energy level
            float energy = visEvent.GetCurrentEnergy();
            if (displayMode == MODE.ACCUMULATE)
            {
                maxEnergy += energy;
            }
            else
            {
                if (energy > maxEnergy)
                {
                    // Normalize graph again if new energy level exceeds the current maximum
                    // Debug.Log("New max energy = " + energy);
                    for (int i = 0; i < points.Length; i++)
                    {
                        // Get raw value
                        float rawEnergy = points[i].y * maxEnergy;
                        // Normalize it
                        points[i].y = rawEnergy / energy;
                    }
                    maxEnergy = energy; // Update maximum energy level
                }
            }

            // Update graph base on displayMode
            switch (displayMode)
            {
                case MODE.INCREMENT: // Stacks energy levels at every index on new event
                    points[idx].y = (points[idx].y * maxEnergy + graphHeight * energy) / maxEnergy;
                    break;
                case MODE.OVERWRITE: // Starts new graph on new event
                    if (idx == 0)
                    {
                        maxEnergy = (energy > START_ENERGY) ? energy : START_ENERGY;
                    }
                    points[idx].y = (graphHeight * energy) / maxEnergy;
                    break;
                case MODE.ACCUMULATE:
                    if (idx == 0)
                    {
                        maxEnergy = (energy > START_ENERGY) ? energy : START_ENERGY;
                    }
                    points[idx].y = ((maxEnergy / totalEventEnergy) * graphHeight);
                    break;
                default:
                    // Debug.Log("Something went wrong, displayMode is invalid: " + displayMode);
                    points[idx].y = 0f;
                    break;
            }
            
        }
	}
}
