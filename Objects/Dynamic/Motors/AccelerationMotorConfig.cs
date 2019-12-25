// ---------------------------------------------------------------------------
// <copyright file="AccelerationMotorConfig.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Dynamic.Motors
{
    /// <summary>Configuration data to <see cref="AccelerationMotor" />.</summary>
    public struct AccelerationMotorConfig : IMotorConfig
    {
        /// <summary>Maximum speed limit.</summary>
        public float MaxSpeed;

        /// <summary>Maximum rotation speed.</summary>
        public float MaxTurn;

        /// <summary>Full throttle forward acceleration value.</summary>
        public float MaxAcceleration;

        /// <summary>Full reverse acceleration value.</summary>
        public float MaxDeceleration;

        /// <summary>Current rotation quaternion.</summary>
        public Quaternion Rotation;
    }
}
