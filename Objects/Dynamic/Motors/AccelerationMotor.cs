// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Dynamic.Motors
{
    /// <summary><see cref="AccelerationMotor" /> configuration attributes.</summary>
    public enum AccelerationMotorAttributes
    {
        /// <summary>Maximum speed limit. Value type is <c>float</c>.</summary>
        MaxSpeed,

        /// <summary>Maximum rotation speed. Value type is <c>float</c>.</summary>
        MaxTurn,

        /// <summary>
        /// Full throttle forward acceleration value. Value type is
        /// <c>float</c>.
        /// </summary>
        MaxAcceleration,

        /// <summary>
        /// Full reverse acceleration value. Value type is
        /// <c>float</c>.
        /// </summary>
        MaxDeceleration,

        /// <summary>
        /// Current rotation quaternion. Value type is
        /// <c>Quaternion</c>.
        /// </summary>
        Rotation
    }


    /// <inheritdoc />
    /// <remarks>A motor that works on constant acceleration.</remarks>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AccelerationMotor : ThrottledMotor
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected float _maxAcceleration;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected float _maxDeceleration;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected float _maxSpeed;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected float _maxTurn;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected Quaternion _roll;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected float _velocity;

        /// <inheritdoc />
        public AccelerationMotor(Dictionary<int, object> config) :
            base(config) { }

        /// <inheritdoc />
        public override float Acceleration =>
            (this.Throttle < 0 ? this._maxDeceleration : this._maxAcceleration)
            * this.Throttle;

        /// <inheritdoc />
        public override void Configure(Dictionary<int, object> config)
        {
            object v;
            this._maxSpeed = 100;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxSpeed,
                out v
            )) {
                if (v is float f) {
                    this._maxSpeed = f;
                }
            }

            this._maxTurn = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxTurn,
                out v
            )) {
                if (v is float f) {
                    this._maxTurn = f;
                }
            }

            this._maxAcceleration = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxAcceleration,
                out v
            )) {
                if (v is float f) {
                    this._maxAcceleration = f;
                }
            }

            this._maxDeceleration = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxDeceleration,
                out v
            )) {
                if (v is float f) {
                    this._maxDeceleration = f;
                }
            }

            this._roll = Quaternion.identity;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.Rotation,
                out v
            )) {
                if (v is Quaternion f) {
                    this._roll = f;
                }
            }
        }

        /// <inheritdoc />
        /// <returns>The acceleration value.</returns>
        public override float GenerateThrust() => this.Acceleration;

        /// <inheritdoc />
        /// <returns>The rotation delta.</returns>
        public override float GenerateTorque(float deltaTime) =>
            this._maxTurn * deltaTime;

        /// <summary>Returns the next rotation quaternion in <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public virtual Quaternion GetRoll(float deltaTime)
        {
            if (this._turnTarget == Vector3.zero) {
                return Quaternion.identity;
            }

            Quaternion look = Quaternion.LookRotation(this._turnTarget);
            float thrust = this.GenerateTorque(deltaTime);
            this._roll = Quaternion.Lerp(this._roll, look, thrust);

            return this._roll;
        }

        /// <summary>Calculates the velocity value in <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public virtual float GetVelocity(float deltaTime)
        {
            this._velocity = Mathf.Clamp(
                this._velocity + this.Acceleration * deltaTime,
                0,
                this._maxSpeed
            );
            return this._velocity;
        }
    }
}
