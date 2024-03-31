using Godot;
using System;

public partial class Boundary : Area3D
{
	public override void _Ready()
	{
		base._Ready();

		BodyEntered += (Node3D n) =>
		{
			if (n.Name == "Player")
				Spawn.QueueSpawn();
			else
			{
				// Delete object (if it's a physics object)
				if (n is PhysicsBody3D)
					n.QueueFree();
			}
		};
	}
}
