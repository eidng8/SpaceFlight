// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

using System.Collections.Generic;
using eidng8.SpaceFlight.Objects.Dynamic.Motors;
using eidng8.SpaceFlight.States;
using UnityEngine;
using Motion = eidng8.SpaceFlight.States.Motion;



namespace eidng8.SpaceFlight.Objects.Interactive.Automated
{
    /// <summary>
    /// FlightController is one of the main handlers of space flight
    /// simulation. It uses acceleration. So it's not fully physical.
    /// Controller act like state machines, providing limited degrees
    /// of autonomy. Such as moving forward, accelerating, etc.
    /// Controllers don't contain any activity logic, such as flying
    /// towards a target. Such logic are implemented by other classes
    /// such as the <see cref="FlightAI"/>.
    ///
    /// <para>
    /// This controller doesn't allow setting velocity or acceleration
    /// directly. Use the <see cref="Throttle"/> property to control
    /// movement acceleration. A <see cref="FullThrottle"/> method is
    /// provided for convenience. Use <see cref="FullStop"/> to cut
    /// acceleration to 0 and start decelerating. There is no way to apply
    /// deceleration directly.
    /// </para>
    /// </summary>
    /// 
    /// <remarks>
    /// Physics used: Motion with constant acceleration.
    ///
    /// <para>
    /// The reason of naming this <c>FlightController</c> instead of
    /// <c>FlightSimulator</c> is to follow Unity's naming convention.
    /// As this script will be added to game objects' script component.
    /// And the main script component of game objects usually ends with
    /// the word <c>Controller</c>.
    /// </para>
    /// 
    /// </remarks>
    [RequireComponent(typeof(Rigidbody))]
    public class FlightController : SpaceObject
    {
        /// <summary>
        /// Maximum forward velocity.
        /// </summary>
        [Tooltip("Maximum forward velocity."), Range(0, 300000)]
        public float maxSpeed = 200;

        /// <summary>
        /// Determines how quickly can the object turn on its sides.
        /// </summary>
        [Tooltip("Determines how quickly can the object turn."), Range(0, 360)]
        public float maxTurn = 10;

        /// <summary>
        /// Full throttle acceleration. This is used to speed up the object
        /// until it reaches <see cref="maxSpeed"/>.
        /// </summary>
        [Tooltip(
            "Full throttle acceleration. This is used to speed up the object"
            + " until it reaches maxSpeed"
        )]
        public float acceleration = 100;

        /// <summary>
        /// Maximum deceleration. This is used to slow down the object until
        /// fully stopped.
        /// </summary>
        [Tooltip(
            "Maximum deceleration. This is used to slow down the object until"
            + " fully stopped."
        )]
        public float deceleration = 200;

        /// <summary>
        /// Current forward velocity. <c>Vc</c> stands for Thrust current.
        /// With the small <c>c</c> as subscript.
        /// </summary>
        public float Vc { get; private set; }

        /// <summary>
        /// Between 0 ~ 1, fraction of full throttle.
        /// The <see cref="acceleration"/> is multiplied by <c>Throttle</c>
        /// during calculation.
        /// </summary>
        public float Throttle {
            get => this.Motor.Throttle;
            set => this.Motor.Throttle = value;
        }

        /// <summary>
        /// The direction of facing.
        /// </summary>
        public Vector3 Bearing { get; set; }

        /// <summary>
        /// State data of the game object.
        /// </summary>
        protected FlightState State {
            get {
                if (null == this._state) {
                    this.Init();
                }

                return this._state;
            }
            set => this._state = value;
        }

        private FlightState _state;

        protected AccelerationMotor Motor;


        /// <summary>
        /// Sets <see cref="Throttle"/> to <c>1</c>,
        /// and <see cref="Stopping"/> to <c>false</c>.
        /// </summary>
        public void FullThrottle()
        {
            this.Motor.FullThrottle();
        }

