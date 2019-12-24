// ---------------------------------------------------------------------------
// <copyright file="IFlightAi.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    /// <summary>
    /// Logic that makes objects move. Sub-class of this interface are
    /// supposed to work with concrete implementation of
    /// <see cref="IFlightController" />. To concrete sub-classes of this
    /// interface, you'll likely want to add the attribute
    /// <c>[RequireComponent(typeof(...))]</c> to a concrete class that
    /// implements the <see cref="IFlightController" />.
    /// </summary>
    public interface IFlightAi
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
