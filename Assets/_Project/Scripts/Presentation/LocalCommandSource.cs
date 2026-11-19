using Tumble.Simulation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tumble.Presentation
{
    // The only place gameplay input is read.
    public sealed class LocalCommandSource : MonoBehaviour, ICommandSource
    {
        [SerializeField] InputActionAsset actions;
        [SerializeField] float lookDegreesPerPixel = 0.12f;
        [SerializeField] float startPitch = 15f;
        [SerializeField] float minPitch = -20f;
        [SerializeField] float maxPitch = 60f;

        InputActionMap map;
        InputAction move;
        InputAction look;
        InputAction jump;
        InputAction dive;
        InputAction grab;
        bool jumpLatched;
        bool diveLatched;

        public float LookYaw { get; private set; }
        public float LookPitch { get; private set; }

        void Awake()
        {
            map = actions.FindActionMap("Runner", true);
            move = map.FindAction("Move", true);
            look = map.FindAction("Look", true);
            jump = map.FindAction("Jump", true);
            dive = map.FindAction("Dive", true);
            grab = map.FindAction("Grab", true);
            LookPitch = startPitch;
        }

        void OnEnable()
        {
            map.Enable();
            SetCursorLocked(true);
        }

        void OnDisable()
        {
            map.Disable();
            SetCursorLocked(false);
        }

        void Update()
        {
            if (jump.WasPressedThisFrame())
                jumpLatched = true;
            if (dive.WasPressedThisFrame())
                diveLatched = true;

            UpdateCursorLock();
            if (Cursor.lockState != CursorLockMode.Locked)
                return;

            Vector2 delta = look.ReadValue<Vector2>();
            LookYaw = Mathf.Repeat(LookYaw + delta.x * lookDegreesPerPixel, 360f);
            LookPitch = Mathf.Clamp(LookPitch - delta.y * lookDegreesPerPixel, minPitch, maxPitch);
        }

        public void SetLookYaw(float yaw)
        {
            LookYaw = Mathf.Repeat(yaw, 360f);
        }

        public PlayerCommand CreateCommand(int tick)
        {
            var command = new PlayerCommand
            {
                Tick = tick,
                Move = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f),
                LookYaw = LookYaw,
                JumpPressed = jumpLatched,
                JumpHeld = jump.IsPressed(),
                DivePressed = diveLatched,
                GrabHeld = grab.IsPressed(),
            };
            jumpLatched = false;
            diveLatched = false;
            return command;
        }

        void UpdateCursorLock()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                SetCursorLocked(false);
            else if (mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
                SetCursorLocked(true);
        }

        static void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
