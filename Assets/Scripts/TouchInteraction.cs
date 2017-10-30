using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures;

/// <summary>
/// Transforms touch events into GameObject Interaction.
/// </summary>
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
    [Header("Touch Settings")]
    public bool rotate = true;
    public bool rotateAround = true;
    [Tooltip("Sensitivity of object rotation")]
    public float rotationSensitivity = 1f;

    #endregion

    #region Private Properties

    private bool isMoving;

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
            isMoving = true;
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
        if (enabled && isMoving) onmove();
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
        if (pos.x < Screen.width * 0.15f
            && pos.y < Screen.height * 0.3f) {
            if (rotate || rotateAround)
            {
                if (rotationSensitivity < 1f) rotationSensitivity = 1f;
                // Get previous cursor position relative to main camera
                Vector2 prevpos = metaGesture.PreviousScreenPosition;
                // Compute displacement vector
                Vector2 delta = (pos - prevpos) / rotationSensitivity;

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
    }

    /// <summary>
    /// Performs action on touch release.
    /// </summary>
    private void onrelease()
    {

    }

    #endregion

}
