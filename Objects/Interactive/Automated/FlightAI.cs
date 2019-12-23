// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System;
using UnityEngine;
using Random = UnityEngine.Random;
using eidng8.SpaceFlight.Events;



namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    /// <summary>
    /// In flight logic. Tells <see cref="FlightController"/> how to move.
    /// </summary>
    [RequireComponent(typeof(FlightController))]
    public class FlightAI : MonoBehaviour
    {
        /// <summary>
        /// The distance to keep from target.
        /// </summary>
        [Tooltip("The distance to keep from target.")]
        public float safeDistance = 5;

        /// <summary>
        /// Reference to the selected target object.
        /// </summary>
        public Transform Target { get; set; }

        /// <summary>
        /// Whether a target has been chosen.
        /// </summary>
        ///
        /// <remarks>
        /// Directly check the <see cref="Target"/> against <c>null</c>
        /// is an expensive operation. So we use this field to track the
        /// status of target selection.
        /// </remarks>
        public bool HasTarget { get; private set; }

        /// <summary>
        /// Reference to the attached <see cref="FlightController"/>.
        /// </summary>
        protected FlightController Control {
            get {
                if (!this._controlAttached) {
                    this._control = this.GetComponent<FlightController>();
                }

                return this._control;
            }
            set {
                this._control = value;
                this._controlAttached = true;
            }
        }

        private FlightController _control;
        private bool _controlAttached;

        protected void Awake()
        {
            EventManager.Mgr.OnUserEvent(
                UserEvents.Select,
                this.OnSelectTarget
            );
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.W)) {
                this.Control.FullThrottle();
                Debug.Log("Forward thrust applied.");
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                this.Control.FullReverse();
                Debug.Log("Backward thrust applied.");
            }
        }

        protected void FixedUpdate()
        {
            this.TurnToTarget();
            this.DetermineThrottle();
        }

        /// <summary>
        /// Determines whether the object should start slowing down in order
        /// to stop closet the target position.
        /// </summary>
        protected bool ShouldBrake()
        {
            FlightController control = this.Control;
            float v = control.Vc;
            float a = control.deceleration;

            // We calculate how much time is needed for the speed to reach `v`
            // with acceleration `a`. From deceleration point of view, this
            // means how much time is needed to stop fully.
            float t = v / a;

            // Remember to take safe distance into account.
            float d = control.DistanceTo(this.Target.position)
                      - this.safeDistance;

            // Why:
            // => vt=D
            // => at²=D
            // Meaning if we keep current speed `v` or use the acceleration `a`
            // we can cover the distance `D` in the same period of time `t`.
            // So if we just combine both cases, we can express something like:
            // => at² + vt= 2D
            // We just replace `2D` with a letter `d`. It doesn't mean that `d`
            // is same as `2D`. It's just that the distance we are discussing
            // is not important. So it doesn't matter what letter we use to
            // denote that "some distance or same distance" is traversed.
            // Deceleration is same as acceleration applied in reverse.
            // From acceleration point of view, the above means that we
            // keep accelerating the object in time `t`, and then keep the
            // terminal speed and traveling `t` time more. We end up cover
            // total distance `d`.
            // Reverse the acceleration to deceleration. Then it means at speed
            // `v` we've traveled a part of distance `d` in time `t`, then
            // decelerate in time `t`, and eventually arrived at the full
            // distance `d`.
            // Can you see when we start decelerating now? Yes, right in the
            // middle.
            return v * t / 2 >= d;
        }

        /// <summary>
        /// Determine acceleration throttle.
        /// </summary>
        protected void DetermineThrottle()
        {
            if (!this.HasTarget) {
                return;
            }

            // If we start accelerating while facing away from the target,
            // we'll make a bit of roundabout. So we don't do this.
            if (!this.IsFacingTarget()) {
                this.Control.FullStop();
                return;
            }

            // We've arrived at a distance that needs to slow down.
            if (this.ShouldBrake()) {
                this.Control.FullReverse();
                return;
            }

            // Always use full throttle.
            this.Control.FullThrottle();
        }

        /// <summary>
        /// Determine if we are facing the target. Facing doesn't mean we are
        /// directly facing it, we can have around ±45º buffer.
        /// </summary>
        protected bool IsFacingTarget() =>
            this.Control.IsFacing(this.Target.position);

        /// <summary>
        /// Tells <see cref="FlightController"/> to face target.
        /// </summary>
        protected void TurnToTarget()
        {
            if (!this.HasTarget) {
                return;
            }

            Vector3 dir = this.Target.position - this.transform.position;
            this.Control.Bearing = dir;
        }

        /// <summary>
        /// The objected selected event handler. Sets <see cref="Target"/> to
        /// the selected object.
        /// </summary>
        protected void OnSelectTarget(ExtendedEventArgs arg0)
        {
            this.Target = arg0.Source.transform;
            this.HasTarget = true;
        }
    }
}
