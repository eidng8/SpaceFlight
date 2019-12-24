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
        MaxSpeed,
        MaxTurn,
        MaxAcceleration,
        MaxDeceleration,

        /// <summary>Current rotation quaternion</summary>
        Rotation
    }


    public class AccelerationMotor : ThrottledMotor
    {
        private float _acceleration;
        private Vector3 _bearing;
        private float _maxAcceleration;
        private float _maxDeceleration;
        private float _maxSpeed;
        private float _maxTurn;
        private Quaternion _roll;
        private Vector3 _turnTarget;
        private float _velocity;

        /// <summary>Construct a new instance and apply the given configuration.</summary>
        /// <param name="config"></param>
        public AccelerationMotor(Dictionary<int, object> config)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.Configure(config);
        }

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
                this._maxSpeed = (float)v;
            }

            this._maxTurn = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxTurn,
                out v
            )) {
                this._maxTurn = (float)v;
            }

            this._maxAcceleration = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxAcceleration,
                out v
            )) {
                this._maxAcceleration = (float)v;
            }

            this._maxDeceleration = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxDeceleration,
                out v
            )) {
                this._maxDeceleration = (float)v;
            }

            this._roll = Quaternion.identity;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.Rotation,
                out v
            )) {
                this._roll = (Quaternion)v;
            }
        }

        /// <summary>Returns the next rotation quaternion in <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public Quaternion GetRoll(float deltaTime)
        {
            if (this._turnTarget == Vector3.zero) {
                return Quaternion.identity;
            }

            Quaternion look = Quaternion.LookRotation(this._turnTarget);
            float thrust = this.GetRollThrust(deltaTime);
            this._roll = Quaternion.Lerp(this._roll, look, thrust);

            return this._roll;
        }

        /// <inheritdoc />
        public override float GetRollThrust(float deltaTime) =>
            this._maxTurn * deltaTime;

        /// <summary>Current acceleration value.</summary>
        public override float GetThrust() => this.Acceleration;

        /// <summary>Calculates the velocity value in <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public float GetVelocity(float deltaTime)
        {
            this._velocity = Mathf.Clamp(
                this._velocity + this.Acceleration * deltaTime,
                0,
                this._maxSpeed
            );
            return this._velocity;
        }

        /// <inheritdoc />
        public override void TurnTo(Vector3 target)
        {
            this._turnTarget = target;
        }
    }
}
