// ---------------------------------------------------------------------------
// <copyright file="FlightController.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    public interface IFlightController
    {
        /// <summary>Returns the current acceleration value.</summary>
        float Acceleration { get; }

        /// <summary>Current thrust value.</summary>
        float Throttle { get; set; }

        /// <summary>Returns the current velocity magnitude.</summary>
        float Velocity { get; }

        /// <summary>Calculates distance to the given target.</summary>
        /// <param name="target"></param>
        /// <returns></returns>
        float DistanceTo(Vector3 target);

        /// <summary>
        /// Estimates the arrival time according to current velocity and acceleration.
        /// </summary>
        float EstimatedArrival(float distance);

        /// <summary>Full throttle backward, or decelerate.</summary>
        void FullReverse();

        /// <summary>Completely turn off thrust.</summary>
        void FullStop();

        /// <summary>Full throttle forward.</summary>
        void FullThrottle();

        /// <summary>
        /// Determine if we are facing the target. Facing doesn't mean we are directly
        /// facing it, we can have around ±45º buffer by default.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        bool IsFacing(Vector3 target, float tolerance = 45);

        /// <summary>Rotate the object to face the given target.</summary>
        /// <param name="target"></param>
        void TurnTo(Vector3 target);
    }
}
