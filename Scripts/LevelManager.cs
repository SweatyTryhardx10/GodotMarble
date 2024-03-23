using Godot;
using System;

public partial class LevelManager : Node3D
{
	private float startTime;
	private float timeElapsed => Time.GetTicksMsec() - startTime;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		startTime = Time.GetTicksMsec();
		GetTree().CreateTimer(3.0d).Timeout += () => { Spawn.Instance.SpawnPlayer(); };
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HUD.SetTimer(timeElapsed / 1000.0f);
	}
}
