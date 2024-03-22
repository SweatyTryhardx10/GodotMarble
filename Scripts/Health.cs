using Godot;
using System;

public partial class Health : Node
{
	[Signal]
	public delegate void HasDiedEventHandler();

	[Export]
	private int startHealth = 3;

	private int Value { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Value = startHealth;
	}

	public void Damage(int amount)
	{
		Value -= amount;
		
		if (Value <= 0)
			EmitSignal(SignalName.HasDied);
	}
}
