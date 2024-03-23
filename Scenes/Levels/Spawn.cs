using Godot;
using System;

public partial class Spawn : Node3D
{
	public static Spawn Instance { get; private set; }
	[Export]
	private PackedScene playerPrefab;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		
		SpawnPlayer();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_focus_next"))
			SpawnPlayer();
	}

	private void SpawnPlayer()
	{
		PlayerController player = PlayerController.Instance;
		if (player == null)
		{
			player = playerPrefab.Instantiate<PlayerController>();

			GetTree().Root.AddChild(player);
		}

		// Reset player's position to the spawn position
		PlayerController.Instance.Position = Position;
		PlayerController.Instance.LinearVelocity = Vector3.Zero;
		PlayerController.Instance.AngularVelocity = Vector3.Zero;
	}
}
