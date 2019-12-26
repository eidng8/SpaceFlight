// ---------------------------------------------------------------------------
// <copyright file="AccelerationAutoPilot.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using eidng8.SpaceFlight.Objects.Interactive.Pilot.Ai;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Controllers
{
    public class AccelerationAutoPilot
        : AccelerationController<AccelerationAi, AccelerationAiConfig>
    {
        protected override void Awake()
        {
            base.Awake();
            this.Pilot.Control = this;
        }
    }
}
