using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NeutrinoCaptureGame : MonoBehaviour {

    public GameObject stringDriller;
    public GameObject iclString;

    List<GameObject> iceCubeStrings;


	// Use this for initialization
	void Start () {
        iceCubeStrings = new List<GameObject>();   
	}
	
	// Update is called once per frame
	void Update () {
        if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.F))
        {
            if(stringDriller != null)
            {
                //add a new string..
                GameObject i = (GameObject)Instantiate(iclString);
                i.transform.SetParent(transform);
                Vector3[] pos = new Vector3[2];
                pos[0] = stringDriller.transform.position;
                pos[1] = pos[0] - new Vector3(0.0f, 1.0f, 0.0f);
                //Debug.Log(pos[0]);
                //Debug.Log(pos[1]);
                i.GetComponent<ICLString>().SetStringPositions(pos);
                iceCubeStrings.Add(i);
            }
        }
        else if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.G))
        {
            iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().AddDOM(stringDriller.transform.position);
        }
        else if(UnityEngine.Input.GetKey(UnityEngine.KeyCode.DownArrow))
        {
            Vector3[] pos = new Vector3[2];
            pos[0] = stringDriller.transform.position;
            iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().stringDepth -= 1.0f;
            pos[1].x = pos[0].x;
            pos[1].z = pos[0].z;
            pos[1].y = iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().stringDepth;
            iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().SetStringPositions(pos);
        }
        else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.UpArrow))
        {
            Vector3[] pos = new Vector3[2];
            pos[0] = stringDriller.transform.position;
            iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().stringDepth += 1.0f;
            pos[1].x = pos[0].x;
            pos[1].z = pos[0].z;
            pos[1].y = iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().stringDepth;
            iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>().SetStringPositions(pos);
        }
        else if(UnityEngine.Input.GetKey(UnityEngine.KeyCode.PageDown))
        {
            ICLString i = iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>();
            Vector3 v = i.domList[i.domList.Count - 1].transform.position;
            v.y -= 0.5f;
            i.domList[i.domList.Count - 1].transform.position = v;
        }
        else if(UnityEngine.Input.GetKey(UnityEngine.KeyCode.PageUp))
        {
            ICLString i = iceCubeStrings[iceCubeStrings.Count - 1].GetComponent<ICLString>();
            Vector3 v = i.domList[i.domList.Count - 1].transform.position;
            v.y += 0.5f;
            i.domList[i.domList.Count - 1].transform.position = v;
        }
        else
        {
            /*if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.W))
            {
                if (stringDriller.transform.eulerAngles.y != 90.0f)
                {
                    stringDriller.transform.Rotate(Vector3.up * 90.0f, Space.Self);
                }
            }
            else if(UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.S))
            {
                if (stringDriller.transform.eulerAngles.y != -90.0f)
                {
                    stringDriller.transform.Rotate(Vector3.up * -90.0f, Space.Self);
                }
            }*/
            
            const float moveSpeed = 0.5f;
            if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.W))
            {
                stringDriller.transform.Translate(new Vector3(0.0f, 0.0f, -moveSpeed), Space.World);
            }
            else if(UnityEngine.Input.GetKey(UnityEngine.KeyCode.A))
            {
                stringDriller.transform.Translate(new Vector3(moveSpeed, 0.0f, 0.0f), Space.World);
            }
            else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.S))
            {
                stringDriller.transform.Translate(new Vector3(0.0f, 0.0f, moveSpeed), Space.World);
            }
            else if (UnityEngine.Input.GetKey(UnityEngine.KeyCode.D))
            {
                stringDriller.transform.Translate(new Vector3(-moveSpeed, 0.0f, 0.0f), Space.World);
            }
        }
	}
}
