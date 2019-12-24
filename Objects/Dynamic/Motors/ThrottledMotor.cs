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
    /// <summary>A motor whose thrust is clamped within certain limit.</summary>
    public abstract class ThrottledMotor : IMotor
    {
        private float _throttle;

        /// <inheritdoc />
        public abstract float Acceleration { get; }

        /// <inheritdoc />
        /// <remarks>It is clamped to [-1, 1]</remarks>
        public float Throttle {
            get => this._throttle;
            set => this._throttle = Mathf.Clamp(value, -1, 1);
        }

        /// <inheritdoc />
        public abstract void Configure(Dictionary<int, object> config);

        /// <inheritdoc />
        public void FullReverse() => this._throttle = -1;

        /// <inheritdoc />
        public void FullStop() => this._throttle = 0;

        /// <inheritdoc />
        public void FullThrottle() => this._throttle = 1;

        /// <inheritdoc />
        public abstract float GetRollThrust(float deltaTime);

        /// <inheritdoc />
        public abstract float GetThrust();

        /// <inheritdoc />
        public abstract void TurnTo(Vector3 target);
    }
}
