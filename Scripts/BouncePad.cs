using Godot;
using System;

public partial class BouncePad : Node3D
{
	private enum BounceMode
	{
		Bounce,
		Launch
	}

	[Export] private BounceMode mode;
	[Export] private float bounceMultiplier = 1f;
	[Export] private float launchImpulse = 20f;
	[Export] private Node3D mesh;

	private float tAnim;

	public override void _Ready()
	{
		GetNode<Area3D>("Area3D").BodyEntered += BodyHasEntered;
	}

    public override void _Process(double delta)
    {
		// Animation code
		if (tAnim < 1f)
		{
			tAnim += (float)delta;
			mesh.Position = Vector3.Up * 0.25f * (1f - tAnim) * (1f - tAnim);
			
			if (tAnim + delta >= 1f)
				mesh.Position = Vector3.Zero;
		}
    }

    private void BodyHasEntered(Node body)
	{
		GD.Print("Body entered bounce pad: " + body.Name);

		if (body is RigidBody3D)
		{
			RigidBody3D b = body as RigidBody3D;
			
			switch (mode)
			{
				case BounceMode.Bounce:
					Bounce(b);
					break;
				case BounceMode.Launch:
					Launch(b);
					break;
				
				default:
					break;
			}

			// Start the procedural animation
			tAnim = 0f;
		}
	}

	private void Bounce(RigidBody3D rb)
	{
		Vector3 surfaceNormal = GlobalBasis.Column1;

		int physicsFPS = (int)ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
		Vector3 impulse = -rb.LinearVelocity.Project(surfaceNormal) * bounceMultiplier * rb.Mass * 2f;
		rb.ApplyCentralImpulse(impulse);
	}
	
	private void Launch(RigidBody3D rb)
	{
		Vector3 surfaceNormal = GlobalBasis.Column1;

		int physicsFPS = (int)ProjectSettings.GetSetting("physics/common/physics_ticks_per_second");
		Vector3 impulseReset = -rb.LinearVelocity.Project(surfaceNormal) * bounceMultiplier * rb.Mass;
		
		Vector3 impulseLaunch = surfaceNormal * launchImpulse * rb.Mass;
		
		rb.ApplyCentralImpulse(impulseReset + impulseLaunch);
	}
}
