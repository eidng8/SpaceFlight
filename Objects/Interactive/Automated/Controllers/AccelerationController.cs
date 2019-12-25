// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System.Collections.Generic;
using eidng8.SpaceFlight.Objects.Dynamic.Motors;
using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Controllers
{
    /// <inheritdoc />
    /// <remarks>
    /// This controller uses acceleration. So it's not fully physical.
    /// Physics used: Motion with constant acceleration.
    /// </remarks>
    [RequireComponent(typeof(Rigidbody))]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AccelerationController : FlightController<AccelerationMotor>
    {
        /// <summary>
        /// Full throttle acceleration. This is used to speed up the object
        /// until it reaches <see cref="maxSpeed" />.
        /// </summary>
        [Tooltip(
            "Full throttle acceleration. This is used to speed up the object"
            + " until it reaches maxSpeed"
        )]
        public float maxAcceleration = 20;

        /// <summary>
        /// Maximum deceleration. This is used to slow down the object until
        /// fully stopped.
        /// </summary>
        [Tooltip(
            "Maximum deceleration. This is used to slow down the object until"
            + " fully stopped."
        )]
        public float maxDeceleration = 5;

        /// <summary>Maximum forward velocity.</summary>
        [Tooltip("Maximum forward velocity."), Range(0, 300000)]
        public float maxSpeed = 200;

        /// <summary>Determines how quickly can the object turn on its sides.</summary>
        [Tooltip("Determines how quickly can the object turn."), Range(0, 360)]
        public float maxTurn = 10;

        /// <summary>
        /// Calculate and apply velocity to game object. It should be called in
        /// <c>FixedUpdate()</c>.
        /// <para>
        /// Remember that it is doing actual physics calculation here. Though
        /// Unity reliefs us from a lot of burden. We still need to have
        /// Newton's Laws in mind to understand the outcome of such
        /// calculation.
        /// </para>
        /// </summary>
        protected virtual void ApplySpeed()
        {
            float velocity = this.Motor.GetVelocity(Time.fixedDeltaTime);
            this.Body.velocity = velocity * this.transform.forward;
            if (velocity.Equals(0)) {
                this.FullStop();
            }
        }

        /// <summary>Actually makes the turn.</summary>
        protected virtual void ApplyTurn()
        {
            Quaternion bearing = this.Motor.GetRoll(Time.fixedDeltaTime);
            this.transform.rotation = bearing;
        }

        protected void FixedUpdate()
        {
            this.ApplyTurn();
            this.ApplySpeed();
        }

        protected void OnEnable()
        {
            Dictionary<int, object> config = new Dictionary<int, object> {
                [(int)AccelerationMotorAttributes.MaxTurn] = this.maxTurn,
                [(int)AccelerationMotorAttributes.MaxSpeed] = this.maxSpeed,
                [(int)AccelerationMotorAttributes.MaxAcceleration] =
                    this.maxAcceleration,
                [(int)AccelerationMotorAttributes.MaxDeceleration] =
                    this.maxDeceleration,
                [(int)AccelerationMotorAttributes.Rotation] =
                    this.transform.rotation
            };
            this.Motor = new AccelerationMotor(config);
        }
    }
}
