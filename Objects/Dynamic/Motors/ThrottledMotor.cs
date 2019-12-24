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
    public abstract class ThrottledMotor : IMotor
    {
        private float _throttle;

        /// <inheritdoc />
        public abstract float Acceleration { get; }

        /// <inheritdoc />
        public abstract Vector3 Bearing { get; set; }

        /// <inheritdoc />
        public float Throttle {
            get => _throttle;
            set => _throttle = Mathf.Clamp(value, -1, 1);
        }

        /// <inheritdoc />
        public abstract void Configure(Dictionary<int, object> config);

        /// <inheritdoc />
        public void FullReverse() => _throttle = -1;

        /// <inheritdoc />
        public void FullStop() => _throttle = 0;

        /// <inheritdoc />
        public void FullThrottle() => _throttle = 1;

        /// <inheritdoc />
        public abstract float GetRoll(float deltaTime);

        /// <inheritdoc />
        public abstract float GetThrust();

        /// <inheritdoc />
        public abstract void TurnTo(Vector3 target);
    }
}
