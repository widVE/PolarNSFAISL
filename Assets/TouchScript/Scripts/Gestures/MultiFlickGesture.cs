/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using System.Collections.Generic;
using TouchScript.Utils;
using TouchScript.Pointers;
using UnityEngine;

namespace TouchScript.Gestures
{
    /// <summary>
    /// Recognizes fast movement before releasing pointers. Doesn't care how much time pointers were on surface and how much they moved.
    /// </summary>
    [AddComponentMenu("TouchScript/Gestures/Multi Flick Gesture")]
    [HelpURL("http://touchscript.github.io/docs/html/T_TouchScript_Gestures_FlickGesture.htm")]
    public class MultiFlickGesture : Gesture
    {
        #region Constants

        /// <summary>
        /// Message name when gesture is recognized
        /// </summary>
        public const string FLICK_MESSAGE = "OnMultiFlick";

        /// <summary>
        /// Direction of a flick.
        /// </summary>
        public enum GestureDirection
        {
            /// <summary>
            /// Direction doesn't matter.
            /// </summary>
            Any,

            /// <summary>
            /// Only horizontal.
            /// </summary>
            Horizontal,

            /// <summary>
            /// Only vertical.
            /// </summary>
            Vertical,
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when gesture is recognized.
        /// </summary>
        public event EventHandler<EventArgs> Flicked
        {
            add { flickedInvoker += value; }
            remove { flickedInvoker -= value; }
        }

        // Needed to overcome iOS AOT limitations
        private EventHandler<EventArgs> flickedInvoker;

        #endregion


        #region Private variables

        //this is somehow not showing up on the unity inspector, and is being auto set to .1 even though we initialize it here differently...
        //same is probably true for minDistance and movementThreshold...
        public float flickTime = 2f;

        [SerializeField]
        private float minDistance = .1f;

        [SerializeField]
        private float movementThreshold = 0.1f;

        [SerializeField]
        private GestureDirection direction = GestureDirection.Any;

        private Dictionary<int, TimedSequence<Vector2>> multiDeltaSequence = new Dictionary<int, TimedSequence<Vector2>>();
        private Dictionary<int, bool> isActive = new Dictionary<int, bool>();
        private Dictionary<int, bool> moving = new Dictionary<int, bool>();
        private Dictionary<int, Vector2> movementBuffer = new Dictionary<int, Vector2>();
        public Dictionary<int, Vector2> PreviousPos = new Dictionary<int, Vector2>();
        public Dictionary<int, Vector2> ScreenFlicks = new Dictionary<int, Vector2>();
        public int recognizedId = -1;
        private Dictionary<int, float> FlickTimes = new Dictionary<int, float>();

        #endregion

        #region Unity methods

        /// <inheritdoc />
        protected void LateUpdate()
        {
            for(int i = 0; i < activePointers.Count; ++i)
            {
                if(isActive[activePointers[i].Id])
                {
                    if (!multiDeltaSequence.ContainsKey(activePointers[i].Id))
                    {
                        multiDeltaSequence.Add(activePointers[i].Id, new TimedSequence<Vector2>());
                    }
                    else
                    {
                        float check = (activePointers[i].Position - activePointers[i].PreviousPosition).sqrMagnitude;
                       // Debug.Log(check);
                        if (check > 0.0f && check < 10000.0f)
                        {
                            multiDeltaSequence[activePointers[i].Id].Add(activePointers[i].Position - activePointers[i].PreviousPosition);
                            //Debug.Log(activePointers[i].Position);
                            //Debug.Log(activePointers[i].PreviousPosition);//previous position value here is sometimes out of wack... causing first value added to have huge magnitude, so added 10000 check above
                            //Debug.Log(multiDeltaSequence[activePointers[i].Id].Count() + " " + (activePointers[i].PreviousPosition - activePointers[i].Position));
                        }
                    }

                    
                }
            }
        }

        #endregion

        #region Gesture callbacks

