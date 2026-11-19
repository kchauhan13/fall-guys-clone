using UnityEngine;

namespace Tumble.Presentation
{
    // M1 stiff orbit. Damping, collision pull-in, and the dive FOV kick belong to SPEC-006.
    [RequireComponent(typeof(Camera))]
    public sealed class OrbitCamera : MonoBehaviour
    {
        [SerializeField] CameraProfile profile;
        [SerializeField] LocalCommandSource look;

        Camera view;
        Transform target;

        void Awake()
        {
            view = GetComponent<Camera>();
        }

        public void Follow(Transform followTarget)
        {
            target = followTarget;
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            Quaternion orbit = Quaternion.Euler(look.LookPitch, look.LookYaw, 0f);
            Vector3 pivot = target.position + Vector3.up * profile.cameraHeight;
            transform.SetPositionAndRotation(pivot - orbit * Vector3.forward * profile.cameraDistance, orbit);
            view.fieldOfView = profile.cameraFov;
        }
    }
}
