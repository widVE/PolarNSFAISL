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
VRTouchMoveSimple is a Simplfied movement system missing the teleportation and other options

This script moves the OVRCameraRig(yourRig) in whatever direction you are pointing. 
If you want to go up, point the controller up and press the Move button. The same goes for down, left, or anywhere, in 360 Degrees.

These are the Toggles and the Features of this Movement System, generally they all work well together.

* canMove - Default Movement Toggle, disable this to stop the player being able to move.

* useButtonRotate – Uses the Rotate Button which rotates you to the angle the controller is pointed.


* useNonTouch - Overrides all movement controls to allow someone to navigate the scene with the Oculus Remote and Controller. Useful for testing when Touch Controllers are sparse.

Remote is Default

*Controller uses Primary Thumb Stick for Forward and back Bumpers rotate you 45 degrees
*Remote - rward and Back work by using the CenterCamera left and right rotate you 45 degrees


* Note about Controllers Graphical Stutter Error:
If your controllers seem to be lagging behind besure of the following:

    A) They are Parented to the OVRCameraRig
    B) In the TouchController Script change the LateUpdate to FixedUpdate/Update

* Note about Controllers Graphical Stutter Error:
If your controllers seem to be lagging behind besure of the following:

    A) They are Parented to the OVRCameraRig
    B) In the TouchController Script change the LateUpdate to FixedUpdate/Update


 *******************************************************/

using UnityEngine;
using System.Collections;
using DG.Tweening;

public class VRTouchMoveSimple : MonoBehaviour
{
   [Header("-Controller Settings-")]
   public OVRInput.Controller myController;  //Controller Choice
   public OVRInput.Button MoveButton;        //Button for Default Movement
   public OVRInput.Button RotateButton;      //Rotate to Where Controller Is Located
   [Header("-Toggles-")]
   public bool canMove = true;
   public bool useButtonRotate = true;       //Use Button Rotation
   [Header("-Settings-")]
   public float moveSpeed = 1;               // Your MovementSpeed
   public float rotateTime = .3f;            // ButtonRotation
   [Header("-Hookups-")]
   public CharacterController yourRig;      //Ensure the Charactor Controller is the correct size for your play space
   public Transform headRig;                //Slot for the Center Camera
   public Transform fowardDirection;        //Use either a touch controller or the headRig;
   //private Vector3 rotPosition; 
   float curSpeed;
   float curRotSpeed;
    [Header("-NonTouchMode-")]
    public bool useNonTouch;
    public enum InputType { Remote, Controller };
    public InputType inputType;                   //OverRide Movement with Remote
    void Update()
    {
        if(useNonTouch)
        {
            DebugFlight();
        }
        else
        {
            FlightSystem();
        }
    }

    /// <summary>
    /// Button Rotation ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
    ///  Changes the Rotation based on where the Controller is pointing and where you are looking only only in one direction. This allows someone to quickly look behind them.
    /// </summary>

    void ChangeOrentationHeadRig()
   {
       //Get Position in front of Object
       Vector3 holder =  new Ray(fowardDirection.position, fowardDirection.forward).GetPoint(1);
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




   /// <summary>
   /// Simple Flight System ||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||||
   /// </summary>
   void FlightSystem()
   {
       if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
       {
           if (!canMove)
           {
               return;
           }
               yourRig.Move(fowardDirection.forward * moveSpeed * Time.deltaTime * OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch));
       }
       if (useButtonRotate)
       {
           if (OVRInput.GetDown(OVRInput.Button.Four))
           {
               ChangeOrentationHeadRig();
           }
       }

   }



    public void DebugFlight()
    {
        float v = 0;
        switch (inputType)
        {
            case InputType.Remote:

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
            case InputType.Controller:

                v = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.Gamepad).y;
                if (OVRInput.GetDown(OVRInput.Button.PrimaryShoulder, OVRInput.Controller.Gamepad))
                {
                    RotateByDegrees(-45);
                }
                if (OVRInput.GetDown(OVRInput.Button.SecondaryShoulder, OVRInput.Controller.Gamepad))
                {
                    RotateByDegrees(45);
                }
                yourRig.Move((headRig.forward * v) * moveSpeed * Time.deltaTime);
                break;
            default:
                break;
        }
    }

    void RotateByDegrees(float degrees)
    {
        Vector3 holder1;
        holder1 = yourRig.transform.rotation.eulerAngles;
        holder1.y += degrees;
        Vector3 rotPosition = holder1;
        yourRig.transform.DORotate(rotPosition, rotateTime);
    }

}
