// ---------------------------------------------------------------------------
// <copyright file="EventChannels.cs" company="eidng8">
//      GPLv3
// </copyright>
// <summary>
// 
// </summary>
// ---------------------------------------------------------------------------

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
    /// directly. Use the <seealso cref="Throttle"/> property to control
    /// movement acceleration. A <seealso cref="FullThrottle"/> method is
    /// provided for convenience. Use <seealso cref="FullStop"/> to cut
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
        [Tooltip("Maximum forward velocity.")]
        public float maxSpeed = 200;

        /// <summary>
        /// Determines how quickly can the object turn on its sides.
        /// </summary>
        [Tooltip("Determines how quickly can the object turn.")]
        public float maxTurn = 10;

        /// <summary>
        /// Full throttle acceleration. This is used to speed up the object
        /// until it reaches <seealso cref="maxSpeed"/>.
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
        /// Current forward velocity. <c>Vc</c> stands for Velocity current.
        /// With the small <c>c</c> as subscript.
        /// </summary>
        public float Vc { get; private set; }

        /// <summary>
        /// Between 0 ~ 1, fraction of full throttle.
        /// The <seealso cref="acceleration"/> is multiplied by <c>Throttle</c>
        /// during calculation.
        /// </summary>
        public float Throttle {
            get => this._throttle;
            set => this._throttle = Mathf.Clamp(value, 0, 1);
        }

        private float _throttle;

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

        /// <summary>
        /// Whether it is slowing down.
        /// </summary>
        protected bool stopping;


        /// <summary>
        /// Sets <seealso cref="Throttle"/> to <c>1</c>,
        /// and <seealso cref="stopping"/> to <c>false</c>.
        /// </summary>
        public void FullThrottle()
        {
            this._throttle = 1;
            this.stopping = false;
        }

        /// <summary>
        /// Sets <seealso cref="Throttle"/> to <c>0</c>,
        /// and <seealso cref="stopping"/> to <c>true</c>.
        /// </summary>
        public void FullStop()
        {
            this._throttle = 0;
            this.stopping = true;
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

            float t = (n - v) / a2;
            if (t > 0) {
                return t;
            }

            return (-n - v) / a2;
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

        protected void Init()
        {
            this.State = new FlightState(
                new Existence(this.Body.mass, this.transform),
                new Motion(this.maxSpeed, this.maxTurn)
            );
        }

        protected void UpdateExistence()
        {
            this.State.Existence =
                new Existence(this.Body.mass, this.transform);
        }

        /// <summary>
        /// Calculate the forward thrust of the game object.
        /// It updates the <c>thrust</c> in <seealso cref="State"/>,
        /// taking <c>thrustThreshold</c> into account.
        ///
        /// <para>
        /// Remember that it is doing actual physics calculation here.
        /// Though Unity reliefs us from a lot of burden. We still need
        /// to have Newton's Laws in mind to understand the outcome of
        /// such calculation.
        /// </para>
        /// 
        /// </summary>
        /// <returns>
        /// The forward thrust vector, suitable to be passed directly to
        /// <see cref="UnityEngine.Rigidbody.AddForce(Vector3)"/>
        /// </returns>
        protected void ApplySpeed()
        {
            if (this.stopping) {
                this.Decelerate();
                return;
            }

            this.Accelerate();
        }

        protected void Accelerate()
        {
            float throttle = Mathf.Clamp(this.Throttle, 0, 1);
            this.Vc += throttle * this.acceleration * Time.deltaTime;
            this.Vc = Mathf.Clamp(this.Vc, 0, this.maxSpeed);

            Motion mo = this.State.Motion;
            if (mo.Speed.Equals(this.Vc)) {
                return;
            }

            mo.Speed =
                this.Vc;
            this.State.Motion = mo;
            this.Body.velocity = this.Vc.Equals(0)
                ? Vector3.zero
                : this.Vc * this.transform.forward;
        }

        protected void Decelerate()
        {
            Motion mo = this.State.Motion;
            if (mo.Speed < 0.001) {
                mo.Speed = 0;
                return;
            }

            this.Vc -= this.deceleration * Time.deltaTime;
            this.Vc = Mathf.Clamp(this.Vc, 0, this.maxSpeed);
            mo.Speed = this.Vc;
            this.State.Motion = mo;
            this.Body.velocity = this.Vc < 0.001
                ? Vector3.zero
                : this.Vc * this.transform.forward;
        }


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
                this.State.Motion.TurnMax * Time.deltaTime
            );

            this.transform.rotation = bearing;
            mo.Bearing = bearing;
            this.State.Motion = mo;
        }
    }
}
