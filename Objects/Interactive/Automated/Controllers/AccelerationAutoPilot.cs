// ---------------------------------------------------------------------------
// <copyright file="AccelerationAutoPilot.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Controllers
{
    public class AccelerationAutoPilot : AccelerationController
    {
        protected override void Awake()
        {
            base.Awake();
            this.Pilot.Control = this;
        }
    }
}