        /// <inheritdoc />
        protected override void pointersPressed(IList<Pointer> pointers)
        {
            base.pointersPressed(pointers);

            for(int i = 0; i < pointers.Count; ++i)
            {
                //Debug.LogError("Pointer pressed: " + pointers[i].Id);
                isActive[pointers[i].Id] = true;
                moving[pointers[i].Id] = false;
                movementBuffer[pointers[i].Id] = Vector2.zero;
                PreviousPos[pointers[i].Id] = pointers[i].Position; //Vector2.zero;
                ScreenFlicks[pointers[i].Id] = Vector2.zero;
                FlickTimes[pointers[i].Id] = 0.0f;
                //if (!multiDeltaSequence.ContainsKey(pointers[i].Id))
                //{
                //    multiDeltaSequence.Add(pointers[i].Id, new TimedSequence<Vector2>());
                //}
            }
        }

        /// <inheritdoc />
        protected override void pointersUpdated(IList<Pointer> pointers)
        {
            base.pointersUpdated(pointers);

           /* for (int i = 0; i < pointers.Count; ++i)
            {
                if (isActive[pointers[i].Id] || !moving[pointers[i].Id])
                {
                    movementBuffer[pointers[i].Id] += (pointers[i].Position - pointers[i].PreviousPosition);//ScreenPosition - PreviousScreenPosition;
                    var dpiMovementThreshold = MovementThreshold * touchManager.DotsPerCentimeter;
                    //Debug.Log(dpiMovementThreshold + " " + movementBuffer[pointers[i].Id].sqrMagnitude);
                    if (movementBuffer[pointers[i].Id].sqrMagnitude >= dpiMovementThreshold * dpiMovementThreshold)
                    {
                        moving[pointers[i].Id] = true;
                    }
                }
            }*/
        }

        /// <inheritdoc />
        protected override void pointersReleased(IList<Pointer> pointers)
        {
            base.pointersReleased(pointers);
            //this only appears to pass in the pointers that were actually released...

            for (int i = 0; i < pointers.Count; ++i)
            {
                //Debug.LogError("Released pointer: " + pointers[i].Id);
                if(isActive[pointers[i].Id])// && moving[pointers[i].Id])
                {
                    //don't want to add to the sequence here...
                    //multiDeltaSequence[pointers[i].Id].Add(pointers[i].PreviousPosition - pointers[i].Position);
                    //Debug.Log(pointers[i].Id + " " + multiDeltaSequence[pointers[i].Id].Count());
                    //float lastTime;
                    //Debug.Log("**" + Time.time);
                    var deltas = multiDeltaSequence[pointers[i].Id].FindElementsLaterThan(Time.time - 1.5f);//, out lastTime);
                    var totalMovement = Vector2.zero;
                    var count = deltas.Count;
                    Debug.Log("Deltas count: " + count);
                    for (var j = 0; j < count; j++)
                    {
                       // Debug.Log(deltas[j]);
                        totalMovement += deltas[j];
                    }

                    //Debug.Log(totalMovement);
                    if (totalMovement.magnitude > minDistance * touchManager.DotsPerCentimeter)
                    {
                        //setState(GestureState.Failed);
                    // }
                    // else
                    //{
                        ScreenFlicks[pointers[i].Id] = totalMovement;
                        //FlickTimes[pointers[i].Id] = Time.time - lastTime;
                        PreviousPos[pointers[i].Id] = pointers[i].PreviousPosition;
                        recognizedId = pointers[i].Id;
                        if (flickedInvoker != null) flickedInvoker.InvokeHandleExceptions(this, EventArgs.Empty);
                    }
                    //added for better editor testing (id is always 0 in editor)
                    //multiDeltaSequence[pointers[i].Id].Clear();
                }
            }
        }

        /// <inheritdoc />
        protected override void onRecognized()
        {
            base.onRecognized();
            if (flickedInvoker != null) flickedInvoker.InvokeHandleExceptions(this, EventArgs.Empty);
            if (UseSendMessage && SendMessageTarget != null) SendMessageTarget.SendMessage(FLICK_MESSAGE, this, SendMessageOptions.DontRequireReceiver);
        }

        /// <inheritdoc />
        protected override void reset()
        {
            base.reset();
            //Debug.LogError("Reseting multiflick");
        }

        #endregion
    }
}