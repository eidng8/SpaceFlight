using UnityEngine;

namespace eidng8.SpaceFlight.Objects
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class SpaceObject : MonoBehaviour
    {
        protected Rigidbody Body {
            get {
                if (this._set) {
                    return this._body;
                }

                this._body = this.GetComponent<Rigidbody>();
                this._body.useGravity = false;
                this._body.drag = 0;
                this._body.angularDrag = 0;
                this._body.isKinematic = false;
                this._set = true;
                return this._body;
            }
        }

        private Rigidbody _body;

        private bool _set = false;
    }
}
