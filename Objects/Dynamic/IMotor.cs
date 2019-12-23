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



namespace eidng8.SpaceFlight.Objects.Dynamic
{
    public interface IMotor
    {
        /// <summary>
        /// Current acceleration value.
        /// </summary>
        float Acceleration { get; }

        /// <summary>
        /// Current throttle value.
        /// </summary>
        float Throttle { get; set; }

        /// <summary>
        /// Current bearing value.
        /// </summary>
        Vector3 Bearing { get; set; }

        void Configure(Dictionary<int, object> config);

        /// <summary>
        /// Convenient method to apply max <see cref="Throttle"/>.
        /// </summary>
        void FullThrottle();

        /// <summary>
        /// Convenient method to apply zero <see cref="Throttle"/>.
        /// </summary>
        void FullStop();

        /// <summary>
        /// Convenient method to apply max reverse <see cref="Acceleration"/>.
        /// </summary>
        void FullReverse();

        /// <summary>
        /// Turn to face the <c>target</c>.
        /// </summary>
        /// <param name="target"></param>
        void TurnTo(Vector3 target);

        /// <summary>
        /// Current forward thrust value.
        /// </summary>
        float GetThrust(float deltaTime);

        /// <summary>
        /// Current rotation value.
        /// </summary>
        float GetRoll(float deltaTime);
    }
}
