using UnityEngine;

namespace Tumble.App
{
    // Trigger collider on the CourseTrigger layer. Found by overlap query, not by trigger callbacks.
    public sealed class Checkpoint : MonoBehaviour
    {
        [SerializeField] Transform respawnPoint;

        public Pose RespawnPose
        {
            get
            {
                Transform point = respawnPoint != null ? respawnPoint : transform;
                return new Pose(point.position, Quaternion.Euler(0f, point.eulerAngles.y, 0f));
            }
        }
    }
}
