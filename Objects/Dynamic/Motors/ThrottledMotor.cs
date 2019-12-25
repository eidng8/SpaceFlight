// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Dynamic.Motors
{
    /// <summary>A motor whose thrust is clamped to rang [-1,1].</summary>
    public abstract class ThrottledMotor<TC> : IMotor
        where TC : IMotorConfig, new()
    {
        // ReSharper disable once InconsistentNaming
        protected Vector3 _turnTarget;

        private IMotorConfig _config;

        private float _throttle;

        protected ThrottledMotor()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.Configure(new TC());
        }

        protected ThrottledMotor(TC config)
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            this.Configure(config);
        }

        /// <inheritdoc />
        public abstract float Acceleration { get; }

        /// <inheritdoc />
        /// <remarks>It is clamped to [-1, 1]</remarks>
        public float Throttle {
            get => this._throttle;
            set => this._throttle = Mathf.Clamp(value, -1, 1);
        }

        protected TC Config {
            get {
                if (this._config is TC config) {
                    return config;
                }

                config = new TC();
                return config;
            }
        }

        /// <inheritdoc />
        public virtual void Configure(IMotorConfig config) =>
            this._config = config;

        /// <inheritdoc />
        public void FullReverse() => this._throttle = -1;

        /// <inheritdoc />
        public void FullStop() => this._throttle = 0;

        /// <inheritdoc />
        public void FullThrottle() => this._throttle = 1;

        /// <inheritdoc />
        public abstract float GenerateThrust();

        /// <inheritdoc />
        public abstract float GenerateTorque(float deltaTime);

        /// <inheritdoc />
        public virtual void TurnTo(Vector3 target)
        {
            this._turnTarget = target;
        }
    }
}
