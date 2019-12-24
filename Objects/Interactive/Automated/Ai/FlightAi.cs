// ---------------------------------------------------------------------------
// <copyright file="FlightAi.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Interactive.Automated.Ai
{
    /// <inheritdoc cref="IFlightAi" />
    public abstract class FlightAi : MonoBehaviour, IFlightAi
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once InconsistentNaming
        protected Transform _target;

        /// <inheritdoc />
        public virtual bool HasTarget { get; protected set; }

        /// <inheritdoc />
        public virtual Transform Target {
            get => this._target;
            set {
                this._target = value;
                this.HasTarget = null != value;
            }
        }
    }
}
