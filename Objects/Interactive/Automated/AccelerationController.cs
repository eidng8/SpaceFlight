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
using eidng8.SpaceFlight.States;
using UnityEngine;
using Motion = eidng8.SpaceFlight.States.Motion;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    /// <inheritdoc />
    /// <remarks>
    /// This controller uses acceleration. So it's not fully physical. Physics used:
    /// Motion with constant acceleration.
    /// </remarks>
    [RequireComponent(typeof(Rigidbody))]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AccelerationController : FlightController
    {
        /// <summary>
        /// Full throttle acceleration. This is used to speed up the object until it
        /// reaches <see cref="maxSpeed" />.
        /// </summary>
        [Tooltip(
            "Full throttle acceleration. This is used to speed up the object"
            + " until it reaches maxSpeed"
        )]
        public float maxAcceleration = 20;

        /// <summary>
        /// Maximum deceleration. This is used to slow down the object until fully stopped.
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

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once InconsistentNaming
        protected FlightState _state;

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once InconsistentNaming
        protected float _velocity;

        /// <inheritdoc />
        public override float Acceleration {
            get {
                if (!(this.Motor is AccelerationMotor motor)) {
                    return 0;
                }

                return motor.Acceleration;
            }
        }

        /// <inheritdoc />
        public override float Velocity => this._velocity;

        /// <summary>State data of the game object.</summary>
        protected virtual FlightState State {
            get {
                if (null == this._state) {
                    this.Init();
                }

                return this._state;
            }
            set => this._state = value;
        }

        /// <summary>
        /// Calculate and apply velocity to game object. It should be called in
        /// <c>FixedUpdate()</c>. Also updates the <see cref="State" />.
        /// <para>
        /// Remember that it is doing actual physics calculation here. Though Unity reliefs
        /// us from a lot of burden. We still need to have Newton's Laws in mind to
        /// understand the outcome of such calculation.
        /// </para>
        /// </summary>
        protected virtual void ApplySpeed()
        {
            if (!(this.Motor is AccelerationMotor motor)) {
                return;
            }

            this._velocity = motor.GetVelocity(Time.fixedDeltaTime);

            Motion mo = this.State.Motion;
            if (mo.Speed.Equals(this.Velocity)) {
                return;
            }

            mo.Speed = this.Velocity;
            this.State.Motion = mo;
            this.Body.velocity = this.Velocity * this.transform.forward;

            if (this.Velocity.Equals(0)) {
                this.FullStop();
            }
        }

        /// <summary>Actually makes the turn. Also updates the <see cref="State" />.</summary>
        protected virtual void ApplyTurn()
        {
            if (!(this.Motor is AccelerationMotor motor)) {
                return;
            }

            Motion mo = this.State.Motion;
            Quaternion bearing = motor.GetRoll(Time.fixedDeltaTime);

            this.transform.rotation = bearing;
            mo.Bearing = bearing;
            this.State.Motion = mo;
        }

        protected void FixedUpdate()
        {
            this.UpdateExistence();
            this.ApplyTurn();
            this.ApplySpeed();
        }

        /// <summary>
        /// Initialize the instance, allocating a new <see cref="FlightState" /> instance
        /// with default values.
        /// </summary>
        protected virtual void Init()
        {
            this.State = new FlightState(
                new Existence(this.Body.mass, this.transform),
                new Motion(this.maxSpeed, this.maxTurn)
            );
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

        protected void Reset()
        {
            this.Init();
        }

        /// <summary>
        /// Update <see cref="State" /> information from underlying game object.
        /// </summary>
        protected virtual void UpdateExistence()
        {
            this.State.Existence =
                new Existence(this.Body.mass, this.transform);
        }
    }
}
