using Godot;
using System;

public partial class Spawn : Node3D
{
	[Signal]
	public delegate void OnSpawnEventHandler(Vector3 position);

	private static Spawn Instance { get; set; }
	[Export]
	private PackedScene playerPrefab;

	public override void _EnterTree()
	{
		base._EnterTree();
		Instance = this;
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_focus_next"))
			SpawnPlayer();
	}

	public static void SpawnPlayer()
	{
		if (!IsInstanceValid(PlayerController.Instance))
		{
			PlayerController player = Instance.playerPrefab.Instantiate<PlayerController>();
			Instance.GetTree().Root.AddChild(player);
		}

		// // Emit spawn signal
		// Instance.EmitSignal(SignalName.OnSpawn, "position", Instance.Position);
		PlayerController.Respawn(Instance.GlobalPosition);
	}
}
