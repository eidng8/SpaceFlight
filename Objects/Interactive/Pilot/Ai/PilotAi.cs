// ---------------------------------------------------------------------------
// <copyright file="PilotAi.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using eidng8.SpaceFlight.Objects.Interactive.Automated;


namespace eidng8.SpaceFlight.Objects.Interactive.Pilot.Ai
{
    /// <inheritdoc cref="IPilotAi" />
    /// <typeparam name="TC">
    /// The type of <see cref="IFlightController" /> implementation to work
    /// with.
    /// </typeparam>
    public abstract class PilotAi<TC> : Pilot<TC>, IPilotAi
        where TC : IFlightController
    {
        /// <summary>
        /// Calculates the appropriate throttle, and applies to the attached
        /// flight controller. This method <i>must</i> directly sets the
        /// controller's <c>Throttle</c>.
        /// </summary>
        protected abstract void DetermineThrottle();

        protected void FixedUpdate()
        {
            this.TurnToTarget();
            this.DetermineThrottle();
        }

        /// <summary>
        /// Calculates the appropriate rotation, and applies to the attached
        /// flight controller. This method <i>must</i> directly calls the
        /// controller's <c>TurnTo()</c> method.
        /// </summary>
        protected abstract void TurnToTarget();
    }
}
