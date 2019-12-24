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
        MaxDeceleration
    }


    public class AccelerationMotor : ThrottledMotor, IMotor
    {
        private float _acceleration;
        private Vector3 _bearing;
        private float _maxAcceleration;
        private float _maxDeceleration;
        private float _maxSpeed;
        private float _maxTurn;
        private float _roll;
        private Vector3 _turnTarget;
        private float _velocity;

        public AccelerationMotor(Dictionary<int, object> config)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            Configure(config);
        }


        /// <inheritdoc />
        public override float Acceleration =>
            (Throttle < 0 ? _maxDeceleration : _maxAcceleration)
            * Throttle;

        /// <inheritdoc />
        public override Vector3 Bearing {
            get => _bearing;
            set => _bearing = value;
        }

        /// <inheritdoc />
        public override void Configure(Dictionary<int, object> config)
        {
            object v;
            _maxSpeed = 100;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxSpeed,
                out v
            )) {
                _maxSpeed = (float)v;
            }

            _maxTurn = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxTurn,
                out v
            )) {
                _maxTurn = (float)v;
            }

            _maxAcceleration = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxAcceleration,
                out v
            )) {
                _maxAcceleration = (float)v;
            }

            _maxDeceleration = 10;
            if (config.TryGetValue(
                (int)AccelerationMotorAttributes.MaxDeceleration,
                out v
            )) {
                _maxDeceleration = (float)v;
            }
        }

        /// <inheritdoc />
        public override float GetRoll(float deltaTime) =>
            _maxTurn * Time.deltaTime;

        /// <summary>Current acceleration value.</summary>
        public override float GetThrust() => Acceleration;

        /// <summary>Calculates the velocity value after <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public float GetVelocity(float deltaTime)
        {
            _velocity = Mathf.Clamp(
                _velocity + Acceleration * deltaTime,
                0,
                _maxSpeed
            );
            return _velocity;
        }

        /// <inheritdoc />
        public override void TurnTo(Vector3 target)
        {
            _turnTarget = target;
        }
    }
}
