/*******************************************************
 * Copyright (C) 2016 3lbGames - Chris Castaldi
 * 
 * VRTouchMove
 *
 * VRTouchMove can not be copied and/or distributed without the express
 * permission of 3lbGames - Chris Castaldi.

 For additional Information or unity development services please contact services@3lbgames.com
 
 For technical support contact support@3lbgames.com

 DoTween is being used to help with easing and simulator sickness reduction.
 
 *******************************************************/


/*******************************************************
Version V-1.7.3

Added TaggedPoint Teleport 
Added Low Poly Touch Controllers

VRTouchMove is a comprehensive movement System which contains the following features

-Teleportation
-Blink
-RubberBand Movememnt
-Flight / FPS Mode
-Point and Shoot Rotation
-Fading System
-Line Renderer/Arc
-Quick and Slow Stick Movemement
-NonTouch Controls - Controller/Remote/Keyboard

These are the Toggles and the Features of this Movement System, generally they all work well together.

    Controller Settings:

        To Enable Stick move Set up the foward and backward buttons to "Primary Thumb Stick Up" and "Primary Thumb Stick Down".
        Backwards button can be set to none if you do not want reverse.

Movement Modes

* canMove - Default Movement Toggle, disable this to stop the player being able to move.

    Movement Modes:
       Flight   - Freedom from gravity and the ablity to move in 360
       Grounded - Applys gravity so it works like an FPS Controller;
       None     - Disables Movement (You can still use the Alternative Movements)
       RubberBand - This Movemement systems places a drag point anchor you drag the controller from the object to adjust speed and direction
       Controller - Switches Movement to Xbox Controller
       Remote - Switches Movement to Oculus Remote
       Keyboard  - Switches Movement to Keyboard

      Keyboard controls:
      WASD - Strafes Charactor 
      Shift - SpeedBoost
      Q/E - Rotates you 45 degrees

      Controller Controls:
      Primary Thumbstick - Strafes Charactor
      Bumpers - Rotate you 45 degrees

      Remote:
      Foward/Back - Moves you Foward or Backward
      Left/Right - Rotates you 45 degrees

      -These can all be changed in the Debug Flight Section;

    Rotation Modes:
        PointAndShoot  - Uses the Rotate Button which rotates you to the angle the controller is pointed.
                ---WARNING--- Slow Stick Tends to Cause Simulator Sickness you have been warned. --WARNING---
        SlowStick   - Left and Right motion of the Stick to rotate your Rig slowly. 
        QuickStick  - Left and right on the stick rotate the Rig 45 degrees best used with Fade
        None        - Disables Rotation

    Alternative Movement:
        Teleport - Uses the Teleportation System And it's settings
        Blink    - Blinks the Player forward where the controller is pointing a set distance see settings
        TeleportAndBlink - Uses both Teleport and Blink
        None     - Disables any Alt Movement   

    Teleport Types:
		Tag			- Teleport will happen on a Tagged Object set below
		TaggedPoint	- Teleport will happen to the Center of a Tagged object (Useful for Creating Teleport Points)
		AnyCollider - Teleport to Any Collider (Useful for Speedruning Colliders)
		NavMesh		- Teleport to Baked Navmesh !!!!Will fail without a Baked NavMesh!!!!

Non Touch Information

       Controller - Switches Movement to Xbox Controller
       Remote - Switches Movement to Oculus Remote
       Keyboard  - Switches Movement to Keyboard




* Note about Controllers Graphical Stutter Error:
If your controllers seem to be lagging behind besure of the following:

    A) They are Parented to the OVRCameraRig
    B) In the TouchController Script change the LateUpdate to FixedUpdate/Update


 *******************************************************/

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VRTouchMove : MonoBehaviour
{
    [Header("-Controller Settings-")]
    public OVRInput.Controller myController;  //Controller Choice
    public OVRInput.Button ForwardButton;        //Button for Default Movement
    public OVRInput.Button BackwardButton;
    public OVRInput.Button TeleportButton;     //Teleport Button
    public OVRInput.Button BlinkButton;       //Blink Foward
    public OVRInput.Button RotateButton;      //Rotate to Where Controller Is Located
                                              //public enum InputType { Remote, Controller };
                                              //public InputType NonTouchInput;
    [Header("-Movement Modes-")]
    public bool canMove = true;
    public enum eMovementMode { Flight, Grounded, None, RubberBand, RubberBandGrounded, Remote, Controller, Keyboard };
    public eMovementMode MovementMode = eMovementMode.Flight;
    public enum eRotationMode { PointAndShoot, QuickStick, SlowStick, None };
    public eRotationMode RotationMode = eRotationMode.PointAndShoot;
    public enum eAlterativeMove { Teleport, Blink, TeleportAndBlink, None };
    public eAlterativeMove AlterativeMove = eAlterativeMove.None;
    public eBlinkMode BlinkMode;
    public enum eBlinkMode {Normal,HoldRelease};

    [Header("-General Settings-")]
    public float moveSpeed = 1;               // Your MovementSpeed shared across all Movement Systems
    public float rotateTime = .3f;            // ButtonRotation
    public float AltMoveTime = .4f;            // Blink and Teleport Speed, 0 Is Instan
    public float blinkDistance = 10;          //Max Blink Distance
    public float rotateSpeed = .7f;           //Speed for Stick Rotate
    public float PlayerGravity = 50;          //Player Gravity for the FPS Controller


    public enum TeleportType { NavMesh, TaggedPoint, AnyCollider,Tag }; //Mode fof Teleportation Baked Navmesh is required for NavMesh Teleportation
    [Header("-Teleport Settings-")]
    public TeleportType TeleportMode;
    public float TeleMinDstance = 4;          // Min TeleportDistance
    public float TeleMaxDistance = 500;       //Max Teleport Distance
    public LineArcSystem teleportLine;       //Use Line Arch System for Teleporter To Disable Set to None
    public string theTag;                    //Tag for Tag Teleport Type
    [Header("-Fade Settings-")]
    public bool fadeRotate;                  //Fade When Rotate it is recommended you set RotateTime to 0 When doing this
    public bool fadeTeleport;                //Teleport Fade When Rotate it is recommended you set AltMoveSpeed to 0 When doing this
    public float fadeTime = .3f;
    public VRFadeScript myFade;
    [Header("-Acceleration Settings-")]
    public bool accelSpeed = true;            //Enable Speed Acceleration
    public float decay = .9f;                 //Speed Decay 
    [Range(0, 2)]
    public float acclAmount = .5f;             //Acceleration Curve 
    float acc = .1f;
    [Header("-Hookups-")]
    public CharacterController yourRig;      //Ensure the Charactor Controller is the correct size for your play space
    public Transform headRig;                //Slot for the Center Camera
    public Transform selectedController;        //Use either a touch controller or the headRig;
    public Transform TeleportPoint;          //Your Teleport Object
    public Transform DragPoint;              // Drag Point for RubberBand Movement Must be Parented to OVRCamerRig
    public LineArcSystem RubberBandLine;
    float curSpeed;
    float curRotSpeed;


    Vector3 moveDirection = Vector3.zero;
    void Awake()
    {
        Invoke("HideAllVisuals", .2f);
        if (fadeRotate || fadeTeleport)
        {
            if (!myFade)
            {
                Debug.Log("MyFade Not Assigned Disabling Fade toggles");
                fadeRotate = false;
                fadeTeleport = false;
            }
        }
        if (RotationMode == eRotationMode.SlowStick && fadeRotate)
        {
            fadeRotate = false;
            Debug.Log("Slow Stick is not compatible with fading rotation - Disabling");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            if (MovementMode == eMovementMode.Controller || MovementMode == eMovementMode.Remote || MovementMode == eMovementMode.Keyboard)
            {
                //DEBUG
                DebugFlight();
            }
            if (MovementMode == eMovementMode.Grounded)
            {
                FPSMovement();
            }
            if (MovementMode == eMovementMode.Flight)
                AdvancedFlight();
        }
        if (MovementMode == eMovementMode.RubberBand || MovementMode == eMovementMode.RubberBandGrounded)
        {
            RubberBandMove();
        }
        if (AlterativeMove == eAlterativeMove.Teleport || AlterativeMove == eAlterativeMove.TeleportAndBlink)
        {
            TeleportSystem();
        }
        if (AlterativeMove == eAlterativeMove.Blink || AlterativeMove == eAlterativeMove.TeleportAndBlink)
        {
            FowardBlink();
        }
        if (RotationMode == eRotationMode.QuickStick)
        {
            QuickStickRotate();
        }
        if (RotationMode == eRotationMode.PointAndShoot)
        {
            PointAndShootRotation();
        }
        if (RotationMode == eRotationMode.SlowStick)
        {
            SlowStickRotate();
        }
    }

    /// <summary>
    /// Rubber Band Move
    ///     Holding the Foward button places an object moving the controller around that object moves you around the space. the further away from the point the faster you go. this can be clamped!
    ///     You may want to increase your move speed if you are using this system. I have hard coded a 250% movemement speed increase into the system
    ///     ---IMPORTANT--- the Drag Point must be Parented
    /// </summary>
    /// 
    bool showRubberBandLine = false;
    public void RubberBandMove()
    {

        if (OVRInput.GetDown(ForwardButton, myController))
        {
            DragPoint.gameObject.SetActive(true);
            DragPoint.position = selectedController.position;
        }

        if (OVRInput.Get(ForwardButton, myController))
        {
            Vector3 holder = selectedController.position - DragPoint.position;
            //
            if (MovementMode == eMovementMode.RubberBandGrounded)
            {
                holder.y = 0;

                holder.y -= PlayerGravity * Time.deltaTime * moveSpeed;
            }
            //HardCoded Increase
            holder = holder * (moveSpeed * 1.5f) * Time.deltaTime;
            yourRig.Move(holder);
            showRubberBandLine = true;
        }
        if (OVRInput.GetUp(ForwardButton, myController))
        {
            showRubberBandLine = false;
            RubberBandLine.HideLine();
            DragPoint.gameObject.SetActive(false);
        }
    }

    void LateUpdate()
    {
        if (showRubberBandLine)
        {
            RubberBandLine.CreateLine(DragPoint.position, selectedController.position, Color.blue);
        }
    }
    /// <summary>
    /// Advanced Flight System
    ///      This is the Main Function for the Movement system. It is designed to be an arcade flight controller in order to navigate around in a space. 
    ///      Press the Move button and you are dragged foward where ever the touch controller is pointed.
    /// </summary>
    public void AdvancedFlight()
    {
        acc = moveSpeed * acclAmount;
        float v = 0; //Place Holder for Foward Stick
        if (OVRInput.Get(BackwardButton, myController))
        {
            v = GetAxisFromButton(BackwardButton); //Get axis if using Button
            if (v == -10)
            {
                v = -1;
            }
        }
        if (OVRInput.Get(ForwardButton, myController))
        {
            v = GetAxisFromButton(ForwardButton); //Get axis if using Button
            if (v == -10)
            {
                v = 1;
            }
        }
        if (accelSpeed)
        {
            //Decay Speed
            if (Mathf.Abs(v) <= 0)
            {
                curSpeed *= decay;
            }
            else
            {
                //Apply Acceloration
                curSpeed += acc * v * Time.deltaTime;
                //curSpeed += acc * v2 * Time.deltaTime;
            }
            curSpeed = Mathf.Clamp(curSpeed, -moveSpeed, moveSpeed);
        }
        else
        {
            curSpeed = moveSpeed * v;
        }
        curSpeed = Mathf.Clamp(curSpeed, -curSpeed * v, curSpeed * v);
        yourRig.Move(selectedController.forward * curSpeed * Time.deltaTime);
    }
    /// <summary>
    /// FPS Movement System
    ///      When using this movement system ensure that your charactor controller is big enough to deal with someone moving around the space. 
    ///      Room Scale setups are not recommended for this setup. This also can be used to ground the player and keep them from flying.
    /// </summary>
    public void FPSMovement()
    {
        //Acceleration System
        moveDirection = Vector3.zero;
        float v = 0; //Place Holder for System
        if (OVRInput.Get(BackwardButton, myController))
        {
            v = GetAxisFromButton(BackwardButton);
            if (v == -10)
            {
                v = -1;
            }
            moveDirection = selectedController.forward * v;

        }
        if (OVRInput.Get(ForwardButton, myController))
        {
            v = GetAxisFromButton(BackwardButton);
            if (v == -10)
            {
                v = 1;
            }
            moveDirection = selectedController.forward * v;
        }
        if (OVRInput.Get(BackwardButton, myController))
        {

            moveDirection.y = 0;
        }
        if (OVRInput.Get(ForwardButton, myController))
        {
            moveDirection = selectedController.forward * 1;
            //moveDirection.y = 0;
        }
        //Speed Assign
        moveDirection *= moveSpeed;
        //Gravity Assign
        moveDirection.y -= PlayerGravity * Time.deltaTime * moveSpeed;
        //Apply Movement
        yourRig.Move(moveDirection * Time.deltaTime);
    }
    /// <summary>
    /// Slow Stick Rotation
    /// Here is the function for the slow stick rotation, This one does seem to cause simulator sickness however many have requested it. Speed is adjusted using rotateSpeed
    /// </summary
    void SlowStickRotate()
    {
        float h = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, myController).x;

        if (Mathf.Abs(h) > .1f)
        {
            RotateByDegrees(h * rotateSpeed);
        }
    }

    /// <summary>
	/// Foward Blink System  ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    ///     Blink Foward Set Direction.
    /// </summary>
    public void FowardBlink()
    {
        switch (BlinkMode)
        {
            case eBlinkMode.Normal:
                if (OVRInput.GetDown(BlinkButton, myController))
                {
                    //Cast Ray Foward
                    Ray ray = new Ray(selectedController.position, selectedController.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, blinkDistance))
                    {
                        //Blink to Point
                        yourRig.transform.DOMove(hit.point, AltMoveTime);
                        Invoke("BumpMe", AltMoveTime + Time.deltaTime);
                    }
                    else
                    {
                        //Blink to Max Distance
                        yourRig.transform.DOMove(ray.GetPoint(blinkDistance), AltMoveTime);
                        Invoke("BumpMe", AltMoveTime + Time.deltaTime);
                    }
                }
                break;
            case eBlinkMode.HoldRelease:
                if (OVRInput.Get(BlinkButton, myController))
                {
                    //Debug.Log("WORKING");
                    //Cast Ray Foward
                    Ray ray = new Ray(selectedController.position, selectedController.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, blinkDistance))
                    {
                        TeleportPoint.transform.DOMove(hit.point, .05f);
                        TeleportPoint.gameObject.SetActive(true);
                        ////Blink to Point
                        //yourRig.transform.DOMove(hit.point, AltMoveTime);
                    }
                    else
                    {
                        TeleportPoint.transform.DOMove(ray.GetPoint(blinkDistance), .05f);
                        TeleportPoint.gameObject.SetActive(true);
                        ////Blink to Max Distance
                        //yourRig.transform.DOMove(ray.GetPoint(blinkDistance), AltMoveTime);
                    }
                }
                if (OVRInput.GetUp(BlinkButton, myController))
                {
                    teleportLine.HideLine();
                    TeleportPoint.gameObject.SetActive(false);
                    //Cast Ray Foward
                    Ray ray = new Ray(selectedController.position, selectedController.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, blinkDistance))
                    {
                        //Blink to Point
                        yourRig.transform.DOMove(hit.point, AltMoveTime);
                        Invoke("BumpMe", AltMoveTime + Time.deltaTime);
                    }
                    else
                    {
                        //Blink to Max Distance
                        yourRig.transform.DOMove(ray.GetPoint(blinkDistance), AltMoveTime);
                        Invoke("BumpMe", AltMoveTime + Time.deltaTime);
                    }
                }
                    break;
            default:
                break;
        }
        //Get Button
    
    }

    /// <summary>
    ///  Teleport System Area ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    /// </summary>
    void TeleportSystem()
    {
        if (OVRInput.Get(TeleportButton, myController))
        {
            //Get Point
            GetTeleportPoint();
            //Forces Teleporter to Look Right At you
            TeleportPoint.transform.DOLookAt(selectedController.position, .1f, AxisConstraint.Y);
        }
        if (OVRInput.GetUp(TeleportButton, myController))
        {
            Teleport();
            if (teleportLine)
            {
                teleportLine.HideLine();
            }
            TeleportPoint.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Teleportation Area |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    /// </summary>
    void Teleport()
    {
        if (!TeleportPoint.gameObject.activeInHierarchy)
        {
            //OnlyTeleport if Active
            return;
        }
        //Instant if Zero
        if (fadeTeleport)
        {
            myFade.StartFadeIn(fadeTime);

            yourRig.transform.position = TeleportPoint.position;
            BumpMe();
            return;
        }
        if (AltMoveTime == 0)
        {
            yourRig.transform.position = TeleportPoint.position;
            BumpMe();
        }
        else
        {
            yourRig.transform.DOMove(TeleportPoint.position, AltMoveTime);
            Invoke("BumpMe", AltMoveTime + Time.deltaTime);
        }
    }

    void BumpMe()
    {
        yourRig.Move(Vector3.down * .003f);
    }
    /// <summary>
    /// Get Teleport Point |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    /// </summary>
    void GetTeleportPoint()
    {

        Ray ray = new Ray(selectedController.position, selectedController.forward);
        RaycastHit hit;
        bool foundPoint = false;
        //FireRayCast
        if (Physics.Raycast(ray, out hit, TeleMaxDistance))
        {
            //Inviso ColliderFinder!
            // Debug.Log(hit.collider.name);
            if (Vector3.Distance(hit.point, yourRig.transform.position) > TeleMinDstance)
            {
                //Only Show Teleport greater then Min
                switch (TeleportMode)
                {
                    case TeleportType.NavMesh:
                        foundPoint = NavMeshTeleport(hit);
                        break;
                    case TeleportType.TaggedPoint:
                        foundPoint = TagTeleport(hit);
                        break;
                    case TeleportType.AnyCollider:
                        foundPoint = ColliderTeleport(hit);
                        break;
                    case TeleportType.Tag:
                        foundPoint = ColliderTeleport(hit);
                        break;
                    default:
                        break;
                }
                if (foundPoint)
                {
                    TeleportPoint.gameObject.SetActive(true);
                    if (teleportLine)
                    {
                        if (TeleportMode == TeleportType.TaggedPoint)
                        {
                            teleportLine.CreateLine(selectedController.position, hit.transform.position, Color.green);
                        }
                        else
                        {
                            teleportLine.CreateLine(selectedController.position, hit.point, Color.green);
                        }

                    }
                    if (TeleportMode == TeleportType.TaggedPoint)
                    {
                        TeleportPoint.transform.DOMove(hit.transform.position, 0);

                    }
                    else
                    {
                        TeleportPoint.transform.DOMove(hit.point, .05f);
                    }
                    //Smooths Teleporter there

                }
                else
                {
                    if (teleportLine)
                    {
                        teleportLine.CreateLine(selectedController.position, hit.point, Color.red);
                    }
                    if (TeleportMode == TeleportType.AnyCollider)
                    {
                        if (teleportLine)
                        {
                            teleportLine.CreateLine(selectedController.position, hit.point, Color.green);
                        }
                    }
                    //Did not Hit Correct Point
                    TeleportPoint.gameObject.SetActive(false);
                }
            }
            else
            {
                if (teleportLine)
                {
                    teleportLine.HideLine();
                }
                //Not in Min Distance
                TeleportPoint.gameObject.SetActive(false);
            }
        }
        else
        {
            if (teleportLine)
            {
                teleportLine.HideLine();
            }
            //Extra Else Just in case of Weirdness
            TeleportPoint.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// Teleport Functions Seporated Cleaner Code |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    /// </summary>

    bool NavMeshTeleport(RaycastHit hit)
    {
        //Calcuate if Point is on NavMesh
        Vector3 holder = Vector3.zero;
        if (RandomPoint(hit.point, .01f, out holder))
        {
            return true;
        }
        else
        {
            //Not On NavMesh
            return false;
        }
    }


    bool TagTeleport(RaycastHit hit)
    {
        if (hit.collider.tag == theTag)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool ColliderTeleport(RaycastHit hit)
    {
        return true;
    }

    /// <summary>
    /// NavMesh Helper Teleportation |||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    /// Ripped Straight out of Documentation - Finds the closest point to where you point on Navmesh.
    /// https://docs.unity3d.com/ScriptReference/NavMesh.SamplePosition.html
    /// </summary>
    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        Vector3 holder = center + Random.insideUnitSphere * range;
        UnityEngine.AI.NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(holder, out hit, 1.0f, 1 << UnityEngine.AI.NavMesh.GetNavMeshLayerFromName("Walkable")))
        {
            result = hit.position;
            return true;
        }
        result = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Point and Shoot Rotation ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    ///  Changes the Rotation based on where the Controller is pointing and where you are looking only only in one direction. This allows someone to quickly look behind them.
    /// </summary>

    void PointAndShootRotation()
    {
        if (OVRInput.GetDown(RotateButton, myController))
        {
            if (fadeRotate)
            {
                myFade.StartFadeIn(fadeTime);
            }
            //Get Position in front of Object
            Vector3 holder = new Ray(selectedController.position, selectedController.forward).GetPoint(1);
            //Get Rotational Direction
            Vector3 lookPos = holder - headRig.transform.position;
            //Remove Y Component
            lookPos.y = 0;
            //Transform rotation
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            //Get Rotational Direction
            Vector3 rotPosition = rotation.eulerAngles - headRig.transform.localRotation.eulerAngles;
            //Flatten Rotational Return
            rotPosition.x = 0;
            rotPosition.z = 0;
            //Apply Rotation
            yourRig.transform.DORotate(rotPosition, rotateTime);
        }
    }

    /// <summary>
    /// Debug Flight for Non Touch Systems ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    /// </summary>
    public void DebugFlight()
    {
        float v = 0;
        float h = 0;
        switch (MovementMode)
        {
            case eMovementMode.Remote:

                if (OVRInput.Get(OVRInput.Button.Up, OVRInput.Controller.Remote))
                {
                    v = 1;
                }
                if (OVRInput.Get(OVRInput.Button.Down, OVRInput.Controller.Remote))
                {
                    v = -1;
                }
                if (OVRInput.GetDown(OVRInput.Button.Left, OVRInput.Controller.Remote))
                {
                    RotateByDegrees(-45);
                }
                if (OVRInput.GetDown(OVRInput.Button.Right, OVRInput.Controller.Remote))
                {
                    RotateByDegrees(45);
                }
                yourRig.Move((headRig.forward * v) * moveSpeed * Time.deltaTime);
                break;
            case eMovementMode.Controller:

                v = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Gamepad).y;
                h = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Gamepad).x;
                if (OVRInput.GetDown(OVRInput.Button.PrimaryShoulder, OVRInput.Controller.Gamepad))
                {
                    RotateByDegrees(-45);
                }
                if (OVRInput.GetDown(OVRInput.Button.SecondaryShoulder, OVRInput.Controller.Gamepad))
                {
                    RotateByDegrees(45);
                }
                yourRig.Move(headRig.TransformDirection(new Vector3(h, 0, v)) * moveSpeed * Time.deltaTime);
                break;
            case eMovementMode.Keyboard:
                v = Input.GetAxis("Vertical");
                h = Input.GetAxis("Horizontal");
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    RotateByDegrees(-45);
                }
                if (Input.GetKeyDown(KeyCode.E))
                {
                    RotateByDegrees(45);
                }
                float speedAdd = 0;
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    speedAdd = moveSpeed;
                }
                yourRig.Move(headRig.TransformDirection(new Vector3(h, 0, v)) * (moveSpeed + speedAdd) * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    void QuickStickRotate()
    {
        if (OVRInput.GetDown(OVRInput.Button.Left, myController))
        {
            RotateByDegrees(-45);
        }
        if (OVRInput.GetDown(OVRInput.Button.Right, myController))
        {
            RotateByDegrees(45);
        }
    }

    void RotateByDegrees(float degrees)
    {
        if (RotationMode == eRotationMode.SlowStick && fadeRotate)
        {
            fadeRotate = false;
            Debug.Log("Slow Stick is not compatible with fading rotation - Disabling");
        }
        if (fadeRotate)
        {
            myFade.StartFadeIn(fadeTime);
        }
        Vector3 holder1;
        holder1 = yourRig.transform.rotation.eulerAngles;
        holder1.y += degrees;
        Vector3 rotPosition = holder1;
        yourRig.transform.DORotate(rotPosition, rotateTime);
    }

    void HideAllVisuals()
    {
        if (teleportLine)
        {
            teleportLine.HideLine();
        }
        if (TeleportPoint)
        {
            TeleportPoint.gameObject.SetActive(false);
        }
        if (DragPoint)
        {
            DragPoint.gameObject.SetActive(false);
        }
        if (RubberBandLine)
        {
            RubberBandLine.HideLine();
        }

    }

    float GetAxisFromButton(OVRInput.Button theButton)
    {
        switch (theButton)
        {
            case OVRInput.Button.PrimaryHandTrigger:
                return OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, myController);
            case OVRInput.Button.PrimaryIndexTrigger:
                return OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, myController);
            default:
                return -10; //This return is to for things without Axises
        }
    }
}