        /// <summary>
        /// Sets <see cref="Throttle"/> to <c>0</c>,
        /// and <see cref="Stopping"/> to <c>true</c>.
        /// </summary>
        public void FullStop()
        {
            this.Motor.FullStop();
        }

        /// <summary>
        /// Sets <see cref="Throttle"/> to <c>0</c>,
        /// </summary>
        public void FullReverse()
        {
            this.Motor.FullReverse();
        }

        /// <summary>
        /// Calculates the distance to the specified target position.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public float DistanceTo(Vector3 target) =>
            Vector3.Distance(this.transform.position, target);

        /// <summary>
        /// Determine if we are facing the target. Facing doesn't mean we are
        /// directly facing it, we can have around ±45º buffer.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tolerance"></param>
        /// <returns></returns>
        public bool IsFacing(Vector3 target, float tolerance = 45)
        {
            Transform me = this.transform;
            Vector3 dir = target - me.position;
            float ang = Vector3.Angle(dir, me.forward);
            tolerance = Mathf.Clamp(tolerance, 0, 360);
            return ang >= -tolerance && ang <= tolerance
                   || ang >= 360 - tolerance;
        }

        /// <summary>
        /// Estimates the arrival time according to current velocity and acceleration.
        /// </summary>
        /// 
        /// <remarks>
        /// <a href="https://www.math10.com/en/algebra/formulas-for-short-multiplication.html">Polynomial Identities</a>
        /// and
        /// <a href="https://opentextbc.ca/physicstestbook2/chapter/motion-equations-for-constant-acceleration-in-one-dimension/">Motion</a>.
        /// 
        /// Ah! Back to physics and maths. The two links provide
        /// lectures needed for this calculation.
        /// Here we have to find out the time needed to cover the distance.
        /// We use the formula with initial speed and constant acceleration:
        /// <c>d=vt+at²</c>. The formula is then transformed as following:
        ///
        /// <code>
        /// => at² + vt = d
        /// 
        /// * Both side divide by `a`
        ///          v      d
        /// => t² + ---t = ---
        ///          a      a
        /// 
        /// * We add the same "thing" to both side of the formula, to make
        /// * it a quadratic formula, so we can use Polynomial Identities.
        /// * Which reads <c>(x + y)² = x² + 2xy + y²</c>
        /// * The tricky bit here is to find that "thing".
        ///
        ///          v       v       d      v
        /// => t² + ───t + (───)² = ─── + (───)²
        ///          a      2a       a     2a
        ///
        ///              ^^^^^^^^       ^^^^^^^^
        /// 
        /// * Now we also need to simplify the right side a bit too.
        /// * Multiplying same "thing" to both numerator and denominator
        /// * won't change the faction.
        ///
        ///          v       v       4ad     v
        /// => t² + ───t + (───)²  = ─── + (───)²
        ///          a      2a       4a²    2a
        ///                          ^^^
        ///                          ×`4a`
        ///
        /// * Polynomial Identities to the left, and simplified the right
        /// 
        ///          v      4ad + v²
        /// => (t + ───)² = ────────
        ///         2a         4a²
        ///
        /// * Square root both sides, remember the `±` sign.
        ///                 __________
        ///         v    ± √ 4ad + v²
        /// => t + ─── = ─────────────
        ///        2a         2a
        /// 
        ///           __________
        ///        ± √ 4ad + v²     v
        /// => t = ───────────── − ───
        ///             2a         2a
        /// 
        ///           __________
        ///        ± √ 4ad + v²  − v
        /// => t = ─────────────────
        ///               2a
        /// </code>
        /// </remarks>
        /// 
        /// <param name="distance">
        /// Distance to be estimated.
        /// </param>
        /// <param name="dec">
        /// Use deceleration if this is <c>true</c>.
        /// Otherwise use acceleration.
        /// </param>
        /// <returns>
        /// The estimated time of arrival. In case of deceleration,
        /// <c>float.PositiveInfinity</c> may be returned if it couldn't
        /// reach the target.
        /// The actual unit is not crucial in most circumstances.
        /// One could think it were in seconds.
        /// </returns>
        public float EstimatedArrival(float distance, bool dec = false)
        {
            float v = this.Vc;
            float a = this.acceleration;
            if (dec) {
                a = -this.deceleration;
            }

            //                         __________
            // We first calculate the √ 4ad + v²  part.
            // If we're decelerating, `4ad + v²` could become negative.
            // Because `a` could be a big negative number.
            // Which means we'll never reach target if decelerate.
            float n = 4 * a * distance + Mathf.Pow(v, 2);
            if (n <= 0) {
                return float.PositiveInfinity;
            }

            n = Mathf.Sqrt(n);
            float a2 = 2 * a;

            // We first check the positive sign, if it yields a positive
            // value, there is no need to check the negative part.
            float t = (n - v) / a2;
            if (t > 0) {
                return t;
            }

            return (-n - v) / a2;
        }

