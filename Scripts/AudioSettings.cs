using Godot;
using System;

public partial class AudioSettings : AudioStreamPlayer
{
	[Export] private bool loop = false;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (loop)
			Finished += () => Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
