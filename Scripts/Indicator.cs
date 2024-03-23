using Godot;
using System;

public partial class Indicator : TextureRect
{
	private Node3D followTarget;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TryGetTarget();
	}

	private void TryGetTarget()
	{
		followTarget = PlayerController.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (followTarget == null)
		{
			TryGetTarget();
			return;
		}

		Position = GetViewport().GetCamera3D().UnprojectPosition(followTarget.Position);
	}
}
