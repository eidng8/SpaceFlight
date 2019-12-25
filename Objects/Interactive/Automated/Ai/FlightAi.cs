// ---------------------------------------------------------------------------
// <copyright file="FlightAi.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using eidng8.SpaceFlight.Events;
using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Ai
{
    /// <inheritdoc cref="IFlightAi" />
    public abstract class FlightAi<TC> : MonoBehaviour, IFlightAi
    {
        private TC _control;
        private bool _controlAttached;
        private bool _listeningEvents;
        private Transform _target;

        /// <inheritdoc />
        public virtual bool HasTarget { get; protected set; }

        /// <inheritdoc />
        public Transform Target {
            get => this._target;
            set {
                this._target = value;
                this.HasTarget = null != value;
            }
        }

        /// <summary>
        /// Reference to the attached <see cref="IFlightController" />
        /// implementation.
        /// </summary>
        protected TC Control {
            get {
                if (this._controlAttached) {
                    return this._control;
                }

                this._control = this.GetComponent<TC>();
                this._controlAttached = true;
                return this._control;
            }
        }

        protected void FixedUpdate()
        {
            this.GenerateTorque();
            this.GenerateThrust();
        }

        protected abstract void GenerateThrust();

        protected abstract void GenerateTorque();

        protected virtual void OnEnable()
        {
            if (this._listeningEvents) {
                return;
            }

            this.RegisterEvents();
            this._listeningEvents = true;
        }

        /// <summary>
        /// The objected selected event handler. Sets <c>Target</c> to the
        /// selected object.
        /// </summary>
        protected virtual void OnSelectTarget(ExtendedEventArgs arg0)
        {
            this.Target = arg0.Source.transform;
        }

        /// <summary>Register listeners to game events.</summary>
        protected virtual void RegisterEvents()
        {
            EventManager.Mgr.OnUserEvent(
                UserEvents.Select,
                this.OnSelectTarget
            );
        }
    }
}
