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
    /// State of existence
    /// </summary>
    public struct Existence
    {
        public float Mass;

        /// <summary>
        /// It is for ease of use in Unity, that this field is declared in
        /// `Transform` type. It shall be serialized into three 3D vectors
        /// of position, rotation, and scale, for network transmission.
        /// </summary>
        public Transform Transform;

        public Existence(float mass, Transform transform)
        {
            this.Mass = mass;
            this.Transform = transform;
        }
    }
}
