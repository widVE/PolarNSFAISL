/*******************************************************
VR Movement System for Oculus Touch 

Copyright (C) 2016 3lbGames - Chris Castaldi

This Update requires you to Delete the entire VR Package

VRTouchMove can not be copied and/or distributed without the express permission of 3lbGames - Chris Castaldi.

For additional Information or unity development services please contact services@3lbgames.com

For technical support contact support@3lbgames.com

 *******************************************************/

 **********************************************************************
 We cannot provide you the controller Models those are owned by Oculus
 **********************************************************************

DoTween is being included to help with easing and simulator sickness reduction.
http://dotween.demigiant.com/ Thanks for making such a great Tweener!

Update DoTween Here https://www.assetstore.unity3d.com/en/#!/content/27676

Instructions for use:

--Setup--

1) Import OVRUtlities  (https://developer3.oculus.com/downloads/)
2) Apply VRTouchMove Script to Empty Object in Scene

--Apply Required Hookups--

3) Apply a Charactor controller to the OVRCameraRig
4) SelectedController = LeftHandAnchor or RightHandAnchor Touch Controller 
5) Your Rig = OVRCameraRig
6) Head Rig = CenterEyeAnchor

--Apply Optional Hookups---

7) (Optional) Teleport Point = What you want to use a Teleport Point (Ignoring Colliders or Having None)
8) (Optional) Teleport Line
9) (Optional) Apply Camera Fade script to center Eye camera and drag into myFade
10)(Optional) Apply Drag Point
11)(Optional) Apply RubberBand Line 

--Done!--

12) Set up the Rest of your buttons And Play!

Version V-1.7.3

Added TaggedPoint Teleport 
Added Low Poly Touch Controllers

VRTouchMove is a comprehensive movement System which contains the following features

-Teleportation
-Blink
-RubberBand Movement 
-Flight / FPS Mode
-Point and Shoot Rotation
-Fading System
-Line Renderer/Arc
-Quick and Slow Stick Movemement
-NonTouch Controls - Controller/Remote/K

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