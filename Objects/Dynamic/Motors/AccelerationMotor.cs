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
    public enum AccelerationMotorAttributes
    {
        MaxSpeed,
        MaxTurn,
        MaxAcceleration,
        MaxDeceleration,
    }


    public class AccelerationMotor : ThrottledMotor, IMotor
    {
        private float _maxSpeed;
        private float _maxTurn;
        private float _maxAcceleration;
        private float _maxDeceleration;

        private float _acceleration;
        private float _velocity;
        private Vector3 _bearing;
        private float _roll;
        private Vector3 _turnTarget;


        /// <inheritdoc />
        public override float Acceleration => this._acceleration;

        public AccelerationMotor(Dictionary<int, object> config)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.Configure(config);
        }

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
        }

        /// <inheritdoc />
        public override Vector3 Bearing {
            get => this._bearing;
            set => this._bearing = value;
        }

        /// <inheritdoc />
        public override void TurnTo(Vector3 target)
        {
            this._turnTarget = target;
        }

        public float GetVelocity(float deltaTime)
        {
            this._velocity = Mathf.Clamp(
                this._velocity + this.GetThrust(deltaTime),
                0,
                this._maxSpeed
            );
            return this._velocity;
        }

        /// <summary>
        /// Current velocity magnitude.
        /// </summary>
        public override float GetThrust(float deltaTime)
        {
            float thrust = this.Throttle < 0
                ? this._maxDeceleration * this.Throttle
                : this._maxAcceleration * this.Throttle;
            return thrust * deltaTime;
        }

        /// <inheritdoc />
        public override float GetRoll(float deltaTime) =>
            this._maxTurn * Time.deltaTime;
    }
}
