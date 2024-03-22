using Godot;
using System;

public partial class Indicator : TextureRect
{
	private Node3D followTarget;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		string sceneName = GetTree().CurrentScene.Name;
		followTarget = GetNode<Node3D>($"/root/{sceneName}/Enemy");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position = GetViewport().GetCamera3D().UnprojectPosition(followTarget.Position);
	}
}
