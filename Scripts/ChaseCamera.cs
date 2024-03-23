using Godot;

public partial class ChaseCamera : Camera3D
{
    [Export] public Node3D target;
    [Export] public float hightOffset = 5;
    [Export] public float distanceOffset = 5;
    [Export] public float smoothing = 1f;

    public override void _Process(double delta)
    {
        if (target == null) return;

        LookAt(target.Transform.Origin, Vector3.Up);

        Vector3 direction = target.GlobalTransform.Origin.DirectionTo(GlobalTransform.Origin);
        direction.Y = 0;

        Vector3 targetPosition = direction * distanceOffset + Vector3.Up * hightOffset + target.GlobalTransform.Origin;

        GlobalPosition = GlobalPosition.Lerp(targetPosition, smoothing * (float)delta);
    }
}
