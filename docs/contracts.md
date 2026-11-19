# Contracts

Shape of the data every system shares. Behavior lives in the specs. Initial numbers live in [game-feel.md](game-feel.md).

Namespaces match assembly definitions: `Tumble.Simulation`, `Tumble.Presentation`, `Tumble.App`.

## Identity

```csharp
readonly struct PlayerId
{
    public readonly int Value;
}
```

The local human is a `PlayerId` chosen at spawn, not a singleton inside the motor. Gym spawns use `PlayerId(0)`.

## Command

Produced by a command source. Consumed once per physics tick by the motor.

```csharp
struct PlayerCommand
{
    public int Tick;
    public Vector2 Move;   // x right, y forward, length clamped to 1
    public float LookYaw;  // degrees, world yaw
    public bool JumpPressed;
    public bool JumpHeld;
    public bool DivePressed;
    public bool GrabHeld;
}
```

`JumpPressed` and `DivePressed` are edges for that tick. `JumpHeld` and `GrabHeld` are levels. `JumpHeld` drives the variable-height jump cut (see [SPEC-002](specs/SPEC-002-jump.md)). The local source samples the Input System in `Update`, stores edges, and attaches them to the next fixed tick. Extra fixed steps in the same frame get a command with edges cleared but levels repeated. Stick magnitude and yaw are levels, so they repeat.

`Move` is stick space. The motor turns it into a world wish direction with `LookYaw`. The camera is not an argument.

## Motor state

Explicit values so later snapshots stay stable. There is no knock-down state: ADR-0008 removed it.

```csharp
enum MotorState : byte
{
    Upright = 0,
    Diving = 1,
    GettingUp = 2,
    Grabbing = 3,
}
```

`GettingUp` is the post-dive recovery. The runner gets up on their own; nothing else puts them there.

## Snapshot

What presentation, and later the network, are allowed to read:

```csharp
struct PlayerSnapshot
{
    public PlayerId Id;
    public int Tick;
    public MotorState State;
    public Vector3 Position;
    public Quaternion Rotation;
    public Vector3 Velocity;
    public bool Grounded;
    public bool Qualified;
}
```

Presentation may also subscribe to moments: `Jumped`, `Dived`, `Landed`, `GotUp`, `Grabbed`, `GrabBroke`, `Qualified`, `Fell`. Subscribers do not change velocity, state, or qualification.

## Movement profile

`MovementProfile : ScriptableObject` in `Tumble.Simulation`. Fields match the table in [game-feel.md](game-feel.md), camelCase, serialized. Camera fields (`cameraDistance` through `diveFovDuration`) live on `CameraProfile` in `Tumble.Presentation` instead, so the motor assembly does not own camera data.

The motor reads the profile. It does not contain a second copy of the numbers as constants.

## Clock

```csharp
sealed class RoundClock
{
    public int Tick { get; }
    public float Time { get; } // Tick * fixedDeltaTime
    public void Advance();     // one physics step
}
```

`PlayerMotor` and obstacle drivers take the clock. They do not read `Time.time`, `Time.timeSinceLevelLoad`, or `DateTime` for gameplay timing. The Gym's clock advances once per `FixedUpdate`. A future server clock replaces the caller, not the motor.

## Surfaces the motor depends on

```csharp
interface ICommandSource
{
    PlayerCommand CreateCommand(int tick);
}

interface IMotorEvents
{
    // Presentation implements this. Simulation calls it.
}
```

`LocalCommandSource` is the only implementation until the multiplayer milestone. A replay source is allowed in the Gym if a spec asks for it.

## Layers

Set up in M1. Names are exact.

| Layer | Collides with |
|---|---|
| `Runner` | Ground, Obstacle, Runner, Grabbable |
| `Ground` | Runner, Obstacle, Grabbable |
| `Obstacle` | Runner, Ground, Obstacle, Grabbable |
| `Grabbable` | Runner, Ground, Obstacle |
| `CourseTrigger` | Nothing. Overlap queries only |

Cosmetic meshes use `Ignore Raycast` or a dedicated `Cosmetic` layer that collides with nothing. The capsule on `Runner` is the only player collider.

## Prefab

`Assets/_Project/Prefabs/Runner/Runner.prefab`

Root at the feet. Children: `Collider` (capsule), `Visual` (mesh, no collider), `GrabOrigin` (empty, chest height, about `y = 1.1`). Rigidbody on the root.
