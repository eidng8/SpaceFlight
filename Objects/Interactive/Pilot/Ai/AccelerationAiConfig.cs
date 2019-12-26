// ---------------------------------------------------------------------------
// <copyright file="AccelerationAiConfig.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System;
using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Pilot.Ai
{
    [Serializable]
    public struct AccelerationAiConfig : IPilotConfig
    {
        /// <summary>The distance to keep from target.</summary>
        [Tooltip("The distance to keep from target.")]
        public float safeDistance;
    }
}
