// ---------------------------------------------------------------------------
// <copyright file="IPilot.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Pilot
{
    public interface IPilot
    {
        /// <summary>Whether a target has been chosen.</summary>
        /// <remarks>
        /// Directly check the <see cref="Target" /> against <c>null</c> is an
        /// expensive operation. So we have to use this field to track the
        /// status of target selection.
        /// </remarks>
        bool HasTarget { get; }

        /// <summary>Reference to the selected target object.</summary>
        Transform Target { get; set; }
    }
}
