// ---------------------------------------------------------------------------
// <copyright file="AccelerationWanderer.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using eidng8.SpaceFlight.Objects.Dynamic.Motors;
using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Controllers
{
    public class AccelerationWanderer : AccelerationAutoPilot
    {
        private float _lastChoiceTime;

        private GameObject _waypoint;

        private bool _shouldDestroyWaypoint;

        /// <inheritdoc />
        protected override void Awake()
        {
            this.pilotConfig.playerShip = false;
            base.Awake();
        }

        /// <inheritdoc />
        protected override void FixedUpdate()
        {
            this.Wander();
            base.FixedUpdate();
        }

        private void Wander()
        {
            // don't change decision within 5s
            float t = Time.fixedTime - this._lastChoiceTime - Random.value * 10;
            if (t < 5) {
                return;
            }

            AccelerationMotorConfig mc = this.motorConfig;
            float range = Random.Range(mc.maxSpeed, mc.maxSpeed * 10);
            Vector3 v = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            );

            var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.transform.localScale = Vector3.zero;
            target.transform.position = v * range;
            GameObject wp = this._waypoint;

            this.Pilot.Target = target.transform;
            this._waypoint = target;
            if (this._shouldDestroyWaypoint) {
                Object.DestroyImmediate(wp);
            }

            this._lastChoiceTime = Time.fixedTime;
            this._shouldDestroyWaypoint = true;
        }
    }
}
