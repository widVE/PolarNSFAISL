using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

/// <summary>
/// Transforms touch events into GameObject Interaction.
/// </summary>
[DisallowMultipleComponent]
public class TouchInteraction : MonoBehaviour {

    #region Public Properties

    public bool enabled = true;
    [Header("Object Settings")]
    [Tooltip("The target of the interaction")]
    public GameObject targetObject;
    [Tooltip("Reference object for relative interaction (rotate around)")]
    public GameObject referenceObject;
    [Tooltip("The meta gesture on the surface")]
    public MetaGesture metaGesture;
    [Header("Touchable Area")]
    public Vector2 startPos = new Vector2(0f, 0f);
    [Range(0, 1)]
    public float width = 0f, height = 0f;
    [Header("Touch Settings")]
    public bool rotate = true;
    public bool rotateAround = true;
    [Tooltip("Sensitivity of object rotation")]
    public float rotationSensitivity = 1f;

    #endregion

    #region Private Properties

    private bool isMoving;
    private Vector2 delta;

    #endregion

    #region Unity methods

    void Start () {
        isMoving = false;
    }
    private void OnEnable()
    {
        metaGesture.PointerPressed += pressHandler;
        metaGesture.PointerReleased += releaseHandler;
    }

    private void pressHandler(object sender, System.EventArgs e)
    {
        if (enabled)
        {
            onpress();

            // Start moving if touch down was within bounding box
            Vector2 pos = metaGesture.ScreenPosition;
            if (pos.x > Screen.width * startPos.x
                && pos.x < Screen.width * (startPos.x + width)
                && pos.y > Screen.height * startPos.y
                && pos.y < Screen.height * (startPos.y + height))
            {
                isMoving = true;
            }
        }
    }
    private void releaseHandler(object sender, System.EventArgs e)
    {
        if (enabled)
        {
            isMoving = false;
            onrelease();
        }
    }
    
    void Update () {
        if (enabled) {
            if (isMoving) onmove();
            else onidle();
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Performs action on touch press.
    /// </summary>
    private void onpress()
    {

    }

    /// <summary>
    /// Performs action on touch move.
    /// </summary>
    private void onmove()
    {
        // Get current cursor position relative to main camera
        Vector2 pos = metaGesture.ScreenPosition;
        // Stop move interaction if touch falls out of scope
        if (pos.x < Screen.width * startPos.x
            || pos.x > Screen.width * (startPos.x + width)
            || pos.y < Screen.height * startPos.y
            || pos.y > Screen.height * (startPos.y + height))
        { 
            isMoving = false;
            return;
        }
        // Perform interaction per frame
        if (rotate || rotateAround) // Check component settings
        {
            if (rotationSensitivity < 1f) rotationSensitivity = 1f;
            // Get previous cursor position relative to main camera
            Vector2 prevpos = metaGesture.PreviousScreenPosition;
            // Compute displacement vector
            delta = (pos - prevpos) / rotationSensitivity;

            Transform transform = targetObject.GetComponent<Transform>();
            if (rotate)
            {
                // Use displacement vector to rotate target
                transform.Rotate(new Vector3(delta.y, -delta.x, 0), Space.World);
            }
            if (rotateAround && referenceObject != null)
            {
                Transform refTransf = referenceObject.GetComponent<Transform>();
                Vector3 cross = Vector3.Cross(transform.forward, transform.up);
                cross.Normalize();
                // Use displacement vector to rotate target around reference
                transform.RotateAround(refTransf.position, transform.up, delta.x);
                transform.RotateAround(refTransf.position, cross, delta.y);
            }
        }
    }

    /// <summary>
    /// Performs action when touch not moving.
    /// </summary>
    private void onidle()
    {
        if (rotate || rotateAround)
        {
            // Gradually decrease motion in last direction of move (momentum)
            delta = delta * 0.75f;

            Transform transform = targetObject.GetComponent<Transform>();
            if (rotate)
            {
                // Use displacement vector to rotate target
                transform.Rotate(new Vector3(delta.y, -delta.x, 0), Space.World);
            }
            if (rotateAround && referenceObject != null)
            {
                Transform refTransf = referenceObject.GetComponent<Transform>();
                Vector3 cross = Vector3.Cross(transform.forward, transform.up);
                cross.Normalize();
                // Use displacement vector to rotate target around reference
                transform.RotateAround(refTransf.position, transform.up, delta.x);
                transform.RotateAround(refTransf.position, cross, delta.y);
            }
        }
    }


    /// <summary>
    /// Performs action on touch release.
    /// </summary>
    private void onrelease()
    {

    }

    #endregion

}
