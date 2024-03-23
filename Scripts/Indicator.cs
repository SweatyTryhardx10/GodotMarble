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
		followTarget = Goal.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (followTarget == null)
		{
			TryGetTarget();
			return;
		}

		Camera3D cam = GetViewport().GetCamera3D();
		Vector2 viewportSize = GetViewportRect().Size;

		float borderWidth = 80f;
		Vector2 uiPos = cam.UnprojectPosition(followTarget.Position);
		float x = Math.Clamp(uiPos.X, borderWidth, viewportSize.X - borderWidth);
		float y = Math.Clamp(uiPos.Y, borderWidth, viewportSize.Y - borderWidth);

		// Flip position if target is behind camera
		if (cam.IsPositionBehind(followTarget.Position))
		{
			x += ((viewportSize.X / 2f) - x) * 2f;
			y += ((viewportSize.Y / 2f) - y) * 2f;
		}

		Position = new Vector2(x, y);
	}
}
