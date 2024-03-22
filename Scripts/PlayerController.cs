using Godot;

public partial class PlayerController : RigidBody3D
{
	[Export] private float Speed = 5.0f;
	[Export] private float JumpVelocity = 4.5f;
	[Export] private float SlamVelocity = 4.5f;
	[Export] private float SlamForce = 500f;
	[Export] private Area3D area3D;
	private bool isSlamming = false;
	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		var cameraBasis = GetViewport().GetCamera3D().Transform.Basis;
		Vector3 direction = inputDir.X * cameraBasis.X.Normalized() + inputDir.Y * cameraBasis.Z.Normalized();
		direction.Y = 0;
		ApplyForce(direction.Normalized() * Speed);

		if (isSlamming)
		{
			ApplyForce(Vector3.Down * SlamVelocity);
			if (GetContactCount() > 0)
			{
				isSlamming = false;
				foreach (var item in area3D.GetOverlappingBodies())
				{
					if (item is RigidBody3D item2)
					{
						item2.ApplyForce(Vector3.Up * SlamForce);
					}
				}
			}
		}
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept") && GetContactCount() > 0)
		{
			ApplyCentralImpulse(Vector3.Up * JumpVelocity);
		}
		if (Input.IsActionJustPressed("ui_accept") && GetContactCount() <= 0)
		{
			isSlamming = true;
		}
	}
}