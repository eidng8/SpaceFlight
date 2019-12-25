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
    /// <inheritdoc />
    /// <remarks>A motor that works on constant acceleration.</remarks>
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class AccelerationMotor : ThrottledMotor<AccelerationMotorConfig>
    {
        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected Quaternion _roll;

        // ReSharper disable once InconsistentNaming
        // ReSharper disable once MemberCanBePrivate.Global
        protected float _velocity;

        /// <inheritdoc />
        public AccelerationMotor(AccelerationMotorConfig config) :
            base(config) { }

        /// <inheritdoc />
        public override float Acceleration =>
            (this.Throttle < 0
                ? this.Config.MaxDeceleration
                : this.Config.MaxAcceleration)
            * this.Throttle;

        /// <inheritdoc />
        public override void Configure(IMotorConfig config)
        {
            base.Configure(config);
            this._roll = this.Config.Rotation;
        }

        /// <inheritdoc />
        /// <returns>The acceleration value.</returns>
        public override float GenerateThrust() => this.Acceleration;

        /// <inheritdoc />
        /// <returns>The rotation delta.</returns>
        public override float GenerateTorque(float deltaTime) =>
            this.Config.MaxTurn * deltaTime;

        /// <summary>Returns the next rotation quaternion in <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public virtual Quaternion GetRoll(float deltaTime)
        {
            if (this._turnTarget == Vector3.zero) {
                return Quaternion.identity;
            }

            Quaternion look = Quaternion.LookRotation(this._turnTarget);
            float thrust = this.GenerateTorque(deltaTime);
            this._roll = Quaternion.Lerp(this._roll, look, thrust);

            return this._roll;
        }

        /// <summary>Calculates the velocity value in <c>deltaTime</c>.</summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public virtual float GetVelocity(float deltaTime)
        {
            this._velocity = Mathf.Clamp(
                this._velocity + this.Acceleration * deltaTime,
                0,
                this.Config.MaxSpeed
            );
            return this._velocity;
        }
    }
}
