using System;
using UnityEngine;
using Random = UnityEngine.Random;
using eidng8.SpaceFlight.Events;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    [RequireComponent(typeof(FlightController))]
    public class FlightAI : MonoBehaviour
    {
        public float safeDistance = 5;

        protected FlightController _control;

        protected bool _hasTarget = false;

        protected bool _shouldMove = false;

        public Transform Target { get; set; }

        public bool HasTarget => this._hasTarget;

        protected void Start() {
            this._control = this.GetComponent<FlightController>();
        }

        protected void Awake() {
            EventManager.Mgr.OnUserEvent(UserEvents.Select,
                this.OnSelectTarget);
        }

        protected void Update() {
            if (Input.GetKeyDown(KeyCode.W)) {
                this._control.FullThrottle();
                Debug.Log("Forward thrust applied.");
            }

            if (Input.GetKeyDown(KeyCode.S)) {
                this._control.FullStop();
                Debug.Log("Backward thrust applied.");
            }
        }

        protected void FixedUpdate() {
            this.TurnToTarget();
            this.DetermineThrottle();
        }

        protected float TargetDistance() {
            return Vector3.Distance(this.transform.position,
                this.Target.position);
        }

        protected bool ShouldBrake() {
            FlightController control = this._control;
            float v = control.Vc;
            float a = control.deceleration;
            float t = v / a;
            float d = this.TargetDistance() - this.safeDistance;
            return v * t / 2 >= d;
        }

        protected void DetermineThrottle() {
            if (!this._hasTarget || !this._shouldMove) {
                return;
            }

            if (!this.IsFacingTarget()) {
                this._control.FullStop();
                return;
            }

            if (this.ShouldBrake()) {
                this._control.FullStop();
                return;
            }

            this._control.FullThrottle();
        }

        protected bool IsFacingTarget() {
            Transform me = this.transform;
            Vector3 dir = this.Target.position - me.position;
            float ang = Vector3.Angle(dir, me.forward);
            return ang >= -45 && ang <= 45 || ang >= 315;
        }

        protected void TurnToTarget() {
            if (!this._hasTarget) {
                return;
            }

            Vector3 dir = this.Target.position - this.transform.position;
            this._control.Bearing=dir;
        }

        protected void OnSelectTarget(ExtendedEventArgs arg0) {
            this.Target = arg0.Source.transform;
            this._hasTarget = true;
            this._shouldMove = true;
        }
    }
}
