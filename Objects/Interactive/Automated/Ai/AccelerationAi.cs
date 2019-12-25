// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using eidng8.SpaceFlight.Objects.Interactive.Automated.Controllers;
using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Ai
{
    /// <inheritdoc />
    /// <remarkes>
    /// This component works with <see cref="AccelerationController" />.
    /// </remarkes>
    [RequireComponent(typeof(AccelerationController))]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AccelerationAi : FlightAi<AccelerationController>
    {
        /// <summary>The distance to keep from target.</summary>
        [Tooltip("The distance to keep from target.")]
        public float safeDistance = 5;

        /// <summary>Determine acceleration throttle.</summary>
        protected override void GenerateThrust()
        {
            if (!this.HasTarget) {
                return;
            }

            // If we start accelerating while facing away from the target,
            // we'll make a bit of roundabout. So we don't do this.
            if (!this.Control.IsFacing(this.Target.position)) {
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
        /// Tells <see cref="AccelerationController" /> to face
        /// target.
        /// </summary>
        protected override void GenerateTorque()
        {
            if (!this.HasTarget) {
                return;
            }

            Vector3 dir = this.Target.position - this.transform.position;
            this.Control.TurnTo(dir);
        }

        /// <summary>
        /// Determines whether the object should start slowing down in order to
        /// stop closet the target position.
        /// </summary>
        protected virtual bool ShouldBrake()
        {
            AccelerationController control = this.Control;
            float v = control.Velocity;
            float a = control.maxDeceleration;

            // We calculate how much time is needed for the speed to reach `v`
            // with acceleration `a`. From deceleration point of view, this
            // means how much time is needed to stop completely.
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
    }
}
