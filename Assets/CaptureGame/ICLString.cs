using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ICLString : MonoBehaviour {

    public GameObject dom;
    public float stringDepth;
    public List<GameObject> domList;
    LineRenderer l = null;

	// Use this for initialization
	void Start () {
        domList = new List<GameObject>();
        stringDepth = 1.0f;
	}

	// Doesn't actually add, just instantiates as a child at position v
	public GameObject AddDOM(Vector3 v)
    {
        GameObject d = Instantiate(dom);
        d.transform.parent = transform;
        d.transform.position = v;
        //domList.Add(d);
		return d;
    }

	// Actually adds the dom to the string's list of references, making it permanent
	public void addDOMtoList(GameObject DOMtoAdd) {
		domList.Add (DOMtoAdd);
		GetComponentInParent<DomSpot> ().incrementNumDoms ();
	}

    public void SetStringPositions(Vector3[] pos)
    {
        if(l == null)
        {
            l = gameObject.GetComponent<LineRenderer>();
        }

        if (l != null)
        {
            l.SetPosition(0, pos[0]);
            l.SetPosition(1, pos[1]);
        }
    }

    public void UpdateStringDepth(float d)
    {
        stringDepth += d;
		if (stringDepth < 1.0f) {
			stringDepth = 1.0f;
		}
		Vector3[] pos = new Vector3[2];
		pos [0] = this.gameObject.transform.parent.position;
		pos [1] = pos [0] - new Vector3 (0, stringDepth, 0);
		Debug.Log ("top of string at: " + pos [0]);
		Debug.Log ("Bottom of string at: " + pos [1]);
		this.SetStringPositions (pos);
    }

	// Update is called once per frame
	void Update () {
	    //check if any DOMs on this string are in range of an event...
	}
}
