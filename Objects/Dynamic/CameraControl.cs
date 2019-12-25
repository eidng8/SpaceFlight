using UnityEngine;


namespace eidng8.SpaceFlight.Objects.Dynamic
{
    public class CameraControl : MonoBehaviour
    {
        public Rigidbody target;

        private void Follow()
        {
            Transform me = this.transform;
            Quaternion dir = Quaternion.LookRotation(
                this.target.position - me.position
            );
            this.transform.rotation = Quaternion.Lerp(
                me.rotation,
                dir,
                Time.deltaTime
            );
        }

        // Update is called once per frame
        private void Update()
        {
            this.Follow();
        }
    }
}
