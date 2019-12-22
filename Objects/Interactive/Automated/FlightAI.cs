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
        /// Reference to the attached <see cref="FlightController"/>.
        /// </summary>
        /// <todo>
        /// Change to lazy-init property.
        /// </todo>
        protected FlightController _control;

        /// <summary>
        /// Whether a target has been chosen.
        /// </summary>
        ///
        /// <remarks>
        /// Directly check the <see cref="Target"/> against <c>null</c>
        /// is an expensive operation. So we use this field to track the
        /// status of target selection.
        /// </remarks>
        protected bool _hasTarget = false;

        /// <summary>
        /// Tracks whether the object should be moving.
        /// </summary>
        protected bool _shouldMove = false;

        /// <summary>
        /// Reference to the selected target object.
        /// </sumary>
        public Transform Target { get; set; }

        /// <summary>
        /// Whether a target has been chosen.
        /// </summary>
        public bool HasTarget => this._hasTarget;

        protected void Start() {
            this._control = this.GetComponent<FlightController>();
        }

        protected void Awake() {
            EventManager.Mgr.OnUserEvent(UserEvents.Select,
                this.OnSelectTarget);
        }

        protected void Update() {
            if (Input.GetKeyDown(KeyCode.W)) {
                this._control.FullThrottle();
                Debug.Log("Forward thrust applied.");
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                this._control.FullStop();
                Debug.Log("Backward thrust applied.");
            }
        }

        protected void FixedUpdate() {
            this.TurnToTarget();
            this.DetermineThrottle();
        }

        /// <summary>
        /// Distance to target.
        /// </summary>
        /// <todo>
        /// Create a distance method in FlightController, and call that method.
        /// </todo>
        protected float TargetDistance() {
            return Vector3.Distance(this.transform.position,
                this.Target.position);
        }

        /// <summary>
        /// Determines whether the object should start slowing down in order
        /// to stop closet the target position.
        /// </summary>
        protected bool ShouldBrake() {
            FlightController control = this._control;
            float v = control.Vc;
            float a = control.deceleration;

            // We calculate how much time is needed for the speed to reach `v`
            // with acceleration `a`. From deceleration point of view, this
            // means how much time is needed to stop fully.
            float t = v / a;

            // Remember to take safe distance into account.
            float d = this.TargetDistance() - this.safeDistance;

            // Why:
            // => vt=D
            // => at²=D
            // Meaning if we keep current speed `v` or use the acceleration `a`
            // we can cover the distance `D` in the same period of time.
            // So if we just combine both cases, we can express something like:
            // => at² + vt= 2D
            // We just replace `2D` with a lettre `d`. It doesn't mean that `d`
            // is same as `2D`. It's just that the distance we are discussing
            // is not important. So it doesn't matter what letter we use. To
            // denote that "some distance, or same distance" is traversed.
            // Deceleration is same as acceleration applied in reverse.
            // With accleration point of view, the above means that we
            // keep accelerating the object in time `t`, and then keep the
            // terminal speed and travel `t` more time. Covered disntance `d`.
            // Reverse the acceleration to deceleration. Then it means at speed
            // `v` we've traveled a part of distance `d` in time `t`, then
            // decelerate in time `t`, and eventualy arrived at our goal.
            // Can you see when we start decelerating now? Yes, right in the
            // middle.
            return v * t / 2 >= d;
        }

        /// <summary>
        /// Determine acceleration throttle.
        /// </summary>
        protected void DetermineThrottle() {
            if (!this._hasTarget || !this._shouldMove) {
                return;
            }

            // If we start accelerating while facing away from the target,
            // we'll make a bit of roundabout. So we don't do this.
            if (!this.IsFacingTarget()) {
                this._control.FullStop();
                return;
            }

            // We've arrived at a distance that needs to slow down.
            if (this.ShouldBrake()) {
                this._control.FullStop();
                return;
            }

            // Always use full throttle.
            this._control.FullThrottle();
        }

        /// <summary>
        /// Determine if we are facing the target. Facing doesn't mean we are
        /// directly facing it, we can have around ±45º buffer.
        /// </summary>
        protected bool IsFacingTarget() {
            Transform me = this.transform;
            Vector3 dir = this.Target.position - me.position;
            float ang = Vector3.Angle(dir, me.forward);
            return ang >= -45 && ang <= 45 || ang >= 315;
        }

        /// <summary>
        /// Tells <see cref="FlightController"/> to face target.
        /// </summary>
        protected void TurnToTarget() {
            if (!this._hasTarget) {
                return;
            }

            Vector3 dir = this.Target.position - this.transform.position;
            this._control.Bearing=dir;
        }

        /// <summary>
        /// The objected selected event handler. Sets <see cref="Target"/> to
        /// the selected object.
        /// </summary>
        protected void OnSelectTarget(ExtendedEventArgs arg0) {
            this.Target = arg0.Source.transform;
            this._hasTarget = true;
            this._shouldMove = true;
        }
    }
}
