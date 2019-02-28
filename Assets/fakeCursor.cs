using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeCursor : MonoBehaviour
{

    public Camera cam;
    public Vector3 cameraRelative;
    public GameObject cursor;
    public bool switchCoordinates = true;

    // Start is called before the first frame update
    void Start()
    {
        cursor = GameObject.Find("Pointer");
        //
    }

    // Update is called once per frame
    void Update()
    {
        cursor = GameObject.Find("Pointer");
        Vector3 newPos = Camera.main.ScreenToWorldPoint(cursor.transform.position);
        //cameraRelative = cam.InverseTransformPoint(cursor.transform.position);
        //cameraRelative = Camera.main.ScreenToWorldPoint(cursor.transform.position);
        // Vector3 newPos = cam.WorldToScreenPoint(cursor.transform.position);

        if (!switchCoordinates)
        {
            GameObject.Find("fakeTrail").transform.position = cursor.transform.position;
        } else
        {
            //Debug.Log(cursor.transform.position.x + "." + cursor.transform.position.y + "." + cursor.transform.position.z);
            Debug.Log(newPos.x + "|||" + newPos.y + "|||" + newPos.z);

            GameObject.Find("fakeTrail").transform.position = newPos;
        }
    }
}
