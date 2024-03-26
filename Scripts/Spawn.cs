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
		if (!IsInstanceValid(Instance))
			return;

		if (!IsInstanceValid(PlayerController.Instance))
		{
			PlayerController player = Instance.playerPrefab.Instantiate<PlayerController>();
			player.TreeEntered += PositionPlayer;	// Position player after they have entered the tree
			Instance.GetTree().Root.CallDeferred(MethodName.AddChild, player);  // For some reason, this method call must be 'deferred' otherwise
																				// the child is added before the tree is built
		}
		else
		{
			PositionPlayer();
		}
	}

	private static void PositionPlayer()
	{
		PlayerController.Respawn(Instance.GlobalPosition);
	}
}
