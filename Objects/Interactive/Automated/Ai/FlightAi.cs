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
    /// <typeparam name="TC">
    /// The type of <see cref="IFlightController" /> implementation to work
    /// with.
    /// </typeparam>
    public abstract class FlightAi<TC> : MonoBehaviour, IFlightAi
        where TC : IFlightController
    {
        private TC _control;
        private bool _controlAttached;
        private bool _listeningEvents;
        private Transform _target;

        /// <inheritdoc />
        public bool HasTarget { get; private set; }

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

        /// <summary>
        /// Calculates the appropriate rotation, and applies to the attached
        /// flight controller. This method <i>must</i> directly calls the
        /// controller's <c>TurnTo()</c> method.
        /// </summary>
        protected abstract void TurnToTarget();
    }
}
