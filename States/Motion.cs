// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;

namespace eidng8.SpaceFlight.States
{
    /// <summary>
    /// State of motion
    /// </summary>
    public struct Motion
    {
        /// <summary>
        /// Main thrust that pushes the object forward or backward.
        /// </summary>
        public float Speed;

        /// <summary>
        /// Maximum force of main thrust. The <c>x</c> vector is the
        /// forward threshold, the <c>y</c> vector is the backward threshold.
        /// </summary>
        public float SpeedMax;

        /// <summary>
        /// Force to object's sides to make it turn.
        /// </summary>
        public Quaternion Bearing;

        /// <summary>
        /// Maximum force of torque.
        /// </summary>
        public float TurnMax;

        public Motion(float maxSpeed, float maxTurn)
        {
            this.Speed = 0;
            this.SpeedMax = maxSpeed;
            this.Bearing = Quaternion.identity;
            this.TurnMax = maxTurn;
        }
    }
}