        protected void OnEnable()
        {
            Dictionary<int, object> config = new Dictionary<int, object>() {
                [(int)AccelerationMotorAttributes.MaxTurn] = this.maxTurn,
                [(int)AccelerationMotorAttributes.MaxSpeed] = this.maxSpeed,
                [(int)AccelerationMotorAttributes.MaxAcceleration] =
                    this.acceleration,
                [(int)AccelerationMotorAttributes.MaxDeceleration] =
                    this.deceleration,
            };
            this.Motor = new AccelerationMotor(config);
        }

        protected void FixedUpdate()
        {
            this.UpdateExistence();
            this.ApplyTurn();
            this.ApplySpeed();
        }

        protected void Reset()
        {
            this.Init();
        }

        /// <summary>
        /// Initialize the instance, allocating a new <see cref="FlightState"/>
        /// instance with default values.
        /// </summary>
        protected void Init()
        {
            this.State = new FlightState(
                new Existence(this.Body.mass, this.transform),
                new Motion(this.maxSpeed, this.maxTurn)
            );
        }

        /// <summary>
        /// Update <see cref="State"/> information from underlying game object.
        /// </summary>
        protected void UpdateExistence()
        {
            this.State.Existence =
                new Existence(this.Body.mass, this.transform);
        }

        /// <summary>
        /// Calculate and apply velocity to game object. It should be called
        /// in <c>FixedUpdate()</c>. Also updates the <see cref="State"/>.
        ///
        /// <para>
        /// Remember that it is doing actual physics calculation here.
        /// Though Unity reliefs us from a lot of burden. We still need
        /// to have Newton's Laws in mind to understand the outcome of
        /// such calculation.
        /// </para>
        /// 
        /// </summary>
        protected void ApplySpeed()
        {
//            if (this.Stopping) {
//                this.Motor.FullReverse();
//            } else {
//                this.FullThrottle();
//            }

            this.Vc = this.Motor.GetVelocity(Time.fixedDeltaTime);
            Motion mo = this.State.Motion;
            if (mo.Speed.Equals(this.Vc)) {
                return;
            }

            mo.Speed = this.Vc;
            this.State.Motion = mo;
            this.Body.velocity = this.Vc * this.transform.forward;

            if (this.Vc.Equals(0)) {
                this.FullStop();
            }
        }

        /// <summary>
        /// Actually makes the turn. Also updates the
        /// <see cref="State"/>.
        /// </summary>
        protected void ApplyTurn()
        {
            Vector3 dir = this.Bearing;
            if (dir == Vector3.zero) {
                return;
            }

            Quaternion to = Quaternion.LookRotation(dir);

            Motion mo = this.State.Motion;
            Quaternion bearing = Quaternion.Lerp(
                this.transform.rotation,
                to,
                this.Motor.GetRoll(Time.fixedDeltaTime)
            );

            this.transform.rotation = bearing;
            mo.Bearing = bearing;
            this.State.Motion = mo;
        }
    }
}
