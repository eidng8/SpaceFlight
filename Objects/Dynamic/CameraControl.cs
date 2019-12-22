using UnityEngine;

namespace eidng8.SpaceFlight.Objects.Dynamic {
    [RequireComponent(typeof(Camera))]
    public class CameraControl : MonoBehaviour
    {
        private Camera cam;

        // Start is called before the first frame update
        void Awake()
        {
            this.cam = this.GetComponent<Camera>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0)) {
                RaycastHit hit;
                Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100)) {
                }
            }
        }
    }
}
