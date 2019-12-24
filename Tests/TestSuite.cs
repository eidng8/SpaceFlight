using System.Collections.Generic;
using eidng8.SpaceFlight.Objects.Dynamic.Motors;
using NUnit.Framework;



namespace eidng8.SpaceFlight.Tests
{
    public class TestAccelerationMotor
    {
        private AccelerationMotor _motor;

        [SetUp]
        public void Setup()
        {
            Dictionary<int, object> config = new Dictionary<int, object> {
                [(int)AccelerationMotorAttributes.MaxSpeed] = 100f,
                [(int)AccelerationMotorAttributes.MaxTurn] = 10f,
                [(int)AccelerationMotorAttributes.MaxAcceleration] = 10f,
                [(int)AccelerationMotorAttributes.MaxDeceleration] = 10f
            };
            _motor = new AccelerationMotor(config);
        }

        [Test]
        public void TestFullReverseShouldDecreaseThrust()
        {
            _motor.FullReverse();
            Assert.AreEqual(-10, _motor.GetThrust());
        }

        [Test]
        public void TestFullStopShouldNotChangeThrust()
        {
            _motor.FullStop();
            Assert.AreEqual(0, _motor.GetThrust());
        }

        [Test]
        public void TestFullThrottleShouldIncreaseThrust()
        {
            _motor.FullThrottle();
            Assert.AreEqual(10, _motor.GetThrust());
        }

        [Test]
        public void TestThrottleShouldAffectThrust()
        {
            _motor.Throttle = .8f;
            Assert.AreEqual(8, _motor.GetThrust());
        }

        [Test]
        public void TestThrustIsAcceleration()
        {
            _motor.Throttle = .8f;
            Assert.AreEqual(16, _motor.GetVelocity(2));
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
//        [UnityTest]
//        public IEnumerator TestSuiteWithEnumeratorPasses()
//        {
//            // Use the Assert class to test conditions.
//            // Use yield to skip a frame.
//            yield return null;
//        }
    }
}
