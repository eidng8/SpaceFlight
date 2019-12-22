// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System;
using eidng8.SpaceFlight.States;
using Motion = eidng8.SpaceFlight.States.Motion;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    /// <summary>
    /// In flight state data.
    /// </summary>
    public class FlightState : StateObject
    {
        public Existence Existence { get; set; }

        public Motion Motion { get; set; }

        public FlightState()
        {
            this.Existence = new Existence();
            this.Motion = new Motion(0, 0);
        }

        public FlightState(Existence existence, Motion motion)
        {
            this.Existence = existence;
            this.Motion = motion;
        }

        /// <inheritdoc />
        public override void SerializeForTransmission()
        {
            throw new NotImplementedException();
        }
    }
}
