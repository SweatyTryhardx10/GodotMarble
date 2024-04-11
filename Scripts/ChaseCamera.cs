using System.Collections.Generic;
using Godot;

public partial class ChaseCamera : Camera3D
{
    [Export] public Node3D target;
    [Export] public float heightOffset = 5;
    [Export] private Vector3 lookAtOffset = new Vector3(0f, 1f, 2f);
    [ExportGroup("Distance Offset")]
    [Export] public float distanceOffsetDefault = 5;
    [Export] public float distanceOffsetMin = 2f;
    [Export] public float distanceOffsetMax = 8f;
    private float distanceOffset;
    [ExportGroup("")]
    [Export(PropertyHint.Range, "0, 5")] private float autoLookCooldown = 1.5f;
    [Export] public float smoothing = 1f;
    [Export] public float mouseSensitivity = 10f;

    // The position buffer system almost completely fixes camera jitter (stemming from the physics system's update rate)
    private Vector3[] targetPositionBuffer = new Vector3[20];
    private Vector3 smoothedTargetPosition
    {
        get
        {
            // Disable smoothing below (note: smoothing is no longer needed after implementing rigidbody interpolation)
            return target.GlobalPosition;
            
            Vector3 sum = Vector3.Zero;
            for (int i = 0; i < targetPositionBuffer.Length; i++)
            {
                sum += targetPositionBuffer[i];
            }
            return sum / targetPositionBuffer.Length;
            
            // int physicsFPS = (int)ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
            // double secsPerUpdate = 1d / physicsFPS;
            
            // float t = (float)(idleClock % secsPerUpdate) / 1f;
            // return targetPositionBuffer[1].Lerp(targetPositionBuffer[0], t);
        }
    }
    
    private double idleClock;

    private Vector3 lastDirection;

    private Vector2 mouseDelta;

    private double autoLookTimer;

    public override void _Ready()
    {
        base._Ready();

        Input.MouseMode = Input.MouseModeEnum.Captured;

        distanceOffset = distanceOffsetDefault;

        lastDirection = ComputeDirection();
    }

    public override void _Process(double delta)
    {
        idleClock += delta;
        
        if (target == null) return;

        // Position history of target
        for (int i = 0; i < targetPositionBuffer.Length - 1; i++)
        {
            targetPositionBuffer[i] = targetPositionBuffer[i + 1];
        }
        targetPositionBuffer[targetPositionBuffer.Length - 1] = target.GlobalPosition;

        // Auto-look
        if (mouseDelta != Vector2.Zero)
            autoLookTimer = 0f;
        else
            autoLookTimer += delta;

        Vector3 toCamera;
        if (autoLookTimer >= autoLookCooldown)
        {
            toCamera = ComputeDirection();
        }
        else
        {
            toCamera = lastDirection;
        }
        toCamera = toCamera.Rotated(Vector3.Up, -mouseDelta.X * (float)delta * mouseSensitivity);
        toCamera = toCamera.Rotated(GlobalBasis.X, -mouseDelta.Y * (float)delta * mouseSensitivity);

        Vector3 targetPosition = smoothedTargetPosition + toCamera * distanceOffset + Vector3.Up * heightOffset;
        GlobalPosition = GlobalPosition.Lerp(targetPosition, (float)delta * smoothing);

        // Rotate to look at
        LookAt(smoothedTargetPosition + Vector3.Up * lookAtOffset.Y - toCamera.Normalized() * lookAtOffset.Z, Vector3.Up);

        lastDirection = toCamera;
        mouseDelta = Vector2.Zero;
    }

    private Vector3 ComputeDirection()
    {
        Vector3 direction = smoothedTargetPosition.DirectionTo(GlobalPosition - Vector3.Up * heightOffset);
        direction = direction.Normalized();

        return direction;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            mouseDelta = motion.Relative;
        }
        if (@event.IsActionPressed("ZoomIn"))
        {
            distanceOffset = Mathf.Clamp(distanceOffset - 0.5f, distanceOffsetMin, distanceOffsetMax);
        }
        if (@event.IsActionPressed("ZoomOut"))
        {
            distanceOffset = Mathf.Clamp(distanceOffset + 0.5f, distanceOffsetMin, distanceOffsetMax);
        }
    }
}
