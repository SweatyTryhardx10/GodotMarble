using Godot;
using System;

public partial class RigidBodyInterpolater : Node3D
{
	// NOTE TO SELF: This script works PERFECTLY as intended.
	// Was fully solved without external information!
	
	[Export] private bool interpolatePosition = true;
	[Export] private bool interpolateRotation = true;

	[Export] private PhysicsBody3D body;
	private Vector3[] positionHistory = new Vector3[2];
	private Quaternion[] rotationHistory = new Quaternion[2];

	private double timePerPhysicsTick;

	private double timer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		timePerPhysicsTick = 1d / (int)ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Set the global position of this mesh to the interpolated value
		float t = (float)(timer / timePerPhysicsTick);
		t = Mathf.Clamp(t, 0f, 1f);

		// Interpolate position
		if (interpolatePosition)
			GlobalPosition = positionHistory[1].Lerp(positionHistory[0], t);

		// Interpolate rotation
		if (interpolateRotation && rotationHistory[0].IsNormalized() && rotationHistory[1].IsNormalized())
			GlobalBasis = new Basis(rotationHistory[1].Slerp(rotationHistory[0], t));
		
		// Integrate interpolation timer
		timer += delta;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(body))
			return;

		// Save new entry to history
		for (int i = positionHistory.Length - 1; i >= 0; i--)
		{
			if (i == 0)
				positionHistory[i] = body.GlobalPosition;
			else
				positionHistory[i] = positionHistory[i - 1];
		}
		// Save new entry to history
		for (int i = rotationHistory.Length - 1; i >= 0; i--)
		{
			if (i == 0)
				rotationHistory[i] = body.GlobalTransform.Basis.GetRotationQuaternion();
			else
				rotationHistory[i] = rotationHistory[i - 1];
		}

		// Reset the "cycle" of the timer (to preserve the "leftovers" from the previous timer interval)
		// e.g. 0.33 % 0.2 == 0.13..
		timer = timer % timePerPhysicsTick;
		
		// Failsafe in case the timer gets out of phase with the physics process
		if (timer > timePerPhysicsTick / 2f)
			timer = 0d;
	}
}
