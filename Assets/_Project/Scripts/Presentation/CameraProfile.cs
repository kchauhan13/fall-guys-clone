using UnityEngine;

namespace Tumble.Presentation
{
    // Values live on the asset. The Start column in docs/game-feel.md is the reference.
    [CreateAssetMenu(menuName = "Tumble/Camera Profile", fileName = "CameraProfile")]
    public sealed class CameraProfile : ScriptableObject
    {
        public float cameraDistance;
        public float cameraHeight;
        public float cameraPositionDamp;
        [Tooltip("Degrees")]
        public float cameraFov;
        public float diveFovKick;
        public float diveFovDuration;
    }
}
