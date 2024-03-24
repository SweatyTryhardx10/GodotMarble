using Godot;
using System;

public partial class Goal : Node3D
{
	public static Goal Instance;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnArea3DBodyEntered(Node3D body)
	{
		GD.Print($"Body entered goal ({body.Name})");
		
		if (body.Name == "Player")
		{
			HUD.ShowSummaryScreen();
		}
	}
}
