using Godot;

public partial class ChaseCamera : Camera3D
{
    [Export] public Node3D target;
    [Export] public float hightOffset = 5;
    [Export] public float distanceOffset = 5;
    [Export] public float smoothing = 1f;
    [Export] public float mouseSensitivity = 10f;

    private Vector2 mouseDelta;

    public override void _Ready()
    {
        base._Ready();

        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        if (target == null) return;

        LookAt(target.Transform.Origin, Vector3.Up);

        Vector3 direction = target.GlobalTransform.Origin.DirectionTo(GlobalTransform.Origin);
        direction.Y = 0;

        // Rotate direction vector based on input
        direction = direction.Rotated(Vector3.Up, -mouseDelta.X * (float)delta * mouseSensitivity);

        Vector3 targetPosition = direction * distanceOffset + Vector3.Up * hightOffset + target.GlobalTransform.Origin;

        GlobalPosition = GlobalPosition.Lerp(targetPosition, smoothing * (float)delta);
        
        mouseDelta = Vector2.Zero;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion)
        {
            mouseDelta = motion.Relative;
        }
    }
}
