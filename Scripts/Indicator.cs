using Godot;
using System;

public partial class Indicator : TextureRect
{
	[Export] private float rotationOffset = 30f;
	
	private Node3D followTarget;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TryGetTarget();
	}

	private void TryGetTarget()
	{
		if (IsInstanceValid(Goal.Instance))
			followTarget = Goal.Instance;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!IsInstanceValid(followTarget))
		{
			TryGetTarget();
			return;
		}

		Camera3D cam = GetViewport().GetCamera3D();
		
		Vector2 viewportSize = GetViewportRect().Size;

		float borderWidth = 30f;
		Vector2 uiPos = cam.UnprojectPosition(followTarget.Position);
		float x = Math.Clamp(uiPos.X, borderWidth, viewportSize.X - borderWidth);
		float y = Math.Clamp(uiPos.Y, borderWidth, viewportSize.Y - borderWidth);

		// Flip position if target is behind camera
		if (cam.IsPositionBehind(followTarget.Position))
		{
			x += ((viewportSize.X / 2f) - x) * 2f;
			y += ((viewportSize.Y / 2f) - y) * 2f;
		}

		// Set position
		Position = new Vector2(x, y);
		
		// Rotate texture accordingly
		Vector2 screenCenter = viewportSize / 2f;
		float offset = Mathf.DegToRad(rotationOffset);
		Rotation = Vector2.Up.AngleTo(Position - screenCenter) + offset;
	}
}
