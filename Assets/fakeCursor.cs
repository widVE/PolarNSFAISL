using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fakeCursor : MonoBehaviour
{
    private Vector3 mousePosition;
    public float moveSpeed = 0.1f;
    public int mouseInt = 0;

    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButton(1)) {
            Debug.Log("Moving to mouse!");
            mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        }

        // Handle screen touches.
        if (Input.touchCount > mouseInt) {
            GetComponent<Image>().enabled = true;
            //Debug.Log("Touch detected!");
            Touch touch = Input.GetTouch(mouseInt);
                mousePosition = touch.position;
                transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
        }
        else
        {
            GetComponent<Image>().enabled = false;
        }
    }
}
    