using System.Collections.Generic;
using Tumble.Presentation;
using Tumble.Simulation;
using UnityEngine;

namespace Tumble.App
{
    // Owns the clock and the tick order for the Gym. See docs/architecture.md, "One FixedUpdate tick".
    public sealed class GymBootstrap : MonoBehaviour
    {
        [SerializeField] PlayerMotor runnerPrefab;
        [SerializeField] MovementProfile movementProfile;
        [SerializeField] Checkpoint spawnCheckpoint;
        [SerializeField] LocalCommandSource localInput;
        [SerializeField] OrbitCamera orbitCamera;
        [SerializeField] LayerMask courseTriggerMask;

        static readonly PlayerId LocalPlayer = new PlayerId(0);

        readonly Collider[] overlaps = new Collider[8];
        readonly Dictionary<PlayerId, Pose> lastCheckpoint = new Dictionary<PlayerId, Pose>();

        RoundClock clock;
        PlayerMotor runner;

        void Start()
        {
            clock = new RoundClock(Time.fixedDeltaTime);

            Pose spawn = spawnCheckpoint.RespawnPose;
            lastCheckpoint[LocalPlayer] = spawn;

            runner = Instantiate(runnerPrefab, spawn.position, spawn.rotation);
            runner.name = $"Runner ({LocalPlayer})";
            runner.Initialize(LocalPlayer, movementProfile);

            localInput.SetLookYaw(spawn.rotation.eulerAngles.y);
            orbitCamera.Follow(runner.transform);
        }

        void FixedUpdate()
        {
            clock.Advance();
            runner.Simulate(localInput.CreateCommand(clock.Tick), clock, Time.fixedDeltaTime);
            ApplyCourseTriggers(runner);
        }

        void ApplyCourseTriggers(PlayerMotor motor)
        {
            CapsuleCollider capsule = motor.Capsule;
            Vector3 feet = motor.Position;
            Vector3 bottom = feet + Vector3.up * capsule.radius;
            Vector3 top = feet + Vector3.up * (capsule.height - capsule.radius);

            int count = Physics.OverlapCapsuleNonAlloc(bottom, top, capsule.radius, overlaps,
                courseTriggerMask, QueryTriggerInteraction.Collide);

            bool fell = false;
            for (int i = 0; i < count; i++)
            {
                if (overlaps[i].TryGetComponent(out Checkpoint checkpoint))
                    lastCheckpoint[motor.Id] = checkpoint.RespawnPose;
                else if (overlaps[i].TryGetComponent(out FallVolume _))
                    fell = true;
            }

            if (!fell)
                return;

            Pose respawn = lastCheckpoint[motor.Id];
            motor.Teleport(respawn.position, respawn.rotation);
        }
    }
}
